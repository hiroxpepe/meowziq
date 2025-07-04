/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

using Meowziq.Core;

namespace Meowziq.Midi {
    /// <summary>
    /// Provides factory methods for creating IMessage interface implementations for MIDI processing.
    /// </summary>
    /// <remarks>
    /// <item>Static class cannot implement interface directly.</item>
    /// <item>Creates concrete objects for MIDI message handling.</item>
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class MessageFactory {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Creates a new IMessage instance for ChannelMessage and Note.
        /// </summary>
        /// <returns>A new instance of IMessage for ChannelMessage and Note.</returns>
        public static IMessage<ChannelMessage, Note> CreateMessage() {
            return new Message();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Provides an implementation of IMessage for ChannelMessage and Note.
        /// </summary>
        /// <remarks>
        /// <item>Delegates all MIDI message operations to the static Midi.Message class.</item>
        /// </remarks>
        public class Message : IMessage<ChannelMessage, Note> {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            /// <summary>
            /// Applies tick processing using the underlying MIDI message logic.
            /// </summary>
            /// <param name="tick">Tick value to process.</param>
            /// <param name="load">Action to perform for each tick.</param>
            public void ApplyTick(int tick, Action<int> load) {
                Midi.Message.ApplyTick(tick, load);
            }

            /// <summary>
            /// Applies a note using the underlying MIDI message logic.
            /// </summary>
            /// <param name="tick">Tick value to process.</param>
            /// <param name="midi_ch">MIDI channel.</param>
            /// <param name="note">Note to apply.</param>
            public void ApplyNote(int tick, int midi_ch, Note note) {
                Midi.Message.ApplyNote(tick, midi_ch, note);
            }

            /// <summary>
            /// Applies a program change using the underlying MIDI message logic.
            /// </summary>
            /// <param name="tick">Tick value to process.</param>
            /// <param name="midi_ch">MIDI channel.</param>
            /// <param name="program_num">Program number to apply.</param>
            public void ApplyProgramChange(int tick, int midi_ch, int program_num) {
                Midi.Message.ApplyProgramChange(tick, midi_ch, program_num);
            }

            /// <summary>
            /// Applies a volume change using the underlying MIDI message logic.
            /// </summary>
            /// <param name="tick">Tick value to process.</param>
            /// <param name="midi_ch">MIDI channel.</param>
            /// <param name="volume">Volume value to apply.</param>
            public void ApplyVolume(int tick, int midi_ch, int volume) {
                Midi.Message.ApplyVolume(tick, midi_ch, volume);
            }

            /// <summary>
            /// Applies a pan change using the underlying MIDI message logic.
            /// </summary>
            /// <param name="tick">Tick value to process.</param>
            /// <param name="midi_ch">MIDI channel.</param>
            /// <param name="pan">Pan value to apply.</param>
            public void ApplyPan(int tick, int midi_ch, Pan pan) {
                Midi.Message.ApplyPan(tick, midi_ch, pan);
            }

            /// <summary>
            /// Applies mute using the underlying MIDI message logic.
            /// </summary>
            /// <param name="tick">Tick value to process.</param>
            /// <param name="midi_ch">MIDI channel.</param>
            /// <param name="mute">True to mute, false to unmute.</param>
            public void ApplyMute(int tick, int midi_ch, bool mute) {
                Midi.Message.ApplyMute(tick, midi_ch, mute);
            }

            /// <summary>
            /// Clears the internal state using the underlying MIDI message logic.
            /// </summary>
            public void Clear() {
                Midi.Message.Clear();
            }

            /// <summary>
            /// Gets the list of ChannelMessages for the specified tick using the underlying MIDI message logic.
            /// </summary>
            /// <param name="tick">Tick value to retrieve messages for.</param>
            /// <returns>List of ChannelMessage objects for the specified tick.</returns>
            public List<ChannelMessage> GetBy(int tick) {
                return Midi.Message.GetBy(tick);
            }

            /// <summary>
            /// Determines whether the specified tick exists using the underlying MIDI message logic.
            /// </summary>
            /// <param name="tick">Tick value to check.</param>
            /// <returns>True if the tick exists; otherwise, false.</returns>
            public bool Has(int tick) {
                return Midi.Message.Has(tick);
            }

            /// <summary>
            /// Inverts the internal flag using the underlying MIDI message logic.
            /// </summary>
            public void Invert() {
                Midi.Message.Invert();
            }
        }
    }
}
