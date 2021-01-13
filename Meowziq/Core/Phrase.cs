
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

        Field field;

        Value.Note note;

        Value.Chord chord;

        Value.Exp exp;

        Data data;

        List<Note> noteList; // Pattern の設定回数分の Note を格納

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            this.field = new Field();
            this.note = new Value.Note();
            this.chord = new Value.Chord();
            this.exp = new Value.Exp();
            this.data = new Data(); // json から詰められるデータ
            this.noteList = new List<Note>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public string Type {
            get => field.Type;
            set => field.Type = value;
        }

        public string Name {
            get => field.Name;
            set => field.Name = value;
        }

        public string Note {
            set => note.Text = value;
        }

        public int Oct {
            set => note.Oct = value; // デフォルト値は 0
        }

        public string Chord {
            set => chord.Text = value;
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
                chord.Range = new Range(
                    int.Parse(_rangeArray[0]),
                    int.Parse(_rangeArray[1])
                );
                if (chord.Range.Min < 0) {
                    throw new ArgumentException("invalid range min.");
                }
                if (chord.Range.Max > 127) {
                    throw new ArgumentException("invalid range max.");
                }
                if (chord.Range.Max - chord.Range.Min != 11) { // オクターブの範囲外
                    var _okMax = chord.Range.Min + 11;
                    var _okMin = chord.Range.Max - 11;
                    throw new ArgumentException($"invalid range length,\r\nmust set {chord.Range.Min}:{_okMax} or {_okMin}:{chord.Range.Max}.");
                }
            }
        }

        public string Pre {
            set => exp.Pre = value;
        }

        public string Post {
            set => exp.Post = value;
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
                switch (defineDataType()) {
                    case DataType.Mono:
                        return measCount(note.Text);
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
                if (!field.Type.ToLower().Contains("drum")) { // ドラム以外 TODO: これで良いか確認
                    optimize(); // 最適化する
                }
                return noteList;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Note データを生成します
        /// NOTE: Player オブジェクトから呼ばれます
        /// </summary>
        public void Build(int position, Key key, Pattern pattern) {
            onBuild(position, key, pattern);
        }

        /// <summary>
        /// シンコペーションで被る Note を除外します
        /// </summary>
        public void RemoveBy(Note target) {
            noteList = noteList
                .Where(x => !(!x.StopPre && x.Tick == target.Tick)) // FIXME: ドラムは音毎？
                .ToList(); // 優先ノートではなく tick が同じものを削除
            if (target.Gate > Tick.Of8beat.Int32()) { // シンコぺが 符点8分音符 の場合
                noteList = noteList
                    .Where(x => !(!x.StopPre && x.Tick == target.Tick + Tick.Of16beat.Int32())) // さらに被る16音符を削除
                    .ToList();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        /// <summary>
        /// Note データを生成します
        /// </summary>
        protected void onBuild(int position, Key key, Pattern pattern) {
            //if (BeatCount != pattern.BeatCount) {
            //    throw new ArgumentException("invalid beatCount.");
            //}
            // NOTE: Type で分岐：プラグイン拡張出来るように
            var _generator = new Generator(noteList); // TODO: コンストラクタで生成
            var _dataType = defineDataType(); // TODO: switch で置き換え
            if (_dataType == DataType.Mono) {
                var _param = new Param(note, exp, _dataType);
                _generator.ApplyNote(position, pattern.BeatCount, key, pattern.AllSpan, _param);
            }
            if (_dataType == DataType.Chord) {
                var _param = new Param(chord, exp, _dataType);
                _generator.ApplyNote(position, pattern.BeatCount, key, pattern.AllSpan, _param);
            }
            if (_dataType == DataType.Multi) {
                for (var _idx = 0; _idx < data.NoteArray.Length; _idx++) {
                    var _param = new Param(
                        new Value.Note(data.NoteArray[_idx], data.OctArray[_idx]),
                        new Value.Exp(data.PreArray[_idx], data.PostArray[_idx]),
                        _dataType
                    );
                    _generator.ApplyNote(position, pattern.BeatCount, key, pattern.AllSpan, _param);
                }
            }
            if (_dataType == DataType.Drum) {
                for (var _idx = 0; _idx < data.NoteArray.Length; _idx++) {
                    var _param = new Param(
                        new Value.Note(data.NoteArray[_idx], 0),
                        (int) data.PercussionArray[_idx],
                        new Value.Exp(data.PreArray[_idx], ""),
                        _dataType
                    );
                    _generator.ApplyDrumNote(position, pattern.BeatCount, _param);
                }
            }
            if (_dataType == DataType.Sequence) {
                _generator.ApplyRandomNote(position, pattern.BeatCount, key, pattern.AllSpan);
            }
            // UI 表示情報作成
            _generator.ApplyInfo(position, pattern.BeatCount, key, pattern.AllSpan);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void optimize() {
            // MEMO: 消したい音はこのフレーズではない場合もある
            foreach (var _stopNote in noteList.Where(x => x.StopPre)) { // 優先ノートのリスト
                foreach (var _note in noteList) { // このフレーズの全てのノートの中で
                    if (_note.Tick == _stopNote.Tick) { // 優先ノートと発音タイミングがかぶったら
                        RemoveBy(_note); // ノートを削除
                    }
                }
            }
        }

        /// <summary>
        /// json に記述されたデータのタイプを判定します
        /// </summary>
        DataType defineDataType() {
            if (data.NoteArray == null && note.Text != null && chord.Text == null && data.PercussionArray == null) {
                return DataType.Mono; // 単体ノート記述 
            } else if (data.NoteArray != null && note.Text == null && chord.Text == null && data.PercussionArray == null) {
                return DataType.Multi; // 複合ノート記述
            } else if (data.NoteArray == null && note.Text == null && chord.Text != null && data.PercussionArray == null) {
                return DataType.Chord; // コード記述
            } else if (data.NoteArray != null && note.Text == null && chord.Text == null && data.PercussionArray != null) {
                return DataType.Drum; // ドラム記述 
            } else if (data.NoteArray == null && note.Text == null && chord.Text == null && data.PercussionArray == null) {
                return DataType.Sequence; // TODO: 暫定
            }
            throw new ArgumentException("not understandable DataType.");
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

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Field クラス
        /// TODO: 必要？
        /// </summary>
        class Field {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjectives] 

            public string Type {
                get; set;
            }
            public string Name {
                get; set;
            }
        }
    }
}
