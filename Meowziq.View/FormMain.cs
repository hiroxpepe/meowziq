
using System;
using System.Collections.Generic;
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
            // MEMO: 30 tick単位でしか呼ばれていない
            if (this.Visible) {
                this.Invoke((MethodInvoker) (
                    () => textBoxBeat.Text = ((sequencer.Position / 480) + 1).ToString() // 0開始 ではなく 1開始として表示
                ));

                List<ChannelMessage> _list = message.GetBy(sequencer.Position);
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

        void buttonPlay_Click(object sender, EventArgs e) {
            try {
                buildSong();
                sequence.Load("./data/default.mid");
                sequencer.Position = 0;
                sequencer.Start();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        void buttonStop_Click(object sender, EventArgs e) {
            try {
                sequencer.Stop();
                allSoundOff();
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
            var _patternLoader = new PatternLoader(@"./data/pattern.jsonc");
            var _patternList = _patternLoader.BuildPatternList();

            // Song をロード
            var _songLoader = new SongLoader(@"./data/song.jsonc");
            _songLoader.PatternList = _patternList; // Song に Pattern のリストを渡す
            song = _songLoader.BuildSong();

            // Phrase をロード
            var _phraseLoader = new PhraseLoader(@"./data/phrase.jsonc");
            var _phraseList = _phraseLoader.BuildPhraseList();

            // Player をロード
            var _playerLoader = new PlayerLoader(@"./data/player.jsonc");
            _playerLoader.PhraseList = _phraseList; // PlayerLoader に Phrase のリストを渡す
            var _playerList = _playerLoader.BuildPlayerList();
            foreach (var _player in _playerList) {
                _player.Song = song; // Song データを設定
                _player.Build(message); // MIDI データを構築
            }
        }

        /// <summary>
        /// オールサウンドオフ
        /// </summary>
        void allSoundOff() {
            for (int i = 0; i < 16; i++) {
                midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, i, 120));
            }
        }
    }
}
