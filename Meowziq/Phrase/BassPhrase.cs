
using Meowziq.Core;

namespace Meowziq.Phrase {
    // フレーズはキー、スケールは外部から与えられる
    // ここに聴いたことのあるフレーズを集めていく！
    // テキスト編集で作曲

    // [****|****|****|****] とかテキストをパターンに変換 : ノートON と ノートOFF
    // [****|****|****|****] 1小節

    // ドラム
    // [o-o-|o-o-|o-o-|o-o-]
    // [----|o---|----|o---]
    // [o---|----|o---|----]

    // メロ
    // [X>>-|X>--|X>>-|X>>>] x が半価

    // [1223567] とか音程
    // [xoxoxoxoxox] とかアクセント

    /// <summary>
    /// 8ビートのルート刻みベース
    /// </summary>
    public class RootBass8BeatPhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public RootBass8BeatPhrase() {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuild(int position, Key key, Span span) {
            int _8BeatCount = span.Beat * 2; // 8ビート
            int _note = Utils.GetRootNote(key, span.Degree, span.Mode);
            for (int i = 0; i < _8BeatCount; i++) {
                Add(new Note(position + (240 * i), (_note - 24), 240, 127)); // gate最大、velo最大
            }
        }
    }
}
