
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

        Note note; // note記述：Key 旋法の度数指定 ⇒ 歌メロなどを想定

        Auto auto; // auto記述：Span 旋法の度数指定 ⇒ ベースラインなどを想定

        Chord chord;

        Exp exp;

        Percussion[] percussionArray; // data記述：ドラム Track パーカッション　Note No

        string[] beatArray; // data記述：ドラム音符

        string[] noteArray; // data記述：Key 旋法の度数指定 ⇒ 歌メロなどを想定

        string[] autoArray; // data記述：Span 旋法の度数指定 ⇒ ベースラインなどを想定

        int[] octArray;

        string[] preArray;

        string[] postArray;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Data() {
            this.note = new Note();
            this.auto = new Auto();
            this.chord = new Chord();
            this.exp = new Exp();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public Note Note {
            get => note;
            set => note = value;
        }

        public Auto Auto {
            get => auto;
            set => auto = value;
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
            set {
                if (value.Length != noteArray.Length) {
                    throw new ArgumentException("percussionArray must be same count as noteTextArray.");
                } else {
                    percussionArray = value;
                }
            }
        }

        /// <summary>
        /// Beat テキストの配列
        /// </summary>
        public string[] BeatArray {
            get => beatArray;
            set => beatArray = value;
        }

        /// <summary>
        /// Note テキストの配列
        /// </summary>
        public string[] NoteArray {
            get => noteArray;
            set => noteArray = value;
        }

        /// <summary>
        /// Auto テキストの配列
        /// </summary>
        public string[] AutoArray {
            get => autoArray;
            set => autoArray = value;
        }

        /// <summary>
        /// Note, Auto テキストのオクターブ設定
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
                    octArray = value; // TODO: バリデーション
                }
            }
        }

        /// <summary>
        /// Note, Auto テキストの前方音価設定
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
        /// Note, Auto テキストの後方音価設定
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

        // TODO: このバリデーションを使う
        /// <summary>
        /// FIXME: バリデーターは Loader ではなく Value クラスに移動する
        /// TODO: 使用可能な文字
        /// </summary>
        static string validateValue(string target) {
            if (target == null) {
                return target; // 値がなければそのまま返す FIXME:
            }
            // 拍のデータの数が4文字かどうか
            var _target1 = target;
            // 文字の置き換え
            _target1 = _target1.Replace("[", "|").Replace("]", "|");
            // 区切り文字で切り分ける
            var _array1 = _target1.Split('|')
                .Where(x => !string.IsNullOrWhiteSpace(x)) // 空文字以外
                .Where(x => x.Length != 4) // データが4文字ではない
                .ToArray();
            // そのデータがあれば例外を投げる
            if (_array1.Length != 0) {
                throw new FormatException("data count must be 4.");
            }
            // バリデーションOKなら元々の文字列を返す
            return target;
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
        // Properties [noun, adjective] 

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
        // Properties [noun, adjective] 

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
    /// Keyの旋法で度数数値を Note No に変換します
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
        // Properties [noun, adjective] 

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
    /// Spanの旋法で度数数値を Note No に変換します
    /// </summary>
    public class Auto {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Auto() {
            Text = "";
            Oct = 0;
        }

        public Auto(string text, int oct) {
            Text = text;
            Oct = oct;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

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
        // Properties [noun, adjective] 

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
        /// MEMO: メロディ、ハモリ記述と共用出来る？ ⇒ 多少無駄があっても別々で組んで後で統合した方が現状の破壊が少ない
        /// 
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
        // Properties [noun, adjective] 

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

        /// <summary>
        /// MEMO: ここで char 配列が返せれば Generator 側は問題ない
        /// </summary>
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
