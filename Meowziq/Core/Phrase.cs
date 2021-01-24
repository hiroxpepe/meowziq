
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

        Data data; // json から読み込んだデータを格納

        Item<Note> noteItem; // Tick 毎の Note のリスト、Pattern の設定回数分の Note を格納

        HashSet<int> hashSet; // ※Dictionary.ContainsKey() が遅いのでその対策

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            this.data = new Data(); // json から詰められるデータ
            this.noteItem = new Item<Note>();
            this.hashSet = new HashSet<int>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Type {
            get; set;
        }

        public string Name {
            get; set;
        }

        public string Note {
            set => data.Note.Text = value;
        }

        public string Auto {
            set => data.Auto.Text = value;
        }

        public int Oct {
            set => data.Note.Oct = value; // デフォルト値は 0
        }

        public string Chord {
            set => data.Chord.Text = value;
        }

        /// <summary>
        /// TODO: default レンジ
        /// </summary>
        public string Range {
            set {
                if (value == null) return;
                var _rangeText = value;
                if (!_rangeText.Contains(":")) {
                    throw new ArgumentException("invalid range format.");
                }
                var _rangeArray = _rangeText.Split(':');
                data.Chord.Range = new Range(
                    int.Parse(_rangeArray[0]),
                    int.Parse(_rangeArray[1])
                );
                if (data.Chord.Range.Min < 0) {
                    throw new ArgumentException("invalid range min.");
                }
                if (data.Chord.Range.Max > 127) {
                    throw new ArgumentException("invalid range max.");
                }
                if (data.Chord.Range.Max - data.Chord.Range.Min != 11) { // オクターブの範囲外
                    var _okMax = data.Chord.Range.Min + 11;
                    var _okMin = data.Chord.Range.Max - 11;
                    throw new ArgumentException($"invalid range length,\r\nmust set {data.Chord.Range.Min}:{_okMax} or {_okMin}:{data.Chord.Range.Max}.");
                }
            }
        }

        public string Pre {
            set => data.Exp.Pre = value;
        }

        public string Post {
            set => data.Exp.Post = value;
        }

        public Data Data {
            get => data;
            set => data = value;
        }

        /// <summary>
        /// TBA: シーケンス
        /// Phrase の拍数を返す
        /// </summary>
        public int BeatCount {
            get {
                switch (defineWay()) {
                    case Way.Mono:
                        return measCount(data.Note.Text);
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
                return noteItem.SelectMany(x => x.Value).Select(x => x).ToList();
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
        /// FIXME: バグあり ⇒ シンコペーションは小節の頭だけ許可する
        /// </summary>
        public void RemoveBy(Note target) {
            var _tick1 = target.Tick;
            var _noteList1 = noteItem.Get(_tick1);
            _noteList1 = _noteList1.Where(x => !(!x.HasPre && x.Tick == _tick1)).ToList(); // 優先ノートではなく tick が同じものを削除 // FIXME: ドラムは音毎？
            noteItem.SetBy(_tick1, _noteList1);
            if (target.PreCount > 1) { // さらにシンコぺの設定値が2の場合
                var _tick2 = target.Tick + Length.Of16beat.Int32();
                var _noteList2 = noteItem.Get(_tick2);
                if (_noteList2 != null) {
                    _noteList2 = _noteList2.Where(x => !(!x.HasPre && x.Tick == _tick2)).ToList(); // さらに被る16音符を削除
                    noteItem.SetBy(_tick2, _noteList2);
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
            var _generator = new Generator(noteItem); // NOTE: コンストラクタで生成ではNG
            var _way = defineWay();
            switch (_way) {
                case Way.Mono:
                    {
                        var _param = new Param(data.Note, data.Exp, _way);
                        _generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, _param);
                    }
                    break;
                case Way.Chord:
                    {
                        var _param = new Param(data.Chord, data.Exp, _way);
                        _generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, _param);
                    }
                    break;
                case Way.Multi:
                    for (var _idx = 0; _idx < data.NoteArray.Length; _idx++) { // TODO: for の置き換え
                        var _param = new Param(
                            new Value.Note(data.NoteArray[_idx], data.OctArray[_idx]),
                            new Value.Exp(data.PreArray[_idx], data.PostArray[_idx]),
                            _way
                        );
                        _generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, _param);
                    }
                    break;
                case Way.Drum:
                    for (var _idx = 0; _idx < data.NoteArray.Length; _idx++) { // TODO: for の置き換え
                        var _param = new Param(
                            new Value.Note(data.NoteArray[_idx], 0),
                            (int) data.PercussionArray[_idx],
                            new Value.Exp(data.PreArray[_idx], ""),
                            _way
                        );
                        _generator.ApplyDrumNote(tick, pattern.BeatCount, _param);
                    }
                    break;
                case Way.Sequence:
                    _generator.ApplyRandomNote(tick, pattern.BeatCount, pattern.AllSpan);
                    break;
            }
            // UI 表示情報作成
            _generator.ApplyInfo(tick, pattern.BeatCount, pattern.AllSpan);
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
            var _noteList = noteItem.SelectMany(x => x.Value).Select(x => x).ToList();
            foreach (var _stopNote in _noteList.Where(x => x.HasPre)) { // 優先ノートのリスト
                foreach (var _note in _noteList) { // このフレーズの全てのノートの中で
                    if (_note.Tick == _stopNote.Tick) { // 優先ノートと発音タイミングがかぶったら
                        RemoveBy(_note); // ノートを削除
                    }
                }
            }
        }

        /// <summary>
        /// json に記述されたデータのタイプを判定します
        /// </summary>
        Way defineWay() {
            if (data.NoteArray == null && data.Note.Text != null && data.Chord.Text == null && data.PercussionArray == null) {
                return Way.Mono; // 単体ノート記述 
            } else if (data.NoteArray != null && data.Note.Text == null && data.Chord.Text == null && data.PercussionArray == null) {
                return Way.Multi; // 複合ノート記述
            } else if (data.NoteArray == null && data.Note.Text == null && data.Chord.Text != null && data.PercussionArray == null) {
                return Way.Chord; // コード記述
            } else if (data.NoteArray != null && data.Note.Text == null && data.Chord.Text == null && data.PercussionArray != null) {
                return Way.Drum; // ドラム記述 
            } else if (data.NoteArray == null && data.Note.Text == null && data.Chord.Text == null && data.PercussionArray == null) {
                return Way.Sequence; // TODO: 暫定
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
            var _measStringArray = target.Replace("][", "@")  // まず "][" を "@" に置き換え
                .Split('@') // 小節で切り分ける
                .Select(x => x.Replace("[", "").Replace("]", "")).ToArray(); // 不要文字削除
            // FIXME: 1小節を4拍として計算
            return _measStringArray.Length * 4;
        }
    }
}
