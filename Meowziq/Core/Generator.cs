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

using Meowziq.Value;
using static Meowziq.Utils;

namespace Meowziq.Core {
    /// <summary>
    /// Generate Note objects and add them to the Item object.
    /// </summary>
    /// <note>
    /// + Called from Meowziq.Core.Phrase.onBuild().
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Generator {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Item<Note> _note_item;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        Generator(Item<Note> note_item) {
            _note_item = note_item;
        }

        /// <summary>
        /// Get an initialized instance.
        /// </summary>
        public static Generator GetInstance(Item<Note> note_item) {
            return new Generator(note_item);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Creates and applies a Note object.
        /// </summary>
        public void ApplyNote(int start_tick, int beat_count, List<Span> span_list, Param param) {
            for (Index I6beat_index = new(beat_count); I6beat_index.HasNext; I6beat_index.Increment()) {
                char text = param.TextCharArray[I6beat_index.Idx];
                if (param.IsMatch(text)) {
                    Span span = span_list[I6beat_index.SpanIndex]; // 4 times of 16 beats advance 1 beat.
                    int[] note_num_array = (new int[7]).Select(selector: x => -1).ToArray();  // Initializes an array of notes with -1 : to remove it later.
                    if (param.IsNote) { // "note", "auto" notated.
                        note_num_array[0] = ToNote(
                            key: span.Key, degree: span.Degree, key_mode: span.KeyMode, span_mode: span.SpanMode, number: text.Int32(), auto_note: param.AutoNote
                        );
                    } else if (param.IsChord) { // "chord" notated.
                        note_num_array = ToNoteArray(
                            key: span.Key, degree: span.Degree, key_mode: span.KeyMode, span_mode: span.SpanMode, number: text.Int32()
                        );
                        note_num_array = applyRange(target: note_num_array, range: param.Chord.Range); // Applies octave range to affect chord inversions.
                    }
                    // Creates the length of the note as a Gate object.
                    Gate gate = new(search_idx: I6beat_index.Idx, beat_count: beat_count);
                    for (; gate.HasNextSearch; gate.IncrementSearch()) {
                        char search = param.TextCharArray[gate.SearchIndex];
                        if (search.Equals('>')) { // Has a '>' character in the search result.
                            gate.IncrementGate(); // Extends length by 16 beats.
                        }
                        if (!search.Equals('>')) {
                            break; // Has not '>' character in the search result: exits.
                        }
                    }
                    if (param.Exp.HasPre) { // Has a pre parameter, it's syncopation.
                        char pre = param.Exp.PreCharArray[I6beat_index.Idx];
                        if (param.Exp.IsMatchPre(pre)) {
                            gate.ApplyPre(pre_string: pre.ToString());
                        }
                    }
                    if (param.Exp.HasPost) {
                        // TODO: To a long note if has a post parameter.
                    }
                    int tick = gate.PreLength + start_tick + To16beatLength(I6beat_index.Idx);
                    // Adds a Note object to Item.
                    note_num_array.Where(predicate: x => x != -1).ToList().ForEach(action: x => 
                        add(tick: tick, note: new Note(tick: tick, num: x + param.Interval, gate: gate.Value, velo: 104, pre_count: gate.PreCount))
                    );
                }
            }
        }

        /// <summary>
        /// Creates and applies a Note object for drum.
        /// </summary>
        public void ApplyDrumNote(int start_tick, int beat_count, Param param) {
            for (Index I6beat_index = new(beat_count); I6beat_index.HasNext; I6beat_index.Increment()) {
                char text = param.TextCharArray[I6beat_index.Idx];
                if (param.IsMatch(text)) {
                    Gate gate = new();
                    if (param.Exp.HasPre) { // Has a pre parameter, it's syncopation.
                        char pre = param.Exp.PreCharArray[I6beat_index.Idx];
                        if (param.Exp.IsMatchPre(pre)) {
                            gate.ApplyPre(pre_string: pre.ToString());
                        }
                    }
                    int tick = gate.PreLength + start_tick + To16beatLength(I6beat_index.Idx);
                    add(tick: tick, note: new Note(tick: tick, num: param.PercussionNoteNum, gate: gate.Value, velo: 104, pre_count: gate.PreCount));
                }
            }
        }

        /// <summary>
        /// Creates and applies a sequence Note object.
        /// </summary>
        public void ApplySequeNote(int start_tick, int beat_count, List<Span> span_list, Param param) {
            for (Index I6beat_index = new(beat_count); I6beat_index.HasNext; I6beat_index.Increment()) {
                Span span = span_list[I6beat_index.SpanIndex]; // 4 times of 16 beats advance 1 beat.
                int note_num = ToRandomNote(key: span.Key, degree: span.Degree, key_mode: span.KeyMode, span_mode: span.SpanMode);
                if (param.Seque.Range is not null) {
                    note_num = applyRange(target: new int[] { note_num }, range: param.Seque.Range)[0]; // TODO: Single note ver of applyRange.
                }
                if (param.HasTextCharArray) {
                    char text = param.TextCharArray[I6beat_index.Idx];
                    if (param.Seque.Text.HasValue() && param.IsMatch(text)) {
                        int tick = start_tick + To16beatLength(index: I6beat_index.Idx);
                        add(tick: tick, note: new Note(tick: tick, num: note_num, gate: Seque.ToGateValue(target: text.ToString()), velo: 104));
                    }
                } else {
                    int tick = start_tick + To16beatLength(index: I6beat_index.Idx);
                    add(tick: tick, note: new Note(tick: tick, num: note_num, gate: 30, velo: 104)); // TODO: Can be removed if you have a default Text description. : automatically generated by pattern length.
                }
            }
        }

        /// <summary>
        /// Creates information for UI display (one State object per 16 beats).
        /// </summary>
        public void ApplyInfo(int start_tick, int beat_count, List<Span> span_list) {
            for (Index I6beat_index = new(beat_count); I6beat_index.HasNext; I6beat_index.Increment()) {
                Span span = span_list[I6beat_index.SpanIndex]; // 4 times of 16 beats advance 1 beat.
                int tick = start_tick + To16beatLength(index: I6beat_index.Idx); // Processed every tick of 16 beats.
                if (State.HashSet.Add(item: tick)) { // Only once per tick.
                    State.ItemMap.Add(key: tick, value: new State.Item16beat { // A State object is created every 16 beats.
                        Tick = tick,
                        Key = span.Key,
                        Degree = span.Degree,
                        KeyMode = span.KeyMode,
                        SpanMode = span.SpanMode
                    });
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// Converts all notes into the specified range (octave shift if out of range).
        /// </summary>
        int[] applyRange(int[] target, Range range) {
            int[] new_array = new int[target.Length];
            for (int index = 0; index < target.Length; index++) {
                if (target[index] < range.Min) { new_array[index] = target[index] + 12; } 
                else if (target[index] > range.Max) { new_array[index] = target[index] - 12; } 
                else { new_array[index] = target[index]; }
            }
            if ((target.Min() >= range.Min) && (target.Max() <= range.Max)) { // All notes became in the specified range.
                return new_array;
            }
            return applyRange(target: new_array, range); // Recursion
        }

        /// <summary>
        /// Adds a Note to the Item object at the specified tick.
        /// </summary>
        void add(int tick, Note note) {
            _note_item.Add(key: tick, value: note);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Holds note gate (length, syncopation) information.
        /// </summary>
        class Gate {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Const [nouns]

            const int TEXT_VALUE_LENGTH = 1;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int _beat_count, _gate_count, _search_index, _pre_length, _pre_count;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Gate(int search_idx, int beat_count) {
                _beat_count = beat_count;
                _search_index = search_idx + TEXT_VALUE_LENGTH;
                _gate_count = _pre_length = _pre_count = 0;
            }

            public Gate() {
                _gate_count = _pre_length = _pre_count = 0;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective] 

            /// <summary>
            /// Gets a value indicating whether there is a next 16-beat in this Pattern.
            /// </summary>
            public bool HasNextSearch {
                get {
                    if (_search_index < To16beatCount(_beat_count)) {
                        return true; // True if the index is less than the 16-beat length of the Pattern.
                    }
                    return false;
                }
            }

            /// <summary>
            /// Gets the index pointing to the array position.
            /// </summary>
            public int SearchIndex { get => _search_index; }

            /// <summary>
            /// Gets the value that moves the tick of notes forward.
            /// </summary>
            public int PreLength { get => _pre_length; }

            /// <summary>
            /// Gets the count of syncopation.
            /// </summary>
            public int PreCount { get => _pre_count; }

            /// <summary>
            /// Gets the gate value of the note.
            /// </summary>
            public int Value { get => To16beatLength(_gate_count + TEXT_VALUE_LENGTH); }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            /// <summary>
            /// Applies a syncopation parameter.
            /// </summary>
            public void ApplyPre(string pre_string) {
                int pre_int = int.Parse(pre_string.ToString());
                _gate_count += pre_int;
                _pre_length = -To16beatLength(pre_int);
                if (_pre_length != 0) {
                    _pre_count = pre_int;
                }
            }

            /// <summary>
            /// Increments the search value.
            /// </summary>
            public void IncrementSearch() {
                _search_index++;
            }

            /// <summary>
            /// Increments the gate value.
            /// </summary>
            public void IncrementGate() {
                _gate_count++;
            }
        }

        /// <summary>
        /// Manages 16-beat index and Span index.
        /// </summary>
        /// <note>
        /// + Returns the Span index incremented by 1 every 4 increments.
        /// + Always 1 beat per Span object here.
        /// </note>
        class Index {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int _beat_count;

            int _index_for_16beat;

            int _index, _span_index;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Index(int beat_count) {
                _beat_count = beat_count;
                _index_for_16beat = 0;
                _span_index = 0;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective] 

            /// <summary>
            /// Gets the index of the current 16-beat position.
            /// </summary>
            public int Idx { get => _index; }

            /// <summary>
            /// Gets the index of the current Span object.
            /// </summary>
            public int SpanIndex { get => _span_index; }

            /// <summary>
            /// Returns true if there is a next 16-beat in this Pattern.
            /// </summary>
            public bool HasNext {
                get {
                    if (_index < To16beatCount(_beat_count)) {
                        return true;
                    }
                    return false;
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            /// <summary>
            /// Increments the 16-beat index.
            /// </summary>
            public void Increment() {
                _index++; // Increments the 16-beat index.
                _index_for_16beat++;
                if (_index_for_16beat == 4) {
                    _index_for_16beat = 0;
                    _span_index++;
                }
            }
        }
    }
}
