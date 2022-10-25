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

using System.Text.RegularExpressions;

namespace Meowziq.Value {
    /// <summary>
    /// parameter class to provide as an argument to the generator class.
    /// </summary>
    /// <memo>
    /// whether it can be an immutable object.
    /// </memo>
   /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Param {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Note _note;

        Chord _chord;

        Seque _seque;

        Exp _exp;

        int _percussion_note_num;

        DataType _type;

        bool _auto_note;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// creates as a "note" notated parameter.
        /// </summary>
        public Param(Note note, Exp exp, DataType type, bool auto_note = true) {
            _note = note;
            _exp = exp;
            _type = type;
            _auto_note = auto_note;
        }

        /// <summary>
        /// creates as a "beat" notated parameter.
        /// </summary>
        public Param(Note note, int percussion_note_num, Exp exp, DataType type) {
            _note = note;
            _percussion_note_num = percussion_note_num;
            _exp = exp;
            _type = type;
        }

        /// <summary>
        /// creates as a "chord" notated parameter.
        /// </summary>
        public Param(Chord chord, Exp exp, DataType type) {
            _chord = chord;
            _exp = exp;
            _type = type;
        }

        /// <summary>
        /// creates as a "seque" notated parameter.
        /// </summary>
        public Param(Seque seque, DataType type) {
            _seque = seque;
            _type = type;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <memo>
        /// range is used from Generator.
        /// </memo>
        public Chord Chord { get => _chord; }

        /// <memo>
        /// range is used from Generator.
        /// </memo>
        public Seque Seque { get => _seque; }

        public Exp Exp { get => _exp; }

        public int PercussionNoteNum { get => _percussion_note_num; }

        public DataType Type { get => _type; }

        /// <memo>
        /// if a char array can be returned here, there is no problem on the Generator side.
        /// </memo>
        public char[] TextCharArray {
            get {
                if (_type is DataType.Mono || _type is DataType.Multi || _type is DataType.Drum) { return _note.TextCharArray; }
                if (_type is DataType.Chord) { return _chord.TextCharArray; }
                if (_type is DataType.Seque) { return _seque.TextCharArray; }
                return null;
            }
        }

        public bool HasTextCharArray {
            get => !(_note is null || _note.TextCharArray is null) || !(_chord is null || _chord.TextCharArray is null) || !(_seque is null || _seque.TextCharArray is null);
        }

        public int Interval {
            get {
                if (_type is DataType.Mono || _type is DataType.Multi) { return _note.Interval; }
                return 0; // Chord has no interval.
            }
        }

        /// <summary>
        /// whether "note" notated parameter.
        /// </summary>
        /// <memo>
        /// used from the Generator class.
        /// </memo>
        public bool IsNote { get => _type is DataType.Mono || _type is DataType.Multi; }

        /// <summary>
        /// whether "chord" notated parameter.
        /// </summary>
        /// <memo>
        /// used from the Generator class.
        /// </memo>
        public bool IsChord { get => _type is DataType.Chord; }

        /// <summary>
        /// whether gets a note number in span mode.
        /// </summary>
        public bool AutoNote { get => _auto_note; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public bool IsMatch(char target) {
            if (_type is DataType.Mono || _type is DataType.Multi) { return Regex.IsMatch(target.ToString(), @"^[1-7]+$"); } // 1 to 7 for Mono and Multi.
            if (_type is DataType.Chord) { return Regex.IsMatch(target.ToString(), @"^[1-9]+$"); } // 1 to 9 for Chord.
            if (_type is DataType.Drum) { return target.ToString().Equals("x"); } // Drum is 'x'.
            if (_type is DataType.Seque) { return target.ToString().Equals("+") || target.ToString().Equals("*") || target.ToString().Equals(">"); } // Seque is '+', '*', '>'.
            return false;
        }
    }
}
