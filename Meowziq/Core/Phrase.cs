﻿
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

        string type;

        Data data; // json から読み込んだデータを格納

        Item<Note> noteItem; // Tick 毎の Note のリスト、Pattern の設定回数分の Note を格納

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            this.data = new Data(); // json から詰められるデータ
            this.noteItem = new Item<Note>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Type {
            get => type;
            set {
                type = value;
                if (type.Equals("seque")) {
                    data.Seque.Use = true;
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
            get => data;
            set => data = value;
        }

        /// <summary>
        /// TODO: default レンジ
        /// </summary>
        public string Range {
            set {
                if (value is null) {
                    return;
                }
                var _rangeText = value;
                if (!_rangeText.Contains(":")) {
                    throw new ArgumentException("invalid range format.");
                }
                var _rangeArray = _rangeText.Split(':');
                if (_rangeArray.Length != 2) {
                    throw new ArgumentException("invalid range format.");
                }
                if (data.HasChord) {
                    data.Chord.Range = new Range(
                        int.Parse(_rangeArray[0]),
                        int.Parse(_rangeArray[1])
                    );
                } else if (data.HasSeque) {
                    data.Seque.Range = new Range(
                        int.Parse(_rangeArray[0]),
                        int.Parse(_rangeArray[1])
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
        /// FIXME: バグあり ⇒ シンコペーションは小節の頭だけ許可する？
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
                        var _param = new Param(data.Note, data.Exp, _way); // NOTE: "note", "auto" データ判定済
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
                    var _stringArray = data.Auto ? data.AutoArray : data.NoteArray;
                    for (var _idx = 0; _idx < _stringArray.Length; _idx++) { // TODO: for の置き換え
                        var _param = new Param(
                            new Value.Note(_stringArray[_idx], data.OctArray[_idx]),
                            new Value.Exp(data.PreArray[_idx], data.PostArray[_idx]),
                            _way,
                            data.Auto
                        );
                        _generator.ApplyNote(tick, pattern.BeatCount, pattern.AllSpan, _param);
                    }
                    break;
                case Way.Drum:
                    data.BeatArray.ToList().Select((x, idx) => new Param(
                        new Value.Note(x, 0), // オクターブは常に 0
                        (int) data.PercussionArray[idx],
                        new Value.Exp(data.PreArray[idx], ""),
                        _way
                    )).ToList().ForEach(x => _generator.ApplyDrumNote(tick, pattern.BeatCount, x));
                    break;
                case Way.Seque:
                    {
                        var _param = new Param(data.Seque, _way);
                        _generator.ApplySequeNote(tick, pattern.BeatCount, pattern.AllSpan, _param);
                    }
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
            if (!data.HasMulti && (data.HasNote || data.HasAuto)) {
                return Way.Mono; // 単体 "note", "auto" ノート記述 
            }
            else if (data.HasMulti && (data.HasNote || data.HasAuto)) {
                return Way.Multi; // 複合 "note", "auto" 記述
            }
            else if (data.HasChord) {
                return Way.Chord; // "chord" 記述
            }
            else if (data.HasBeat) {
                return Way.Drum; // "beat" 記述 
            }
            else if (data.HasSeque) {
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
            var _measStringArray = target.Replace("][", "@")  // まず "][" を "@" に置き換え
                .Split('@') // 小節で切り分ける
                .Select(x => x.Replace("[", "").Replace("]", "")).ToArray(); // 不要文字削除
            // FIXME: 1小節を4拍として計算
            return _measStringArray.Length * 4;
        }
    }
}
