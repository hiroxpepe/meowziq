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

namespace Meowziq {
    /// <summary>
    /// envelope class
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Env {
#nullable enable

        public static readonly int MIDI_TRACK_BASE = 0;
        public static readonly int MIDI_TRACK_COUNT = 16;
        public static readonly int MIDI_CH_DRUM = 9;

        /// <summary>
        /// tick interval of the sequencer.
        /// </summary>
        public static readonly int TICK_INTERVAL = 30;
        /// <summary>
        /// note resolution of the sequencer.
        /// </summary>
        public static readonly int NOTE_RESOLUTION = 480;
        /// <summary>
        /// beat count in 1 measure.
        /// </summary>
        public static readonly int BEAT_COUNT_IN_MEASURE = 4;
        /// <summary>
        /// 1 measure consists of 4 beats.
        /// </summary>
        public static readonly int TIMES_TO_MEASURE = 4;
        /// <summary>
        /// set it to 4 can change every 1 measure. <br/>
        /// also set it to 1 can change every 1 beat. 
        /// </summary>
        public static readonly int LOAD_EVERY_BEAT = 256;

        public static readonly string COUNT_PATTERN = "count";
        public static readonly string CONDUCTOR_MIDI = "./data/conductor.mid";
    }
}
