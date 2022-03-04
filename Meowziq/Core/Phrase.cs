
using System;
using System.Collections.Generic;
using System.Linq;

using Meowziq.Value;

namespace Meowziq.Core {
    /// <summary>
    /// Phrase クラス
    ///     + キーと旋法は外部から与えられる
    ///     + Note オブジェクトのリストを管理する
    /// </summary>
    public class Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _type;

        Data _data; // json から読み込んだデータを格納

        Item<Note> _noteItem; // Tick 毎の Note のリスト、Pattern の設定回数分の Note を格納

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            _data = new Data(); // json から詰められるデータ
            _noteItem = new Item<Note>();
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
                var rangeText = value;
                if (!rangeText.Contains(":")) {
                    throw new ArgumentException("invalid range format.");
                }
                var rangeArray = rangeText.Split(':');
                if (rangeArray.Length != 2) {
                    throw new ArgumentException("invalid range format.");
                }
                if (_data.HasChord) {
                    _data.Chord.Range = new Range(
                        int.Parse(rangeArray[0]),
                        int.Parse(rangeArray[1])
                    );
                } else if (_data.HasSeque) {
                    _data.Seque.Range = new Range(
                        int.Parse(rangeArray[0]),
                        int.Parse(rangeArray[1])
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
                return _noteItem.SelectMany(x => x.Value).Select(x => x).ToList();
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
            var noteList1 = _noteItem.Get(tick1);
            noteList1 = noteList1.Where(x => !(!x.HasPre && x.Tick == tick1)).ToList(); // 優先ノートではなく tick が同じものを削除 // FIXME: ドラムは音毎？
            _noteItem.SetBy(tick1, noteList1);
            if (target.PreCount > 1) { // さらにシンコぺの設定値が2の場合
                var tick2 = target.Tick + Length.Of16beat.Int32();
                var noteList2 = _noteItem.Get(tick2);
                if (noteList2 != null) {
                    noteList2 = noteList2.Where(x => !(!x.HasPre && x.Tick == tick2)).ToList(); // さらに被る16音符を削除
                    _noteItem.SetBy(tick2, noteList2);
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
            var generator = new Generator(_noteItem); // NOTE: コンストラクタで生成ではNG
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
                    var stringArray = _data.Auto ? _data.AutoArray : _data.NoteArray;
                    for (var idx = 0; idx < stringArray.Length; idx++) { // TODO: for の置き換え
                        var param = new Param(
                            new Value.Note(stringArray[idx], _data.OctArray[idx]),
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
                        new Value.Exp(_data.PreArray[idx], ""),
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
            var noteList = _noteItem.SelectMany(x => x.Value).Select(x => x).ToList();
            foreach (var stopNote in noteList.Where(x => x.HasPre)) { // 優先ノートのリスト
                foreach (var note in noteList) { // このフレーズの全てのノートの中で
                    if (note.Tick == stopNote.Tick) { // 優先ノートと発音タイミングがかぶったら
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
            var measStringArray = target.Replace("][", "@")  // まず "][" を "@" に置き換え
                .Split('@') // 小節で切り分ける
                .Select(x => x.Replace("[", "").Replace("]", "")).ToArray(); // 不要文字削除
            // FIXME: 1小節を4拍として計算
            return measStringArray.Length * 4;
        }
    }
}
