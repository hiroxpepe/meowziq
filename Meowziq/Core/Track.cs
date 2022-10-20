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
    /// track class
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Track {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Map<string, Item<Note>> _note_item_map = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _type;

        Item<Note> _note_item;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Track(string type) {
            _type = type;
            _note_item = new();
            if (_note_item_map.ContainsKey(key: type)) {
                _note_item_map.Remove(key: type);
            }
            _note_item_map.Add(key: type, value: _note_item);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public Item<Note> NoteItem {
            get => _note_item;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// initializes the contents.
        /// </summary>
        /// <note>
        /// not used yet.
        /// </note>
        public void Clear() {
            foreach (Item<Note> value in _note_item_map.Values) { value.Clear(); }
            _note_item_map.Clear();
        }
    }
}
