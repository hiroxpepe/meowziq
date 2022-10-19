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
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Generator {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region Fields

        Item<Note> _note_item;

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region Constructor

        Generator(Item<Note> note_item) {
            _note_item = note_item;
        }

        /// <summary>
        /// gets an initialized instance.
        /// </summary>
        public static Generator GetInstance(Item<Note> note_item) {
            return new Generator(note_item);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region public Methods [verb]

        /// <summary>
        /// creates and applies a Note object.
        /// </summary>
        public void ApplyNote(int start_tick, int beat_count, List<Span> span_list, Param param) {
            for (Index I6beat_index = new(beat_count); I6beat_index.HasNext; I6beat_index.Increment()) {
                char text = param.TextCharArray[I6beat_index.Idx];
                if (param.IsMatch(text)) {
                    Span span = span_list[I6beat_index.SpanIndex]; // 4 times of 16 beats advance 1 beat.
                    int[] note_num_array = (new int[7]).Select(selector: x => -1).ToArray();  // initializes an array of notes with -1 : to remove it later.
                    if (param.IsNote) { // "note", "auto" notated.
                        note_num_array[0] = ToNote(
                            key: span.Key, degree: span.Degree, key_mode: span.KeyMode, span_mode: span.SpanMode, number: text.Int32(), auto_note: param.AutoNote
                        );
                    } else if (param.IsChord) { // "chord" notated.
                        note_num_array = ToNoteArray(
                            key: span.Key, degree: span.Degree, key_mode: span.KeyMode, span_mode: span.SpanMode, number: text.Int32()
                        );
                        note_num_array = applyRange(target: note_num_array, range: param.Chord.Range); // applies octave range to affect chord inversions.
                    }
                    // creates the length of the note as a Gate object.
                    Gate gate = new(search_idx: I6beat_index.Idx, beat_count: beat_count);
                    for (; gate.HasNextSearch; gate.IncrementSearch()) {
                        char search = param.TextCharArray[gate.SearchIndex];
                        if (search.Equals('>')) { // has a '>' character in the search result.
                            gate.IncrementGate(); // extends length by 16 beats.
                        }
                        if (!search.Equals('>')) {
                            break; // has not '>' character in the search result: exits.
                        }
                    }
                    if (param.Exp.HasPre) { // has a pre parameter, it's syncopation.
                        char pre = param.Exp.PreCharArray[I6beat_index.Idx];
                        if (param.Exp.IsMatchPre(pre)) {
                            gate.ApplyPre(pre_string: pre.ToString());
                        }
                    }
                    if (param.Exp.HasPost) {
                        // TODO: to a long note if has a post parameter.
                    }
                    int tick = gate.PreLength + start_tick + To16beatLength(I6beat_index.Idx);
                    // adds a Note object to Item.
                    note_num_array.Where(predicate: x => x != -1).ToList().ForEach(action: x => 
                        add(tick: tick, note: new Note(tick: tick, num: x + param.Interval, gate: gate.Value, velo: 104, pre_count: gate.PreCount))
                    );
                }
            }
        }

        /// <summary>
        /// creates and applies a Note object for drum.
        /// </summary>
        public void ApplyDrumNote(int start_tick, int beat_count, Param param) {
            for (Index I6beat_index = new(beat_count); I6beat_index.HasNext; I6beat_index.Increment()) {
                char text = param.TextCharArray[I6beat_index.Idx];
                if (param.IsMatch(text)) {
                    Gate gate = new();
                    if (param.Exp.HasPre) { // has a pre parameter, it's syncopation.
                        char pre = param.Exp.PreCharArray[I6beat_index.Idx];
                        if (param.Exp.IsMatchPre(pre)) {
                            gate.ApplyPre(pre_string: pre.ToString());
                        }
                    }
                    int tick = gate.PreLength + start_tick + To16beatLength(I6beat_index.Idx);
                    add(tick: tick, note: new Note(tick: tick, num: param.PercussionNoteNum, gate: gate.Value, velo: 104, pre_count: gate.PreCount));
                }
            }
        }

        /// <summary>
        /// creates and applies a sequence Note object.
        /// </summary>
        public void ApplySequeNote(int start_tick, int beat_count, List<Span> span_list, Param param) {
            for (Index I6beat_index = new(beat_count); I6beat_index.HasNext; I6beat_index.Increment()) {
                Span span = span_list[I6beat_index.SpanIndex]; // 4 times of 16 beats advance 1 beat.
                int note_num = ToRandomNote(key: span.Key, degree: span.Degree, key_mode: span.KeyMode, span_mode: span.SpanMode);
                if (param.Seque.Range is not null) {
                    note_num = applyRange(target: new int[] { note_num }, range: param.Seque.Range)[0]; // TODO: single note ver of applyRange.
                }
                if (param.HasTextCharArray) {
                    char text = param.TextCharArray[I6beat_index.Idx];
                    if (param.Seque.Text.HasValue() && param.IsMatch(text)) {
                        int tick = start_tick + To16beatLength(index: I6beat_index.Idx);
                        add(tick: tick, note: new Note(tick: tick, num: note_num, gate: Seque.ToGateValue(target: text.ToString()), velo: 104));
                    }
                } else {
                    int tick = start_tick + To16beatLength(index: I6beat_index.Idx);
                    add(tick: tick, note: new Note(tick: tick, num: note_num, gate: 30, velo: 104)); // TODO: can be removed if you have a default Text description. : automatically generated by pattern length.
                }
            }
        }

        /// <summary>
        /// creates information for UI display.
        /// </summary>
        public void ApplyInfo(int start_tick, int beat_count, List<Span> span_list) {
            for (Index I6beat_index = new(beat_count); I6beat_index.HasNext; I6beat_index.Increment()) {
                Span span = span_list[I6beat_index.SpanIndex]; // 4 times of 16 beats advance 1 beat.
                int tick = start_tick + To16beatLength(index: I6beat_index.Idx); // processed every tick of 16 beats.
                if (State.HashSet.Add(item: tick)) { // only once per tick.
                    State.ItemMap.Add(key: tick, value: new State.Item16beat { // a State object is created every 16 beats.
                        Tick = tick,
                        Key = span.Key,
                        Degree = span.Degree,
                        KeyMode = span.KeyMode,
                        SpanMode = span.SpanMode
                    });
                }
            }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region private Methods [verb]

        /// <summary>
        /// converts all notes into the specified range.
        /// </summary>
        int[] applyRange(int[] target, Range range) {
            int[] new_array = new int[target.Length];
            for (int index = 0; index < target.Length; index++) {
                if (target[index] < range.Min) { new_array[index] = target[index] + 12; } 
                else if (target[index] > range.Max) { new_array[index] = target[index] - 12; } 
                else { new_array[index] = target[index]; }
            }
            if ((target.Min() >= range.Min) && (target.Max() <= range.Max)) { // all notes became in the specified range.
                return new_array;
            }
            return applyRange(target: new_array, range); // recursion
        }

        /// <summary>
        /// adds a Note to the Item object with the tick.
        /// </summary>
        void add(int tick, Note note) {
            _note_item.Add(key: tick, value: note);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region inner Classes

        #region Gate

        /// <summary>
        /// class that holds note value information.
        /// </summary>
        class Gate {

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Const [nouns]

            const int TEXT_VALUE_LENGTH = 1;

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Fields

            int _beat_count, _gate_count, _search_index, _pre_length, _pre_count;

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Constructor

            public Gate(int search_idx, int beat_count) {
                _beat_count = beat_count;
                _search_index = search_idx + TEXT_VALUE_LENGTH;
                _gate_count = _pre_length = _pre_count = 0;
            }

            public Gate() {
                _gate_count = _pre_length = _pre_count = 0;
            }

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Properties [noun, adjective] 

            /// <summary>
            /// この Pattern に次の 16beat が存在するかどうか
            /// </summary>
            public bool HasNextSearch {
                get {
                    if (_search_index < To16beatCount(_beat_count)) {
                        return true; // index 値がパターンの16beatの長さ以下なら true
                    }
                    return false;
                }
            }

            public int SearchIndex {
                get => _search_index;
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
                get => To16beatLength(_gate_count + TEXT_VALUE_LENGTH);
            }

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region public Methods [verb]

            /// <summary>
            /// シンコペーションの設定を適用します
            /// </summary>
            public void ApplyPre(string pre_string) {
                int pre_int = int.Parse(pre_string.ToString());
                _gate_count += pre_int; // pre の数値を音価に加算
                _pre_length = -To16beatLength(pre_int); // pre の数値 * 16beat分前にする
                if (_pre_length != 0) {
                    _pre_count = pre_int; // シンコペーションの数値設定
                }
            }

            public void IncrementSearch() {
                _search_index++;
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

            int _index_for_16beat; // 16beatを4回数える用

            int _index, _span_index;

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Constructor

            public Index(int beat_count) {
                _beat_count = beat_count;
                _index_for_16beat = 0;
                _span_index = 0;
            }

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region Properties [noun, adjective] 

            /// <summary>
            /// 16beat の index 値
            /// </summary>
            public int Idx {
                get => _index;
            }

            public int SpanIndex {
                get => _span_index;
            }

            /// <summary>
            /// この Pattern に次の 16beat が存在するかどうか
            /// </summary>
            public bool HasNext {
                get {
                    if (_index < To16beatCount(_beat_count)) {
                        return true; // index 値がパターンの16beatの長さ以下なら true
                    }
                    return false;
                }
            }

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////
            #region public Methods [verb]

            /// <summary>
            /// increments 16 beats.
            /// </summary>
            public void Increment() {
                _index++; // increments 16 beats.
                _index_for_16beat++; // 16beat のカウントをインクリメント
                if (_index_for_16beat == 4) { // 4カウント溜まったら
                    _index_for_16beat = 0;
                    _span_index++; // Span index をインクリメント MEMO: 1拍のこと
                } // TODO: 必要なのは1小節をカウントすることとそのindex値
            }

            #endregion
        }

        #endregion

        #endregion
    }
}
