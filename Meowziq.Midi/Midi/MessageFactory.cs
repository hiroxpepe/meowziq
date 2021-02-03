
using System;
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

using Meowziq.Core;

namespace Meowziq.Midi {
    /// <summary>
    /// IMessage インタフェースに対して具象オブジェクトを返します
    /// MEMO: static クラスは インターフェィスを実装出来ません
    /// </summary>
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

            public void ApplyNote(int tick, int midiCh, Note note) {
                Midi.Message.ApplyNote(tick, midiCh, note);
            }

            public void ApplyProgramChange(int tick, int midiCh, int programNum) {
                Midi.Message.ApplyProgramChange(tick, midiCh, programNum);
            }

            public void ApplyVolume(int tick, int midiCh, int volume) {
                Midi.Message.ApplyVolume(tick, midiCh, volume);
            }

            public void ApplyPan(int tick, int midiCh, Pan pan) {
                Midi.Message.ApplyPan(tick, midiCh, pan);
            }

            public void ApplyMute(int tick, int midiCh, bool mute) {
                Midi.Message.ApplyMute(tick, midiCh, mute);
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
