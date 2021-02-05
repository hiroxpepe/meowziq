
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Meowziq.Value {
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

        Seque seque; // seque記述

        Exp exp;

        Percussion[] percussionArray; // data記述：ドラム Track パーカッション Note No

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
            this.seque = new Seque();
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

        public Seque Seque {
            get => seque;
            set => seque = value;
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
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
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
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
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
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
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
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
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
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                    postArray = value;
                }
            }
        }

        /// <summary>
        /// "beat" 記述のデータを持つかどうか
        /// </summary>
        public bool HasBeat {
            get => !hasNote && !hasAuto && !hasChord && !hasSeque && hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// "chord" 記述のデータを持つかどうか
        /// </summary>
        public bool HasChord {
            get => !hasNote && !hasAuto && hasChord && !hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// "seque" 記述のデータを持つかどうか
        /// </summary>
        public bool HasSeque {
            get => !hasNote && !hasAuto && !hasChord && hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray;
        }

        /// <summary>
        /// "note" 記述のデータを持つかどうか
        /// </summary>
        public bool HasNote {
            get => (hasNote || hasNoteArray) && !hasAuto && !hasChord && !hasSeque && !hasBeatArray && !hasAutoArray;
        }

        /// <summary>
        /// "auto" 記述のデータを持つかどうか
        /// </summary>
        public bool HasAuto {
            get => !hasNote && !hasNoteArray && (hasAuto || hasAutoArray) && !hasChord && !hasSeque && !hasBeatArray;
        }

        /// <summary>
        /// Array 記述のデータを持つかどうか
        /// </summary>
        public bool HasMulti {
            get => !hasNote && !hasAuto && !hasChord && !hasSeque && (hasBeatArray || hasNoteArray || hasAutoArray);
        }

        /// <summary>
        /// データを持たないかどうか
        /// </summary>
        public bool HasNoData {
            get => !hasNote && !hasAuto && !hasChord && !hasSeque && !hasBeatArray && !hasNoteArray && !hasAutoArray;
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
            get => chord.Text != null; // TODO: Text.Equals("") では？
        }

        bool hasSeque {
            get => seque.Use;
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
    /// note: Keyの旋法で度数数値を Note No に変換します
    /// auto: Spanの旋法で度数数値を Note No に変換します
    /// </summary>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string text;

        // TODO: gate で Bass のゴーストノートなどを表現 

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
            set => text = Value.Validater.PhraseValue(value);
        }

        public int Oct {
            get; set;
        }

        public int Interval {
            get => Oct * 12; // オクターブを音程差に変換
        }

        public char[] TextCharArray {
            get => Text.HasValue() ? Utils.Filter(Text).ToCharArray() : null;
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
        // Constructor

        public Chord() {
            Text = "";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Text {
            get => text;
            set => text = Value.Validater.PhraseValue(value);
        }

        public Range Range {
            get; set;
        }

        public char[] TextCharArray {
            get => Text.HasValue() ? Utils.Filter(Text).ToCharArray() : null;
        }
    }

    /// <summary>
    /// アルペジオ用設定
    /// </summary>
    public class Seque {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// MEMO: 120 が16分音符、30:60:90:120 で4音価にするか？ ⇒ 100%音価は効果が薄いので必要ない
        ///     '-' は無効の文字としてのみ使用されるべき
        ///     '+*>' の3文字で設定: ⇒ -: 0%, +: 25%, *:50%, >75%
        /// </summary>
        string text; // gate 設定用

        /// <summary>
        /// NOTE: Chord 記述の数値と同じ概念
        /// </summary>
        int stack; // 何音コードにするか

        bool use;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Seque() {
            Text = "";
            stack = 3;
            use = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Text {
            get => text;
            set => text = Value.Validater.PhraseValue(value);
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
            get => use;
            set => use = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// 音価に変換して返します
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
}
