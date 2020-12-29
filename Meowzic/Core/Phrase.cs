using System.Collections.Generic;

namespace Meowzic.Core {
    /// <summary>
    /// フレーズはキー、スケールを外部から与えられる
    ///     + プリセットフレーズ
    ///     + ユーザーがフレーズを拡張出来る
    /// </summary>
    abstract public class Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        protected int beat; // 拍数 ⇒ 4 で 4/4 の 1小節

        protected List<Note> noteList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase(int beat) {
            this.beat = beat; // これは必要ない？
            noteList = new List<Note>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void Add(Note note) {
            noteList.Add(note);
        }

        public void Build(int position, Key key, Part pattern) {
            onBuild(position, key, pattern);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        abstract protected void onBuild(int position, Key key, Part pattern);
    }
}
