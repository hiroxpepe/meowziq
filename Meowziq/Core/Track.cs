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
    /// Represents a musical track that manages note items and their operations.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Track {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// Stores the mapping from track type to its corresponding note item.
        /// </summary>
        static Map<string, Item<Note>> _note_item_map = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Stores the type of the track.
        /// </summary>
        string _type;

        /// <summary>
        /// Stores the note item associated with this track.
        /// </summary>
        Item<Note> _note_item;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class with the specified type.
        /// </summary>
        /// <param name="type">The type of the track.</param>
        public Track(string type) {
            _type = type;
            _note_item = new();
            if (_note_item_map.ContainsKey(key: type)) {
                _note_item_map.Remove(key: type);
            }
            _note_item_map.Add(key: type, value: _note_item);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// Gets all <see cref="Note"/> objects of this song.
        /// </summary>
        /// <remarks>
        /// <item>Not used yet.</item>
        /// </remarks>
        public static List<Note> AllNote {
            get => _note_item_map.SelectMany(selector: x => x.Value).SelectMany(selector: x => x.Value).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets the <see cref="Item{Note}"/> object associated with this track.
        /// </summary>
        public Item<Note> NoteItem {
            get {
               optimize();
               return _note_item; 
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Initializes the contents of all note items and clears the note item map.
        /// </summary>
        /// <remarks>
        /// <item>Not used yet.</item>
        /// </remarks>
        public void Clear() {
            foreach (Item<Note> value in _note_item_map.Values) { value.Clear(); }
            _note_item_map.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// Optimizes notes by removing overlapping notes with syncopation.
        /// </summary>
        void optimize() {
            List<Note> all_note_list = _note_item.SelectMany(selector: x => x.Value).ToList();
            foreach (Note syncopation_note in all_note_list.Where(predicate: x => x.HasPre)) {
                foreach (Note note in all_note_list) {
                    if (note.Tick == syncopation_note.Tick) {
                        removeBy(note);
                    }
                }
            }
        }

        /// <summary>
        /// Removes notes that overlap with syncopation for the specified target note.
        /// </summary>
        /// <param name="target">The target note to use for overlap removal.</param>
        public void removeBy(Note target) {
            int tick1 = target.Tick;
            List<Note> note_list1 = _note_item.Get(key: tick1);
            note_list1 = note_list1.Where(predicate: x => !(!x.HasPre && x.Tick == tick1)).ToList(); // Removes the same tick and a non-priority note.
            _note_item.SetBy(key: tick1, value: note_list1);
            if (target.PreCount > 1) { // Has a syncopation parameter 2.
                int tick2 = target.Tick + Length.Of16beat.Int32();
                List<Note> note_list2 = _note_item.Get(tick2);
                if (note_list2 != null) {
                    note_list2 = note_list2.Where(predicate: x => !(!x.HasPre && x.Tick == tick2)).ToList(); // Removes more overlapping 16 beats notes.
                    _note_item.SetBy(key: tick2, value: note_list2);
                }
            }
        }
    }
}
