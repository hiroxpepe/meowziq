
using System.Collections.Generic;
using System.Linq;

using Meowziq.Value;

namespace Meowziq.Core {
    /// <summary>
    /// Generator クラス
    ///     + Note データを生成します
    /// </summary>
    public class Generator {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Item<Note> noteItem;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Generator(Item<Note> noteItem) {
            this.noteItem = noteItem;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Note オブジェクトを作成して適用します
        /// TODO: ハモリ記述：メロディ記述の noteNum に対して メロの旋法 に Degree と上下パラメータ？ で算出  
        /// TODO: ブルーノートは？ ⇒ 音を任意に+,-出来る設定(帯)を持たせる？
        /// </summary>
        public void ApplyNote(int tick, int beatCount, List<Span> spanList, Param param) {
            for (var _16beatIdx = new Index(beatCount); _16beatIdx.HasNext; _16beatIdx.Increment()) {
                var _text = param.TextCharArray[_16beatIdx.Idx];
                if (param.IsMatch(_text)) {
                    var _span = spanList[_16beatIdx.SpanIdx]; // 16beat 4個で1拍進む
                    var _noteNumArray = (new int[7]).Select(x => -1).ToArray();  // ノートNo 配列を -1 で初期化: 後ではじく為
                    if (param.IsNote) { // "note", "auto" 記述
                        _noteNumArray[0] = Utils.ToNote(
                            _span.Key, _span.Degree, _span.KeyMode, _span.SpanMode, _text.Int32(), param.AutoNote
                        );
                    } else if (param.IsChord) { // "chord" 記述
                        _noteNumArray = Utils.ToNoteArray(
                            _span.Key, _span.Degree, _span.KeyMode, _span.SpanMode, _text.Int32()
                        );
                        _noteNumArray = applyRange(_noteNumArray, param.Chord.Range); // コード展開形の範囲を適用
                    }
                    // この音の音価を調査する
                    var _gate = new Gete(_16beatIdx.Idx, beatCount);
                    for (; _gate.HasNextSearch; _gate.IncrementSearch()) { // +1 は数値文字の分
                        var _search = param.TextCharArray[_gate.SearchIdx];
                        if (_search.Equals('>')) {
                            _gate.IncrementGate(); // 16beat分長さを伸ばす
                        }
                        if (!_search.Equals('>')) {
                            break; // '>' が途切れたら終了
                        }
                    }
                    if (param.Exp.HasPre) { // pre設定があれば シンコペーション
                        var _pre = param.Exp.PreCharArray[_16beatIdx.Idx];
                        if (param.Exp.IsMatchPre(_pre)) {
                            _gate.ApplyPre(_pre.ToString());
                        }
                    }
                    if (param.Exp.HasPost) { // TODO: post設定があれば ロングノート
                    }
                    var _tick = _gate.PreLength + tick + Utils.To16beatLength(_16beatIdx.Idx);
                    _noteNumArray.Where(x => x != -1).ToList().ForEach(x => 
                        add(_tick, new Note(_tick, x + param.Interval, _gate.Value, 127, _gate.PreCount
                    )));
                }
            }
        }

        /// <summary>
        /// ドラム用 Note オブジェクトを作成して適用します
        /// </summary>
        public void ApplyDrumNote(int tick, int beatCount, Param param) {
            for (var _16beatIdx = new Index(beatCount); _16beatIdx.HasNext; _16beatIdx.Increment()) {
                var _text = param.TextCharArray[_16beatIdx.Idx];
                if (param.IsMatch(_text)) {
                    var _gate = new Gete();
                    if (param.Exp.HasPre) { // pre設定があれば シンコペーション
                        var _pre = param.Exp.PreCharArray[_16beatIdx.Idx];
                        if (param.Exp.IsMatchPre(_pre)) {
                            _gate.ApplyPre(_pre.ToString());
                        }
                    }
                    var _tick = _gate.PreLength + tick + Utils.To16beatLength(_16beatIdx.Idx);
                    add(_tick, new Note(_tick, param.PercussionNoteNum, _gate.Value, 127, _gate.PreCount));
                }
            }
        }

        /// <summary>
        /// シーケンス用 Note オブジェクトを作成して適用します
        /// </summary>
        public void ApplyRandomNote(int tick, int beatCount, List<Span> spanList) {
            for (var _16beatIdx = new Index(beatCount); _16beatIdx.HasNext; _16beatIdx.Increment()) {
                var _span = spanList[_16beatIdx.SpanIdx]; // 16beat 4個で1拍進む
                var _note = Utils.ToNoteRandom(_span.Key, _span.Degree, _span.KeyMode, _span.SpanMode); // 16の倍数
                var _tick = tick + Utils.To16beatLength(_16beatIdx.Idx);
                add(_tick, new Note(_tick, _note, 30, 127)); // gate 短め
            }
        }

        /// <summary>
        /// UI 表示用の情報を作成します
        /// </summary>
        public void ApplyInfo(int tick, int beatCount, List<Span> spanList) {
            for (var _16beatIdx = new Index(beatCount); _16beatIdx.HasNext; _16beatIdx.Increment()) {
                var _span = spanList[_16beatIdx.SpanIdx]; // 16beat 4個で1拍進む
                var _tick = tick + Utils.To16beatLength(_16beatIdx.Idx); // 16beat の tick 毎に処理
                if (State.HashSet.Add(_tick)) { // tick につき1度だけ
                    State.ItemMap.Add(_tick, new State.Item16beat {
                        Tick = _tick,
                        Key = _span.Key.ToString(),
                        Degree = _span.Degree.ToString(),
                        KeyMode = _span.KeyMode.ToString(),
                        SpanMode = _span.SpanMode.ToString()
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
            var _newArray = new int[target.Length];
            for (var _idx = 0; _idx < target.Length; _idx++) {
                if (target[_idx] < range.Min) {
                    _newArray[_idx] = target[_idx] + 12;
                } else if (target[_idx] > range.Max) {
                    _newArray[_idx] = target[_idx] - 12;
                } else {
                    _newArray[_idx] = target[_idx];
                }
            }
            if ((target.Min() >= range.Min) && (target.Max() <= range.Max)) { // 全てのノートが範囲指定以内
                return _newArray;
            }
            return applyRange(_newArray, range); // 再帰処理
        }

        /// <summary>
        /// Note を Item オブジェクトに追加します
        /// </summary>
        void add(int tick, Note note) {
            noteItem.Add(tick, note);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// 音価の情報を保持するクラス
        /// </summary>
        class Gete {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int beatCount;

            int gateCount;

            int searchIdx;

            int preLength;

            int preCount;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Gete(int searchIdx, int beatCount) {
                this.beatCount = beatCount;
                this.searchIdx = searchIdx + 1; // +1 はテキスト数値文字の分
                this.gateCount = 0;
                this.preLength = 0;
                this.preCount = 0;
            }

            public Gete() {
                this.gateCount = 0;
                this.preLength = 0;
                this.preCount = 0;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective] 

            /// <summary>
            /// この Pattern に次の 16beat が存在するかどうか
            /// </summary>
            public bool HasNextSearch {
                get {
                    if (searchIdx < Utils.To16beatCount(beatCount)) {
                        return true; // index 値がパターンの16beatの長さ以下なら true
                    }
                    return false;
                }
            }

            public int SearchIdx {
                get => searchIdx;
            }

            /// <summary>
            /// Note を前方に移動する数値 tick を返します
            /// NOTE: シンコペーション
            /// </summary>
            public int PreLength {
                get => preLength;
            }

            /// <summary>
            /// シンコペーションの数値設定
            /// </summary>
            public int PreCount {
                get => preCount;
            }

            public int Value {
                get => Utils.To16beatLength(gateCount + 1); // 音の長さ：+1 はテキスト数値文字の分
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            /// <summary>
            /// シンコペーションの設定を適用します
            /// </summary>
            public void ApplyPre(string pre) {
                var _preInt = int.Parse(pre.ToString());
                gateCount += _preInt; // pre の数値を音価に加算
                preLength = -Utils.To16beatLength(_preInt); // pre の数値 * 16beat分前にする
                if (preLength != 0) {
                    this.preCount = _preInt; // シンコペーションの数値設定
                }
            }

            public void IncrementSearch() {
                searchIdx++;
            }

            public void IncrementGate() {
                gateCount++;
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

            int beatCount; // この Pattern の拍数

            int idxFor16beat; // 16beatを4回数える用

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Index(int beatCount) {
                this.beatCount = beatCount;
                this.idxFor16beat = 0;
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
                    if (Idx < Utils.To16beatCount(beatCount)) {
                        return true; // index 値がパターンの16beatの長さ以下なら true
                    }
                    return false;
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            public void Increment() {
                Idx++; // 16beat をインクリメントする
                idxFor16beat++; // 16beat のカウントをインクリメント
                if (idxFor16beat == 4) { // 4カウント溜まったら
                    idxFor16beat = 0;
                    SpanIdx++; // Span index をインクリメント MEMO: 1拍のこと
                } // TODO: 必要なのは1小節をカウントすることとそのindex値
            }
        }
    }
}
