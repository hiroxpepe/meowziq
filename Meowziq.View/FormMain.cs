
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

using Meowziq.Loader;

namespace Meowziq.View {
    public partial class FormMain : Form {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        static bool playing = false;

        static bool played = false;

        static bool stopping = false;

        static object locked = new object();

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
        /// TODO: メッセージ送信のタイミングは独自実装出来るのでは？
        /// </summary>
        void handleChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
            // NOTE: default.midi のメッセージはスルーする  midi.OutDevice.Send(e.Message);
            if (stopping) {
                return;
            }
            // MEMO: 30 tick単位でしか呼ばれていない
            if (this.Visible) {
                var _beat = ((sequencer.Position / 480) + 1).ToString(); // 0開始 ではなく 1開始として表示
                var _meas = ((int.Parse(_beat) - 1) / 4 + 1).ToString();
                Invoke((MethodInvoker) (() => {
                    textBoxBeat.Text = _beat;
                    textBoxMeas.Text = _meas; 
                }));
                // UI情報表示
                var _itemDictionary = Info.ItemDictionary;
                var _tick = sequencer.Position - 1; // 1, 31 と来る
                if (_itemDictionary.ContainsKey(_tick)) {
                    var _item = _itemDictionary[_tick];
                    Invoke((MethodInvoker) (() => {
                        textBoxKey.Text = _item.Key;
                        textBoxDegree.Text = _item.Degree;
                        textBoxKeyMode.Text = _item.KeyMode;
                        textBoxCode.Text = Utils.GetSimpleCodeName(
                            Key.Enum.Parse(_item.Key),
                            Degree.Enum.Parse(_item.Degree), 
                            Mode.Enum.Parse(_item.KeyMode),
                            Mode.Enum.Parse(_item.SpanMode)
                        );
                        if (_item.KeyMode == _item.SpanMode) { // 自動旋法
                            var _autoMode = Utils.GetModeBy(Degree.Enum.Parse(_item.Degree), Mode.Enum.Parse(_item.KeyMode));
                            textBoxAutoMode.Text = _autoMode.ToString();
                            textBoxAutoMode.BackColor = Color.PaleGreen;
                            textBoxSpanMode.Text = "---";
                            textBoxSpanMode.BackColor = Color.DarkOliveGreen;
                        } else {
                            textBoxAutoMode.Text = "---";
                            textBoxAutoMode.BackColor = Color.DarkOliveGreen;
                            textBoxSpanMode.Text = _item.SpanMode;
                            textBoxSpanMode.BackColor = Color.PaleGreen;
                        }
                    }));
                }
                // メッセージのリストを取得
                var _list = Message.GetBy(sequencer.Position);
                if (_list != null) {
                    _list.ForEach(x => {
                        midi.OutDevice.Send(x); // MIDIデバイスにメッセージを追加送信
                        if (x.MidiChannel != 9 && x.MidiChannel != 1) { // FIXME: 暫定シーケンス
                            pianoControl.Send(x); // ドラム以外はピアノロールに表示
                        }
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
                    Message.Reset();
                    Info.Reset();
                    textBoxSongName.Text = buildSong(targetPath); // TODO: リロード
                    sequence.Load("./data/default.mid");
                    sequencer.Position = 0;
                    sequencer.Start();
                    labelPlay.ForeColor = Color.Lime;
                    playing = true;
                    played = false;
                    var _item = Info.ItemDictionary; // DDDDD
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                    stopping = true;
                    stopSound();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                x.Build(); // MIDI データを構築
            });

            // Song の名前を返す
            return _song.Name;
        }

        /// <summary>
        /// オールサウンドオフ
        /// </summary>
        async void stopSound() {
            await Task.Run(() => {
                for (int _idx = 0; _idx < 15; _idx++) {
                    midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, _idx, 120));
                }
                stopping = false;
                playing = false;
                sequencer.Stop();
                Invoke((MethodInvoker) (() => { 
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
                }));
            });
        }
    }
}
