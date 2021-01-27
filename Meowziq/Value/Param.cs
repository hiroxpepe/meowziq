
using System;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// MEMO: 不変オブジェクトに出来るかどうか
/// </summary>
namespace Meowziq.Value {
    /// <summary>
    /// NOTE: 入力値バリデーション クラス
    /// </summary>
    internal class Validate {
        /// <summary>
        /// TODO: 使用可能な文字の判定
        /// </summary>
        internal static string PhraseValue(string target) {
            if (target is null) {
                return target; // 値がなければそのまま返す FIXME:
            }
            // 拍のデータの数が4文字かどうか
            var _target1 = target;
            _target1 = _target1.Replace("[", "|").Replace("]", "|"); // 文字の置き換え
            var _array1 = _target1.Split('|') // 区切り文字で切り分ける
                .Where(x => !string.IsNullOrWhiteSpace(x)) // 空文字以外が対象で
                .Where(x => x.Length != 4) // そのデータが4文字ではないものを抽出
                .ToArray();
            if (_array1.Length != 0) { // そのデータがあれば例外を投げる
                throw new FormatException("data count must be 4.");
            }
            return target; // バリデーションOKなら元々の文字列を返す
        }
    }

    /// <summary>
    /// NOTE: 入力値用の Utils クラス
    /// </summary>
    internal class Utils {
        /// <summary>
        /// 不要文字 "[", "]", "|", を削除します
        /// </summary>
        internal static string Filter(string target) {
            return target.Replace("|", "").Replace("[", "").Replace("]", ""); // 不要文字削除
        }
    }

    /// <summary>
    /// Phrase クラスの Data クラス
    /// NOTE: Loader クラスから操作されるので公開必須
    /// </summary>
    public class Data {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Note note; // note記述：Key 旋法の度数指定 ⇒ 歌メロなどを想定、auto記述：Span 旋法の度数指定 ⇒ ベースラインなどを想定

        bool auto; // auto記述かどうか

        Chord chord; // chord記述

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
            this.auto = false;
            this.chord = new Chord();
            this.exp = new Exp();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public Note Note {
            get => note;
            set => note = value;
        }

        public bool Auto {
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
                if (value.Length != arrayLength) {
                    throw new ArgumentException("percussionArray must be same count as beatArray.");
                } else {
                    percussionArray = value;
                }
            }
        }

        /// <summary>
        /// "beat" テキストの配列
        /// </summary>
        public string[] BeatArray {
            get => beatArray;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(x => Value.Validate.PhraseValue(x));
                }
                beatArray = value;
            }
        }

        /// <summary>
        /// "note" テキストの配列
        /// </summary>
        public string[] NoteArray {
            get => noteArray;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(x => Value.Validate.PhraseValue(x));
                }
                noteArray = value;
            }
        }

        /// <summary>
        /// "auto" テキストの配列
        /// </summary>
        public string[] AutoArray {
            get => autoArray;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(x => Value.Validate.PhraseValue(x));
                }
                autoArray = value;
                if (!(value is null)) {
                    auto = true;
                }
            }
        }

        /// <summary>
        /// "note", "auto" テキストのオクターブ設定
        /// </summary>
        public int[] OctArray {
            get => octArray;
            set {
                checkTextArray();
                if (value is null) {
                    octArray = new int[arrayLength];
                    octArray.Select(x => x = 0); // オクターブの設定を自動生成
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("noteOctArray must be same count as noteArray or autoArray.");
                } else {
                    octArray = value; // TODO: バリデーション
                }
            }
        }

        /// <summary>
        /// "beat", "note", "auto" テキストの前方音価設定
        /// </summary>
        public string[] PreArray {
            get => preArray;
            set {
                checkTextArray();
                if (value is null) {
                    preArray = new string[arrayLength];
                    preArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("preArray must be same count as beatArray or noteArray or autoArray.");
                } else {
                    value.ToList().ForEach(x => Value.Validate.PhraseValue(x));
                    preArray = value;
                }
            }
        }

        /// <summary>
        /// "note", "auto" テキストの後方音価設定
        /// </summary>
        public string[] PostArray {
            get => postArray;
            set {
                checkTextArray();
                if (value is null) {
                    postArray = new string[arrayLength];
                    postArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("postArray must be same count as noteArray or autoArray.");
                } else {
                    value.ToList().ForEach(x => Value.Validate.PhraseValue(x));
                    postArray = value;
                }
            }
        }

        /// <summary>
        /// "beat" 記述のデータを持つかどうか
        /// </summary>
        public bool HasBeat {
            get => !hasNote && !hasAuto && !hasChord && hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// "chord" 記述のデータを持つかどうか
        /// </summary>
        public bool HasChord {
            get => !hasNote && !hasAuto && hasChord && !hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// "note" 記述のデータを持つかどうか
        /// </summary>
        public bool HasNote {
            get => (hasNote || hasNoteArray) && !hasAuto && !hasChord && !hasBeatArray && !hasAutoArray;
        }

        /// <summary>
        /// "auto" 記述のデータを持つかどうか
        /// </summary>
        public bool HasAuto {
            get => !hasNote && !hasNoteArray && (hasAuto || hasAutoArray) && !hasChord && !hasBeatArray;
        }

        /// <summary>
        /// Array 記述のデータを持つかどうか
        /// </summary>
        public bool HasMulti {
            get => !hasNote && !hasAuto && !hasChord && (hasBeatArray || hasNoteArray || hasAutoArray);
        }

        /// <summary>
        /// データを持たないかどうか
        /// </summary>
        public bool HasNoData {
            get => !hasNote && !hasAuto && !hasChord && !hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Properties [noun, adjective] 

        int arrayLength {
            get {
                if (beatArray != null) {
                    return beatArray.Length;
                }
                if (noteArray != null) {
                    return noteArray.Length;
                }
                if (autoArray != null) {
                    return autoArray.Length;
                }
                return 0;
            }
        }

        bool hasAnyArray {
            get {
                if (beatArray != null || noteArray != null || autoArray != null) { // 何かは持つ
                    return true;
                }
                return false; // beat、note、auto いずれも持たない
            }
        }

        public bool hasBeatArray {
            get => beatArray != null;
        }

        public bool hasNoteArray {
            get => noteArray != null && auto == false;
        }

        public bool hasAutoArray {
            get => autoArray != null && auto == true;
        }

        bool hasNote {
            get => !note.Text.Equals("") && auto == false;
        }

        bool hasAuto {
            get => !note.Text.Equals("") && auto == true;
        }

        bool hasChord {
            get => chord.Text != null;
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
    /// Range パラメータクラス
    /// TODO: 範囲を小節内で指定出来るように
    /// </summary>
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
            if (max - min != 11) { // オクターブの範囲外
                var _okMax = min + 11;
                var _okMin = max - 11;
                throw new ArgumentException($"invalid range length,\r\nmust set {min}:{_okMax} or {_okMin}:{max}.");
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
                if (Pre is null) {
                    return false;
                }
                return !Pre.Equals("");
            }
        }

        public bool HasPost {
            get {
                if (Post is null) {
                    return false;
                }
                return !Post.Equals("");
            }
        }

        public char[] PreCharArray {
            get {
                if (Pre.Equals("")) {
                    return null;
                }
                return Utils.Filter(Pre).ToCharArray();
            }
        }

        public char[] PostCharArray {
            get {
                if (Post.Equals("")) {
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
    /// note: Keyの旋法で度数数値を Note No に変換します
    /// auto: Spanの旋法で度数数値を Note No に変換します
    /// </summary>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string text;

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
            get => text;
            set => text = Value.Validate.PhraseValue(value);
        }

        public int Oct {
            get; set;
        }

        public int Interval {
            get => Oct * 12; // オクターブを音程差に変換
        }

        public char[] TextCharArray {
            get {
                if (Text.Equals("")) {
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
        // Fields

        string text;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Text {
            get => text;
            set => text = Value.Validate.PhraseValue(value);
        }

        public Range Range {
            get; set;
        }

        public char[] TextCharArray {
            get {
                if (Text.Equals("")) {
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
        public Param(Note note, Exp exp, Way way, bool autoNote = true) {
            this.note = note;
            Exp = exp;
            Way = way;
            AutoNote = autoNote;
        }

        /// <summary>
        /// ドラム記述用パラメータ
        /// </summary>
        public Param(Note note, int percussionNoteNum, Exp exp, Way way) {
            this.note = note;
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

        public Chord Chord {
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
                }
                return null;
            }
        }

        public int Interval {
            get {
                if (Way is Way.Mono || Way is Way.Multi) {
                    return note.Interval;
                }
                return 0; // Chord はインターバルなし
            }
        }

        public bool IsNote {
            get {
                if (Way is Way.Mono || Way is Way.Multi) {
                    return true;
                }
                return false;
            }
        }

        public bool IsChord {
            get {
                if (Way is Way.Chord) {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Span の旋法で　Note No を取得
        /// </summary>
        public bool AutoNote {
            get; set;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public bool IsMatch(char target) {
            if (Way is Way.Mono || Way is Way.Multi) {
                return Regex.IsMatch(target.ToString(), @"^[1-7]+$"); // 1～7まで度数の数値がある時
            } else if (Way is Way.Chord) {
                return Regex.IsMatch(target.ToString(), @"^[1-9]+$"); // chord モードは1～9
            } else if (Way is Way.Drum) {
                return target.ToString().Equals("x");
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
