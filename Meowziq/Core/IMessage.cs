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

namespace Meowziq {
    /// <summary>
    /// Defines the interface for a Message.
    /// </summary>
    /// <typeparam name="T1">Type of the main message element.</typeparam>
    /// <typeparam name="T2">Type of the note/message value.</typeparam>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public interface IMessage<T1, T2> {

        /// <summary>
        /// Gets the list of T1 at the specified tick.
        /// </summary>
        /// <param name="tick">Tick position to retrieve the list.</param>
        /// <returns>List of T1 at the specified tick, or null if not found.</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Returns null if the list of T1 for the specified tick does not exist.</item>
        /// </list>
        /// </remarks>
        List<T1> GetBy(int tick);

        /// <summary>
        /// Gets a value indicating whether there is an item at the specified tick.
        /// </summary>
        /// <param name="tick">Tick position to check.</param>
        /// <returns>True if an item exists at the specified tick; otherwise, false.</returns>
        bool Has(int tick);

        /// <summary>
        /// Applies switching processing starting from the specified tick.
        /// </summary>
        /// <param name="tick">Tick position to start processing.</param>
        /// <param name="load">Action to perform for each tick.</param>
        void ApplyTick(int tick, Action<int> load);

        /// <summary>
        /// Applies program change (timbre) as T1.
        /// </summary>
        /// <param name="tick">Tick position to apply the change.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="program_num">Program number (timbre).</param>
        void ApplyProgramChange(int tick, int midi_ch, int program_num);

        /// <summary>
        /// Applies volume as T1.
        /// </summary>
        /// <param name="tick">Tick position to apply the volume.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="volume">Volume value.</param>
        void ApplyVolume(int tick, int midi_ch, int volume);

        /// <summary>
        /// Applies pan as T1.
        /// </summary>
        /// <param name="tick">Tick position to apply the pan.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="pan">Pan value.</param>
        void ApplyPan(int tick, int midi_ch, Pan pan);

        /// <summary>
        /// Applies mute as T1.
        /// </summary>
        /// <param name="tick">Tick position to apply mute.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="mute">Mute flag.</param>
        void ApplyMute(int tick, int midi_ch, bool mute);

        /// <summary>
        /// Applies T2 as T1 (note on/off or similar message).
        /// </summary>
        /// <param name="tick">Tick position to apply the note/message.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="note">Note/message value.</param>
        void ApplyNote(int tick, int midi_ch, T2 note);

        /// <summary>
        /// Clears the state of the message.
        /// </summary>
        void Clear();

        /// <summary>
        /// Inverts the internal flag/state.
        /// </summary>
        void Invert();
    }
}
