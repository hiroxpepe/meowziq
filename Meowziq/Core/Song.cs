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

namespace Meowziq.Core {
    /// <summary>
    /// song class
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
        /// gets the song name.
        /// </summary>
        public string Name {
            get => _name;
        }

        /// <summary>
        /// gets the song tempo.
        /// </summary>
        public int Tempo {
            get => _tempo;
        }

        /// <summary>
        /// gets all Section objects.
        /// </summary>
        public List<Section> AllSection {
            get => _section_list;
        }

        /// <summary>
        /// gets all Pattern objects.
        /// </summary>
        /// <note>
        /// not used yet.
        /// </note>
        public List<Pattern> AllPattern {
            get {
                return _section_list.Select(x => x.AllPattern).SelectMany(x => x).ToList();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// not used yet.
        /// </summary>
        Pattern checkeMode(Pattern pattern) {
            pattern.AllMeas.ForEach(action: x => x.AllSpan.ForEach(action: _x => _x.KeyMode = Mode.Undefined));
            return pattern;
        }
    }

    /// <summary>
    /// section class
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
        /// gets the key of this section.
        /// </summary>
        public Key Key {
            get => _key;
        }

        /// <summary>
        /// gets the church mode of this section.
        /// </summary>
        public Mode KeyMode {
            get => _key_mode;
        }

        /// <summary>
        /// gets all Pattern objects.
        /// </summary>
        public List<Pattern> AllPattern {
            get => _pattern_list;
        }
    }
}
