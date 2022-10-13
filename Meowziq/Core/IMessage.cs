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
    /// interface of Message.
    /// </summary>
    /// <author>
    /// h.adachi (STUDIO MeowToon)
    /// </author>
    public interface IMessage<T1, T2> {

        /// <summary>
        /// returns a list of T1 of the argument tick
        /// NOTE: returns null if the list of T1 for the argument tick does not exist
        /// </summary>
        List<T1> GetBy(int tick);

        /// <summary>
        /// returns whether it has an item with the argument tick
        /// </summary>
        bool Has(int tick);

        /// <summary>
        /// switching processing is performed starting from the argument tick
        /// </summary>
        void ApplyTick(int tick, Action<int> load);

        /// <summary>
        /// apply the program number (timbre) as T1
        /// </summary>
        void ApplyProgramChange(int tick, int midi_ch, int program_num);

        /// <summary>
        /// apply volume as T1
        /// </summary>
        void ApplyVolume(int tick, int midi_ch, int volume);

        /// <summary>
        /// apply pan as T1
        /// </summary>
        void ApplyPan(int tick, int midi_ch, Pan pan);

        /// <summary>
        /// apply mute as T1
        /// </summary>
        void ApplyMute(int tick, int midi_ch, bool mute);

        /// <summary>
        /// apply T2 as T1
        /// </summary>
        void ApplyNote(int tick, int midi_ch, T2 note);

        /// <summary>
        /// initialize the state
        /// </summary>
        void Clear();

        /// <summary>
        /// invert the internal flag
        /// </summary>
        void Invert();
    }
}
