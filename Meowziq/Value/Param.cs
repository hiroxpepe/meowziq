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
    /// Provides parameter objects for use as arguments to the Generator class and related processes.
    /// </summary>
    /// <remarks>
    /// <item>Determines the type and content of musical data for note, chord, seque, and percussion.</item>
    /// <item>Used as a context object for note/chord generation.</item>
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Param {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Stores the Note data for this parameter.
        /// </summary>
        private Note _note;

        /// <summary>
        /// Stores the Chord data for this parameter.
        /// </summary>
        private Chord _chord;

        /// <summary>
        /// Stores the Seque data for this parameter.
        /// </summary>
        private Seque _seque;

        /// <summary>
        /// Stores the Exp data for this parameter.
        /// </summary>
        private Exp _exp;

        /// <summary>
        /// Stores the percussion note number for this parameter.
        /// </summary>
        private int _percussion_note_num;

        /// <summary>
        /// Stores the DataType for this parameter.
        /// </summary>
        private DataType _type;

        /// <summary>
        /// Indicates whether auto note mode is enabled.
        /// </summary>
        private bool _auto_note;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Creates a parameter object for a note notation.
        /// </summary>
        /// <param name="note">Note object to assign.</param>
        /// <param name="exp">Exp object to assign.</param>
        /// <param name="type">DataType to assign.</param>
        /// <param name="auto_note">Indicates whether auto note mode is enabled. Default is true.</param>
        public Param(Note note, Exp exp, DataType type, bool auto_note = true) {
            _note = note;
            _exp = exp;
            _type = type;
            _auto_note = auto_note;
        }

        /// <summary>
        /// Creates a parameter object for a beat notation.
        /// </summary>
        /// <param name="note">Note object to assign.</param>
        /// <param name="percussion_note_num">Percussion note number to assign.</param>
        /// <param name="exp">Exp object to assign.</param>
        /// <param name="type">DataType to assign.</param>
        public Param(Note note, int percussion_note_num, Exp exp, DataType type) {
            _note = note;
            _percussion_note_num = percussion_note_num;
            _exp = exp;
            _type = type;
        }

        /// <summary>
        /// Creates a parameter object for a chord notation.
        /// </summary>
        /// <param name="chord">Chord object to assign.</param>
        /// <param name="exp">Exp object to assign.</param>
        /// <param name="type">DataType to assign.</param>
        public Param(Chord chord, Exp exp, DataType type) {
            _chord = chord;
            _exp = exp;
            _type = type;
        }

        /// <summary>
        /// Creates a parameter object for a seque notation.
        /// </summary>
        /// <param name="seque">Seque object to assign.</param>
        /// <param name="type">DataType to assign.</param>
        public Param(Seque seque, DataType type) {
            _seque = seque;
            _type = type;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets the Chord object for this parameter.
        /// </summary>
        /// <remarks>Range is used from Generator.</remarks>
        public Chord Chord { get => _chord; }

        /// <summary>
        /// Gets the Seque object for this parameter.
        /// </summary>
        /// <remarks>Range is used from Generator.</remarks>
        public Seque Seque { get => _seque; }

        /// <summary>
        /// Gets the Exp object for this parameter.
        /// </summary>
        public Exp Exp { get => _exp; }

        /// <summary>
        /// Gets the percussion note number for this parameter.
        /// </summary>
        public int PercussionNoteNum { get => _percussion_note_num; }

        /// <summary>
        /// Gets the DataType for this parameter.
        /// </summary>
        public DataType Type { get => _type; }

        /// <summary>
        /// Gets the character array representing the text for this parameter.
        /// </summary>
        /// <remarks>If a char array can be returned here, there is no problem on the Generator side.</remarks>
        public char[] TextCharArray {
            get {
                if (_type is DataType.Mono || _type is DataType.Multi || _type is DataType.Drum) { return _note.TextCharArray; }
                if (_type is DataType.Chord) { return _chord.TextCharArray; }
                if (_type is DataType.Seque) { return _seque.TextCharArray; }
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this parameter has a valid text character array.
        /// </summary>
        public bool HasTextCharArray {
            get => !(_note is null || _note.TextCharArray is null) || !(_chord is null || _chord.TextCharArray is null) || !(_seque is null || _seque.TextCharArray is null);
        }

        /// <summary>
        /// Gets the interval value for this parameter.
        /// </summary>
        /// <remarks>Returns 0 if the type is Chord or not applicable.</remarks>
        public int Interval {
            get {
                if (_type is DataType.Mono || _type is DataType.Multi) { return _note.Interval; }
                return 0; // Chord has no interval.
            }
        }

        /// <summary>
        /// Gets a value indicating whether this parameter is a note notation.
        /// </summary>
        /// <remarks>Used from the Generator class.</remarks>
        public bool IsNote { get => _type is DataType.Mono || _type is DataType.Multi; }

        /// <summary>
        /// Gets a value indicating whether this parameter is a chord notation.
        /// </summary>
        /// <remarks>Used from the Generator class.</remarks>
        public bool IsChord { get => _type is DataType.Chord; }

        /// <summary>
        /// Gets a value indicating whether auto note mode is enabled for span mode.
        /// </summary>
        public bool AutoNote { get => _auto_note; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Determines whether the specified character matches the expected pattern for the current DataType.
        /// </summary>
        /// <param name="target">Target character to check.</param>
        /// <returns>True if the character matches the pattern for the DataType; otherwise, false.</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Mono/Multi: 1-7</item>
        /// <item>Chord: 1-9</item>
        /// <item>Drum: 'x'</item>
        /// <item>Seque: '+', '*', '>'</item>
        /// </list>
        /// </remarks>
        public bool IsMatch(char target) {
            if (_type is DataType.Mono || _type is DataType.Multi) { return Regex.IsMatch(target.ToString(), @"^[1-7]+$"); } // 1 to 7 for Mono and Multi.
            if (_type is DataType.Chord) { return Regex.IsMatch(target.ToString(), @"^[1-9]+$"); } // 1 to 9 for Chord.
            if (_type is DataType.Drum) { return target.ToString().Equals("x"); } // Drum is 'x'.
            if (_type is DataType.Seque) { return target.ToString().Equals("+") || target.ToString().Equals("*") || target.ToString().Equals(">"); } // Seque is '+', '*', '>'.
            return false;
        }
    }
}
