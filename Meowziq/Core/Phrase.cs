
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Meowziq.Core {
    /// <summary>
    /// フレーズはキー、スケールを外部から与えられる
    ///     + プリセットフレーズ
    ///     + ユーザーがフレーズを拡張出来る
    /// </summary>
    abstract public class Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        protected List<Note> noteList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            noteList = new List<Note>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

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

        public void BuildByPattern(int position, Key key, Pattern pattern) {
            onBuildByPattern(position, key, pattern);
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
        // abstract protected Methods [verb]

        abstract protected void onBuildByPattern(int position, Key key, Pattern pattern);

    }
}
