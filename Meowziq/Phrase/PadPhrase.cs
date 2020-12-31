
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Meowziq.Core;

namespace Meowziq.Phrase {
    // テキスト編集で作曲

    // メロ
    // [X>>-|X>--|X>>-|X>>>] x が半価

    // [1223567] とか音程
    // [xoxoxoxoxox] とかアクセント

    /// <summary>
    /// 文字列からフレーズ生成
    /// </summary>
    public class TextPadPhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuildByPattern(int position, Key key, Pattern pattern) {

            // 1パターン4小節(16拍)と分かってる書き方
            string _p1 = "[1>>>|>>--|----|----][1111|----|----|----][1>>>|>>--|----|----][1111|----|----|----]";
            string _p2 = "[3>>>|>>--|----|----][----|----|----|----][3>>>|>>--|----|----][----|----|----|----]";
            string _p3 = "[5>>>|>>--|----|----][5555|----|----|----][5>>>|>>--|----|----][5555|----|----|----]";

            // コード low, mid, high, highhigh?
            //string _mid1 = "[x>>>|>>>>|----|----]";
            //string _mid3 = "[x>>>|>>>>|----|----]";
            //string _mid5 = "[x>>>|>>>>|----|----]";
            //string _mid7 = "[x>>>|>>>>|----|----]";

            // Note 生成
            applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _p1);
            applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _p2);
            applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _p3);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        private void applyMonoNote(int position, int beatCount, Key key, List<Span> spanList, string target) {
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
                    Add(new Note(position + (120 * _index), (int) _noteNum, _gate, 127)); // +1 は数値分
                }
                _index++; // 16beatを進める
                _indexCount++; // Span 用のカウンタも進める
            }
        }
    }
}
