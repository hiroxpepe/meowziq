

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
    }
}
