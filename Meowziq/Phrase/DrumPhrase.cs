
using Meowziq.Core;

namespace Meowziq.Phrase {
    /// <summary>
    /// 文字列からフレーズ生成
    /// </summary>
    public class TextDrumPhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuildByPattern(int position, Key key, Pattern pattern) {
            if (pattern.Name.Equals("verse1")) {
                // 1パターン4小節(16拍)と分かってる書き方
                string _d0 = "[----|----|----|----][----|----|----|----][----|----|----|----][----|----|----|--x-]";
                string _d1 = "[x-x-|x-x-|x-x-|x---][x-x-|x-x-|x-x-|x---][xxxx|xxxx|xxxx|xx--][xxxx|xxxx|xxxx|xx--]";
                string _d2 = "[----|----|----|--x-][----|----|----|--x-][----|----|----|--x-][----|----|----|--x-]";
                string _d3 = "[----|x---|----|x---][----|x---|----|x---][----|x---|----|x---][----|x---|----|x---]";
                string _d4 = "[x---|---x|x---|----][x---|---x|x---|----][x---|---x|x---|----][x---|---x|x---|----]";
                applyDrumNote(position, pattern.BeatCount, _d0, Percussion.Crash_Cymbal_1);
                applyDrumNote(position, pattern.BeatCount, _d1, Percussion.Closed_Hi_hat);
                applyDrumNote(position, pattern.BeatCount, _d2, Percussion.Open_Hi_hat);
                applyDrumNote(position, pattern.BeatCount, _d3, Percussion.Electric_Snare);
                applyDrumNote(position, pattern.BeatCount, _d4, Percussion.Electric_Bass_Drum);
            }
        }
    }
}
