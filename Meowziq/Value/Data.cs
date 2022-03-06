
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

        Note _note; // note記述：Key 旋法の度数指定 ⇒ 歌メロなどを想定、auto記述：Span 旋法の度数指定 ⇒ ベースラインなどを想定

        bool _auto; // auto記述かどうか

        Chord _chord; // chord記述

        Seque _seque; // seque記述

        Exp _exp;

        Percussion[] _percussionArray; // data記述：ドラム Track パーカッション Note No

        string[] _beatArray; // data記述：ドラム音符

        string[] _noteArray; // data記述：Key 旋法の度数指定 ⇒ 歌メロなどを想定

        string[] _autoArray; // data記述：Span 旋法の度数指定 ⇒ ベースラインなどを想定

        int[] _octArray;

        string[] _preArray;

        string[] _postArray;

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
            set => _exp = value; // デフォルト値は 0
        }

        /// <summary>
        /// Percussion の音色設定
        /// </summary>
        public Percussion[] PercussionArray {
            get => _percussionArray;
            set {
                if (value.Length != arrayLength) {
                    throw new ArgumentException("percussionArray must be same count as beatArray.");
                } else {
                    _percussionArray = value;
                }
            }
        }

        /// <summary>
        /// "beat" テキストの配列
        /// </summary>
        public string[] BeatArray {
            get => _beatArray;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                }
                _beatArray = value;
            }
        }

        /// <summary>
        /// "note" テキストの配列
        /// </summary>
        public string[] NoteArray {
            get => _noteArray;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                }
                _noteArray = value;
            }
        }

        /// <summary>
        /// "auto" テキストの配列
        /// </summary>
        public string[] AutoArray {
            get => _autoArray;
            set {
                if (!(value is null)) {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                }
                _autoArray = value;
                if (!(value is null)) {
                    _auto = true;
                }
            }
        }

        /// <summary>
        /// "note", "auto" テキストのオクターブ設定
        /// </summary>
        public int[] OctArray {
            get => _octArray;
            set {
                checkTextArray();
                if (value is null) {
                    _octArray = new int[arrayLength];
                    _octArray.Select(x => x = 0); // オクターブの設定を自動生成
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("noteOctArray must be same count as noteArray or autoArray.");
                } else {
                    _octArray = value; // TODO: バリデーション
                }
            }
        }

        /// <summary>
        /// "beat", "note", "auto" テキストの前方音価設定
        /// </summary>
        public string[] PreArray {
            get => _preArray;
            set {
                checkTextArray();
                if (value is null) {
                    _preArray = new string[arrayLength];
                    _preArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("preArray must be same count as beatArray or noteArray or autoArray.");
                } else {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                    _preArray = value;
                }
            }
        }

        /// <summary>
        /// "note", "auto" テキストの後方音価設定
        /// </summary>
        public string[] PostArray {
            get => _postArray;
            set {
                checkTextArray();
                if (value is null) {
                    _postArray = new string[arrayLength];
                    _postArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != arrayLength) {
                    throw new ArgumentException("postArray must be same count as noteArray or autoArray.");
                } else {
                    value.ToList().ForEach(x => Value.Validater.PhraseValue(x));
                    _postArray = value;
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
                if (_beatArray != null) {
                    return _beatArray.Length;
                }
                if (_noteArray != null) {
                    return _noteArray.Length;
                }
                if (_autoArray != null) {
                    return _autoArray.Length;
                }
                return 0;
            }
        }

        bool hasAnyArray {
            get {
                if (_beatArray != null || _noteArray != null || _autoArray != null) { // 何かは持つ
                    return true;
                }
                return false; // beat、note、auto いずれも持たない
            }
        }

        public bool hasBeatArray {
            get => _beatArray != null;
        }

        public bool hasNoteArray {
            get => _noteArray != null && _auto == false;
        }

        public bool hasAutoArray {
            get => _autoArray != null && _auto == true;
        }

        bool hasNote {
            get => !_note.Text.Equals("") && _auto == false;
        }

        bool hasAuto {
            get => !_note.Text.Equals("") && _auto == true;
        }

        bool hasChord {
            get => _chord.Text != null; // TODO: Text.Equals("") では？
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
    /// note: Keyの旋法で度数数値を Note No に変換します
    /// auto: Spanの旋法で度数数値を Note No に変換します
    /// </summary>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _text;

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
            get => _text;
            set => _text = Value.Validater.PhraseValue(value);
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

        string _text;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Chord() {
            Text = "";
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
        string _text; // gate 設定用

        /// <summary>
        /// NOTE: Chord 記述の数値と同じ概念
        /// </summary>
        int _stack; // 何音コードにするか

        bool _use;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Seque() {
            Text = "";
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
                var okMax = min + 11;
                var okMin = max - 11;
                throw new ArgumentException($"invalid range length,\r\nmust set {min}:{okMax} or {okMin}:{max}.");
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
