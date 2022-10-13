/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Linq;

using Meowziq.Value;
using static Meowziq.Utils;

namespace Meowziq.Core {
    /// <summary>
    /// generates Note objects and adds to the Item object.
    /// </summary>
    /// <note>
    /// + called from Meowziq.Core.Phrase.onBuild(). <br/>
    /// </note>
    /// <author>
    /// h.adachi (STUDIO MeowToon)
    /// </author>
    public class Generator {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region Fields

        Item<Note> _note_item;

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region Constructor

        public Generator(Item<Note> note_item) {
            _note_item = note_item;
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region public Methods [verb]

        /// <summary>
        /// creates and applies a Note object.
        /// </summary>
        public void ApplyNote(int start_tick, int beat_count, List<Span> span_list, Param param) {
            for (var I6beat_idx = new Index(beat_count); I6beat_idx.HasNext; I6beat_idx.Increment()) {
                var text = param.TextCharArray[I6beat_idx.Idx];
                if (param.IsMatch(text)) {
                    var span = span_list[I6beat_idx.SpanIdx]; // 16beat 4個で1拍進む
                    var note_num_array = (new int[7]).Select(x => -1).ToArray();  // ノートNo 配列を -1 で初期化: 後ではじく為
                    if (param.IsNote) { // "note", "auto" 記述
                        note_num_array[0] = ToNote(
                            span.Key, span.Degree, span.KeyMode, span.SpanMode, text.Int32(), param.AutoNote
                        );
                    } else if (param.IsChord) { // "chord" 記述
                        note_num_array = ToNoteArray(
                            span.Key, span.Degree, span.KeyMode, span.SpanMode, text.Int32()
                        );
                        note_num_array = applyRange(target: note_num_array, range: param.Chord.Range); // コード展開形の範囲を適用
                    }
                    // check the note value of this sound
                    var gate = new Gete(search_idx: I6beat_idx.Idx, beat_count: beat_count);
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
                        var pre = param.Exp.PreCharArray[I6beat_idx.Idx];
                        if (param.Exp.IsMatchPre(pre)) {
                            gate.ApplyPre(pre.ToString());
                        }
                    }
                    if (param.Exp.HasPost) { // TODO: post設定があれば ロングノート
                    }
                    var tick = gate.PreLength + start_tick + To16beatLength(I6beat_idx.Idx);
                    note_num_array.Where(x => x != -1).ToList().ForEach(x => 
                        add(tick: tick, note: new Note(tick: tick, num: x + param.Interval, gate: gate.Value, velo: 104, pre_count: gate.PreCount))
                    );
                }
            }
        }

        /// <summary>
        /// creates and applies a Note object for drum.
        /// </summary>
        public void ApplyDrumNote(int start_tick, int beat_count, Param param) {
            for (var I6beat_idx = new Index(beat_count); I6beat_idx.HasNext; I6beat_idx.Increment()) {
                var text = param.TextCharArray[I6beat_idx.Idx];
                if (param.IsMatch(text)) {
                    var gate = new Gete();
                    if (param.Exp.HasPre) { // pre設定があれば シンコペーション
                        var pre = param.Exp.PreCharArray[I6beat_idx.Idx];
                        if (param.Exp.IsMatchPre(pre)) {
                            gate.ApplyPre(pre.ToString());
                        }
                    }
                    var tick = gate.PreLength + start_tick + To16beatLength(I6beat_idx.Idx);
                    add(tick: tick, note: new Note(tick: tick, num: param.PercussionNoteNum, gate: gate.Value, velo: 104, pre_count: gate.PreCount));
                }
            }
        }

        /// <summary>
        /// creates and applies a sequence Note object.
        /// </summary>
        public void ApplySequeNote(int start_tick, int beat_count, List<Span> span_list, Param param) {
            for (var I6beat_idx = new Index(beat_count); I6beat_idx.HasNext; I6beat_idx.Increment()) {
                var span = span_list[I6beat_idx.SpanIdx]; // 16beat 4個で1拍進む
                var note_num = ToRandomNote(key: span.Key, degree: span.Degree, key_mode: span.KeyMode, span_mode: span.SpanMode); // 16の倍数
                if (param.Seque.Range is not null) {
                    note_num = applyRange(target: new int[] { note_num }, range: param.Seque.Range)[0]; // TODO: applyRange の単音 ver
                }
                if (param.HasTextCharArray) { // TODO: 判定方法の改善
                    var text = param.TextCharArray[I6beat_idx.Idx];
                    if (param.Seque.Text.HasValue() && param.IsMatch(text)) {
                        var tick = start_tick + To16beatLength(index: I6beat_idx.Idx);
                        add(tick: tick, note: new Note(tick: tick, num: note_num, gate: Seque.ToGate(target: text.ToString()), velo: 104));
                    }
                } else {
                    var tick = start_tick + To16beatLength(index: I6beat_idx.Idx);
                    add(tick: tick, note: new Note(tick: tick, num: note_num, gate: 30, velo: 104)); // TODO: デフォルトの Text 記述を持たせればこの分岐は削除出来る：パターンの長さで自動生成
                }
            }
        }

        /// <summary>
        /// creates information for UI display.
        /// </summary>
        public void ApplyInfo(int start_tick, int beat_count, List<Span> span_list) {
            for (var I6beat_idx = new Index(beat_count); I6beat_idx.HasNext; I6beat_idx.Increment()) {
                var span = span_list[I6beat_idx.SpanIdx]; // 16beat 4個で1拍進む
                var tick = start_tick + To16beatLength(index: I6beat_idx.Idx); // 16beat の tick 毎に処理
                if (State.HashSet.Add(item: tick)) { // tick につき1度だけ
                    State.ItemMap.Add(key: tick, value: new State.Item16beat {
                        Tick = tick,
                        Key = span.Key.ToString(),
                        Degree = span.Degree.ToString(),
                        KeyMode = span.KeyMode.ToString(),
                        SpanMode = span.SpanMode.ToString()
                    });
                }
            }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region private Methods [verb]

        /// <summary>
        /// 全てのノートを Range 範囲指定以内に変換します
        /// </summary>
        int[] applyRange(int[] target, Range range) {
            var new_array = new int[target.Length];
            for (var idx = 0; idx < target.Length; idx++) {
                if (target[idx] < range.Min) {
                    new_array[idx] = target[idx] + 12;
                } else if (target[idx] > range.Max) {
                    new_array[idx] = target[idx] - 12;
                } else {
                    new_array[idx] = target[idx];
                }
            }
            if ((target.Min() >= range.Min) && (target.Max() <= range.Max)) { // 全てのノートが範囲指定以内
                return new_array;
            }
            return applyRange(new_array, range); // 再帰処理
        }

        /// <summary>
        /// Note を Item オブジェクトに追加します
        /// </summary>
        void add(int tick, Note note) {
            _note_item.Add(tick, note);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region inner Classes

        #region Gete

        /// <summary>
        /// 音価の情報を保持するクラス
        /// </summary>
        class Gete {

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Fields

            int _beat_count;

            int _gate_count;

            int _search_idx;

            int _pre_length;

            int _pre_count;

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Constructor

            public Gete(int search_idx, int beat_count) {
                _beat_count = beat_count;
                _search_idx = search_idx + 1; // +1 はテキスト数値文字の分
                _gate_count = 0;
                _pre_length = 0;
                _pre_count = 0;
            }

            public Gete() {
                _gate_count = 0;
                _pre_length = 0;
                _pre_count = 0;
            }

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Properties [noun, adjective] 

            /// <summary>
            /// この Pattern に次の 16beat が存在するかどうか
            /// </summary>
            public bool HasNextSearch {
                get {
                    if (_search_idx < To16beatCount(_beat_count)) {
                        return true; // index 値がパターンの16beatの長さ以下なら true
                    }
                    return false;
                }
            }

            public int SearchIdx {
                get => _search_idx;
            }

            /// <summary>
            /// Note を前方に移動する数値 tick を返します
            /// NOTE: シンコペーション
            /// </summary>
            public int PreLength {
                get => _pre_length;
            }

            /// <summary>
            /// シンコペーションの数値設定
            /// </summary>
            public int PreCount {
                get => _pre_count;
            }

            public int Value {
                get => To16beatLength(_gate_count + 1); // 音の長さ：+1 はテキスト数値文字の分
            }

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region public Methods [verb]

            /// <summary>
            /// シンコペーションの設定を適用します
            /// </summary>
            public void ApplyPre(string pre) {
                var pre_int = int.Parse(pre.ToString());
                _gate_count += pre_int; // pre の数値を音価に加算
                _pre_length = -To16beatLength(pre_int); // pre の数値 * 16beat分前にする
                if (_pre_length != 0) {
                    _pre_count = pre_int; // シンコペーションの数値設定
                }
            }

            public void IncrementSearch() {
                _search_idx++;
            }

            public void IncrementGate() {
                _gate_count++;
            }

            #endregion
        }

        #endregion

        #region Index

        /// <summary>
        /// 16beatのカウントを保持するクラス
        /// </summary>
        /// <note>
        /// + 4インクリメントされる毎に1インクリメントした Span の index 値も返す
        /// + ここの Span は必ず1拍である
        /// </note>
        class Index {

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Fields

            int _beat_count; // この Pattern の拍数

            int _idx_for_16beat; // 16beatを4回数える用

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Constructor

            public Index(int beat_count) {
                _beat_count = beat_count;
                _idx_for_16beat = 0;
                SpanIdx = 0;
            }

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Properties [noun, adjective] 

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
                    if (Idx < To16beatCount(_beat_count)) {
                        return true; // index 値がパターンの16beatの長さ以下なら true
                    }
                    return false;
                }
            }

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region public Methods [verb]

            public void Increment() {
                Idx++; // increments 16 beats
                _idx_for_16beat++; // 16beat のカウントをインクリメント
                if (_idx_for_16beat == 4) { // 4カウント溜まったら
                    _idx_for_16beat = 0;
                    SpanIdx++; // Span index をインクリメント MEMO: 1拍のこと
                } // TODO: 必要なのは1小節をカウントすることとそのindex値
            }

            #endregion
        }

        #endregion

        #endregion
    }
}
