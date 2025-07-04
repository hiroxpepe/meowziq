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

using System;
using System.Linq;
using System.Text.RegularExpressions;

using static Meowziq.Env;
using static Meowziq.Value.Utils;
using static Meowziq.Value.Validater;

namespace Meowziq.Value {
    /// <summary>
    /// Represents the data structure for a musical phrase, including notes, chords, percussion, and related parameters.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>Used from the Loader class, so it must be made public.</description></item>
    /// </list>
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Data {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Gets or sets the note parameter for the phrase.
        /// </summary>
        Note _note;

        /// <summary>
        /// Gets or sets whether the phrase uses auto notation.
        /// </summary>
        bool _auto;

        /// <summary>
        /// Gets or sets the chord parameter for the phrase.
        /// </summary>
        Chord _chord;

        /// <summary>
        /// Gets or sets the arpeggio (seque) parameter for the phrase.
        /// </summary>
        Seque _seque;

        /// <summary>
        /// Gets or sets the expansion parameter for the phrase.
        /// </summary>
        Exp _exp;

        /// <summary>
        /// Gets or sets the percussion names array.
        /// </summary>
        Percussion[] _percussion_array;

        /// <summary>
        /// Gets or sets the drum value array.
        /// </summary>
        string[] _beat_array;

        /// <summary>
        /// Gets or sets the degree value array for key_mode.
        /// </summary>
        string[] _note_array;

        /// <summary>
        /// Gets or sets the degree value array for span_mode.
        /// </summary>
        string[] _auto_array;

        /// <summary>
        /// Gets or sets the octave value array.
        /// </summary>
        int[] _oct_array;

        /// <summary>
        /// Gets or sets the pre value array.
        /// </summary>
        string[] _pre_array;

        /// <summary>
        /// Gets or sets the post value array.
        /// </summary>
        string[] _post_array;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Data"/> class.
        /// </summary>
        public Data() {
            _note = new();
            _auto = false;
            _chord = new();
            _seque = new();
            _exp = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets or sets the note parameter for the phrase.
        /// </summary>
        public Note Note { get => _note; set => _note = value; }

        /// <summary>
        /// Gets or sets whether the phrase uses auto notation.
        /// </summary>
        public bool Auto { get => _auto; set => _auto = value; }

        /// <summary>
        /// Gets or sets the chord parameter for the phrase.
        /// </summary>
        public Chord Chord { get => _chord; set => _chord = value; }

        /// <summary>
        /// Gets or sets the arpeggio (seque) parameter for the phrase.
        /// </summary>
        public Seque Seque { get => _seque; set => _seque = value; }

        /// <summary>
        /// Gets or sets the expansion parameter for the phrase.
        /// </summary>
        public Exp Exp { get => _exp; set => _exp = value; }

        /// <summary>
        /// Gets or sets the array of percussion sounds.
        /// </summary>
        /// <value>Array of percussion names.</value>
        public Percussion[] PercussionArray {
            get => _percussion_array;
            set {
                if (value.Length != arrayLength) { throw new ArgumentException("percussionArray must be same count as beatArray."); }
                _percussion_array = value;
            }
        }

        /// <summary>
        /// Gets or sets the array of "beat" notated text.
        /// </summary>
        /// <value>Array of beat notation strings.</value>
        public string[] BeatArray {
            get => _beat_array;
            set {
                if (value is not null) { value.ToList().ForEach(action: x => PhraseValue(target: x)); }
                _beat_array = value;
            }
        }

        /// <summary>
        /// Gets or sets the array of "note" notated text.
        /// </summary>
        /// <value>Array of note notation strings.</value>
        public string[] NoteArray {
            get => _note_array;
            set {
                if (value is not null) { value.ToList().ForEach(action: x => PhraseValue(target: x)); }
                _note_array = value;
            }
        }

        /// <summary>
        /// Gets or sets the array of "auto" notated text.
        /// </summary>
        /// <value>Array of auto notation strings.</value>
        public string[] AutoArray {
            get => _auto_array;
            set {
                if (value is not null) { value.ToList().ForEach(action: x => PhraseValue(target: x)); _auto = true; }
                _auto_array = value;
            }
        }

        /// <summary>
        /// Gets or sets the octave parameter for "note" and "auto" notated text.
        /// </summary>
        /// <value>Array of octave values.</value>
        public int[] OctArray {
            get => _oct_array;
            set {
                checkTextArray();
                if (value is null) {
                    _oct_array = new int[arrayLength];
                    _oct_array.Select(selector: x => x = 0); // automatically generate octave values.
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("noteOctArray must be same count as noteArray or autoArray.");
                } else {
                    _oct_array = value; // TODO: validation
                }
            }
        }

        /// <summary>
        /// Gets or sets the pre note parameter for "beat", "note" and "auto" text.
        /// </summary>
        /// <value>Array of pre note values.</value>
        public string[] PreArray {
            get => _pre_array;
            set {
                checkTextArray();
                if (value is null) {
                    _pre_array = new string[arrayLength];
                    _pre_array.Select(selector: x => x = null); // automatically generate initial values.
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("preArray must be same count as beatArray or noteArray or autoArray.");
                } else {
                    value.ToList().ForEach(action: x => PhraseValue(target: x));
                    _pre_array = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the post note parameter for "note" and "auto" text.
        /// </summary>
        /// <value>Array of post note values.</value>
        public string[] PostArray {
            get => _post_array;
            set {
                checkTextArray();
                if (value is null) {
                    _post_array = new string[arrayLength];
                    _post_array.Select(selector: x => x = null); // automatically generate initial values.
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("postArray must be same count as noteArray or autoArray.");
                } else {
                    value.ToList().ForEach(action: x => PhraseValue(target: x));
                    _post_array = value;
                }
            }
        }

        /// <summary>
        /// Gets whether the phrase has "beat" notated data.
        /// </summary>
        public bool HasBeat { get => !hasNote && !hasAuto && !hasChord && !hasSeque && hasBeatArray && !hasNoteArray && !hasAutoArray; }

        /// <summary>
        /// Gets whether the phrase has "chord" notated data.
        /// </summary>
        public bool HasChord { get => !hasNote && !hasAuto && hasChord && !hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray; }

        /// <summary>
        /// Gets whether the phrase has "seque" notated data.
        /// </summary>
        public bool HasSeque { get => !hasNote && !hasAuto && !hasChord && hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray; }

        /// <summary>
        /// Gets whether the phrase has "note" notated data.
        /// </summary>
        public bool HasNote { get => (hasNote || hasNoteArray) && !hasAuto && !hasChord && !hasSeque && !hasBeatArray && !hasAutoArray; }

        /// <summary>
        /// Gets whether the phrase has "auto" notated data.
        /// </summary>
        public bool HasAuto { get => !hasNote && !hasNoteArray && (hasAuto || hasAutoArray) && !hasChord && !hasSeque && !hasBeatArray; }

        /// <summary>
        /// Gets whether the phrase has array notated data.
        /// </summary>
        public bool HasMulti { get => !hasNote && !hasAuto && !hasChord && !hasSeque && (hasBeatArray || hasNoteArray || hasAutoArray); }

        /// <summary>
        /// Gets whether the phrase has no data.
        /// </summary>
        public bool HasNoData { get => !hasNote && !hasAuto && !hasChord && !hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Properties [noun, adjective] 

        /// <summary>
        /// Gets the array length for the current data arrays.
        /// </summary>
        int arrayLength {
            get {
                if (_beat_array != null) { return _beat_array.Length; }
                if (_note_array != null) { return _note_array.Length; }
                if (_auto_array != null) { return _auto_array.Length; }
                return 0;
            }
        }

        /// <summary>
        /// Gets whether any data array is present.
        /// </summary>
        bool hasAnyArray {
            get {
                if (_beat_array != null || _note_array != null || _auto_array != null) { return true; }
                return false;
            }
        }

        /// <summary>
        /// Gets whether the beat array is present.
        /// </summary>
        public bool hasBeatArray { get => _beat_array != null; }

        /// <summary>
        /// Gets whether the note array is present and auto is false.
        /// </summary>
        public bool hasNoteArray { get => _note_array != null && _auto == false; }

        /// <summary>
        /// Gets whether the auto array is present and auto is true.
        /// </summary>
        public bool hasAutoArray { get => _auto_array != null && _auto == true; }

        /// <summary>
        /// Gets whether the note is present and auto is false.
        /// </summary>
        bool hasNote { get => !_note.Text.Equals(string.Empty) && _auto == false; }

        /// <summary>
        /// Gets whether the note is present and auto is true.
        /// </summary>
        bool hasAuto { get => !_note.Text.Equals(string.Empty) && _auto == true; }

        /// <summary>
        /// Gets whether the chord is present.
        /// </summary>
        bool hasChord { get => _chord.Text != null; }

        /// <summary>
        /// Gets whether the seque is used.
        /// </summary>
        bool hasSeque { get => _seque.Use; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// Checks that at least one data array is present.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if no data array is set.</exception>
        void checkTextArray() {
            if (!hasAnyArray) { throw new ArgumentException("must set beatArray or noteArray or autoArray."); }
        }
    }

    /// <summary>
    /// Represents the note parameter for a phrase, including text and octave.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>"note" notated: Converts a degree number to a note number in key_mode.</description></item>
    /// <item><description>"auto" notated: Converts a degree number to a note number in span_mode.</description></item>
    /// </list>
    /// </remarks>
    /// <todo>
    /// Representing bass ghost notes with "gate".
    /// </todo>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Gets or sets the note text.
        /// </summary>
        string _text;

        /// <summary>
        /// Gets or sets the octave value.
        /// </summary>
        int _oct;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class.
        /// </summary>
        public Note() {
            _text = string.Empty;
            _oct = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class with the specified text and octave.
        /// </summary>
        /// <param name="text">Note text to parse.</param>
        /// <param name="oct">Octave value.</param>
        public Note(string text, int oct) {
            _text = PhraseValue(target: text);
            _oct = oct;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets or sets the note text.
        /// </summary>
        public string Text { get => _text; set => _text = PhraseValue(target: value); }

        /// <summary>
        /// Gets or sets the octave value.
        /// </summary>
        public int Oct { get => _oct; set => _oct = value; }

        /// <summary>
        /// Gets the pitch difference of notes (octave * 12).
        /// </summary>
        /// <value>Pitch difference in semitones.</value>
        public int Interval { get => _oct * 12; }

        /// <summary>
        /// Gets the note text as a filtered character array.
        /// </summary>
        /// <value>Filtered character array of note text, or null if not set.</value>
        public char[] TextCharArray { get => _text.HasValue() ? Filter(target: _text).ToCharArray() : null; }
    }

    /// <summary>
    /// Represents the chord parameter for a phrase, including text and range.
    /// </summary>
    public class Chord {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Gets or sets the chord text.
        /// </summary>
        string _text;

        /// <summary>
        /// Gets or sets the range for the chord.
        /// </summary>
        Range _range;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Chord"/> class.
        /// </summary>
        public Chord() {
            _text = string.Empty;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets or sets the chord text.
        /// </summary>
        public string Text { get => _text; set => _text = PhraseValue(target: value); }

        /// <summary>
        /// Gets or sets the range for the chord.
        /// </summary>
        public Range Range { get => _range; set => _range = value; }

        /// <summary>
        /// Gets the chord text as a filtered character array.
        /// </summary>
        /// <value>Filtered character array of chord text, or null if not set.</value>
        public char[] TextCharArray { get => _text.HasValue() ? Filter(target: _text).ToCharArray() : null; }
    }

    /// <summary>
    /// Represents the arpeggio (seque) parameter for a phrase, including text, stack, and range.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>Same concept as chord notated numbers (how many chords).</description></item>
    /// <item><description>The concept of Range is easier to handle if it is one octave. May be easier with a default range.</description></item>
    /// </list>
    /// </remarks>
    public class Seque {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Gets or sets the gate notation text.
        /// </summary>
        string _text;

        /// <summary>
        /// Gets or sets the stack count for arpeggio.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Same concept as chord notated numbers.</item>
        /// <item>Indicates how many chords are stacked.</item>
        /// </list>
        /// </remarks>
        int _stack;

        /// <summary>
        /// Gets or sets the range for the arpeggio.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The concept of Range is easier to handle if it is one octave.</item>
        /// <item>May be easier with a default range.</item>
        /// </list>
        /// </remarks>
        Range _range;

        /// <summary>
        /// Gets or sets whether the arpeggio is used.
        /// </summary>
        bool _use;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Seque"/> class.
        /// </summary>
        public Seque() {
            _text = string.Empty;
            _stack = 3;
            _use = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets or sets the gate notation text.
        /// </summary>
        public string Text { get => _text; set => _text = PhraseValue(target: value); }

        /// <summary>
        /// Gets or sets the stack count for arpeggio.
        /// </summary>
        public int Stack { get => _stack; set => _stack = value; }

        /// <summary>
        /// Gets or sets the range for the arpeggio.
        /// </summary>
        public Range Range { get => _range; set => _range = value; }

        /// <summary>
        /// Gets the gate notation text as a filtered character array.
        /// </summary>
        /// <value>Filtered character array of gate notation text, or null if not set.</value>
        public char[] TextCharArray { get => Text.HasValue() ? Filter(target: Text).ToCharArray() : null; }

        /// <summary>
        /// Gets or sets whether the arpeggio is used.
        /// </summary>
        public bool Use { get => _use; set => _use = value; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Converts the text mark to the note value.
        /// </summary>
        /// <param name="target">Text mark to convert.</param>
        /// <returns>Note value corresponding to the text mark.</returns>
        public static int ToGateValue(string target) {
            if (target.Equals(">")) { return TICK_INTERVAL * 3; }
            if (target.Equals("*")) { return TICK_INTERVAL * 2; }
            if (target.Equals("+")) { return TICK_INTERVAL * 1; }
            return TICK_INTERVAL * 0;
        }
    }

    /// <summary>
    /// Represents the range parameter for a phrase.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Range can be specified for each measure.</item>
    /// </list>
    /// </remarks>
    public class Range {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Gets or sets the minimum value of the range.
        /// </summary>
        int _min;
        /// <summary>
        /// Gets or sets the maximum value of the range.
        /// </summary>
        int _max;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class with the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">Minimum value of the range.</param>
        /// <param name="max">Maximum value of the range.</param>
        /// <exception cref="ArgumentException">Thrown if the range is invalid.</exception>
        public Range(int min, int max) {
            if (min < 0) {
                throw new ArgumentException("invalid range min.");
            }
            if (max > 127) {
                throw new ArgumentException("invalid range max.");
            }
            if (max - min != 11) { // octave out of range.
                int ok_max = min + 11;
                int ok_min = max - 11;
                throw new ArgumentException($"invalid range length,\r\nmust set {min}:{ok_max} or {ok_min}:{max}.");
            }
            _min = min;
            _max = max;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets the minimum value of the range.
        /// </summary>
        /// <value>Minimum value.</value>
        public int Min { get => _min; }

        /// <summary>
        /// Gets the maximum value of the range.
        /// </summary>
        /// <value>Maximum value.</value>
        public int Max { get => _max; }
    }

    /// <summary>
    /// Represents the expansion parameter for a phrase, including pre and post values.
    /// </summary>
    public class Exp {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Gets or sets the pre value for expansion.
        /// </summary>
        string _pre;

        /// <summary>
        /// Gets or sets the post value for expansion.
        /// </summary>
        string _post;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Exp"/> class.
        /// </summary>
        public Exp() {
            _pre = _post = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exp"/> class with the specified pre and post values.
        /// </summary>
        /// <param name="pre">Pre value for expansion.</param>
        /// <param name="post">Post value for expansion.</param>
        public Exp(string pre, string post) {
            _pre = pre;
            _post = post;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets or sets the pre value for expansion.
        /// </summary>
        public string Pre { get => _pre; set => _pre = value; }

        /// <summary>
        /// Gets or sets the post value for expansion.
        /// </summary>
        public string Post { get => _post; set => _post = value; }

        /// <summary>
        /// Gets whether the pre value is set.
        /// </summary>
        public bool HasPre {
            get {
                if (_pre is null) {
                    return false;
                }
                return !_pre.Equals(string.Empty);
            }
        }

        /// <summary>
        /// Gets whether the post value is set.
        /// </summary>
        public bool HasPost {
            get {
                if (_post is null) {
                    return false;
                }
                return !_post.Equals(string.Empty);
            }
        }

        /// <summary>
        /// Gets the pre value as a filtered character array.
        /// </summary>
        /// <value>Filtered character array of pre value, or null if not set.</value>
        public char[] PreCharArray {
            get {
                if (_pre == string.Empty) {
                    return null;
                }
                return Filter(target: _pre).ToCharArray();
            }
        }

        /// <summary>
        /// Gets the post value as a filtered character array.
        /// </summary>
        /// <value>Filtered character array of post value, or null if not set.</value>
        public char[] PostCharArray {
            get {
                if (_post == string.Empty) {
                    return null;
                }
                return Filter(target: _post).ToCharArray();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Gets whether the specified character matches as a pre parameter.
        /// </summary>
        /// <param name="target">Character to check for pre parameter match.</param>
        /// <returns>True if the character matches as a pre parameter; otherwise, false.</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Up to 120 * 2 ticks.</item>
        /// <item>Sixteenth note and eighth note syncopation only.</item>
        /// </list>
        /// </remarks>
        public bool IsMatchPre(char target) {
            return Regex.IsMatch(input: target.ToString(), pattern: @"^[1-2]+$");
        }
    }
}
