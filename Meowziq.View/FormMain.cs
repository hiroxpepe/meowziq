
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
                Invoke((MethodInvoker) (() => textBoxBeat.Text = _beat));
                var _meas = ((int.Parse(_beat) - 1) / 4 + 1).ToString();
                Invoke((MethodInvoker) (() => textBoxMeas.Text = _meas));
                // メッセージのリストを取得
                var _list = Message.GetBy(sequencer.Position);
                if (_list != null) {
                    _list.ForEach(x => {
                        midi.OutDevice.Send(x); // MIDIデバイスにメッセージを追加送信
                        if (x.MidiChannel != 9) {
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
                    textBoxSongName.Text = buildSong(targetPath); // TODO: リロード
                    sequence.Load("./data/default.mid");
                    sequencer.Position = 0;
                    sequencer.Start();
                    labelPlay.ForeColor = Color.Lime;
                    playing = true;
                    played = false;
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
                    allSoundOff();
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
        async void allSoundOff() {
            await Task.Run(() => {
                for (int _idx = 0; _idx < 15; _idx++) {
                    midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, _idx, 120));
                }
                stopping = false;
                playing = false;
                sequencer.Stop();
                Invoke((MethodInvoker) (() => labelPlay.ForeColor = Color.DimGray));
                Invoke((MethodInvoker) (() => textBoxBeat.Text = "0"));
                Invoke((MethodInvoker) (() => textBoxMeas.Text = "0"));
            });
        }
    }
}
