
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
    /// </summary>
    public partial class FormMain : Form {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static bool playing = false;

        static bool played = false;

        static bool stopping = false;

        static object locked = new object();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Midi midi;

        string targetPath;

        Track track = new Track();

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
                if (playing || stopping) {
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
                if (stopping || !played) {
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
                if (await saveSong()) {
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
        void handleChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
            if (stopping) {
                return;
            }
            if (this.Visible) {
                var _tick = sequencer.Position - 1; // NOTE: Position が 1, 31 と来るので予め1引く
                var _beat = (((_tick) / 480) + 1).ToString(); // 0開始 ではなく 1開始として表示
                var _meas = ((int.Parse(_beat) - 1) / 4 + 1).ToString();
                Invoke((MethodInvoker) (() => { // TODO: Infoに格納してから後で表示
                    textBoxBeat.Text = _beat;
                    textBoxMeas.Text = _meas;
                }));
                // UI情報表示
                var _itemDictionary = Info.ItemDictionary;
                if (_itemDictionary.ContainsKey(_tick)) { // FIXME: ContainsKey 大丈夫？
                    var _item = _itemDictionary[_tick];
                    Invoke(updateDisplay(_item));
                }
                Message.Apply(_tick, loadSong); // 1小節ごとに切り替える // MEMO: シンコぺを考慮する
                var _list = Message.GetBy(_tick); // メッセージのリストを取得
                if (_list != null) {
                    _list.ForEach(x => {
                        midi.OutDevice.Send(x); // MIDIデバイスにメッセージを追加送信 MEMO: CCなどは直接ここで投げては？
                        if (x.MidiChannel != 9 && x.MidiChannel != 1) { // FIXME: 暫定シーケンス
                            pianoControl.Send(x); // ドラム以外はピアノロールに表示
                        }
                        track.Insert(_tick, x); // TODO: 静的生成にする
                    });
                }
                played = true;
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
                // Pattern と Song をロード
                SongLoader.PatternList = PatternLoader.Build($"{targetPath}/pattern.json");
                var _song = SongLoader.Build($"{targetPath}/song.json");

                // Phrase と Player をロード
                PlayerLoader.PhraseList = PhraseLoader.Build($"{targetPath}/phrase.json");
                PlayerLoader.Build($"{targetPath}/player.json").ForEach(x => {
                    x.Song = _song; // Song データを設定
                    x.Build(0, save); // MIDI データを構築
                });
                _name = _song.Name;
                Log.Info("load! :)");
            });
            // Song の名前を返す
            return _name;
        }

        /// <summary>
        /// ソングをロード
        /// NOTE: Message クラスから呼ばれます
        /// </summary>
        async void loadSong(int tick) {
            await Task.Run(() => {
                // Pattern と Song をロード
                SongLoader.PatternList = PatternLoader.Build($"{targetPath}/pattern.json");
                var _song = SongLoader.Build($"{targetPath}/song.json");

                // Phrase と Player をロード
                PlayerLoader.PhraseList = PhraseLoader.Build($"{targetPath}/phrase.json");
                PlayerLoader.Build($"{targetPath}/player.json").ForEach(x => {
                    x.Song = _song; // Song データを設定
                    x.Build(tick); // MIDI データを構築
                });
                Log.Info("load! :)");
            });
        }

        /// <summary>
        /// ソングをセーブ
        /// TODO: 曲再生を止める
        /// </summary>
        async Task<bool> saveSong() {
            await Task.Run(async () => {
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
                // セーブ開始
                Message.Reset();
                var _songName = await buildSong(true);
                var _songDir = targetPath.Split(Path.DirectorySeparatorChar).Last();
                Message.Invert();
                track.Clear();
                byte[] _data = new byte[3]{ // MEMO: 120BPM 暫定
                    Convert.ToByte("07", 16),
                    Convert.ToByte("A1", 16),
                    Convert.ToByte("20", 16) 
                };
                var _tempo = new MetaMessage(MetaType.Tempo, _data);
                track.Insert(0, _tempo);
                for (var _idx = 0; _idx < 9999999; _idx++) { // tick を 30間隔でループさせます // FIXME: どこまで回す？
                    var _tick = _idx * 30; // 30 tick を手動生成
                    var _list = Message.GetBy(_tick); // メッセージのリストを取得
                    if (_list != null) {
                        _list.ForEach(x => {
                            track.Insert(_tick, x);
                        });
                    }
                }
                sequence.Load("./data/conductor.mid");
                sequence.Clear();
                sequence.Add(track);
                sequence.Save($"./data/{_songDir}/{_songName}.mid");
                Invoke((MethodInvoker) (() => textBoxSongName.Text = _songName));// Song 名戻す
                _disposer.Dispose(); // タイマー破棄
                Log.Info("save! :D");
                return true;
            });
            return true; // TODO: 戻り値
        }

        /// <summary>
        /// 演奏開始
        /// </summary>
        async void startSound() {
            await Task.Run(async () => {
                Message.Reset();
                textBoxSongName.Text = await buildSong();
                sequence.Load("./data/conductor.mid");
                sequencer.Position = 0;
                sequencer.Start();
                labelPlay.ForeColor = Color.Lime;
                playing = true;
                played = false;
                Log.Info("start! :D");
            });
        }

        /// <summary>
        /// 演奏停止
        /// </summary>
        async void stopSound() {
            await Task.Run(() => {
                stopping = true;
                for (int _idx = 0; _idx < 15; _idx++) {
                    midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, _idx, 120)); // All sound off.
                }
                stopping = false;
                playing = false;
                sequencer.Stop();
                sequence.Clear();
                sequence.Add(track);
                sequence.Save("./data/out.mid"); // TODO: songのディレクトリにsongの名前で
                Invoke(resetDisplay());
                Log.Info("stop. :|");
            });
        }

        /// <summary>
        /// UI表示を更新します
        /// </summary>
        MethodInvoker updateDisplay(Info.Item item) {
            return () => {
                textBoxKey.Text = item.Key;
                textBoxDegree.Text = item.Degree;
                textBoxKeyMode.Text = item.KeyMode;
                textBoxCode.Text = Utils.GetSimpleCodeName(
                    Key.Enum.Parse(item.Key),
                    Degree.Enum.Parse(item.Degree),
                    Mode.Enum.Parse(item.KeyMode),
                    Mode.Enum.Parse(item.SpanMode)
                );
                if (item.KeyMode == item.SpanMode) { // 自動旋法適用の場合
                    var _autoMode = Utils.GetModeBy(Degree.Enum.Parse(item.Degree), Mode.Enum.Parse(item.KeyMode));
                    textBoxAutoMode.Text = _autoMode.ToString();
                    textBoxAutoMode.BackColor = Color.PaleGreen;
                    textBoxSpanMode.Text = "---";
                    textBoxSpanMode.BackColor = Color.DarkOliveGreen;
                } else { // Spanの旋法適用の場合
                         // TODO: キーの転旋法表示？
                    textBoxAutoMode.Text = "---";
                    textBoxAutoMode.BackColor = Color.DarkOliveGreen;
                    textBoxSpanMode.Text = item.SpanMode;
                    textBoxSpanMode.BackColor = Color.PaleGreen;
                }
            };
        }

        /// <summary>
        /// UI表示を初期化します
        /// </summary>
        MethodInvoker resetDisplay() {
            return () => {
                labelPlay.ForeColor = Color.DimGray;
                textBoxBeat.Text = "0";
                textBoxMeas.Text = "0";
                textBoxKey.Text = "---";
                textBoxDegree.Text = "---";
                textBoxKeyMode.Text = "---";
                textBoxSpanMode.Text = "---";
                textBoxAutoMode.Text = "---";
                textBoxCode.Text = "---";
                textBoxAutoMode.BackColor = Color.DarkOliveGreen;
                textBoxSpanMode.BackColor = Color.DarkOliveGreen;
            };
        }
    }
}
