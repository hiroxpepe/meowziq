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
    /// Provides environment constants for the sequencer and MIDI system.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Env {
#nullable enable
        /// <summary>
        /// The base index for MIDI tracks.
        /// </summary>
        public static readonly int MIDI_TRACK_BASE = 0;
        /// <summary>
        /// The total number of MIDI tracks.
        /// </summary>
        public static readonly int MIDI_TRACK_COUNT = 16;
        /// <summary>
        /// The MIDI channel number used for drums (10th channel, zero-based).
        /// </summary>
        public static readonly int MIDI_CH_DRUM = 9;
        /// <summary>
        /// The tick interval of the sequencer.
        /// </summary>
        public static readonly int TICK_INTERVAL = 30;
        /// <summary>
        /// The note resolution of the sequencer (ticks per quarter note).
        /// </summary>
        public static readonly int NOTE_RESOLUTION = 480;
        /// <summary>
        /// The number of beats in one measure.
        /// </summary>
        public static readonly int BEAT_COUNT_IN_MEASURE = 4;
        /// <summary>
        /// The number of beats that make up one measure.
        /// </summary>
        public static readonly int TIMES_TO_MEASURE = 4;
        /// <summary>
        /// The interval for loading data. Set to 4 to change every measure, or 1 to change every beat.
        /// </summary>
        public static readonly int LOAD_EVERY_BEAT = 1;
        /// <summary>
        /// The name of the count pattern.
        /// </summary>
        public static readonly string COUNT_PATTERN = "count";
        /// <summary>
        /// The path to the conductor MIDI file.
        /// </summary>
        public static readonly string CONDUCTOR_MIDI = "./data/conductor.mid";
    }
}
