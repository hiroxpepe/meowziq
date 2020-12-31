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
        // Properties [noun, adjectives] 

        /// <summary>
        /// 全ての Note
        /// </summary>
        public List<Note> AllNote {
            get => noteList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void Add(Note note) {
            noteList.Add(note);
        }

        public void BuildByPattern(int position, Key key, Pattern pattern) {
            onBuildByPattern(position, key, pattern);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        protected static string filter(string target) {
            // 不要文字削除
            return target.Replace("|", "").Replace("[", "").Replace("]", "");
        }

        protected static List<bool?> convertToBool(string target) {
            // on, off, null スイッチ
            List<bool?> _list = new List<bool?>();
            // 文字列を並列に変換
            char[] _charArray = target.ToCharArray();
            // 文字列を判定
            foreach (char _char in _charArray) {
                if (_char.Equals('x')) {
                    _list.Add(true);
                } else if (_char.Equals('-')) {
                    _list.Add(null);
                }
            }
            return _list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // abstract protected Methods [verb]

        abstract protected void onBuildByPattern(int position, Key key, Pattern pattern);

    }
}
