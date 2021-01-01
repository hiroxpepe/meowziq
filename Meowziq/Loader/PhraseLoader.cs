
using System.Linq;

namespace Meowziq.Loader {
    /// <summary>
    /// Phrase のローダークラス
    /// </summary>
    public class PhraseLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string target;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public PhraseLoader(string target) {
            validateValue(target); // バリデート実行
            this.target = target;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        /// <summary>
        /// 拍数
        /// </summary>
        public int BeatCount {
            get {
                var _target1 = target.Replace("[", "|").Replace("]", "|");
                var _count = _target1.Split('|')
                    .Where(x => !string.IsNullOrWhiteSpace(x)) // 空文字以外
                    .ToArray().Length;
                return _count;
            }
        }

        /// <summary>
        /// 小節数
        /// </summary>
        public int MeasCount {
            get {
                return 0;
            }
        }

        public int HogeCount {
            get {
                return 0;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        bool validateValue(string target) {
            // ---
            // 拍のデータの数が4文字かどうか
            var _target1 = target;
            // 文字の置き換え
            _target1 = _target1.Replace("[", "|").Replace("]", "|");
            // 区切り文字で切り分ける
            var _array1 = _target1.Split('|')
                .Where(x => !string.IsNullOrWhiteSpace(x)) // 空文字以外
                .Where(x => x.Length != 4) // データが4文字ではない
                .ToArray();
            // そのデータがあれば例外を投げる
            if (_array1.Length != 0) {
                throw new System.FormatException("data count must be 4.");
            }

            // TODO: 使用可能な文字

            // バリデーションOKなら true を返す
            return true;
        }
    }
}
