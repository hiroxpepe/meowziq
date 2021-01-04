
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Meowziq.Core {
    /// <summary>
    /// フレーズはキー、スケールを外部から与えられる
    ///     + プリセットフレーズ
    ///     + ユーザーがフレーズを拡張出来る
    /// </summary>
    public class Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string type;

        string name;

        string noteText;

        Data data;

        List<Note> noteList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            data = new Data(); // json から詰められるデータ
            noteList = new List<Note>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public string Type {
            get => type;
            set => type = value;
        }

        public string Name {
            set => name = value;
        }

        public string NoteText {
            set => noteText = value;
        }

        public Data DataValue {
            get => data;
            set => data = value;
        }

        /// <summary>
        /// 全ての Note
        /// </summary>
        public List<Note> AllNote {
            get => noteList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void Add(Note note) {
            noteList.Add(note);
        }

        /// <summary>
        /// Player オブジェクトから呼ばれます
        /// </summary>
        public void Build(int position, Key key, Pattern pattern) {
            onBuild(position, key, pattern);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        protected void applyMonoNote(int position, int beatCount, Key key, List<Span> spanList, string target, int interval = 0) {
            int _index = 0; // 16beatのindex
            int _indexCount = 0; // 16beatで1拍をカウントする用
            int _spanIndex = 0; // Span リストの添え字
            var _valueArray = filter(target).ToCharArray();
            foreach (var _value in _valueArray) {
                if (_index > beatCount * 4) {
                    return; // Pattern の長さを超えたら終了 FIXME: 長さに足りない時？ エラー? リピート？
                }
                if (_indexCount == 4) { // 16beatが4回進んだ時(1拍)
                    _indexCount = 0; // カウンタリセット
                    _spanIndex++; // Span の index値
                }
                if (Regex.IsMatch(_value.ToString(), @"^[1-7]+$")) { // 度数の数値がある時
                    var _span = spanList[_spanIndex];
                    int _noteNum;
                    // 曲の旋法と Span の旋法が同じ場合は自動旋法
                    if (_span.KeyMode == _span.Mode) {
                        _noteNum = Utils.GetNoteWithAutoMode(key, _span.Degree, _span.KeyMode, int.Parse(_value.ToString()));
                    }
                    // Span に旋法が設定してあればそちらを適用する
                    else {
                        _noteNum = Utils.GetNoteWithSpanMode(key, _span.Degree, _span.KeyMode, _span.Mode, int.Parse(_value.ToString()));
                    }
                    // この音の音価を調査する
                    int _gateCount = 0;
                    int _all16beatcount = (beatCount * 4);
                    for (var _i = _index + 1; _i < _all16beatcount; _i++) {
                        var _search = _valueArray[_i];
                        if (_search.Equals('>')) {
                            _gateCount++; // 16beat分伸ばす
                        }
                        if (!_search.Equals('>')) {
                            break; // '>' が途切れたら終了
                        }
                    }
                    var _gate = 120 * (_gateCount + 1);
                    Add(new Note(position + (120 * _index), (int) _noteNum + interval, _gate, 127)); // +1 は数値分
                }
                _index++; // 16beatを進める
                _indexCount++; // Span 用のカウンタも進める
            }
        }

        protected void applyDrumNote(int position, int beatCount, string target, Percussion noteNum) {
            int _index = 0;
            foreach (bool? _value in convertToBool(filter(target))) {
                if (_index > beatCount * 4) {
                    return; // Pattern の長さを超えたら終了 FIXME: 長さに足りない時？ エラー? リピート？
                }
                if (_value == true) {
                    Add(new Note(position + (120 * _index), (int) noteNum, 120, 127));
                }
                _index++; // 16beatを進める
            }
        }

        protected static string filter(string target) {
            // 不要文字削除
            return target.Replace("|", "").Replace("[", "").Replace("]", "");
        }

        protected static List<bool?> convertToBool(string target) {
            // on, off, null スイッチ
            List<bool?> _list = new List<bool?>();
            // 文字列を並列に変換
            char[] _charArray = target.ToCharArray();
            // 文字列を判定
            foreach (char _char in _charArray) {
                if (_char.Equals('x')) {
                    _list.Add(true);
                } else if (_char.Equals('-')) {
                    _list.Add(null);
                }
            }
            return _list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        /// <summary>
        /// TODO: パターンとフレーズの長さが一致しているかどうか
        /// </summary>
        protected void onBuild(int position, Key key, Pattern pattern) {
            // パターンの名前が違えば何もしない
            if (!name.Equals(pattern.Name)) {
                return;
            }
            // FIXME: Type で分離 ⇒ 処理のパターン決め込みで良い：プラグイン拡張出来るように
            if (type.Equals("drum")) {
                for (var _i = 0; _i < data.NoteTextArray.Length; _i++) {
                    var _text = data.NoteTextArray[_i];
                    applyDrumNote(position, pattern.BeatCount, _text, data.PercussionArray[_i]);
                }
            }
            if (type.Equals("pad")) {
                for (var _i = 0; _i < data.NoteTextArray.Length; _i++) {
                    var _text = data.NoteTextArray[_i];
                    var _interval = (data.NoteOctArray[_i] * 12);
                    applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _text, _interval);
                }
            }
            if (type.Equals("bass")) {
                var _text = noteText;
                applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _text, -24);
            }
            if (type.Equals("seque")) {
                int _16beatCount = pattern.BeatCount * 4; // この Pattern の16beatの数
                int _indexCount = 0; // 16beatで1拍をカウントする用
                int _spanIndex = 0; // Span リストの添え字
                for (var _i = 0; _i < _16beatCount; _i++) {
                    if (_indexCount == 4) { // 16beatが4回進んだ時(1拍)
                        _indexCount = 0; // カウンタリセット
                        _spanIndex++; // Span の index値
                    }
                    var _span = pattern.AllSpan[_spanIndex];
                    int _note = Utils.GetNote(key, _span.Degree, _span.Mode, Arpeggio.Random, _i); // 16の倍数
                    Add(new Note(position + (120 * _i), _note, 30, 127)); // gate 短め
                    _indexCount++; // Span 用のカウンタも進める
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        public class Data {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            Percussion[] percussionArray;

            string[] noteTextArray;

            int[] noteOctArray;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjectives] 

            public Percussion[] PercussionArray {
                get => percussionArray;
                set => percussionArray = value;
            }

            public string[] NoteTextArray {
                get => noteTextArray;
                set => noteTextArray = value;
            }

            public int[] NoteOctArray {
                get => noteOctArray;
                set {
                    if (noteTextArray == null) {
                        throw new System.ArgumentException("must set noteTextArray.");
                    }
                    if (value == null) {
                        noteOctArray = new int[noteTextArray.Length];
                        for (var _i = 0; _i < noteTextArray.Length; _i++) {
                            noteOctArray[_i] = 0; // オクターブの設定を自動生成
                        }
                    } else if (value.Length != noteTextArray.Length) {
                        throw new System.ArgumentException("noteOctArray must be same count as noteTextArray.");
                    } else {
                        noteOctArray = value;
                    }
                }
            }
        }
    }
}
