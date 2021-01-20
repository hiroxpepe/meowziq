
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

using Meowziq.Loader;

namespace Meowziq.View {
    /// <summary>
    /// TODO: 排他制御
    /// MEMO: 半動的生成で MIDI ノートを出力し MIDI 楽器の音色をリアルタイムで変化させて楽しむ 
    /// </summary>
    public partial class FormMain : Form {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static object locked = new object();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Midi midi;

        string targetPath;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public FormMain() {
            InitializeComponent();

            // MIDIデバイス準備
            midi = new Midi();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        /// <summary>
        /// 演奏を開始します
        /// </summary>
        void buttonPlay_Click(object sender, EventArgs e) {
            try {
                if (textBoxSongName.Text.Equals("------------")) {
                    var _message = "please load a song.";
                    MessageBox.Show(_message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Log.Error(_message);
                    return;
                }
                if (Sound.Playing || Sound.Stopping) {
                    return;
                }
                lock (locked) {
                    startSound();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// 演奏を停止します
        /// </summary>
        void buttonStop_Click(object sender, EventArgs e) {
            try {
                if (Sound.Stopping || !Sound.Played) {
                    return;
                }
                lock (locked) {
                    stopSound();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// データをロードします
        /// </summary>
        async void buttonLoad_Click(object sender, EventArgs e) {
            try {
                folderBrowserDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                var _dr = folderBrowserDialog.ShowDialog();
                if (_dr == DialogResult.OK) {
                    targetPath = folderBrowserDialog.SelectedPath;
                    textBoxSongName.Text = await buildSong();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// データをSMFに変換します
        /// </summary>
        async void buttonConvert_Click(object sender, EventArgs e) {
            try {
                if (textBoxSongName.Text.Equals("------------")) {
                    var _message = "please load a song.";
                    MessageBox.Show(_message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Log.Error(_message);
                    return;
                }
                if (await convertSong()) {
                    MessageBox.Show("converted the song to SMF.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
            }
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
            if (this.Visible) {
                // TODO: カウント分はUIではマイナス表示とする？
                var _tick = sequencer.Position - 1; // NOTE: Position が 1, 31 と来るので予め1引く
                State.Beat = ((_tick / 480) + 1); // 0開始 ではなく 1開始として表示
                State.Meas = ((State.Beat - 1) / 4 + 1);
                // UI情報更新
                var _dictionary = State.Dictionary;
                if (_dictionary.ContainsKey(_tick)) { // FIXME: ContainsKey 大丈夫？
                    var _item = _dictionary[_tick];
                    Invoke(updateDisplay(_item));
                }
                // MIDIメッセージ処理
                Message.Apply(_tick, loadSong); // 1小節ごとに切り替える MEMO: シンコぺを考慮する
                var _list = Message.GetBy(_tick); // MIDIメッセージのリストを取得
                if (_list != null) {
                    _list.ForEach(x => {
                        midi.OutDevice.Send(x); // MIDIデバイスにメッセージを追加送信 MEMO: CCなどは直接ここで投げては？
                        if (x.MidiChannel != 9 && x.MidiChannel != 1) { // FIXME: 暫定:シーケンス除外
                            pianoControl.Send(x); // ドラム以外はピアノロールに表示
                        }
                    });
                }
                Sound.Played = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// ソングをロード
        /// </summary>
        async Task<string> buildSong(bool save = false) {
            var _name = "------------";
            await Task.Run(() => {
                SongLoader.PatternList = PatternLoader.Build($"{targetPath}/pattern.json"); // Pattern と Song をロード
                var _song = SongLoader.Build($"{targetPath}/song.json");
                PlayerLoader.PhraseList = PhraseLoader.Build($"{targetPath}/phrase.json"); // Phrase と Player をロード
                PlayerLoader.Build($"{targetPath}/player.json").ForEach(x => {
                    x.Song = _song; // Song データを設定
                    x.Build(0, save); // MIDI データを構築
                });
                _name = _song.Name;
                Log.Info("load! :)");
            });
            return _name; // Song の名前を返す
        }

        /// <summary>
        /// ソングをロード
        /// NOTE: Message クラスから呼ばれます
        /// </summary>
        async void loadSong(int tick) {
            await Task.Run(() => {
                SongLoader.PatternList = PatternLoader.Build($"{targetPath}/pattern.json"); // Pattern と Song をロード
                var _song = SongLoader.Build($"{targetPath}/song.json");
                PlayerLoader.PhraseList = PhraseLoader.Build($"{targetPath}/phrase.json"); // Phrase と Player をロード
                PlayerLoader.Build($"{targetPath}/player.json").ForEach(x => {
                    x.Song = _song; // Song データを設定
                    x.Build(tick); // MIDI データを構築
                });
                Log.Info("load! :)");
            });
        }

        /// <summary>
        /// ソングをSMFに変換して出力
        /// TODO: 曲再生を止める
        /// </summary>
        async Task<bool> convertSong() {
            return await Task.Run(async () => {
                // 進捗表示用タイマー
                var _message = "PLEASE WAIT";
                var _timer = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                var _disposer = _timer.Subscribe(x => {
                    Log.Info($"converting the song.. ({x})");
                    Invoke((MethodInvoker) (() => {
                        var _dot = (x % 2) == 0 ? "*" : "-";
                        textBoxSongName.Text = $"{_message} {_dot}";
                    }));
                });
                // テンポ追加
                byte[] _data = new byte[3]{ // TODO: 120BPM 暫定
                    Convert.ToByte("07", 16),
                    Convert.ToByte("A1", 16),
                    Convert.ToByte("20", 16) 
                };
                var _tempo = new MetaMessage(MetaType.Tempo, _data);
                var _track = new Track();
                _track.Insert(0, _tempo);
                // MIDI データ生成
                Message.Clear();
                var _songName = await buildSong(true);
                var _songDir = targetPath.Split(Path.DirectorySeparatorChar).Last();
                Message.Invert(); // データ生成後にフラグ反転
                for (var _idx = 0; Message.Has(_idx); _idx++) { // tick を 30間隔でループさせます
                    var _tick = _idx * 30; // 30 tick を手動生成
                    var _list = Message.GetBy(_tick); // メッセージのリストを取得
                    if (_list != null) {
                        _list.ForEach(x => _track.Insert(_tick, x));
                    }
                }
                // SMF ファイル書き出し
                sequence.Load("./data/conductor.mid");
                sequence.Clear();
                sequence.Add(_track);
                sequence.Save($"./data/{_songDir}/{_songName}.mid");
                Invoke((MethodInvoker) (() => textBoxSongName.Text = _songName));// Song 名戻す
                _disposer.Dispose(); // タイマー破棄
                Log.Info("save! :D");
                return true;
            });
        }

        /// <summary>
        /// 演奏開始
        /// </summary>
        async void startSound() {
            await Task.Run(async () => {
                Message.Clear();
                textBoxSongName.Text = await buildSong();
                sequence.Load("./data/conductor.mid");
                sequencer.Position = 0;
                sequencer.Start();
                labelPlay.ForeColor = Color.Lime;
                Sound.Playing = true;
                Sound.Played = false;
                Log.Info("start! :D");
            });
        }

        /// <summary>
        /// 演奏停止
        /// </summary>
        async void stopSound() {
            await Task.Run(() => {
                Sound.Stopping = true;
                for (int _idx = 0; _idx < 15; _idx++) { // All sound off.
                    midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, _idx, 120));
                }
                Sound.Stopping = false;
                Sound.Playing = false;
                sequencer.Stop();
                sequence.Clear();
                Invoke(resetDisplay());
                Log.Info("stop. :|");
            });
        }

        /// <summary>
        /// UI表示を更新します
        /// </summary>
        MethodInvoker updateDisplay(State.Item16beat item) {
            return () => {
                textBoxBeat.Text = State.Beat.ToString();
                textBoxMeas.Text = State.Meas.ToString();
                textBoxKey.Text = item.Key;
                textBoxDegree.Text = item.Degree;
                textBoxKeyMode.Text = item.KeyMode;
                textBoxCode.Text = Utils.ToSimpleCodeName(
                    Key.Enum.Parse(item.Key),
                    Degree.Enum.Parse(item.Degree),
                    Mode.Enum.Parse(item.KeyMode),
                    Mode.Enum.Parse(item.SpanMode),
                    item.AutoMode
                );
                if (item.AutoMode) { // 自動旋法適用の場合
                    var _autoMode = Utils.ToModeSpan(
                        Degree.Enum.Parse(item.Degree),
                        Mode.Enum.Parse(item.KeyMode)
                    );
                    textBoxMode.Text = _autoMode.ToString();
                    labelModulation.ForeColor = Color.DimGray;
                } else { // Spanの旋法適用の場合
                    textBoxMode.Text = item.SpanMode;
                    var _keyMode = Utils.ToModeKey(
                        Key.Enum.Parse(item.Key),
                        Degree.Enum.Parse(item.Degree),
                        Mode.Enum.Parse(item.KeyMode),
                        Mode.Enum.Parse(item.SpanMode)
                    );
                    textBoxKeyMode.Text = _keyMode.ToString().Equals("Undefined") ? "---" : _keyMode.ToString();
                    labelModulation.ForeColor = Color.HotPink; // TODO: 度合によって色変化
                }
            };
        }

        /// <summary>
        /// UI表示を初期化します
        /// </summary>
        MethodInvoker resetDisplay() {
            return () => {
                labelPlay.ForeColor = Color.DimGray;
                labelModulation.ForeColor = Color.DimGray;
                textBoxBeat.Text = "0";
                textBoxMeas.Text = "0";
                textBoxKey.Text = "---";
                textBoxDegree.Text = "---";
                textBoxKeyMode.Text = "---";
                textBoxMode.Text = "---";
                textBoxCode.Text = "---";
            };
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// FormMain オブジェクトの状態を保持するクラス
        /// </summary>
        static class Sound {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            static bool playing;

            static bool played;

            static bool stopping;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Sound() {
                playing = false;
                played = false;
                stopping = false;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            // TODO: フラグを同時に判定操作する

            public static bool Playing { // TODO: block ?
                get => playing;
                set => playing = value;
            }

            public static bool Played { // TODO: block ?
                get => played;
                set => played = value;
            }

            public static bool Stopping { // TODO: block ?
                get => stopping;
                set => stopping = value;
            }
        }
    }
}
