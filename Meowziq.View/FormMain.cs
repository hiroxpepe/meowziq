
using System;
using System.Drawing;
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
                Invoke((MethodInvoker) (() => {
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

        /// <summary>
        /// 演奏を開始します
        /// </summary>
        void buttonPlay_Click(object sender, EventArgs e) {
            try {
                if (textBoxSongName.Text.Equals("------------")) {
                    MessageBox.Show("please load the song.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
            }
        }

        /// <summary>
        /// データをロードします
        /// </summary>
        void buttonLoad_Click(object sender, EventArgs e) {
            try {
                folderBrowserDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                var _dr = folderBrowserDialog.ShowDialog();
                if (_dr == DialogResult.OK) {
                    targetPath = folderBrowserDialog.SelectedPath;
                    textBoxSongName.Text = buildSong(targetPath);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// ソングを作成
        /// </summary>
        string buildSong(string targetDir) {
            // Pattern と Song をロード
            SongLoader.PatternList = PatternLoader.Build($"{targetDir}/pattern.json");
            var _song = SongLoader.Build($"{targetDir}/song.json");

            // Phrase と Player をロード
            PlayerLoader.PhraseList = PhraseLoader.Build($"{targetDir}/phrase.json");
            PlayerLoader.Build($"{targetDir}/player.json").ForEach(x => {
                x.Song = _song; // Song データを設定
                x.Build(0); // MIDI データを構築
            });
            // Song の名前を返す
            return _song.Name;
        }

        /// <summary>
        /// ソングをロード
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
            });
        }

        /// <summary>
        /// 演奏開始
        /// </summary>
        async void startSound() {
            await Task.Run(() => {
                Message.Reset();
                textBoxSongName.Text = buildSong(targetPath); // TODO: リロード
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
        MethodInvoker updateDisplay(Info.Item _item) {
            return () => {
                textBoxKey.Text = _item.Key;
                textBoxDegree.Text = _item.Degree;
                textBoxKeyMode.Text = _item.KeyMode;
                textBoxCode.Text = Utils.GetSimpleCodeName(
                    Key.Enum.Parse(_item.Key),
                    Degree.Enum.Parse(_item.Degree),
                    Mode.Enum.Parse(_item.KeyMode),
                    Mode.Enum.Parse(_item.SpanMode)
                );
                if (_item.KeyMode == _item.SpanMode) { // 自動旋法適用の場合
                    var _autoMode = Utils.GetModeBy(Degree.Enum.Parse(_item.Degree), Mode.Enum.Parse(_item.KeyMode));
                    textBoxAutoMode.Text = _autoMode.ToString();
                    textBoxAutoMode.BackColor = Color.PaleGreen;
                    textBoxSpanMode.Text = "---";
                    textBoxSpanMode.BackColor = Color.DarkOliveGreen;
                } else { // Spanの旋法適用の場合
                         // TODO: キーの転旋法表示？
                    textBoxAutoMode.Text = "---";
                    textBoxAutoMode.BackColor = Color.DarkOliveGreen;
                    textBoxSpanMode.Text = _item.SpanMode;
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
