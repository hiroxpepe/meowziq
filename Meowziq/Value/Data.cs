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
        /// <note_jp>
        /// + note記法とは？ <br/>
        /// + 度数指定に Key_mode の旋法が適用されます <br/>
        /// + 歌メロなどを想定します <br/>
        /// </note_jp>
        Note _note;

        /// <summary>
        /// whether "auto" notated value.
        /// </summary>
        /// <note_jp>
        /// + auto記法とは？ <br/>
        /// + 度数指定に Span_mode の旋法が適用されます <br/>
        /// + ベースラインなどを想定します <br/>
        /// </note_jp>
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
        /// data記述：ドラム Track パーカッション Note No
        /// </summary>
        Percussion[] _percussion_array;

        /// <summary>
        /// data記述：ドラム音符
        /// </summary>
        string[] _beat_array;

        /// <summary>
        /// data記述：Key 旋法の度数指定 ⇒ 歌メロなどを想定
        /// </summary>
        string[] _note_array; 

        /// <summary>
        /// data記述：Span 旋法の度数指定 ⇒ ベースラインなどを想定
        /// </summary>
        string[] _auto_array;

        /// <summary>
        /// data記述：オクターブ
        /// </summary>
        int[] _oct_array;

        string[] _pre_array;

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
            get => _note;
            set => _note = value;
        }

        public bool Auto {
            get => _auto;
            set => _auto = value;
        }

        public Chord Chord {
            get => _chord;
            set => _chord = value;
        }

        public Seque Seque {
            get => _seque;
            set => _seque = value;
        }

        public Exp Exp {
            get => _exp;
            set => _exp = value; // default value is 0
        }

        /// <summary>
        /// percussion sound array.
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
        /// array of "beat" text
        /// </summary>
        public string[] BeatArray {
            get => _beat_array;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                }
                _beat_array = value;
            }
        }

        /// <summary>
        /// array of "note" text
        /// </summary>
        public string[] NoteArray {
            get => _note_array;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                }
                _note_array = value;
            }
        }

        /// <summary>
        /// array of "auto" text
        /// </summary>
        public string[] AutoArray {
            get => _auto_array;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                }
                _auto_array = value;
                if (!(value is null)) {
                    _auto = true;
                }
            }
        }

        /// <summary>
        /// "note", "auto" テキストのオクターブ設定
        /// </summary>
        public int[] OctArray {
            get => _oct_array;
            set {
                checkTextArray();
                if (value is null) {
                    _oct_array = new int[arrayLength];
                    _oct_array.Select(x => x = 0); // automatically generate octave values.
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("noteOctArray must be same count as noteArray or autoArray.");
                } else {
                    _oct_array = value; // TODO: validation
                }
            }
        }

        /// <summary>
        /// "beat", "note", "auto" テキストの前方音価設定
        /// </summary>
        public string[] PreArray {
            get => _pre_array;
            set {
                checkTextArray();
                if (value is null) {
                    _pre_array = new string[arrayLength];
                    _pre_array.Select(x => x = null); // automatically generate initial values.
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("preArray must be same count as beatArray or noteArray or autoArray.");
                } else {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                    _pre_array = value;
                }
            }
        }

        /// <summary>
        /// "note", "auto" テキストの後方音価設定
        /// </summary>
        public string[] PostArray {
            get => _post_array;
            set {
                checkTextArray();
                if (value is null) {
                    _post_array = new string[arrayLength];
                    _post_array.Select(x => x = null); // automatically generate initial values.
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("postArray must be same count as noteArray or autoArray.");
                } else {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                    _post_array = value;
                }
            }
        }

        /// <summary>
        /// whether or not to have "beat" notated data.
        /// </summary>
        public bool HasBeat {
            get => !hasNote && !hasAuto && !hasChord && !hasSeque && hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// whether or not to have "chord" notated data.
        /// </summary>
        public bool HasChord {
            get => !hasNote && !hasAuto && hasChord && !hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// whether or not to have "seque" notated data.
        /// </summary>
        public bool HasSeque {
            get => !hasNote && !hasAuto && !hasChord && hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// whether or not to have "note" notated data.
        /// </summary>
        public bool HasNote {
            get => (hasNote || hasNoteArray) && !hasAuto && !hasChord && !hasSeque && !hasBeatArray && !hasAutoArray;
        }

        /// <summary>
        /// whether or not to have "auto" notated data.
        /// </summary>
        public bool HasAuto {
            get => !hasNote && !hasNoteArray && (hasAuto || hasAutoArray) && !hasChord && !hasSeque && !hasBeatArray;
        }

        /// <summary>
        /// whether or not to have array notated data.
        /// </summary>
        public bool HasMulti {
            get => !hasNote && !hasAuto && !hasChord && !hasSeque && (hasBeatArray || hasNoteArray || hasAutoArray);
        }

        /// <summary>
        /// whether or not to have data.
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
    /// <remarks_jp>
    /// + "note" 記法 <br/>
    ///     + Key_mode の旋法で度数数値を note number に変換します <br/>
    /// + "auto" 記法 <br/> 
    ///     + Span_mode の旋法で度数数値を note number に変換します <br/>
    /// </remarks_jp>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _text;

        // TODO: gate で Bass のゴーストノートなどを表現 

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Note() {
            Text = string.Empty;
            Oct = 0;
        }

        public Note(string text, int oct) {
            Text = text;
            Oct = oct;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Text {
            get => _text;
            set => _text = Value.Validater.PhraseValue(value);
        }

        public int Oct {
            get; set;
        }

        /// <summary>
        /// pitch difference of notes.
        /// </summary>
        public int Interval {
            get => Oct * 12; // converts the octave value to the pitch difference of notes.
        }

        public char[] TextCharArray {
            get => Text.HasValue() ? Utils.Filter(Text).ToCharArray() : null;
        }
    }

    /// <summary>
    /// chord parameter class.
    /// </summary>
    /// <note>
    /// 個別にプロパティ設定が必要
    /// </note>
    public class Chord {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _text;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Chord() {
            Text = string.Empty;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Text {
            get => _text;
            set => _text = Value.Validater.PhraseValue(value);
        }

        public Range Range {
            get; set;
        }

        public char[] TextCharArray {
            get => Text.HasValue() ? Utils.Filter(Text).ToCharArray() : null;
        }
    }

    /// <summary>
    /// arpeggio parameter class.
    /// </summary>
    public class Seque {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// MEMO: 120 が16分音符、30:60:90:120 で4音価にするか？ ⇒ 100%音価は効果が薄いので必要ない
        ///     '-' は無効の文字としてのみ使用されるべき
        ///     '+*>' の3文字で設定: ⇒ -: 0%, +: 25%, *:50%, >75%
        /// </summary>
        string _text; // gate 設定用

        /// <summary>
        /// NOTE: Chord 記述の数値と同じ概念
        /// </summary>
        int _stack; // 何音コードにするか

        bool _use;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Seque() {
            Text = string.Empty;
            _stack = 3;
            _use = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Text {
            get => _text;
            set => _text = Value.Validater.PhraseValue(value);
        }

        public int Stack {
            get; set;
        }

        /// <summary>
        /// MEMO: Range(範囲)という概念は1オクターブとした方が扱いやすいが
        /// </summary>
        public Range Range {
            get; set; // TODO: デフォルト範囲を設けた方が扱いやすいのでは？
        }

        public char[] TextCharArray {
            get => Text.HasValue() ? Utils.Filter(Text).ToCharArray() : null;
        }

        public bool Use {
            get => _use;
            set => _use = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// converts the text mark to the note value.
        /// </summary>
        public static int ToGate(string target) {
            if (target.Equals("+")) {
                return 30;
            } else if (target.Equals("*")) {
                return 60;
            } else if (target.Equals(">")) {
                return 90;
            }
            return 0;
        }
    }

    /// <summary>
    /// range parameter class.
    /// </summary>
    /// <todo>
    /// 範囲を小節内で指定出来るように
    /// </todo>
    public class Range {

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
                var ok_max = min + 11;
                var ok_min = max - 11;
                throw new ArgumentException($"invalid range length,\r\nmust set {min}:{ok_max} or {ok_min}:{max}.");
            }
            Min = min;
            Max = max;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public int Min {
            get;
        }

        public int Max {
            get;
        }
    }

    /// <summary>
    /// expansion parameter class.
    /// </summary>
    /// <note>
    /// 個別にプロパティ設定が必要
    /// </note>
    public class Exp {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Exp() {
            Pre = string.Empty;
            Post = string.Empty;
        }

        public Exp(string pre, string post) {
            Pre = pre;
            Post = post;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Pre {
            get; set;
        }

        public string Post {
            get; set;
        }

        public bool HasPre {
            get {
                if (Pre is null) {
                    return false;
                }
                return !Pre.Equals(string.Empty);
            }
        }

        public bool HasPost {
            get {
                if (Post is null) {
                    return false;
                }
                return !Post.Equals(string.Empty);
            }
        }

        public char[] PreCharArray {
            get {
                if (Pre.Equals(string.Empty)) {
                    return null;
                }
                return Utils.Filter(Pre).ToCharArray();
            }
        }

        public char[] PostCharArray {
            get {
                if (Post.Equals(string.Empty)) {
                    return null;
                }
                return Utils.Filter(Post).ToCharArray();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public bool IsMatchPre(char target) {
            return Regex.IsMatch(target.ToString(), @"^[1-2]+$");  // 120 * 2 tick まで ⇒ 16分・8分音符のシンコぺのみ
        }
    }
}
