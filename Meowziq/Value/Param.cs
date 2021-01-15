
using System;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// MEMO: 不変オブジェクトに出来るかどうか
/// </summary>
namespace Meowziq.Value {
    /// <summary>
    /// Phrase クラスの Data クラス
    /// NOTE: Loader クラスから操作されるので公開必須
    /// </summary>
    public class Data {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Note note;

        Chord chord;

        Exp exp;

        Percussion[] percussionArray;

        string[] noteArray;

        int[] octArray;

        string[] preArray;

        string[] postArray;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Data() {
            this.note = new Note();
            this.chord = new Chord();
            this.exp = new Exp();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public Note Note {
            get => note;
            set => note = value;
        }

        public Chord Chord {
            get => chord;
            set => chord = value;
        }

        public Exp Exp {
            get => exp;
            set => exp = value; // デフォルト値は 0
        }

        /// <summary>
        /// Percussion の音色設定
        /// </summary>
        public Percussion[] PercussionArray {
            get => percussionArray;
            set => percussionArray = value;
        }

        /// <summary>
        /// Note テキストの配列
        /// </summary>
        public string[] NoteArray {
            get => noteArray;
            set => noteArray = value;
        }

        /// <summary>
        /// Note テキストのオクターブ設定
        /// </summary>
        public int[] OctArray {
            get => octArray;
            set {
                checkValue();
                if (value == null) {
                    octArray = new int[noteArray.Length];
                    octArray.Select(x => x = 0); // オクターブの設定を自動生成
                } else if (value.Length != noteArray.Length) {
                    throw new ArgumentException("noteOctArray must be same count as noteTextArray.");
                } else {
                    // TODO: バリデーション
                    octArray = value;
                }
            }
        }

        /// <summary>
        /// Note テキストの前方音価設定
        /// </summary>
        public string[] PreArray {
            get => preArray;
            set {
                checkValue();
                if (value == null) {
                    preArray = new string[noteArray.Length];
                    preArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != noteArray.Length) {
                    throw new ArgumentException("preArray must be same count as noteTextArray.");
                } else {
                    preArray = value; // TODO: バリデーション
                }
            }
        }

        /// <summary>
        /// Note テキストの後方音価設定
        /// </summary>
        public string[] PostArray {
            get => postArray;
            set {
                checkValue();
                if (value == null) {
                    postArray = new string[noteArray.Length];
                    postArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != noteArray.Length) {
                    throw new ArgumentException("postArray must be same count as noteTextArray.");
                } else {
                    postArray = value; // TODO: バリデーション
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void checkValue() {
            if (noteArray == null) {
                throw new ArgumentException("must set noteTextArray.");
            }
        }
    }

    /// <summary>
    /// Range パラメータクラス
    /// TODO: 範囲を小節内で指定出来るように
    /// </summary>
    public class Range {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Range(int min, int max) {
            Min = min;
            Max = max;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public int Min {
            get;
        }

        public int Max {
            get;
        }
    }

    /// <summary>
    /// NOTE: Expansion
    /// NOTE: 個別にプロパティ設定が必要
    /// </summary>
    public class Exp {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Exp() {
            Pre = "";
            Post = "";
        }

        public Exp(string pre, string post) {
            Pre = pre;
            Post = post;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public string Pre {
            get; set;
        }

        public string Post {
            get; set;
        }

        public bool HasPre {
            get {
                if (Pre == null) {
                    return false;
                }
                return !Pre.Equals("");
            }
        }

        public bool HasPost {
            get {
                if (Post == null) {
                    return false;
                }
                return !Post.Equals("");
            }
        }

        public char[] PreCharArray {
            get {
                if (Pre == "") {
                    return null;
                }
                return Utils.Filter(Pre).ToCharArray();
            }
        }

        public char[] PostCharArray {
            get {
                if (Post == "") {
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

    /// <summary>
    /// NOTE: 個別にプロパティ設定が必要
    /// </summary>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Note() {
            Text = "";
            Oct = 0;
        }

        public Note(string text, int oct) {
            Text = text;
            Oct = oct;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public string Text {
            get; set;
        }

        public int Oct {
            get; set;
        }

        public int Interval {
            get => Oct * 12; // オクターブを音程差に変換
        }

        public char[] TextCharArray {
            get {
                if (Text == "") {
                    return null;
                }
                return Utils.Filter(Text).ToCharArray();
            }
        }
    }

    /// <summary>
    /// NOTE: 個別にプロパティ設定が必要
    /// </summary>
    public class Chord {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        //public Chord(string text, Range range) {
        //    Text = text;
        //    Range = range;
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public string Text {
            get; set;
        }

        public Range Range {
            get; set;
        }

        public char[] TextCharArray {
            get {
                if (Text == "") {
                    return null;
                }
                return Utils.Filter(Text).ToCharArray();
            }
        }
    }

    /// <summary>
    /// パラメータクラス
    /// </summary>
    public class Param {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// ノート記述用パラメータ
        /// </summary>
        public Param(Note note, Exp exp, Way way) {
            Note = note;
            Exp = exp;
            Way = way;
        }

        /// <summary>
        /// ドラム記述用パラメータ
        /// </summary>
        public Param(Note note, int percussionNoteNum, Exp exp, Way way) {
            Note = note;
            PercussionNoteNum = percussionNoteNum;
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

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public Note Note {
            get;
        }

        public Chord Chord {
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

        public char[] TextCharArray {
            get {
                if (Way == Way.Mono || Way == Way.Multi || Way == Way.Drum) {
                    return Note.TextCharArray;
                } else if (Way == Way.Chord) {
                    return Chord.TextCharArray;
                }
                return null;
            }
        }

        public int Interval {
            get {
                if (Way == Way.Mono || Way == Way.Multi) {
                    return Note.Interval;
                }
                return 0; // Chord はインターバルなし
            }
        }

        public bool IsNote {
            get {
                if (Way == Way.Mono || Way == Way.Multi) {
                    return true;
                }
                return false;
            }
        }

        public bool IsChord {
            get {
                if (Way == Way.Chord) {
                    return true;
                }
                return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public bool IsMatch(char target) {
            if (Way == Way.Mono || Way == Way.Multi) {
                return Regex.IsMatch(target.ToString(), @"^[1-7]+$"); // 1～7まで度数の数値がある時
            } else if (Way == Way.Chord) {
                return Regex.IsMatch(target.ToString(), @"^[1-9]+$"); // chord モードは1～9
            } else if (Way == Way.Drum) {
                return target.ToString().Equals("x");
            }
            return false;
        }
    }
}
