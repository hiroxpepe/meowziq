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

using System.Collections.Generic;
using System.Linq;
using Sanford.Multimedia.Midi;

using static Meowziq.Env;

namespace Meowziq.Midi {
    /// <summary>
    /// class that holds Sanford.Multimedia.Midi.Track objects for smf output.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Multi {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Map<int, Track> _track_map;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Multi() {
            /// <summary>
            /// initializes track map.
            /// </summary>
            _track_map = new();
            Enumerable.Range(start: MIDI_TRACK_BASE, count: MIDI_TRACK_COUNT).ToList().ForEach(
                action: x => _track_map.Add(key: x, value: new Track())
            );
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective]

        /// <summary>
        /// gets the Track list.
        /// </summary>
        public static List<Track> List { get => _track_map.Select(selector: x => x.Value).ToList(); }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// gets a Track by index.
        /// </summary>
        public static Track Get(int index) {
            return _track_map[index];
        }
    }
}
