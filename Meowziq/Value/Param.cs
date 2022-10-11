
using System.Text.RegularExpressions;

/// <summary>
/// Generator クラスに引数として渡すパラメータークラス
/// MEMO: 不変オブジェクトに出来るかどうか
/// </summary>
namespace Meowziq.Value {
    /// <summary>
    /// パラメータクラス
    /// </summary>
    public class Param {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// ノート記述用パラメータ
        /// MEMO: メロディ、ハモリ記述と共用出来る？ ⇒ 多少無駄があっても別々で組んで後で統合した方が現状の破壊が少ない
        /// 
        /// </summary>
        public Param(Note note, Exp exp, Way way, bool auto_note = true) {
            this.note = note;
            Exp = exp;
            Way = way;
            AutoNote = auto_note;
        }

        /// <summary>
        /// ドラム記述用パラメータ
        /// </summary>
        public Param(Note note, int percussion_note_num, Exp exp, Way way) {
            this.note = note;
            PercussionNoteNum = percussion_note_num;
            Exp = exp;
            Way = way;
        }

        /// <summary>
        /// コード記述用パラメータ
        /// </summary>
        public Param(Chord chord, Exp exp, Way type) {
            Chord = chord;
            Exp = exp;
            Way = type;
        }

        /// <summary>
        /// アルペジオ記述用パラメータ
        /// </summary>
        public Param(Seque seque, Way type) {
            Seque = seque;
            Way = type;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public Chord Chord {
            get; // NOTE: Range が Generator から使用される
        }

        public Seque Seque {
            get; // NOTE: Range が Generator から使用される
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

        /// <summary>
        /// NOTE: ここで char 配列が返せれば Generator 側は問題ない
        /// </summary>
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
        /// ノート記述用パラメータかどうか
        /// NOTE: Generator クラスから使用されます
        /// </summary>
        public bool IsNote {
            get => Way is Way.Mono || Way is Way.Multi;
        }

        /// <summary>
        /// コード記述用パラメータかどうか
        /// NOTE: Generator クラスから使用されます
        /// </summary>
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
