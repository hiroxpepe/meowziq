

using Meowziq.Core;

namespace Meowziq.Phrase {
    /// <summary>
    /// 文字列からフレーズ生成
    /// </summary>
    public class TextBassPhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuildByPattern(int position, Key key, Pattern pattern) {

            // 1パターン4小節(16拍)と分かってる書き方
            string _l1 = "[1-1-|3-3-|5-5-|7-7-][1-1-|3-3-|5-5-|7-7-][1-11|3-33|5-55|3-33][1-11|3-33|5-55|7-77]";

            // Note 生成
            applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _l1, -24);
        }
    }
}
