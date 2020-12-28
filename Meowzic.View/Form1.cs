
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

using Meowzic.Core;

namespace Meowzic.View {
    public partial class Form1 : Form {

        Midi midi;

        MessageStream messageStream;

        Song song;

        public Form1() {
            InitializeComponent();

            midi = new Midi();

            // 曲作成
            song = new Song(Key.A, Mode.Aeolian); // キーでの旋法は？
            song.AddPattern(new Pattern(2, Chord.I)); // 2拍 ⇒ 小節にすべき？
            song.AddPattern(new Pattern(2, Chord.VI));
            song.AddPattern(new Pattern(2, Chord.VII));
            song.AddPattern(new Pattern(2, Chord.I));
            // 転調は？
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
            midi.outDevice.Send(e.Message);

            if (this.Visible) {
                this.Invoke((MethodInvoker) (
                    () => textBox1.Text = ((sequencer1.Position / 480) + 1).ToString() // 0開始 ではなく 1開始として表示
                ));

                List<ChannelMessage> _list = messageStream.GetBy(sequencer1.Position);
                if (_list != null) {
                    _list.ForEach(message => {
                        midi.outDevice.Send(message);
                    });
                }
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e) {
            try {
                CreateMessageStream(song);
                sequence1.Load("default.mid");
                sequencer1.Position = 0;
                sequencer1.Start();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void buttonStop_Click(object sender, EventArgs e) {
            try {
                sequencer1.Stop();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void CreateMessageStream(Song song) {
            // メッセージストリーム作成
            messageStream = new MessageStream(song);
        }

    }
}
