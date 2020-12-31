
using Meowziq.Core;

namespace Meowziq.Phrase {

    public class RandomSequencePhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuildByPattern(int position, Key key, Pattern pattern) {
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
}
