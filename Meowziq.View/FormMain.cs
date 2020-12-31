
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

using Meowziq.Core;
using Meowziq.Player;

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

            // 4小節のパターン
            var _patt1 = new Pattern(new List<Meas>() {
                new Meas(new List<Span>() { new Span(4, Degree.I, Mode.Mix) }),
                new Meas(new List<Span>() { new Span(4, Degree.IV, Mode.Mix) }),
                new Meas(new List<Span>() { new Span(4, Degree.VI) }),
                new Meas(new List<Span>() { new Span(4, Degree.VII) })
            });

            song = new Song(Key.C, Mode.Aeo);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);
            song.Add(_patt1);

            // 暫定
            message = new Message();

            // プレイヤーが build するべき
            var _drums = new DrumPlayer();
            _drums.Song = song;
            _drums.MidiCh = MidiChannel.ch10;
            _drums.Build(message);

            var _bass = new BassPlayer();
            _bass.Song = song;
            _bass.MidiCh = MidiChannel.ch1;
            _bass.Program = Instrument.Synth_Bass_1;
            _bass.Build(message);

            var _sequence = new SequencePlayer();
            _sequence.Song = song;
            _sequence.MidiCh = MidiChannel.ch2;
            _sequence.Program = Instrument.Lead_1_square;
            _sequence.Build(message);

            var _pad = new PadPlayer();
            _pad.Song = song;
            _pad.MidiCh = MidiChannel.ch3;
            _pad.Program = Instrument.Pad_3_polysynth;
            _pad.Build(message);
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
