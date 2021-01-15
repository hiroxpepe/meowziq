
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
        public void ApplyNote(int tick, int beatCount, Key key, List<Span> spanList, Param param) {
            var _16beatIdx = 0; // 16beatのindex
            var _spanIndex = new SpanIndex(); // Span リストの添え字オブジェクト
            foreach (var _text in param.TextCharArray) {
                if (_16beatIdx > beatCount * 4) {
                    return; // Pattern の長さを超えたら終了
                }
                if (param.IsMatch(_text)) {
                    var _span = spanList[_spanIndex.Idx]; // 16beat 4個で1拍進む
                    int[] _noteNumArray = new int[7];
                    _noteNumArray = _noteNumArray.Select(x => x = -1).ToArray(); // -1 で初期化
                    // 曲の旋法と Span の旋法が同じ場合は自動旋法
                    if (_span.AutoMode) {
                        if (param.IsNote) {
                            _noteNumArray[0] = Utils.GetNoteByAutoMode(key, _span.Degree, _span.KeyMode, int.Parse(_text.ToString()));
                        } else if (param.IsChord) {
                            _noteNumArray = Utils.GetNoteArrayByAutoMode(key, _span.Degree, _span.KeyMode, int.Parse(_text.ToString()));
                            _noteNumArray = applyRange(_noteNumArray, param.Chord.Range); // コード展開形の範囲を適用
                        }
                    }
                    // Span に旋法が設定してあればそちらを適用する
                    else {
                        if (param.IsNote) {
                            _noteNumArray[0] = Utils.GetNoteBySpanMode(key, _span.Degree, _span.KeyMode, _span.Mode, int.Parse(_text.ToString()));
                        } else if (param.IsChord) {
                            _noteNumArray = Utils.GetNoteArrayBySpanMode(key, _span.Degree, _span.KeyMode, _span.Mode, int.Parse(_text.ToString()));
                            _noteNumArray = applyRange(_noteNumArray, param.Chord.Range); // コード展開形の範囲を適用
                        }
                    }
                    // この音の音価を調査する
                    var _gateCount = 0;
                    var _all16beatCount = (beatCount * 4); // このパターンの16beatの数
                    for (var _searchIdx = _16beatIdx + 1; _searchIdx < _all16beatCount; _searchIdx++) { // +1 は数値文字の分
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
                        var _pre = param.Exp.PreCharArray[_16beatIdx];
                        if (param.Exp.IsMatchPre(_pre)) {
                            var _preInt = int.Parse(_pre.ToString());
                            _gateCount += _preInt; // pre の数値を音価に加算
                            _preLength = -(Length.Of16beat.Int32() * _preInt); // pre の数値 * 16beat分前にする
                        }
                        if (_preLength != 0) {
                            _stopPre = true; // シンコぺがある場合は優先発音フラグON
                        }
                    }
                    // TODO: 最後の音を伸ばす
                    if (param.Exp.HasPost) {
                    }
                    var _gate = Length.Of16beat.Int32() * (_gateCount + 1); // 音の長さ：+1 は数値文字の分
                    _noteNumArray.Where(x => x != -1).ToList().ForEach(
                        x => add(new Note((_preLength + tick) + (Length.Of16beat.Int32() * _16beatIdx), x + param.Interval, _gate, 127, _stopPre))
                    );
                }
                _16beatIdx++; // 16beatのindex値をインクリメント
                _spanIndex.Increment(); // Span リストの添え字オブジェクトをインクリメント
            }
        }

        /// <summary>
        /// ドラム用 Note を適用します
        /// </summary>
        public void ApplyDrumNote(int tick, int beatCount, Param param) {
            var _16beatIdx = 0;
            foreach (char _text in param.TextCharArray) {
                if (_16beatIdx > beatCount * 4) {
                    return; // Pattern の長さを超えたら終了
                }
                if (param.IsMatch(_text)) {
                    // シンコペーション
                    var _gateCount = 0;
                    var _preLength = 0;
                    var _stopPre = false;
                    if (param.Exp.HasPre) { // pre設定があれば
                        var _pre = param.Exp.PreCharArray[_16beatIdx];
                        if (param.Exp.IsMatchPre(_pre)) {
                            var _preInt = int.Parse(_pre.ToString());
                            _gateCount += _preInt; // pre の数値を音価に加算
                            _preLength = -(Length.Of16beat.Int32() * _preInt); // pre の数値 * 16beat分前にする
                        }
                        if (_preLength != 0) {
                            _stopPre = true; // シンコぺがある場合は優先発音フラグON
                        }
                    }
                    var _gate = Length.Of16beat.Int32() * (_gateCount + 1); // 音の長さ：+1 は数値文字の分
                    add(new Note((_preLength + tick) + (Length.Of16beat.Int32() * _16beatIdx), param.PercussionNoteNum, _gate, 127, _stopPre));
                }
                _16beatIdx++; // 16beatのindex値をインクリメント
            }
        }

        /// <summary>
        /// シーケンス用 ランダム Note を適用します
        /// </summary>
        public void ApplyRandomNote(int tick, int beatCount, Key key, List<Span> spanList) {
            var _all16beatCount = beatCount * 4; // この Pattern の16beatの数
            var _spanIndex = new SpanIndex(); // Span リストの添え字オブジェクト
            for (var _16beatIdx = 0; _16beatIdx < _all16beatCount; _16beatIdx++) {
                var _span = spanList[_spanIndex.Idx]; // 16beat 4個で1拍進む
                var _note = Utils.GetNoteAsRandom(key, _span.Degree, _span.Mode); // 16の倍数
                add(new Note(tick + (Length.Of16beat.Int32() * _16beatIdx), _note, 30, 127)); // gate 短め
                _spanIndex.Increment(); // Span リストの添え字オブジェクトをインクリメント
            }
        }

        /// <summary>
        /// UI 表示用の情報を作成します
        /// </summary>
        public void ApplyInfo(int tick, int beatCount, Key key, List<Span> spanList) {
            var _all16beatCount = beatCount * 4; // この Pattern の16beatの数
            var _spanIndex = new SpanIndex(); // Span リストの添え字オブジェクト
            for (var _16beatIdx = 0; _16beatIdx < _all16beatCount; _16beatIdx++) {
                var _span = spanList[_spanIndex.Idx]; // 16beat 4個で1拍進む
                var _tick = tick + (Length.Of16beat.Int32() * _16beatIdx); // 16beat の tick 毎に処理
                if (Info.HashSet.Add(_tick)) { // tick につき1度だけ
                    Info.ItemDictionary.Add(_tick, new Info.Item {
                        Tick = _tick,
                        Key = key.ToString(),
                        Degree = _span.Degree.ToString(),
                        KeyMode = _span.KeyMode.ToString(),
                        SpanMode = _span.Mode.ToString()
                    });
                }
                _spanIndex.Increment(); // Span リストの添え字オブジェクトをインクリメント
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
        /// 4インクリメントされる毎に1インクリメントした index 値を返す
        /// NOTE: ここの Span は必ず1拍である
        /// </summary>
        class SpanIndex {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int idx4;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public SpanIndex() {
                Idx = 0;
                idx4 = 0;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjectives] 

            public int Idx { 
                get;
                private set;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            public void Increment() {
                idx4++; // 16beat のカウント
                if (idx4 == 4) {
                    idx4 = 0;
                    Idx++; // 1拍のこと
                }
                // TODO: 必要なのは1小節をカウントすることとそのindex値
            }
        }
    }
}
