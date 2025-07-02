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

namespace Meowziq.Core {
    /// <summary>
    /// Represents a song.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Song {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _name;

        int _tempo;

        List<Section> _section_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Song(string name, int tempo, List<Section> section_list) {
            _name = name;
            _tempo = tempo;
            _section_list = section_list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets the song name.
        /// </summary>
        public string Name { get => _name; }

        /// <summary>
        /// Gets the song tempo.
        /// </summary>
        public int Tempo { get => _tempo; }

        /// <summary>
        /// Gets all Section objects.
        /// </summary>
        public List<Section> AllSection { get => _section_list; }
    }

    /// <summary>
    /// Represents a section.
    /// </summary>
    public class Section {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Key _key;

        Mode _key_mode;

        List<Pattern> _pattern_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Section(Key key, Mode key_mode, List<Pattern> pattern_list) {
            _key = key;
            _key_mode = key_mode;
            _pattern_list = pattern_list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets the key of this section.
        /// </summary>
        public Key Key { get => _key; }

        /// <summary>
        /// Gets the church mode of this section.
        /// </summary>
        public Mode KeyMode { get => _key_mode; }

        /// <summary>
        /// Gets all Pattern objects.
        /// </summary>
        public List<Pattern> AllPattern { get => _pattern_list; }
    }
}
