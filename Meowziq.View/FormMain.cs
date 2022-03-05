
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

using Meowziq.Core;
using Meowziq.IO;
using Meowziq.Loader;
using Meowziq.Midi;

namespace Meowziq.View {
    /// <summary>
    /// TODO: 排他制御
    /// MEMO: 半動的生成で MIDI ノートを出力し MIDI 楽器の音色をリアルタイムで変化させて楽しむ 
    /// </summary>
    public partial class FormMain : Form {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Midi.Manager _midi;

        string _targetPath;

        string _exMessage;

        DialogResult _result;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public FormMain() {
            InitializeComponent();

            // MIDIデバイス準備
            _midi = new Midi.Manager();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        /// <summary>
        /// 演奏を開始します
        /// </summary>
        async void buttonPlay_Click(object sender, EventArgs e) {
            try {
                if (_textBoxSongName.Text.Equals("------------")) {
                    var message = "please load a song.";
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Log.Error(message);
                    return;
                }
                if (Sound.Playing || Sound.Stopping) {
                    return;
                }
                await startSound();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// 演奏を停止します
        /// </summary>
        async void buttonStop_Click(object sender, EventArgs e) {
            try {
                if (Sound.Stopping || !Sound.Played) {
                    return;
                }
                await stopSound();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// データをロードします
        /// </summary>
        async void buttonLoad_Click(object sender, EventArgs e) {
            try {
                _folderBrowserDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                var dr = _folderBrowserDialog.ShowDialog();
                if (dr == DialogResult.OK) {
                    _targetPath = _folderBrowserDialog.SelectedPath;
                    _textBoxSongName.Text = await buildSong();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// データをSMFに変換します
        /// </summary>
        async void buttonConvert_Click(object sender, EventArgs e) {
            try {
                if (_textBoxSongName.Text.Equals("------------")) {
                    var message = "please load a song.";
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Log.Error(message);
                    return;
                }
                if (await convertSong()) {
                    MessageBox.Show("converted the song to SMF.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// TODO: 動作確認 ⇒ NG
        /// </summary>
        /*async*/ void formMain_FormClosing(object sender, FormClosingEventArgs e) {
            //var result = await stopSound();
            //if (result) {
            //    Close();
            //}
        }

        /// <summary>
        /// MIDI データをデバイスに投げます
        /// NOTE: conductor.midi 依存で 30 tick単位でしか呼ばれていない
        /// NOTE: conductor.midi のメッセージはスルーする  midi.OutDevice.Send(e.Message);
        /// MEMO: tick と名前を付ける対象は常に絶対値とする
        /// TODO: メッセージ送信のタイミングは独自実装出来るのでは？
        /// </summary>
        void sequencer_ChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
            if (Sound.Stopping) {
                return;
            }
            if (Visible) {
                // TODO: カウント分はUIではマイナス表示とする？
                var tick = _sequencer.Position - 1; // NOTE: Position が 1, 31 と来るので予め1引く
                // MIDIメッセージ処理
                Midi.Message.ApplyTick(tick, loadSong); // 1小節ごとに切り替える MEMO: シンコぺを考慮する
                var list = Midi.Message.GetBy(tick); // MIDIメッセージのリストを取得
                if (list != null) {
                    list.ForEach(x => {
                        _midi.OutDevice.Send(x); // MIDIデバイスにメッセージを追加送信 MEMO: CCなどは直接ここで投げては？
                    }); // MEMO: Parallel.ForEach では逆に遅かった
                    list.ForEach(x => {
                        if (x.MidiChannel != 9 && x.MidiChannel != 1) { // FIXME: 暫定:シーケンス除外
                            _pianoControl.Send(x); // ドラム以外はピアノロールに表示
                        }
                        if (x.MidiChannel == 2) {
                            Log.Debug($"Data1: {x.Data1} Data2: {x.Data2}");
                        }
                    });
                }
                // UI情報更新
                State.Beat = ((tick / 480) + 1); // 0開始 ではなく 1開始として表示
                State.Meas = ((State.Beat - 1) / 4 + 1);
                var map = State.ItemMap;
                if (map.ContainsKey(tick)) { // FIXME: ContainsKey 大丈夫？
                    var item = map[tick];
                    Invoke(updateDisplay(item));
                }
                Sound.Played = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// 停止中に曲データを完全にロードします
        /// NOTE: SMF 出力時にも呼ばれます
        /// </summary>
        async Task<string> buildSong(bool smf = false) {
            var name = "------------";
            await Task.Run(() => {
                Mixer<ChannelMessage>.Clear(); // TODO: ここでOKか確認
                Cache.Clear();
                Cache.Load(_targetPath);
                buildResourse(0, true, smf);
                name = State.Name;
                Log.Info("load! :)");
            });
            return name; // Song の名前を返す
        }

        /// <summary>
        /// 演奏中に曲データの必要な部分をロードします
        /// NOTE: Message クラスから呼ばれます
        /// </summary>
        async void loadSong(int tick) {
            try {
                await Task.Run(() => {
                    Cache.Load(_targetPath); // json ファイルを読み込む
                    buildResourse(tick, true);
                    Cache.Update(); // 正常に読み込めたらキャッシュとして採用
                    _exMessage = "";
                    Log.Trace("load! :)");
                });
            } catch (Exception ex) { // NOTE: ここで ファイル読み込み ⇒ Build() までの全ての例外を捕捉する
                if (!_exMessage.Equals(ex.Message)) { // エラーメッセージが違う場合
                    _ = Task.Factory.StartNew(() => { // エラーダイアログ表示
                        _result = MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        if (_result is DialogResult.OK) { // OKで閉じられたら
                            _exMessage = ""; // フラグを初期化して必要なら再度ダイアログを表示
                        }
                    });
                }
                _exMessage = ex.Message;
                await Task.Run(() => {
                    Log.Fatal("load failed.. :(");
                    buildResourse(tick, false);
                });
            }
        }

        /// <summary>
        /// リソースの json ファイルをメモリに読み込みます
        /// </summary>
        void buildResourse(int tick, bool current = true, bool smf = false) {
            MixerLoader<ChannelMessage>.Build(current ? Cache.Current.MixerStream : Cache.Valid.MixerStream);
            Mixer<ChannelMessage>.Message = MessageFactory.CreateMessage();
            SongLoader.PatternList = PatternLoader.Build(current ? Cache.Current.PatternStream : Cache.Valid.PatternStream);
            var song = SongLoader.Build(current ? Cache.Current.SongStream : Cache.Valid.SongStream);
            PlayerLoader<ChannelMessage>.PhraseList = PhraseLoader.Build(current ? Cache.Current.PhraseStream : Cache.Valid.PhraseStream);
            PlayerLoader<ChannelMessage>.Build(current ? Cache.Current.PlayerStream : Cache.Valid.PlayerStream).ForEach(x => {
                x.Song = song; // Song データを設定
                x.Build(tick, smf); // MIDI データを構築
            });
        }

        /// <summary>
        /// ソングをSMFに変換して出力
        /// TODO: 曲再生を止める
        /// </summary>
        async Task<bool> convertSong() {
            return await Task.Run(async () => {
                // 進捗表示用タイマー
                var message = "PLEASE WAIT";
                var timer = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                var disposer = timer.Subscribe(x => {
                    Log.Info($"converting the song.. ({x})");
                    Invoke((MethodInvoker) (() => {
                        var _dot = (x % 2) == 0 ? "*" : "-";
                        _textBoxSongName.Text = $"{message} {_dot}";
                    }));
                });
                // MIDI データ生成
                Midi.Message.Clear();
                var songName = await buildSong(true);
                var songDir = _targetPath.Split(Path.DirectorySeparatorChar).Last();
                Midi.Message.Invert(); // データ生成後にフラグ反転
                // 曲情報設定
                var conductorTrack = new Track();
                conductorTrack.Insert(0, new MetaMessage(MetaType.Tempo, Value.Converter.ToByteTempo(State.Tempo)));
                conductorTrack.Insert(0, new MetaMessage(MetaType.TrackName, Value.Converter.ToByteArray(State.Name)));
                conductorTrack.Insert(0, new MetaMessage(MetaType.Copyright, Value.Converter.ToByteArray(State.Copyright)));
                State.TrackList.ForEach(x => {
                    var chTrack = Multi.Get(x.MidiCh);
                    chTrack.Insert(0, new MetaMessage(MetaType.TrackName, Value.Converter.ToByteArray(x.Name)));
                    chTrack.Insert(0, new MetaMessage(MetaType.ProgramName, Value.Converter.ToByteArray(x.Instrument))); // TODO: 反映されない？
                });
                // MIDI データ適用
                for (var idx = 0; Midi.Message.Has(idx * 30); idx++) { // tick を 30間隔でループさせます
                    var tick = idx * 30; // 30 tick を手動生成
                    var list = Midi.Message.GetBy(tick); // メッセージのリストを取得
                    if (list != null) {
                        list.ForEach(x => Multi.Get(x.MidiChannel).Insert(tick, x));
                    }
                }
                // SMF ファイル書き出し
                _sequence.Load("./data/conductor.mid"); // TODO: 必要？
                _sequence.Clear();
                _sequence.Format = 1;
                _sequence.Add(conductorTrack);
                Multi.List.Where(x => x.Length > 1).ToList().ForEach(x => _sequence.Add(x));
                _sequence.Save($"./data/{songDir}/{songName}.mid");
                Invoke((MethodInvoker) (() => _textBoxSongName.Text = songName));// Song 名を戻す
                disposer.Dispose(); // タイマー破棄
                Log.Info("save! :D");
                return true;
            });
        }

        /// <summary>
        /// 演奏開始
        /// </summary>
        async Task<bool> startSound() {
            return await Task.Run(async () => {
                Midi.Message.Clear();
                _textBoxSongName.Text = await buildSong();
                Facade.CreateConductor(_sequence);
                _sequence.Load("./data/conductor.mid");
                _sequencer.Position = 0;
                _sequencer.Start();
                _labelPlay.ForeColor = Color.Lime;
                Sound.Playing = true;
                Sound.Played = false;
                Log.Info("start! :D");
                return true;
            });
        }

        /// <summary>
        /// 演奏停止
        /// </summary>
        async Task<bool> stopSound() {
            return await Task.Run(() => {
                Sound.Stopping = true;
                Enumerable.Range(0, 16).ToList().ForEach(
                    x => _midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, x, 120))
                );
                Sound.Stopping = false;
                Sound.Playing = false;
                _sequencer.Stop();
                _sequence.Clear();
                Invoke(resetDisplay());
                Log.Info("stop. :|");
                return true;
            });
        }

        /// <summary>
        /// UI表示を更新します
        /// </summary>
        MethodInvoker updateDisplay(State.Item16beat item) {
            return () => {
                _textBoxBeat.Text = State.Beat.ToString();
                _textBoxMeas.Text = State.Meas.ToString();
                _textBoxKey.Text = item.Key;
                _textBoxDegree.Text = item.Degree;
                _textBoxKeyMode.Text = item.KeyMode;
                _textBoxCode.Text = Utils.ToSimpleCodeName(
                    Key.Enum.Parse(item.Key),
                    Degree.Enum.Parse(item.Degree),
                    Mode.Enum.Parse(item.KeyMode),
                    Mode.Enum.Parse(item.SpanMode),
                    item.AutoMode
                );
                if (item.AutoMode) { // 自動旋法適用の場合
                    var autoMode = Utils.ToModeSpan(
                        Degree.Enum.Parse(item.Degree),
                        Mode.Enum.Parse(item.KeyMode)
                    );
                    _textBoxMode.Text = autoMode.ToString();
                    _labelModulation.ForeColor = Color.DimGray;
                } else { // Spanの旋法適用の場合
                    _textBoxMode.Text = item.SpanMode;
                    var keyMode = Utils.ToModeKey(
                        Key.Enum.Parse(item.Key),
                        Degree.Enum.Parse(item.Degree),
                        Mode.Enum.Parse(item.KeyMode),
                        Mode.Enum.Parse(item.SpanMode)
                    );
                    _textBoxKeyMode.Text = keyMode.ToString().Equals("Undefined") ? "---" : keyMode.ToString();
                    _labelModulation.ForeColor = Color.HotPink; // TODO: 度合によって色変化
                }
            };
        }

        /// <summary>
        /// UI表示を初期化します
        /// </summary>
        MethodInvoker resetDisplay() {
            return () => {
                Enumerable.Range(0, 88).ToList().ForEach(
                    x => _pianoControl.Send(new ChannelMessage(ChannelCommand.NoteOff, 1, x, 0))
                );
                _labelPlay.ForeColor = Color.DimGray;
                _labelModulation.ForeColor = Color.DimGray;
                _textBoxBeat.Text = "0";
                _textBoxMeas.Text = "0";
                _textBoxKey.Text = "---";
                _textBoxDegree.Text = "---";
                _textBoxKeyMode.Text = "---";
                _textBoxMode.Text = "---";
                _textBoxCode.Text = "---";
            };
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// 処理用のフロントクラス
        /// </summary>
        static class Facade {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public static Methods [verb]

            /// <summary>
            /// テンポ制御用の SMF ファイルを作成して出力します
            /// </summary>
            public static void CreateConductor(Sequence sequence) {
                var tempo = new MetaMessage(MetaType.Tempo, Value.Converter.ToByteTempo(State.Tempo));
                var track = new Track();
                track.Insert(0, tempo);
                for (var idx = 0; idx < 100000; idx++) { // tick を 30間隔でループさせます // TODO: ループ回数
                    var tick = idx * 30; // 30 tick を手動生成
                    track.Insert(tick, new ChannelMessage(ChannelCommand.NoteOn, 0, 64, 0));
                    track.Insert(tick + 30, new ChannelMessage(ChannelCommand.NoteOff, 0, 64, 0));
                }
                sequence.Load("./data/conductor.mid"); // TODO: 必要？
                sequence.Clear();
                sequence.Add(track);
                sequence.Save($"./data/conductor.mid"); // テンポ制御用 SMF ファイル書き出し
            }
        }

        /// <summary>
        /// FormMain オブジェクトの状態を保持するクラス
        /// </summary>
        static class Sound {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            static bool _playing;

            static bool _played;

            static bool _stopping;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Sound() {
                _playing = false;
                _played = false;
                _stopping = false;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            // TODO: フラグを同時に判定操作する

            public static bool Playing { // TODO: block ?
                get => _playing;
                set => _playing = value;
            }

            public static bool Played { // TODO: block ?
                get => _played;
                set => _played = value;
            }

            public static bool Stopping { // TODO: block ?
                get => _stopping;
                set => _stopping = value;
            }
        }
    }
}
