
using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// Song クラス
    ///     + Pattern のオブジェクトのリストを管理する
    /// MEMO: フリジアンやロクリアンの調性感というより、上5度転調、下4度転調の概念を取り入れる
    /// </summary>
    public class Song {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _name;

        int _tempo;

        List<Section> _section_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Song(string name, int tempo, List<Section> section_list) {
            _name = name;
            _tempo = tempo;
            _section_list = section_list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// 曲の名前
        /// </summary>
        public string Name {
            get => _name;
        }

        /// <summary>
        /// 曲のテンポ
        /// </summary>
        public int Tempo {
            get => _tempo;
        }

        /// <summary>
        /// 全ての Pattern
        /// </summary>
        public List<Pattern> AllPattern {
            get {
                List<Pattern> new_pattern_list = new();
                _section_list.ForEach(x => x.AllPattern.ForEach(_x => new_pattern_list.Add(_x)));
                return new_pattern_list;
            }
        }

        /// <summary>
        /// 全ての Section
        /// </summary>
        public List<Section> AllSection {
            get => _section_list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// TODO: 使われていない？
        /// </summary>
        Pattern checkeMode(Pattern pattern) {
            pattern.AllMeas.ForEach(x => {
                x.AllSpan.ForEach(_x => _x.KeyMode = Mode.Undefined); // FIXME: 暫定
            });
            return pattern;
        }
    }

    /// <summary>
    /// Section クラス
    ///     + Pattern オブジェクトのリストを管理します
    /// </summary>
    public class Section {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Key _key; // NOTE: ここは単体で良い

        Mode _key_mode; // NOTE: ここは単体で良い

        List<Pattern> _pattern_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Section(Key key, Mode key_mode, List<Pattern> pattern_list) {
            _key = key;
            _key_mode = key_mode;
            _pattern_list = pattern_list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// この Section のキー
        /// </summary>
        public Key Key {
            get => _key; // NOTE: 後から変更できない
        }

        /// <summary>
        /// この Section の旋法
        /// </summary>
        public Mode KeyMode {
            get => _key_mode; // NOTE: 後から変更できない
        }

        /// <summary>
        /// 全ての Pattern
        /// </summary>
        public List<Pattern> AllPattern {
            get => _pattern_list;
        }
    }
}
