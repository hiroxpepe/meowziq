

using Meowziq.Core;

namespace Meowziq.Phrase {
    /// <summary>
    /// 文字列からフレーズ生成
    /// </summary>
    public class TextPadPhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuildByPattern(int position, Key key, Pattern pattern) {
            if (pattern.Name.Equals("verse1")) {
                // 1パターン4小節(16拍)と分かってる書き方
                string _p1 = "[1>>>|>>--|----|----][1111|----|----|----][1>>>|>>--|----|----][1111|----|----|----]";
                string _p2 = "[3>>>|>>--|----|----][----|----|----|----][3>>>|>>--|----|----][----|----|----|----]";
                string _p3 = "[5>>>|>>--|----|----][5555|----|----|----][5>>>|>>--|----|----][5555|----|----|----]";
                applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _p1);
                applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _p2);
                applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _p3);
            }
        }
    }
}
