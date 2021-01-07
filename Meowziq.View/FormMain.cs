
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

using Meowziq.Core;
using Meowziq.Loader;

namespace Meowziq.View {
    public partial class FormMain : Form {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Midi midi;

        Message message;

        Song song;

        string targetDir;

        bool playing = false;

        bool stopping = false;

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

                var _list = message.GetBy(sequencer.Position);
                if (_list != null) {
                    _list.ForEach(message => {
                        midi.OutDevice.Send(message); // MIDIデバイスにメッセージを追加送信
                        if (message.MidiChannel != 9) {
                            pianoControl.Send(message); // ドラム以外はピアノロールに表示
                        }
                    });
                }
            }
        }

        void handleStopped(object sender, StoppedEventArgs e) {
            // NOTE: default.midi のメッセージはなし
            allSoundOff();
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
                buildSong();
                sequence.Load("./data/default.mid");
                sequencer.Position = 0;
                sequencer.Start();
                labelPlay.ForeColor = Color.Lime;
                playing = true;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// 演奏を停止します
        /// </summary>
        void buttonStop_Click(object sender, EventArgs e) {
            try {
                if (stopping) {
                    return;
                }
                stopping = true;
                sequencer.Stop();
                sequence.Clear();
                labelPlay.ForeColor = Color.DimGray;
                textBoxBeat.Text = "0";
                textBoxMeas.Text = "0";
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
                    targetDir = folderBrowserDialog.SelectedPath;
                    loadSongName();
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
        void buildSong() {
            // Message 生成
            message = new Message();

            // Pattern をロード
            var _patternLoader = new PatternLoader($"{targetDir}/pattern.json");
            var _patternList = _patternLoader.BuildPatternList();

            // Song をロード
            var _songLoader = new SongLoader($"{targetDir}/song.json");
            _songLoader.PatternList = _patternList; // Song に Pattern のリストを渡す
            song = _songLoader.BuildSong();

            // Phrase をロード
            var _phraseLoader = new PhraseLoader($"{targetDir}/phrase.json");
            var _phraseList = _phraseLoader.BuildPhraseList();

            // Player をロード
            var _playerLoader = new PlayerLoader($"{targetDir}/player.json");
            _playerLoader.PhraseList = _phraseList; // PlayerLoader に Phrase のリストを渡す
            var _playerList = _playerLoader.BuildPlayerList();
            foreach (var _player in _playerList) {
                _player.Song = song; // Song データを設定
                _player.Build(message); // MIDI データを構築
            }
        }

        /// <summary>
        /// ソングの名前を読み込みます
        /// </summary>
        void loadSongName() {
            // Song を一時的にロード
            var _songLoader = new SongLoader($"{targetDir}/song.json");
            textBoxSongName.Text = _songLoader.GetSongName();
        }

        /// <summary>
        /// オールサウンドオフ
        /// </summary>
        async void allSoundOff() {
            // FIXME: ノートOFF出来ずハングするバグ
            await Task.Run(() => {
                for (int _i = 0; _i < 16; _i++) {
                    midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, _i, 120));
                    Thread.Sleep(24);
                }
                stopping = false;
                playing = false;
            });
        }
    }
}
