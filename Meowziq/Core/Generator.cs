
using System.Collections.Generic;
using System.Linq;

using Meowziq.Value;
using static Meowziq.Utils;

namespace Meowziq.Core {
    /// <summary>
    /// Generator Class
    /// generate note data
    /// </summary>
    public class Generator {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Item<Note> _noteItem;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Generator(Item<Note> noteItem) {
            this._noteItem = noteItem;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// create and apply a note object.
        /// </summary>
        public void ApplyNote(int startTick, int beatCount, List<Span> spanList, Param param) {
            for (var beat16Idx = new Index(beatCount); beat16Idx.HasNext; beat16Idx.Increment()) {
                var text = param.TextCharArray[beat16Idx.Idx];
                if (param.IsMatch(text)) {
                    var span = spanList[beat16Idx.SpanIdx]; // 16beat 4個で1拍進む
                    var noteNumArray = (new int[7]).Select(x => -1).ToArray();  // ノートNo 配列を -1 で初期化: 後ではじく為
                    if (param.IsNote) { // "note", "auto" 記述
                        noteNumArray[0] = ToNote(
                            span.Key, span.Degree, span.KeyMode, span.SpanMode, text.Int32(), param.AutoNote
                        );
                    } else if (param.IsChord) { // "chord" 記述
                        noteNumArray = ToNoteArray(
                            span.Key, span.Degree, span.KeyMode, span.SpanMode, text.Int32()
                        );
                        noteNumArray = applyRange(noteNumArray, param.Chord.Range); // コード展開形の範囲を適用
                    }
                    // check the note value of this sound
                    var gate = new Gete(beat16Idx.Idx, beatCount);
                    for (; gate.HasNextSearch; gate.IncrementSearch()) { // +1 は数値文字の分
                        var search = param.TextCharArray[gate.SearchIdx];
                        if (search.Equals('>')) {
                            gate.IncrementGate(); // 16beat分長さを伸ばす
                        }
                        if (!search.Equals('>')) {
                            break; // '>' が途切れたら終了
                        }
                    }
                    if (param.Exp.HasPre) { // pre設定があれば シンコペーション
                        var pre = param.Exp.PreCharArray[beat16Idx.Idx];
                        if (param.Exp.IsMatchPre(pre)) {
                            gate.ApplyPre(pre.ToString());
                        }
                    }
                    if (param.Exp.HasPost) { // TODO: post設定があれば ロングノート
                    }
                    var tick = gate.PreLength + startTick + To16beatLength(beat16Idx.Idx);
                    noteNumArray.Where(x => x != -1).ToList().ForEach(x => 
                        add(tick, new Note(tick, x + param.Interval, gate.Value, 104, gate.PreCount
                    )));
                }
            }
        }

        /// <summary>
        /// ドラム用 Note オブジェクトを作成して適用します
        /// </summary>
        public void ApplyDrumNote(int startTick, int beatCount, Param param) {
            for (var beat16Idx = new Index(beatCount); beat16Idx.HasNext; beat16Idx.Increment()) {
                var text = param.TextCharArray[beat16Idx.Idx];
                if (param.IsMatch(text)) {
                    var gate = new Gete();
                    if (param.Exp.HasPre) { // pre設定があれば シンコペーション
                        var pre = param.Exp.PreCharArray[beat16Idx.Idx];
                        if (param.Exp.IsMatchPre(pre)) {
                            gate.ApplyPre(pre.ToString());
                        }
                    }
                    var tick = gate.PreLength + startTick + To16beatLength(beat16Idx.Idx);
                    add(tick, new Note(tick, param.PercussionNoteNum, gate.Value, 104, gate.PreCount));
                }
            }
        }

        /// <summary>
        /// シーケンス用 Note オブジェクトを作成して適用します
        /// </summary>
        public void ApplySequeNote(int startTick, int beatCount, List<Span> spanList, Param param) {
            for (var beat16Idx = new Index(beatCount); beat16Idx.HasNext; beat16Idx.Increment()) {
                var span = spanList[beat16Idx.SpanIdx]; // 16beat 4個で1拍進む
                var note = ToNoteRandom(span.Key, span.Degree, span.KeyMode, span.SpanMode); // 16の倍数
                if (!(param.Seque.Range is null)) {
                    note = applyRange(new int[] { note }, param.Seque.Range)[0]; // TODO: applyRange の単音 ver
                }
                if (param.HasTextCharArray) { // TODO: 判定方法の改善
                    var text = param.TextCharArray[beat16Idx.Idx];
                    if (param.Seque.Text.HasValue() && param.IsMatch(text)) {
                        var tick = startTick + To16beatLength(beat16Idx.Idx);
                        add(tick, new Note(tick, note, Seque.ToGate(text.ToString()), 104));
                    }
                } else {
                    var tick = startTick + To16beatLength(beat16Idx.Idx);
                    add(tick, new Note(tick, note, 30, 104)); // TODO: デフォルトの Text 記述を持たせればこの分岐は削除出来る：パターンの長さで自動生成
                }
            }
        }

        /// <summary>
        /// UI 表示用の情報を作成します
        /// </summary>
        public void ApplyInfo(int startTick, int beatCount, List<Span> spanList) {
            for (var beat16Idx = new Index(beatCount); beat16Idx.HasNext; beat16Idx.Increment()) {
                var span = spanList[beat16Idx.SpanIdx]; // 16beat 4個で1拍進む
                var tick = startTick + To16beatLength(beat16Idx.Idx); // 16beat の tick 毎に処理
                if (State.HashSet.Add(tick)) { // tick につき1度だけ
                    State.ItemMap.Add(tick, new State.Item16beat {
                        Tick = tick,
                        Key = span.Key.ToString(),
                        Degree = span.Degree.ToString(),
                        KeyMode = span.KeyMode.ToString(),
                        SpanMode = span.SpanMode.ToString()
                    });
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// 全てのノートを Range 範囲指定以内に変換します
        /// </summary>
        int[] applyRange(int[] target, Range range) {
            var newArray = new int[target.Length];
            for (var idx = 0; idx < target.Length; idx++) {
                if (target[idx] < range.Min) {
                    newArray[idx] = target[idx] + 12;
                } else if (target[idx] > range.Max) {
                    newArray[idx] = target[idx] - 12;
                } else {
                    newArray[idx] = target[idx];
                }
            }
            if ((target.Min() >= range.Min) && (target.Max() <= range.Max)) { // 全てのノートが範囲指定以内
                return newArray;
            }
            return applyRange(newArray, range); // 再帰処理
        }

        /// <summary>
        /// Note を Item オブジェクトに追加します
        /// </summary>
        void add(int tick, Note note) {
            _noteItem.Add(tick, note);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// 音価の情報を保持するクラス
        /// </summary>
        class Gete {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int _beatCount;

            int _gateCount;

            int _searchIdx;

            int _preLength;

            int _preCount;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Gete(int searchIdx, int beatCount) {
                _beatCount = beatCount;
                _searchIdx = searchIdx + 1; // +1 はテキスト数値文字の分
                _gateCount = 0;
                _preLength = 0;
                _preCount = 0;
            }

            public Gete() {
                _gateCount = 0;
                _preLength = 0;
                _preCount = 0;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective] 

            /// <summary>
            /// この Pattern に次の 16beat が存在するかどうか
            /// </summary>
            public bool HasNextSearch {
                get {
                    if (_searchIdx < To16beatCount(_beatCount)) {
                        return true; // index 値がパターンの16beatの長さ以下なら true
                    }
                    return false;
                }
            }

            public int SearchIdx {
                get => _searchIdx;
            }

            /// <summary>
            /// Note を前方に移動する数値 tick を返します
            /// NOTE: シンコペーション
            /// </summary>
            public int PreLength {
                get => _preLength;
            }

            /// <summary>
            /// シンコペーションの数値設定
            /// </summary>
            public int PreCount {
                get => _preCount;
            }

            public int Value {
                get => To16beatLength(_gateCount + 1); // 音の長さ：+1 はテキスト数値文字の分
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            /// <summary>
            /// シンコペーションの設定を適用します
            /// </summary>
            public void ApplyPre(string pre) {
                var preInt = int.Parse(pre.ToString());
                _gateCount += preInt; // pre の数値を音価に加算
                _preLength = -To16beatLength(preInt); // pre の数値 * 16beat分前にする
                if (_preLength != 0) {
                    _preCount = preInt; // シンコペーションの数値設定
                }
            }

            public void IncrementSearch() {
                _searchIdx++;
            }

            public void IncrementGate() {
                _gateCount++;
            }
        }

        /// <summary>
        /// 16beatのカウントを保持するクラス
        /// 4インクリメントされる毎に1インクリメントした Span の index 値も返す
        /// NOTE: ここの Span は必ず1拍である
        /// </summary>
        class Index {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int _beatCount; // この Pattern の拍数

            int _idxFor16beat; // 16beatを4回数える用

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Index(int beatCount) {
                _beatCount = beatCount;
                _idxFor16beat = 0;
                SpanIdx = 0;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective] 

            /// <summary>
            /// 16beat の index 値
            /// </summary>
            public int Idx {
                get;
                private set;
            }

            public int SpanIdx {
                get;
                private set;
            }

            /// <summary>
            /// この Pattern に次の 16beat が存在するかどうか
            /// </summary>
            public bool HasNext {
                get {
                    if (Idx < To16beatCount(_beatCount)) {
                        return true; // index 値がパターンの16beatの長さ以下なら true
                    }
                    return false;
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            public void Increment() {
                Idx++; // increments 16 beats
                _idxFor16beat++; // 16beat のカウントをインクリメント
                if (_idxFor16beat == 4) { // 4カウント溜まったら
                    _idxFor16beat = 0;
                    SpanIdx++; // Span index をインクリメント MEMO: 1拍のこと
                } // TODO: 必要なのは1小節をカウントすることとそのindex値
            }
        }
    }
}
