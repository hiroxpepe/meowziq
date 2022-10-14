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

using System;
using System.Collections.Generic;
using System.Linq;

using Meowziq.Value;

namespace Meowziq.Core {
    /// <summary>
    /// Phrase クラス
    /// </summary>
    /// <note>
    /// + キーと旋法は外部から与えられる
    /// + Note オブジェクトのリストを管理する
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _type;

        Data _data; // json から読み込んだデータを格納

        Item<Note> _note_item; // Tick 毎の Note のリスト、Pattern の設定回数分の Note を格納

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            _data = new(); // json から詰められるデータ
            _note_item = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Type {
            get => _type;
            set {
                _type = value;
                if (_type.Equals("seque")) {
                    _data.Seque.Use = true;
                }
            }
        }

        public string Name {
            get; set;
        }

        public string Base {
            get; set;
        }

        public Data Data {
            get => _data;
            set => _data = value;
        }

        /// <summary>
        /// TODO: default レンジ
        /// </summary>
        public string Range {
            set {
                if (value is null) {
                    return;
                }
                var range_text = value;
                if (!range_text.Contains(":")) {
                    throw new ArgumentException("invalid range format.");
                }
                var range_array = range_text.Split(':');
                if (range_array.Length != 2) {
                    throw new ArgumentException("invalid range format.");
                }
                if (_data.HasChord) {
                    _data.Chord.Range = new(
                        int.Parse(range_array[0]),
                        int.Parse(range_array[1])
                    );
                } else if (_data.HasSeque) {
                    _data.Seque.Range = new(
                        int.Parse(range_array[0]),
                        int.Parse(range_array[1])
                    );
                }
            }
        }

        /// <summary>
        /// returns the number of beats in the phrase.
        /// </summary>
        /// <remarks>
        /// not used yet.
        /// </remarks>
        public int BeatCount {
            get {
                switch (defineDataType()) {
                    case DataType.Mono:
                        return measCount(_data.Note.Text);
                    default:
                        throw new ArgumentException("not understandable DataType.");
                }
            }
        }

        /// <summary>
        /// Note のリストを返します
        /// </summary>
        public List<Note> AllNote {
            get {
                // TODO: 返すたびに optimize 必要？ ⇒ 必要ない：修正が必要
                if (!Type.ToLower().Contains("drum")) { // ドラム以外 TODO: これで良いか確認
                    optimize(); // 最適化する
                }
                return _note_item.SelectMany(x => x.Value).Select(x => x).ToList();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Note データを生成します
        /// NOTE: Player オブジェクトから呼ばれます
        /// </summary>
        public void Build(int tick, Pattern pattern) {
            onBuild(tick, pattern);
        }

        /// <summary>
        /// シンコペーションで被る Note を除外します
        /// NOTE: all sound off された後の tick の note を消せばよい
        /// TODO: この処理の高速化が必要：List が遅い？ LINQ が遅い？
        /// FIXME: バグあり ⇒ シンコペーションは小節の頭だけ許可する？
        /// </summary>
        public void RemoveBy(Note target) {
            var tick1 = target.Tick;
            var note_list1 = _note_item.Get(tick1);
            note_list1 = note_list1.Where(x => !(!x.HasPre && x.Tick == tick1)).ToList(); // 優先ノートではなく tick が同じものを削除 // FIXME: ドラムは音毎？
            _note_item.SetBy(tick1, note_list1);
            if (target.PreCount > 1) { // さらにシンコぺの設定値が2の場合
                var tick2 = target.Tick + Length.Of16beat.Int32();
                var note_list2 = _note_item.Get(tick2);
                if (note_list2 != null) {
                    note_list2 = note_list2.Where(x => !(!x.HasPre && x.Tick == tick2)).ToList(); // さらに被る16音符を削除
                    _note_item.SetBy(tick2, note_list2);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        /// <summary>
        /// Note データを生成します
        /// </summary>
        protected void onBuild(int tick, Pattern pattern) {
            var generator = Generator.GetInstance(_note_item);
            var data_type = defineDataType();
            switch (data_type) {
                case DataType.Mono:
                    {
                        var param = new Param(_data.Note, _data.Exp, data_type);
                        generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, param);
                    }
                    break;
                case DataType.Chord:
                    {
                        var param = new Param(_data.Chord, _data.Exp, data_type);
                        generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, param);
                    }
                    break;
                case DataType.Multi:
                    var string_array = _data.Auto ? _data.AutoArray : _data.NoteArray;
                    for (var idxindex = 0; idxindex < string_array.Length; idxindex++) {
                        var param = new Param(
                            new Value.Note(string_array[idxindex], _data.OctArray[idxindex]),
                            new Value.Exp(_data.PreArray[idxindex], _data.PostArray[idxindex]),
                            data_type,
                            _data.Auto
                        );
                        generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, param);
                    }
                    break;
                case DataType.Drum:
                    _data.BeatArray.ToList().Select((x, idx) => new Param(
                        new Value.Note(text: x, oct: 0), // octave is always 0.
                        (int) _data.PercussionArray[idx],
                        new Value.Exp(pre: _data.PreArray[idx], post: string.Empty),
                        data_type
                    )).ToList().ForEach(x => generator.ApplyDrumNote(tick, pattern.BeatCount, x));
                    break;
                case DataType.Seque:
                    {
                        var param = new Param(seque: _data.Seque, type: data_type);
                        generator.ApplySequeNote(tick, pattern.BeatCount, pattern.AllSpan, param);
                    }
                    break;
            }
            // UI 表示情報作成
            generator.ApplyInfo(tick, pattern.BeatCount, pattern.AllSpan);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// MEMO: 消したい音はこのフレーズではない場合もある ⇒ Player で処理を定義
        /// TODO: この処理の高速化が必須：何が必要で何がひつようでないか
        ///       previous の発音が続いてる Note を識別する？：どのように？
        ///           previous.AllNote.Were(なんとか) 
        ///       current の シンコペ Note () ← 判定済み
        /// </summary>
        /// <memo_jp>
        /// + 消したい音はこのフレーズではない場合もある => Player で処理を定義
        /// + この処理の高速化が必須：何が必要で何が必要でないか
        /// + previous の発音が続いてる Note を識別する？ => どのように？
        ///     + previous.AllNote.Were(なんとか) 
        /// + current の シンコペ Note <= 判定済み
        /// </memo_jp>
        void optimize() {
            var note_list = _note_item.SelectMany(x => x.Value).Select(x => x).ToList();
            foreach (var stop_note in note_list.Where(x => x.HasPre)) { // 優先ノートのリスト
                foreach (var note in note_list) { // このフレーズの全てのノートの中で
                    if (note.Tick == stop_note.Tick) { // 優先ノートと発音タイミングがかぶったら
                        RemoveBy(note); // ノートを削除
                    }
                }
            }
        }

        /// <summary>
        /// determines the type of data written in json.
        /// </summary>
        DataType defineDataType() {
            if (!_data.HasMulti && (_data.HasNote || _data.HasAuto)) {
                return DataType.Mono;
            }
            else if (_data.HasMulti && (_data.HasNote || _data.HasAuto)) {
                return DataType.Multi;
            }
            else if (_data.HasChord) {
                return DataType.Chord;
            }
            else if (_data.HasBeat) {
                return DataType.Drum;
            }
            else if (_data.HasSeque) {
                return DataType.Seque;
            }
            throw new ArgumentException("not understandable DataType.");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// returns the number of measures.
        /// </summary>
        /// <remarks>
        /// not used yet.
        /// </remarks>
        static int measCount(string target) {
            if (target == null) {
                throw new ArgumentException("target must not be null.");
            }
            // divides into measures.
            string[] meas_string_array = target.Replace("][", "@")  // replaces "][" with "@" first.
                .Split('@') // divides with that mark.
                .Select(x => x.Replace("[", string.Empty).Replace("]", string.Empty)).ToArray(); // removes unnecessary characters.
            return meas_string_array.Length * 4; // 1 measure is calculated as 4 beats.
        }
    }
}
