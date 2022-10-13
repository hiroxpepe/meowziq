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
    /// <author>
    /// h.adachi (STUDIO MeowToon)
    /// </author>
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
        /// TBA: シーケンス
        /// Phrase の拍数を返す
        /// </summary>
        public int BeatCount {
            get {
                switch (defineWay()) {
                    case Way.Mono:
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
            //if (BeatCount != pattern.BeatCount) {
            //    throw new ArgumentException("invalid beatCount.");
            //}
            // NOTE: Way で分岐 TODO: プラグイン拡張出来るように
            var generator = new Generator(_note_item); // NOTE: コンストラクタで生成ではNG
            var way = defineWay();
            switch (way) {
                case Way.Mono:
                    {
                        var param = new Param(_data.Note, _data.Exp, way); // NOTE: "note", "auto" データ判定済
                        generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, param);
                    }
                    break;
                case Way.Chord:
                    {
                        var param = new Param(_data.Chord, _data.Exp, way);
                        generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, param);
                    }
                    break;
                case Way.Multi:
                    var string_array = _data.Auto ? _data.AutoArray : _data.NoteArray;
                    for (var idx = 0; idx < string_array.Length; idx++) { // TODO: for の置き換え
                        var param = new Param(
                            new Value.Note(string_array[idx], _data.OctArray[idx]),
                            new Value.Exp(_data.PreArray[idx], _data.PostArray[idx]),
                            way,
                            _data.Auto
                        );
                        generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, param);
                    }
                    break;
                case Way.Drum:
                    _data.BeatArray.ToList().Select((x, idx) => new Param(
                        new Value.Note(x, 0), // オクターブは常に 0
                        (int) _data.PercussionArray[idx],
                        new Value.Exp(_data.PreArray[idx], string.Empty),
                        way
                    )).ToList().ForEach(x => generator.ApplyDrumNote(tick, pattern.BeatCount, x));
                    break;
                case Way.Seque:
                    {
                        var param = new Param(_data.Seque, way);
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
        /// json に記述されたデータのタイプを判定します
        /// </summary>
        Way defineWay() {
            if (!_data.HasMulti && (_data.HasNote || _data.HasAuto)) {
                return Way.Mono; // 単体 "note", "auto" ノート記述 
            }
            else if (_data.HasMulti && (_data.HasNote || _data.HasAuto)) {
                return Way.Multi; // 複合 "note", "auto" 記述
            }
            else if (_data.HasChord) {
                return Way.Chord; // "chord" 記述
            }
            else if (_data.HasBeat) {
                return Way.Drum; // "beat" 記述 
            }
            else if (_data.HasSeque) {
                return Way.Seque; // "seque" 記述
            }
            throw new ArgumentException("not understandable Way.");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// TBA
        /// </summary>
        static int measCount(string target) {
            if (target == null) {
                throw new ArgumentException("target must not be null.");
            }
            // 小節に切り出す
            var meas_string_array = target.Replace("][", "@")  // まず "][" を "@" に置き換え
                .Split('@') // 小節で切り分ける
                .Select(x => x.Replace("[", string.Empty).Replace("]", string.Empty)).ToArray(); // 不要文字削除
            // FIXME: 1小節を4拍として計算
            return meas_string_array.Length * 4;
        }
    }
}
