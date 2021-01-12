
using System;
using System.Collections.Generic;
using System.Linq;

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

        Data data;

        List<Note> noteList; // Pattern の設定回数分の Note を格納

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            this.field = new Field();
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

        public string NoteText {
            set => field.NoteText = value;
        }

        public int Oct {
            set => field.Oct = value; // デフォルト値は 0
        }

        public string ChordText {
            set => field.ChordText = value;
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
                field.RangeMin = int.Parse(_rangeArray[0]);
                field.RangeMax = int.Parse(_rangeArray[1]);
                if (field.RangeMin < 0) {
                    throw new ArgumentException("invalid range min.");
                }
                if (field.RangeMax > 127) {
                    throw new ArgumentException("invalid range max.");
                }
                if (field.RangeMax - field.RangeMin != 11) { // オクターブの範囲外
                    var _okMax = field.RangeMin + 11;
                    var _okMin = field.RangeMax - 11;
                    throw new ArgumentException($"invalid range length,\r\nmust set {field.RangeMin}:{_okMax} or {_okMin}:{field.RangeMax}.");
                }
            }
        }

        public string Pre {
            set => field.Pre = value;
        }

        public string Post {
            set => field.Post = value;
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
                switch (getDataType()) {
                    case DataType.NoteMono:
                        return measCount(field.NoteText);
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
            var _generator = new Generator(noteList);
            var _dataType = getDataType();
            if (_dataType == DataType.NoteMono) {
                var _text = new Text(field.NoteText, getDataType());
                var _interval = field.Oct * 12; // オクターブ設定からインターバル作成
                _generator.ApplyNote(position, pattern.BeatCount, key, pattern.AllSpan, _text, _interval, null, field.Pre, field.Post);
            }
            if (_dataType == DataType.Chord) {
                var _text = new Text(field.ChordText, getDataType());
                var _range = new Range(field.RangeMin, field.RangeMax);
                _generator.ApplyNote(position, pattern.BeatCount, key, pattern.AllSpan, _text, 0, _range, field.Pre, field.Post);
            }
            if (_dataType == DataType.NoteMulti) {
                for (var _idx = 0; _idx < data.NoteTextArray.Length; _idx++) {
                    var _noteText = data.NoteTextArray[_idx];
                    var _interval = data.OctArray[_idx] * 12; // オクターブ設定からインターバル作成
                    var _pre = data.PreArray[_idx];
                    var _post = data.PostArray[_idx];
                    var _text = new Text(_noteText, getDataType());
                    _generator.ApplyNote(position, pattern.BeatCount, key, pattern.AllSpan, _text, _interval, null, _pre, _post);
                }
            }
            if (_dataType == DataType.Drum) {
                for (var _idx = 0; _idx < data.NoteTextArray.Length; _idx++) {
                    var _text = data.NoteTextArray[_idx];
                    var _pre = data.PreArray[_idx];
                    _generator.ApplyDrumNote(position, pattern.BeatCount, _text, data.PercussionArray[_idx], _pre);
                }
            }
            if (_dataType == DataType.Sequence) {
                _generator.ApplyRandomNote(position, pattern.BeatCount, key, pattern.AllSpan);
            }
            // UI 表示情報
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
        DataType getDataType() {
            if (data.NoteTextArray == null && field.NoteText != null && field.ChordText == null && data.PercussionArray == null) {
                return DataType.NoteMono; // 単体ノート記述 
            } else if (data.NoteTextArray != null && field.NoteText == null && field.ChordText == null && data.PercussionArray == null) {
                return DataType.NoteMulti; // 複合ノート記述
            } else if (data.NoteTextArray == null && field.NoteText == null && field.ChordText != null && data.PercussionArray == null) {
                return DataType.Chord; // コード記述
            } else if (data.NoteTextArray != null && field.NoteText == null && field.ChordText == null && data.PercussionArray != null) {
                return DataType.Drum; // ドラム記述 
            } else if (data.NoteTextArray == null && field.NoteText == null && field.ChordText == null && data.PercussionArray == null) {
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
            public string NoteText {
                get; set;
            }
            public int Oct {
                get; set;
            }
            public string ChordText {
                get; set;
            }
            public int RangeMin {
                get; set; // TODO: 範囲を小節内で指定出来るように
            }
            public int RangeMax {
                get; set;
            }
            public string Pre {
                get; set;
            }
            public string Post {
                get; set;
            }
        }
    }
}
