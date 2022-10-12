
using System;
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

using Meowziq.Core;

namespace Meowziq.Midi {
    /// <summary>
    /// create a concrete object for the IMessage interface.
    /// </summary>
    /// <memo>
    /// static class cannot implement interface.<br/>
    /// </memo>
    public static class MessageFactory {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static IMessage<ChannelMessage, Note> CreateMessage() {
            return new Message();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        public class Message : IMessage<ChannelMessage, Note> {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public static Methods [verb]

            public void ApplyTick(int tick, Action<int> load) {
                Midi.Message.ApplyTick(tick, load);
            }

            public void ApplyNote(int tick, int midi_ch, Note note) {
                Midi.Message.ApplyNote(tick, midi_ch, note);
            }

            public void ApplyProgramChange(int tick, int midi_ch, int program_num) {
                Midi.Message.ApplyProgramChange(tick, midi_ch, program_num);
            }

            public void ApplyVolume(int tick, int midi_ch, int volume) {
                Midi.Message.ApplyVolume(tick, midi_ch, volume);
            }

            public void ApplyPan(int tick, int midi_ch, Pan pan) {
                Midi.Message.ApplyPan(tick, midi_ch, pan);
            }

            public void ApplyMute(int tick, int midi_ch, bool mute) {
                Midi.Message.ApplyMute(tick, midi_ch, mute);
            }

            public void Clear() {
                Midi.Message.Clear();
            }

            public List<ChannelMessage> GetBy(int tick) {
                return Midi.Message.GetBy(tick);
            }

            public bool Has(int tick) {
                return Midi.Message.Has(tick);
            }

            public void Invert() {
                Midi.Message.Invert();
            }
        }
    }
}
