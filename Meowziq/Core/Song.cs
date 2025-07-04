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
    /// Represents a song, including its name, tempo, and sections.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Song {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Holds the name of the song.
        /// </summary>
        string _name;

        /// <summary>
        /// Holds the tempo of the song.
        /// </summary>
        int _tempo;

        /// <summary>
        /// Holds the list of sections in the song.
        /// </summary>
        List<Section> _section_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Song"/> class.
        /// </summary>
        /// <param name="name">The name of the song.</param>
        /// <param name="tempo">The tempo of the song.</param>
        /// <param name="section_list">The list of sections in the song.</param>
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
        /// Gets all section objects in the song.
        /// </summary>
        public List<Section> AllSection { get => _section_list; }
    }

    /// <summary>
    /// Represents a section of a song, including its key, mode, and patterns.
    /// </summary>
    public class Section {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Holds the key of this section.
        /// </summary>
        Key _key;

        /// <summary>
        /// Holds the church mode of this section.
        /// </summary>
        Mode _key_mode;

        /// <summary>
        /// Holds the list of patterns in this section.
        /// </summary>
        List<Pattern> _pattern_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        /// <param name="key">The key of this section.</param>
        /// <param name="key_mode">The church mode of this section.</param>
        /// <param name="pattern_list">The list of patterns in this section.</param>
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
        /// Gets all pattern objects in this section.
        /// </summary>
        public List<Pattern> AllPattern { get => _pattern_list; }
    }
}
