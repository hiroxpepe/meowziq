
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

        List<Note> noteList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Generator(List<Note> noteList) {
            this.noteList = noteList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Note を適用します
        /// </summary>
        public void ApplyNote(int tick, int beatCount, List<Span> spanList, Param param) {
            var _16beatIdx = new Index(beatCount); // 16beatのindex
            foreach (var _text in param.TextCharArray) {
                if (!_16beatIdx.HasNext) {
                    return; // Pattern の長さを超えたら終了
                }
                if (param.IsMatch(_text)) {
                    var _span = spanList[_16beatIdx.SpanIdx]; // 16beat 4個で1拍進む
                    var _noteNumArray = (new int[7]).Select(x => -1).ToArray();  // ノート記述を -1 で初期化: はじく為
                    if (param.IsNote) { // ノート記述
                        _noteNumArray[0] = Utils.GetNoteBy(
                            _span.Key, _span.Degree, _span.KeyMode, _span.SpanMode, int.Parse(_text.ToString()), _span.AutoMode
                        );
                    } else if (param.IsChord) { // コード記述
                        _noteNumArray = Utils.GetNoteArrayBy(
                            _span.Key, _span.Degree, _span.KeyMode, _span.SpanMode, int.Parse(_text.ToString()), _span.AutoMode
                        );
                        _noteNumArray = applyRange(_noteNumArray, param.Chord.Range); // コード展開形の範囲を適用
                    }
                    // この音の音価を調査する
                    var _gateCount = 0;
                    for (var _searchIdx = _16beatIdx.Idx + 1; _searchIdx < Utils.To16beatCount(beatCount); _searchIdx++) { // +1 は数値文字の分
                        var _search = param.TextCharArray[_searchIdx];
                        if (_search.Equals('>')) {
                            _gateCount++; // 16beat分長さを伸ばす
                        }
                        if (!_search.Equals('>')) {
                            break; // '>' が途切れたら終了
                        }
                    }
                    // シンコペーション
                    var _preLength = 0;
                    var _stopPre = false;
                    if (param.Exp.HasPre) {
                        var _pre = param.Exp.PreCharArray[_16beatIdx.Idx];
                        if (param.Exp.IsMatchPre(_pre)) {
                            var _preInt = int.Parse(_pre.ToString());
                            _gateCount += _preInt; // pre の数値を音価に加算
                            _preLength = -Utils.To16beatLength(_preInt); // pre の数値 * 16beat分前にする
                        }
                        if (_preLength != 0) {
                            _stopPre = true; // シンコぺがある場合は優先発音フラグON
                        }
                    }
                    if (param.Exp.HasPost) { // TODO: 最後の音を伸ばす
                    }
                    var _gate = Utils.To16beatLength(_gateCount + 1); // 音の長さ：+1 は数値文字の分
                    _noteNumArray.Where(x => x != -1).ToList().ForEach(
                        x => add(new Note((_preLength + tick) + Utils.To16beatLength(_16beatIdx.Idx), x + param.Interval, _gate, 127, _stopPre))
                    );
                }
                _16beatIdx.Increment(); // 16beatのindex値をインクリメント
            }
        }

        /// <summary>
        /// ドラム用 Note を適用します
        /// </summary>
        public void ApplyDrumNote(int tick, int beatCount, Param param) {
            var _16beatIdx = new Index(beatCount); // 16beatのindex
            foreach (var _text in param.TextCharArray) {
                if (!_16beatIdx.HasNext) {
                    return; // Pattern の長さを超えたら終了
                }
                if (param.IsMatch(_text)) {
                    // シンコペーション
                    var _gateCount = 0;
                    var _preLength = 0;
                    var _stopPre = false;
                    if (param.Exp.HasPre) { // pre設定があれば
                        var _pre = param.Exp.PreCharArray[_16beatIdx.Idx];
                        if (param.Exp.IsMatchPre(_pre)) {
                            var _preInt = int.Parse(_pre.ToString());
                            _gateCount += _preInt; // pre の数値を音価に加算
                            _preLength = -Utils.To16beatLength(_preInt); // pre の数値 * 16beat分前にする
                        }
                        if (_preLength != 0) {
                            _stopPre = true; // シンコぺがある場合は優先発音フラグON
                        }
                    }
                    var _gate = Utils.To16beatLength(_gateCount + 1); // 音の長さ：+1 は数値文字の分
                    add(new Note((_preLength + tick) + Utils.To16beatLength(_16beatIdx.Idx), param.PercussionNoteNum, _gate, 127, _stopPre));
                }
                _16beatIdx.Increment(); // 16beatのindex値をインクリメント
            }
        }

        /// <summary>
        /// シーケンス用 ランダム Note を適用します
        /// </summary>
        public void ApplyRandomNote(int tick, int beatCount, List<Span> spanList) {
            for (var _16beatIdx = new Index(beatCount); _16beatIdx.HasNext; _16beatIdx.Increment()) {
                var _span = spanList[_16beatIdx.SpanIdx]; // 16beat 4個で1拍進む
                var _note = Utils.GetNoteAsRandom(_span.Key, _span.Degree, _span.KeyMode, _span.SpanMode, _span.AutoMode); // 16の倍数
                add(new Note(tick + Utils.To16beatLength(_16beatIdx.Idx), _note, 30, 127)); // gate 短め
            }
        }

        /// <summary>
        /// UI 表示用の情報を作成します
        /// </summary>
        public void ApplyInfo(int tick, int beatCount, List<Span> spanList) {
            for (var _16beatIdx = new Index(beatCount); _16beatIdx.HasNext; _16beatIdx.Increment()) {
                var _span = spanList[_16beatIdx.SpanIdx]; // 16beat 4個で1拍進む
                var _tick = tick + Utils.To16beatLength(_16beatIdx.Idx); // 16beat の tick 毎に処理
                if (Info.HashSet.Add(_tick)) { // tick につき1度だけ
                    Info.ItemDictionary.Add(_tick, new Info.Item {
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
            // 全てのノートが範囲指定以内
            if ((target.Min() >= range.Min) && (target.Max() <= range.Max)) {
                return _newArray;
            }
            return applyRange(_newArray, range); // 再帰処理
        }

        /// <summary>
        /// Note リストに追加します
        /// </summary>
        void add(Note note) {
            noteList.Add(note);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// 16beatのカウントを保持するクラス
        /// 4インクリメントされる毎に1インクリメントした Span の index 値も返す
        /// NOTE: ここの Span は必ず1拍である
        /// </summary>
        class Index {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int beatCount; // この Pattern の拍数

            int idx4; // 16beatを4回数える用

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Index(int beatCount) {
                this.beatCount = beatCount;
                idx4 = 0;
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
                idx4++; // 16beat のカウント
                if (idx4 == 4) {
                    idx4 = 0;
                    SpanIdx++; // 1拍のこと
                } // TODO: 必要なのは1小節をカウントすることとそのindex値
            }
        }
    }
}
