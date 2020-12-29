
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

using Meowzic.Core;

namespace Meowzic.View {
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

            // ソングを作成
            song = new Song(Key.A, Mode.Aeo);
            song.Add(new Part(4, Degree.I));
            song.Add(new Part(4, Degree.IV));
            song.Add(new Part(4, Degree.VII));
            song.Add(new Part(4, Degree.I));

            // プレイヤーが build するべき
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        /// <summary>
        /// MEMO: メッセージ送信のタイミングは独自実装出来るのでは？
        /// </summary>
        void handleChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
            midi.outDevice.Send(e.Message);

            if (this.Visible) {
                this.Invoke((MethodInvoker) (
                    () => textBoxBeat.Text = ((sequencer.Position / 480) + 1).ToString() // 0開始 ではなく 1開始として表示
                ));

                List<ChannelMessage> _list = message.GetBy(sequencer.Position);
                if (_list != null) {
                    _list.ForEach(message => {
                        midi.outDevice.Send(message); // MIDIデバイスにメッセージを追加送信
                    });
                }
            }
        }

        void buttonPlay_Click(object sender, EventArgs e) {
            try {
                createMessage(song);
                sequence.Load("default.mid");
                sequencer.Position = 0;
                sequencer.Start();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        void buttonStop_Click(object sender, EventArgs e) {
            try {
                sequencer.Stop();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// メッセージストリームを作成します
        /// </summary>
        void createMessage(Song song) {
            message = new Message(song);
        }

    }
}
