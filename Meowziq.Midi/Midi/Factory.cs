
using System;
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

using Meowziq.Core;

namespace Meowziq.Midi {
    /// <summary>
    /// インタフェースに対して具象オブジェクトを返します
    /// </summary>
    public static class Factory {

        public static IMessage<ChannelMessage, Note> CreateMessage() {
            return new Message();
        }

        public class Message : IMessage<ChannelMessage, Note> {
            public void ApplyMute(int midiCh, int tick, bool mute) {
                Meowziq.Midi.Message.ApplyMute(midiCh, tick, mute);
            }

            public void ApplyNote(int midiCh, Note note) {
                Meowziq.Midi.Message.ApplyNote(midiCh, note);
            }

            public void ApplyPan(int midiCh, int tick, Pan pan) {
                Meowziq.Midi.Message.ApplyPan(midiCh, tick, pan);
            }

            public void ApplyProgramChange(int midiCh, int tick, int programNum) {
                Meowziq.Midi.Message.ApplyProgramChange(midiCh, tick, programNum);
            }

            public void ApplyTick(int tick, Action<int> load) {
                Meowziq.Midi.Message.ApplyTick(tick, load);
            }

            public void ApplyVolume(int midiCh, int tick, int volume) {
                Meowziq.Midi.Message.ApplyVolume(midiCh, tick, volume);
            }

            public void Clear() {
                Meowziq.Midi.Message.Clear();
            }

            public List<ChannelMessage> GetBy(int tick) {
                return Meowziq.Midi.Message.GetBy(tick);
            }

            public bool Has(int tick) {
                return Meowziq.Midi.Message.Has(tick);
            }

            public void Invert() {
                Meowziq.Midi.Message.Invert();
            }
        }
    }
}
