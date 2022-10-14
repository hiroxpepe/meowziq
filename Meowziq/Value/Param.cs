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
        // Constructor

        /// <summary>
        /// creates as a "note" notated parameter.
        /// </summary>
        /// <memo_jp>
        /// + ハモリ記述と共用出来る？ <br/> 
        /// </memo_jp>
        public Param(Note note, Exp exp, DataType type, bool auto_note = true) {
            this.note = note;
            Exp = exp;
            Type = type;
            AutoNote = auto_note;
        }

        /// <summary>
        /// creates as a "beat" notated parameter.
        /// </summary>
        public Param(Note note, int percussion_note_num, Exp exp, DataType type) {
            this.note = note;
            PercussionNoteNum = percussion_note_num;
            Exp = exp;
            Type = type;
        }

        /// <summary>
        /// creates as a "chord" notated parameter.
        /// </summary>
        public Param(Chord chord, Exp exp, DataType type) {
            Chord = chord;
            Exp = exp;
            Type = type;
        }

        /// <summary>
        /// creates as a "seque" notated parameter.
        /// </summary>
        public Param(Seque seque, DataType type) {
            Seque = seque;
            Type = type;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <memo>
        /// range is used from Generator.
        /// </memo>
        public Chord Chord {
            get;
        }

        /// <memo>
        /// range is used from Generator.
        /// </memo>
        public Seque Seque {
            get;
        }

        public Exp Exp {
            get;
        }

        public int PercussionNoteNum {
            get;
        }

        public DataType Type {
            get;
        }

        /// <memo>
        /// if a char array can be returned here, there is no problem on the Generator side.
        /// </memo>
        public char[] TextCharArray {
            get {
                if (Type is DataType.Mono || Type is DataType.Multi || Type is DataType.Drum) {
                    return note.TextCharArray;
                } else if (Type is DataType.Chord) {
                    return Chord.TextCharArray;
                } else if (Type is DataType.Seque) {
                    return Seque.TextCharArray;
                }
                return null;
            }
        }

        public bool HasTextCharArray {
            get => !(note is null || note.TextCharArray is null) || !(Chord is null || Chord.TextCharArray is null) || !(Seque is null || Seque.TextCharArray is null);
        }

        public int Interval {
            get {
                if (Type is DataType.Mono || Type is DataType.Multi) {
                    return note.Interval;
                }
                return 0; // Chord has no interval.
            }
        }

        /// <summary>
        /// whether "note" notated parameter.
        /// </summary>
        /// <memo>
        /// used from the Generator class.
        /// </memo>
        public bool IsNote {
            get => Type is DataType.Mono || Type is DataType.Multi;
        }

        /// <summary>
        /// whether "chord" notated parameter.
        /// </summary>
        /// <memo>
        /// used from the Generator class.
        /// </memo>
        public bool IsChord {
            get => Type is DataType.Chord;
        }

        /// <summary>
        /// whether gets a note number in span mode.
        /// </summary>
        public bool AutoNote {
            get;
            private set;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public bool IsMatch(char target) {
            if (Type is DataType.Mono || Type is DataType.Multi) {
                return Regex.IsMatch(target.ToString(), @"^[1-7]+$"); // 1 to 7 for Mono and Multi.
            } else if (Type is DataType.Chord) {
                return Regex.IsMatch(target.ToString(), @"^[1-9]+$"); // 1 to 9 for Chord.
            } else if (Type is DataType.Drum) {
                return target.ToString().Equals("x"); // Drum is 'x'.
            } else if (Type is DataType.Seque) {
                return target.ToString().Equals("+") || target.ToString().Equals("*") || target.ToString().Equals(">"); // Seque is '+', '*', '>'.
            }
            return false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Properties [noun, adjective] 

        Note note {
            get;
        }
    }
}
