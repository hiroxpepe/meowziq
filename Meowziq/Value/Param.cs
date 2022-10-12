
using System.Text.RegularExpressions;

/// <summary>
/// parameter class to provide as an argument to the generator class.
/// </summary>
/// <memo>
/// whether it can be an immutable object.
/// </memo>
namespace Meowziq.Value {
    /// <summary>
    /// parameter class.
    /// </summary>
    public class Param {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// creates as a "note" notated parameter.
        /// </summary>
        /// <memo>
        /// メロディ、ハモリ記述と共用出来る？ ⇒ 多少無駄があっても別々で組んで後で統合した方が現状の破壊が少ない
        /// </memo>
        public Param(Note note, Exp exp, Way way, bool auto_note = true) {
            this.note = note;
            Exp = exp;
            Way = way;
            AutoNote = auto_note;
        }

        /// <summary>
        /// creates as a "beat" notated parameter.
        /// </summary>
        public Param(Note note, int percussion_note_num, Exp exp, Way way) {
            this.note = note;
            PercussionNoteNum = percussion_note_num;
            Exp = exp;
            Way = way;
        }

        /// <summary>
        /// creates as a "chord" notated parameter.
        /// </summary>
        public Param(Chord chord, Exp exp, Way type) {
            Chord = chord;
            Exp = exp;
            Way = type;
        }

        /// <summary>
        /// creates as a "seque" notated parameter.
        /// </summary>
        public Param(Seque seque, Way type) {
            Seque = seque;
            Way = type;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <remarks>
        /// Range が Generator から使用される
        /// </remarks>
        public Chord Chord {
            get;
        }

        /// <remarks>
        /// Range が Generator から使用される
        /// </remarks>
        public Seque Seque {
            get;
        }

        public Exp Exp {
            get;
        }

        public int PercussionNoteNum {
            get;
        }

        public Way Way {
            get;
        }

        /// <remarks>
        /// ここで char 配列が返せれば Generator 側は問題ない
        /// </remarks>
        public char[] TextCharArray {
            get {
                if (Way is Way.Mono || Way is Way.Multi || Way is Way.Drum) {
                    return note.TextCharArray;
                } else if (Way is Way.Chord) {
                    return Chord.TextCharArray;
                } else if (Way is Way.Seque) {
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
                if (Way is Way.Mono || Way is Way.Multi) {
                    return note.Interval;
                }
                return 0; // Chord はインターバルなし
            }
        }

        /// <summary>
        /// whether "note" notated parameter.
        /// </summary>
        /// <remarks>
        /// Generator クラスから使用されます
        /// </remarks>
        public bool IsNote {
            get => Way is Way.Mono || Way is Way.Multi;
        }

        /// <summary>
        /// whether "chord" notated parameter.
        /// </summary>
        /// <remarks>
        /// Generator クラスから使用されます
        /// </remarks>
        public bool IsChord {
            get => Way is Way.Chord;
        }

        /// <summary>
        /// Span の旋法で Note No を取得
        /// </summary>
        public bool AutoNote {
            get;
            private set;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public bool IsMatch(char target) {
            if (Way is Way.Mono || Way is Way.Multi) {
                return Regex.IsMatch(target.ToString(), @"^[1-7]+$"); // Mono, Multi は 1～7
            } else if (Way is Way.Chord) {
                return Regex.IsMatch(target.ToString(), @"^[1-9]+$"); // Chord は 1～9
            } else if (Way is Way.Drum) {
                return target.ToString().Equals("x"); // Drum は 'x'
            } else if (Way is Way.Seque) {
                return target.ToString().Equals("+") || target.ToString().Equals("*") || target.ToString().Equals(">"); // Seque は '+', '*', '>'
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
