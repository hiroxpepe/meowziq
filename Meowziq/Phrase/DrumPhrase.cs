
using Meowziq.Core;

namespace Meowziq.Phrase {
    /// <summary>
    /// 文字列からフレーズ生成
    /// </summary>
    public class TextDrumPhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        // パターン(4小節)をスパン(キー・度数・旋法)を踏まえて処理するには？

        override protected void onBuildByPattern(int position, Key key, Pattern pattern) {
            // テンプレートを1スパンづつ翻訳していく

            // 1パターン4小節(16拍)と分かってる書き方
            string _d0 = "[----|----|----|----][----|----|----|----][----|----|----|----][----|----|----|--x-]";
            string _d1 = "[xxxx|xxxx|xxxx|xx--][xxxx|xxxx|xxxx|xx--][xxxx|xxxx|xxxx|xx--][xxxx|xxxx|xxxx|xx--]";
            string _d2 = "[----|----|----|--x-][----|----|----|--x-][----|----|----|--x-][----|----|----|--x-]";
            string _d3 = "[----|x---|----|x---][----|x---|----|x---][----|x---|----|x---][----|x---|----|x---]";
            string _d4 = "[x---|---x|x---|----][x---|---x|x---|----][x---|---x|x---|----][x---|---x|x---|----]";

            // コード low, mid, high, highhigh?
            //string _mid1 = "[x>>>|>>>>|----|----]";
            //string _mid3 = "[x>>>|>>>>|----|----]";
            //string _mid5 = "[x>>>|>>>>|----|----]";
            //string _mid7 = "[x>>>|>>>>|----|----]";

            // Note 生成
            applyDrumNote(position, pattern.BeatCount, _d0, Percussion.Crash_Cymbal_1);
            applyDrumNote(position, pattern.BeatCount, _d1, Percussion.Closed_Hi_hat);
            applyDrumNote(position, pattern.BeatCount, _d2, Percussion.Open_Hi_hat);
            applyDrumNote(position, pattern.BeatCount, _d3, Percussion.Electric_Snare);
            applyDrumNote(position, pattern.BeatCount, _d4, Percussion.Electric_Bass_Drum);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        private void applyDrumNote(int position, int beatCount, string target, Percussion noteNum) {
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
    }
}
