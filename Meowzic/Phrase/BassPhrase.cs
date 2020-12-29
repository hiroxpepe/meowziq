
using Meowzic.Core;

namespace Meowzic.Phrase {

    // フレーズはキー、スケールは外部から与えられる

    // ここに Duran Duran で聴いたことのあるフレーズを集めていく！
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

        public RootBass8BeatPhrase(int beat) : base(beat) {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuild(int position, Key key, Part part) {
            int _8BeatCount = part.Beat * 2; // 8ビート
            int _note = Utils.GetRootNote(key, part.Degree, part.Mode);
            for (int i = 0; i < _8BeatCount; i++) {
                Add(new Note(position + (240 * i), (_note - 24), 240, 127)); // gate最大、velo最大
            }
        }
    }
}
