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
    /// data class of phrase class.
    /// </summary>
    /// <note>
    /// used from the Loader class, so it must be made public.
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Data {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// "note" notated value.
        /// </summary>
        Note _note;

        /// <summary>
        /// whether "auto" notated value.
        /// </summary>
        bool _auto;

        /// <summary>
        /// "chord" notated value.
        /// </summary>
        Chord _chord;

        /// <summary>
        /// "seque" notated value.
        /// </summary>
        Seque _seque;

        Exp _exp;

        /// <summary>
        /// "data" notated: percussion names array.
        /// </summary>
        Percussion[] _percussion_array;

        /// <summary>
        /// "data" notated: drum value.
        /// </summary>
        string[] _beat_array;

        /// <summary>
        /// "data" notated: degree value for key_mode.
        /// </summary>
        string[] _note_array;

        /// <summary>
        /// "data" notated: degree value for span_mode.
        /// </summary>
        string[] _auto_array;

        /// <summary>
        /// "data" notated: octave value.
        /// </summary>
        int[] _oct_array;

        /// <summary>
        /// "data" notated: pre value.
        /// </summary>
        string[] _pre_array;

        /// <summary>
        /// "data" notated: post value.
        /// </summary>
        string[] _post_array;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Data() {
            _note = new();
            _auto = false;
            _chord = new();
            _seque = new();
            _exp = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public Note Note {
            get => _note; set => _note = value;
        }

        public bool Auto {
            get => _auto; set => _auto = value;
        }

        public Chord Chord {
            get => _chord; set => _chord = value;
        }

        public Seque Seque {
            get => _seque; set => _seque = value;
        }

        public Exp Exp {
            get => _exp; set => _exp = value; // default value is 0
        }

        /// <summary>
        /// provides the array of percussion sounds.
        /// </summary>
        public Percussion[] PercussionArray {
            get => _percussion_array;
            set {
                if (value.Length != arrayLength) {
                    throw new ArgumentException("percussionArray must be same count as beatArray.");
                } else {
                    _percussion_array = value;
                }
            }
        }

        /// <summary>
        /// provides array of "beat" notated text.
        /// </summary>
        public string[] BeatArray {
            get => _beat_array;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(action: x => PhraseValue(target: x));
                }
                _beat_array = value;
            }
        }

        /// <summary>
        /// provides array of "note" notated text.
        /// </summary>
        public string[] NoteArray {
            get => _note_array;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(action: x => PhraseValue(target: x));
                }
                _note_array = value;
            }
        }

        /// <summary>
        /// provides array of "auto" notated text.
        /// </summary>
        public string[] AutoArray {
            get => _auto_array;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(action: x => PhraseValue(target: x));
                }
                _auto_array = value;
                if (!(value is null)) {
                    _auto = true;
                }
            }
        }

        /// <summary>
        /// provides the octave parameter for "note" and "auto" notated text. 
        /// </summary>
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
        /// provides the pre note parameter for "beat", "note" and "auto" text.
        /// </summary>
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
        /// provides the post note parameter for "note" and "auto" text.
        /// </summary>
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
        /// gets whether has "beat" notated data.
        /// </summary>
        public bool HasBeat {
            get => !hasNote && !hasAuto && !hasChord && !hasSeque && hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// gets whether has "chord" notated data.
        /// </summary>
        public bool HasChord {
            get => !hasNote && !hasAuto && hasChord && !hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// gets whether has "seque" notated data.
        /// </summary>
        public bool HasSeque {
            get => !hasNote && !hasAuto && !hasChord && hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// gets whether has "note" notated data.
        /// </summary>
        public bool HasNote {
            get => (hasNote || hasNoteArray) && !hasAuto && !hasChord && !hasSeque && !hasBeatArray && !hasAutoArray;
        }

        /// <summary>
        /// gets whether has "auto" notated data.
        /// </summary>
        public bool HasAuto {
            get => !hasNote && !hasNoteArray && (hasAuto || hasAutoArray) && !hasChord && !hasSeque && !hasBeatArray;
        }

        /// <summary>
        /// gets whether has the array notated data.
        /// </summary>
        public bool HasMulti {
            get => !hasNote && !hasAuto && !hasChord && !hasSeque && (hasBeatArray || hasNoteArray || hasAutoArray);
        }

        /// <summary>
        /// gets whether has no data.
        /// </summary>
        public bool HasNoData {
            get => !hasNote && !hasAuto && !hasChord && !hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Properties [noun, adjective] 

        int arrayLength {
            get {
                if (_beat_array != null) {
                    return _beat_array.Length;
                }
                if (_note_array != null) {
                    return _note_array.Length;
                }
                if (_auto_array != null) {
                    return _auto_array.Length;
                }
                return 0;
            }
        }

        bool hasAnyArray {
            get {
                if (_beat_array != null || _note_array != null || _auto_array != null) { // have something.
                    return true;
                }
                return false; // does not have beat, note, or auto.
            }
        }

        public bool hasBeatArray {
            get => _beat_array != null;
        }

        public bool hasNoteArray {
            get => _note_array != null && _auto == false;
        }

        public bool hasAutoArray {
            get => _auto_array != null && _auto == true;
        }

        bool hasNote {
            get => !_note.Text.Equals(string.Empty) && _auto == false;
        }

        bool hasAuto {
            get => !_note.Text.Equals(string.Empty) && _auto == true;
        }

        bool hasChord {
            get => _chord.Text != null; // TODO: is Text.Equals("") correct?
        }

        bool hasSeque {
            get => _seque.Use;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void checkTextArray() {
            if (!hasAnyArray) {
                throw new ArgumentException("must set beatArray or noteArray or autoArray.");
            }
        }
    }

    /// <summary>
    /// note parameter class.
    /// </summary>
    /// <memo>
    /// + "note" notated <br/>
    ///     + converts a degree number to a note number in key_mode. <br/>
    /// + "auto" notated <br/> 
    ///     + converts a degree number to a note number in span_mode. <br/>
    /// </memo>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _text;

        int _oct;

        // TODO: representing bass ghost notes with "gate".

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Note() {
            _text = string.Empty;
            _oct = 0;
        }

        public Note(string text, int oct) {
            _text = PhraseValue(target: text);
            _oct = oct;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Text {
            get => _text; set => _text = PhraseValue(target: value);
        }

        public int Oct {
            get => _oct; set => _oct = value;
        }

        /// <summary>
        /// gets the pitch difference of notes.
        /// </summary>
        public int Interval {
            get => _oct * 12; // converts the octave value to the pitch difference of notes.
        }

        public char[] TextCharArray {
            get => _text.HasValue() ? Filter(target: _text).ToCharArray() : null;
        }
    }

    /// <summary>
    /// chord parameter class.
    /// </summary>
    public class Chord {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _text;

        Range _range;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Chord() {
            _text = string.Empty;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Text {
            get => _text; set => _text = PhraseValue(target: value);
        }

        public Range Range {
            get => _range; set => _range = value;
        }

        public char[] TextCharArray {
            get => _text.HasValue() ? Filter(target: _text).ToCharArray() : null;
        }
    }

    /// <summary>
    /// arpeggio parameter class
    /// </summary>
    public class Seque {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// "gate" notated.
        /// </summary>
        string _text;

        /// <memo>
        /// + same concept as "chord" notated numbers. <br/>
        ///     + how many chords? <br/>
        /// </memo>
        int _stack; // * before development.

        /// <memo>
        /// + the concept of Range is easier to handle if it is one octave. <br/>
        /// + isn't it easier to handle with a default range? <br/>
        /// </memo>
        Range _range;

        /// <summary>
        /// whether use or not.
        /// </summary>
        bool _use;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Seque() {
            _text = string.Empty;
            _stack = 3; // * before development.
            _use = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Text {
            get => _text; set => _text = PhraseValue(target: value);
        }

        public int Stack {
            get => _stack; set => _stack = value;
        }

        public Range Range {
            get => _range; set => _range = value; 
        }

        public char[] TextCharArray {
            get => Text.HasValue() ? Filter(target: Text).ToCharArray() : null;
        }

        public bool Use {
            get => _use; set => _use = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// converts the text mark to the note value.
        /// </summary>
        public static int ToGateValue(string target) {
            if (target.Equals(">")) { return TICK_INTERVAL * 3; }
            if (target.Equals("*")) { return TICK_INTERVAL * 2; }
            if (target.Equals("+")) { return TICK_INTERVAL * 1; }
            return TICK_INTERVAL * 0;
        }
    }

    /// <summary>
    /// range parameter class.
    /// </summary>
    /// <todo>
    /// range can be specified for each measure.
    /// </todo>
    public class Range {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int _min, _max;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

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

        public int Min {
            get => _min;
        }

        public int Max {
            get => _max;
        }
    }

    /// <summary>
    /// expansion parameter class.
    /// </summary>
    public class Exp {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _pre, _post;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Exp() {
            _pre = _post = string.Empty;
        }

        public Exp(string pre, string post) {
            _pre = pre;
            _post = post;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Pre {
            get => _pre; set => _pre = value;
        }

        public string Post {
            get => _post; set => _post = value;
        }

        public bool HasPre {
            get {
                if (_pre is null) {
                    return false;
                }
                return !_pre.Equals(string.Empty);
            }
        }

        public bool HasPost {
            get {
                if (_post is null) {
                    return false;
                }
                return !_post.Equals(string.Empty);
            }
        }

        public char[] PreCharArray {
            get {
                if (_pre == string.Empty) {
                    return null;
                }
                return Filter(target: _pre).ToCharArray();
            }
        }

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
        /// gets whether it matches as a pre parameter.
        /// </summary>
        /// <note>
        /// + up to 120 * 2 ticks. <br/>
        /// + sixteenth note and eighth note syncopation only. <br/>
        /// </note>
        public bool IsMatchPre(char target) {
            return Regex.IsMatch(target.ToString(), @"^[1-2]+$");
        }
    }
}
