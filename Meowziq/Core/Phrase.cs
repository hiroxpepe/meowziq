
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Meowziq.Core {
    /// <summary>
    /// Phrase クラス
    ///     + キーと旋法は外部から与えられる
    ///     + Note オブジェクトのリストを管理する
    /// </summary>
    public class Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string type;

        string name;

        string noteText;

        string chordText;

        string pre;

        string post;

        Data data;

        List<Note> noteList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            this.data = new Data(); // json から詰められるデータ
            this.noteList = new List<Note>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public string Type {
            get => type;
            set => type = value;
        }

        public string Name {
            set => name = value;
        }

        public string NoteText {
            set => noteText = value;
        }

        public string ChordText {
            set => chordText = value;
        }

        public string Pre {
            set => pre = value;
        }

        public string Post {
            set => post = value;
        }

        public Data Data {
            get => data;
            set => data = value;
        }

        /// <summary>
        /// 全ての Note
        /// </summary>
        public List<Note> AllNote {
            get {
                // TODO: 最適化する
                optimize();
                return noteList;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Player オブジェクトから呼ばれます
        /// </summary>
        public void Build(int position, Key key, Pattern pattern) {
            onBuild(position, key, pattern);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        /// <summary>
        /// TODO: パターンとフレーズの長さが一致しているかどうか
        /// </summary>
        protected void onBuild(int position, Key key, Pattern pattern) {
            // パターンの名前が違えば何もしない
            if (!name.Equals(pattern.Name)) {
                return;
            }
            // FIXME: Type で分離 ⇒ 処理のパターン決め込みで良い：プラグイン拡張出来るように
            if (type.Equals("drum")) {
                for (var _idx = 0; _idx < data.NoteTextArray.Length; _idx++) {
                    var _noteText = data.NoteTextArray[_idx];
                    applyDrumNote(position, pattern.BeatCount, _noteText, data.PercussionArray[_idx]);
                }
            }
            if (type.Equals("pad")) {
                if (data.NoteTextArray != null) { // TODO: 分岐を何とかする
                    for (var _idx = 0; _idx < data.NoteTextArray.Length; _idx++) {
                        var _noteText = data.NoteTextArray[_idx];
                        var _interval = (data.OctArray[_idx] * 12); // オクターブ設定からインターバル作成
                        applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _noteText, _interval);
                    }
                } else if (chordText != null) {

                }
            }
            if (type.Equals("bass")) {
                applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, noteText, -24, pre, post);
            }
            if (type.Equals("seque")) {
                var _all16beatCount = pattern.BeatCount * 4; // この Pattern の16beatの数
                var _spanIdxCount = 0; // 16beatで1拍をカウントする用
                var _spanIdx = 0; // Span リストの添え字
                for (var _16beatIdx = 0; _16beatIdx < _all16beatCount; _16beatIdx++) {
                    if (_spanIdxCount == 4) { // 16beatが4回進んだ時(1拍)
                        _spanIdxCount = 0; // カウンタリセット
                        _spanIdx++; // Span のindex値をインクリメント
                    }
                    var _span = pattern.AllSpan[_spanIdx];
                    var _note = Utils.GetRandomNote(key, _span.Degree, _span.Mode); // 16の倍数
                    add(new Note(position + (Tick.Of16beat.Int32() * _16beatIdx), _note, 30, 127)); // gate 短め
                    _spanIdxCount++; // Span 用のカウンタも進める
                }
            }
        }

        protected void applyMonoNote(int position, int beatCount, Key key, List<Span> spanList, string noteText, int interval = 0, string pre = null, string post = null) {
            var _16beatIdx = 0; // 16beatのindex
            var _spanIdxCount = 0; // 16beatで1拍をカウントする用
            var _spanIdx = 0; // Span リストの添え字
            var _noteArray = filter(noteText).ToCharArray();
            var _preArray = pre == null ? null : filter(pre).ToCharArray(); // TODO: バリデート
            var _postArray = post == null ? null : filter(post).ToCharArray(); // TODO: バリデート
            foreach (var _note in _noteArray) {
                if (_16beatIdx > beatCount * 4) {
                    return; // Pattern の長さを超えたら終了
                }
                if (_spanIdxCount == 4) { // 16beatが4回進んだ時(1拍)
                    _spanIdxCount = 0; // カウンタリセット
                    _spanIdx++; // Span のindex値をインクリメント
                }
                if (Regex.IsMatch(_note.ToString(), @"^[1-7]+$")) { // 1～7まで度数の数値がある時
                    var _span = spanList[_spanIdx];
                    int _noteNum;
                    // 曲の旋法と Span の旋法が同じ場合は自動旋法
                    if (_span.KeyMode == _span.Mode) {
                        _noteNum = Utils.GetNoteWithAutoMode(key, _span.Degree, _span.KeyMode, int.Parse(_note.ToString()));
                    }
                    // Span に旋法が設定してあればそちらを適用する
                    else {
                        _noteNum = Utils.GetNoteWithSpanMode(key, _span.Degree, _span.KeyMode, _span.Mode, int.Parse(_note.ToString()));
                    }
                    // この音の音価を調査する
                    var _gateCount = 0;
                    var _all16beatCount = (beatCount * 4); // このパターンの16beatの数
                    for (var _searchIdx = _16beatIdx + 1; _searchIdx < _all16beatCount; _searchIdx++) { // +1 は数値文字の分
                        var _search = _noteArray[_searchIdx];
                        if (_search.Equals('>')) {
                            _gateCount++; // 16beat分長さを伸ばす
                        }
                        if (!_search.Equals('>')) {
                            break; // '>' が途切れたら終了
                        }
                    }
                    // シンコペーション
                    var _prePosition = 0;
                    if (_preArray != null) {
                        var _pre = _preArray[_16beatIdx];
                        if (Regex.IsMatch(_pre.ToString(), @"^[1-4]+$")) { // 120 * 4 tick まで
                            var _preInt = int.Parse(_pre.ToString());
                            _gateCount += _preInt; // pre の数値を音価に加算
                            _prePosition = -(Tick.Of16beat.Int32() * _preInt); // pre の数値 * 16beat分前にする
                        }
                    }
                    // TODO: 最後の音を伸ばす
                    if (_postArray != null) {
                    }
                    var _gate = Tick.Of16beat.Int32() * (_gateCount + 1); // 音の長さ：+1 は数値文字の分
                    // シンコぺがある時は直前に1回だけそのchの音を停止する
                    if (_prePosition != 0) {
                        // MEMO: chord の場合1回だけにする
                        add(new Note((_prePosition + position) + (Tick.Of16beat.Int32() * _16beatIdx), 0, 0, 0, true));
                        add(new Note((_prePosition + position) + (Tick.Of16beat.Int32() * _16beatIdx), (int) _noteNum + interval, _gate, 127, true)); // 優先ノート
                    } else {
                        add(new Note((_prePosition + position) + (Tick.Of16beat.Int32() * _16beatIdx), (int) _noteNum + interval, _gate, 127));
                    }
                }
                _16beatIdx++; // 16beatを進める
                _spanIdxCount++; // Span 用のカウンタも進める
            }
        }

        protected void applyDrumNote(int position, int beatCount, string noteText, Percussion noteNum) {
            var _16beatIdx = 0;
            foreach (bool? _note in convertToBool(filter(noteText))) {
                if (_16beatIdx > beatCount * 4) {
                    return; // Pattern の長さを超えたら終了
                }
                if (_note == true) {
                    add(new Note(position + (Tick.Of16beat.Int32() * _16beatIdx), (int) noteNum, 120, 127));
                }
                _16beatIdx++; // 16beatを進める
            }
        }

        protected void add(Note note) {
            noteList.Add(note);
        }

        protected static string filter(string target) {
            return target.Replace("|", "").Replace("[", "").Replace("]", ""); // 不要文字削除
        }

        protected static List<bool?> convertToBool(string target) {
            // on, null スイッチ
            var _list = new List<bool?>();
            target.ToList().ForEach(x => {
                if (x.Equals('x')) {
                    _list.Add(true);
                } else if (x.Equals('-')) {
                    _list.Add(null);
                }
            });
            return _list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void optimize() {
            return;
            // FIXME: 消したい音はこのフレーズではない
            var _stopNoteArray = noteList.Where(x => x.StopPre).ToList(); // 優先ノートのリスト
            var _newNoteList = new List<Note>();
            foreach (var _stopNote in _stopNoteArray) {
                foreach(var _note in noteList) { // このフレーズの全てのノートの中で
                    if (_note.Tick == _stopNote.Tick) { // 優先ノートを発音タイミングがかぶったら
                        // ノートを無視
                    } else {
                        _newNoteList.Add(_note);
                    }
                }
            }
            noteList = _newNoteList; // 置き換え
        }
    }

    /// <summary>
    /// Data クラス
    /// </summary>
    public class Data {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Percussion[] percussionArray;

        string[] noteTextArray;

        int[] octArray;

        string[] preArray;

        string[] postArray;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

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
        public string[] NoteTextArray {
            get => noteTextArray;
            set => noteTextArray = value;
        }

        /// <summary>
        /// Note テキストのオクターブ設定
        /// </summary>
        public int[] OctArray {
            get => octArray;
            set {
                checkValue();
                if (value == null) {
                    octArray = new int[noteTextArray.Length];
                    octArray.Select(x => x = 0); // オクターブの設定を自動生成
                } else if (value.Length != noteTextArray.Length) {
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
                    preArray = new string[noteTextArray.Length];
                    preArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != noteTextArray.Length) {
                    throw new ArgumentException("preArray must be same count as noteTextArray.");
                } else {
                    // TODO: バリデーション
                    preArray = value;
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
                    postArray = new string[noteTextArray.Length];
                    postArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != noteTextArray.Length) {
                    throw new ArgumentException("postArray must be same count as noteTextArray.");
                } else {
                    // TODO: バリデーション
                    postArray = value;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void checkValue() {
            if (noteTextArray == null) {
                throw new ArgumentException("must set noteTextArray.");
            }
        }
    }
}
