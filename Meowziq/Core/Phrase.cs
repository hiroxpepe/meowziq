using System.Collections.Generic;

namespace Meowziq.Core {
    /// <summary>
    /// フレーズはキー、スケールを外部から与えられる
    ///     + プリセットフレーズ
    ///     + ユーザーがフレーズを拡張出来る
    /// </summary>
    abstract public class Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        protected List<Note> noteList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            noteList = new List<Note>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void Add(Note note) {
            noteList.Add(note);
        }

        public void Build(int position, Key key, Span span) {
            onBuild(position, key, span);
        }

        public List<Note> GetAllNote() {
            return noteList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        abstract protected void onBuild(int position, Key key, Span span);
    }
}
