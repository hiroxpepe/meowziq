
using System.Collections.Generic;

using Meowzic.Core;

namespace Meowzic.Phrase {
    /// <summary>
    /// 文字列からフレーズ生成
    /// </summary>
    public class TextPhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public TextPhrase() {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuild(int position, Key key, Span span) {
            // position 基点の絶対値で tick 生成
            // span.Beat の長さ分の note データを生成 TODO: 余りは？

            string _kick   = "[x---|---x|x---|---x]";
            string _snare  = "[----|x---|----|x---]";
            string _hiHatO = "[xxxx|xxxx|xxxx|xxxx]";

            int i = 0;
            foreach (bool? _d1 in convert(filter(_kick))) {
                int _kickNote = 36;
                if (_d1 == true) {
                    Add(new Note(position + (120 * i), _kickNote, 120, 127));
                }
                i++;
            }
            i = 0;
            foreach (bool? _d2 in convert(filter(_snare))) {
                int _snareNote = 38;
                if (_d2 == true) {
                    Add(new Note(position + (120 * i), _snareNote, 120, 127));
                }
                i++;
            }
            i = 0;
            foreach (bool? _d3 in convert(filter(_hiHatO))) {
                int _hiHatONote = 42;
                if (_d3 == true) {
                    Add(new Note(position + (120 * i), _hiHatONote, 120, 127));
                }
                i++;
            }
        }

        static string filter(string target) {
            // 不要文字削除
            return target.Replace("|", "").Replace("[", "").Replace("]", "");
        }

        static List<bool?> convert(string target) {
            // on, off, null スイッチ
            List<bool?> _list = new List<bool?>();
            // 文字列を並列に変換
            char[] _charArray = target.ToCharArray();
            // 文字列を判定
            foreach (char _char in _charArray) {
                if (_char.Equals('x')) {
                    _list.Add(true);
                }
                else if (_char.Equals('-')) {
                    _list.Add(null);
                }
            }
            return _list;
        }
    }
}
