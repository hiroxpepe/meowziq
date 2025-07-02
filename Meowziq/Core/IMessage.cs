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
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public interface IMessage<T1, T2> {

        /// <summary>
        /// Gets the list of T1 at the specified tick.
        /// </summary>
        /// <note>
        /// Returns null if the list of T1 for the specified tick does not exist.<br/>
        /// </note>
        List<T1> GetBy(int tick);

        /// <summary>
        /// Gets a value indicating whether there is an item at the specified tick.
        /// </summary>
        bool Has(int tick);

        /// <summary>
        /// Applies switching processing starting from the specified tick.
        /// </summary>
        void ApplyTick(int tick, Action<int> load);

        /// <summary>
        /// Applies the program number (timbre) as T1.
        /// </summary>
        void ApplyProgramChange(int tick, int midi_ch, int program_num);

        /// <summary>
        /// Applies volume as T1.
        /// </summary>
        void ApplyVolume(int tick, int midi_ch, int volume);

        /// <summary>
        /// Applies pan as T1.
        /// </summary>
        void ApplyPan(int tick, int midi_ch, Pan pan);

        /// <summary>
        /// Applies mute as T1.
        /// </summary>
        void ApplyMute(int tick, int midi_ch, bool mute);

        /// <summary>
        /// Applies T2 as T1.
        /// </summary>
        void ApplyNote(int tick, int midi_ch, T2 note);

        /// <summary>
        /// Clears the state.
        /// </summary>
        void Clear();

        /// <summary>
        /// Inverts the internal flag.
        /// </summary>
        void Invert();
    }
}
