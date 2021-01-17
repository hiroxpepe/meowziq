
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

        Key key; // キー ※ひとまず転調ではなく旋法の切り替えを実装する TODO: 転調キーを変えて再計算

        Mode keyMode; // キー全体の旋法

        string name;

        List<Pattern> patternList; // TODO: patternList が key と mode を持つ構造に改造する

        // TODO: List<Pattern> を保持するクラス ⇒ Section オブジェクトのリスト

        List<Section> sectionList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Song(string name, Key key, Mode keyMode, List<Pattern> patternList) {
            this.name = name;
            this.key = key;
            this.keyMode = keyMode;
            this.patternList = patternList.Select(x => checkeMode(x)).ToList();
        }

        // TODO: 新しいコンストラクタ
        public Song(string name, List<Section> sectionList) {
            this.name = name;
            this.sectionList = sectionList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        /// <summary>
        /// 曲の名前
        /// </summary>
        public string Name {
            get => name;
        }

        /// <summary>
        /// 曲のキー
        /// </summary>
        public Key Key {
            get => key;
        }

        /// <summary>
        /// 全ての Pattern
        /// </summary>
        public List<Pattern> AllPattern {
            get => patternList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// 旋法の設定があるか確認
        /// </summary>
        Pattern checkeMode(Pattern pattern) {
            pattern.AllMeas.ForEach(x => {
                x.AllSpan.ForEach(_x => _x.KeyMode = keyMode); // FIXME: 曲の旋法を変える時は？
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

        Key key;

        Mode keyMode;

        List<string> patternNameList;

        List<Pattern> patternList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Section(Key key, Mode keyMode, List<string> patternNameList) {
            this.key = key;
            this.keyMode = keyMode;
            this.patternNameList = patternNameList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        /// <summary>
        /// 曲のキー
        /// </summary>
        public Key Key {
            get => key;
        }

        /// <summary>
        /// 曲の旋法
        /// </summary>
        public Mode KeyMode {
            get => keyMode;
        }

        /// <summary>
        /// 全ての Pattern
        /// </summary>
        public List<Pattern> AllPattern {
            get => patternList;
        }
    }
}
