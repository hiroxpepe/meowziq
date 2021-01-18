
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

        string name;

        List<Section> sectionList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Song(string name, List<Section> sectionList) {
            this.name = name;
            this.sectionList = sectionList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// 曲の名前
        /// </summary>
        public string Name {
            get => name;
        }

        /// <summary>
        /// 全ての Pattern
        /// </summary>
        public List<Pattern> AllPattern {
            get {
                var _newPatternList = new List<Pattern>();
                sectionList.ForEach(x => x.AllPattern.ForEach(_x => _newPatternList.Add(_x)));
                return _newPatternList;
            }
        }

        /// <summary>
        /// 全ての Section
        /// </summary>
        public List<Section> AllSection {
            get => sectionList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// 旋法の設定があるか確認
        /// MEMO: ここで渡しても参照なのでNGだろう
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

        Key key; // NOTE: ここは単体で良い

        Mode keyMode; // NOTE: ここは単体で良い

        List<Pattern> patternList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Section(Key key, Mode keyMode, List<Pattern> patternList) {
            this.key = key;
            this.keyMode = keyMode;
            this.patternList = patternList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// この Section のキー
        /// </summary>
        public Key Key {
            get => key; // NOTE: 後から変更できない
        }

        /// <summary>
        /// この Section の旋法
        /// </summary>
        public Mode KeyMode {
            get => keyMode; // NOTE: 後から変更できない
        }

        /// <summary>
        /// 全ての Pattern
        /// </summary>
        public List<Pattern> AllPattern {
            get => patternList;
        }
    }

    /// <summary>
    /// KeyAndMode クラス
    ///     + 転調用の引数パラメータ
    /// NOTE: 必ず不変オブジェクトにすること
    /// MEMO: mode の名前は常に keyMode と spanMode として用途を区別する
    /// </summary>
    public class KeyAndMode {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int tick; // 開始時点の tick ※絶対値

        Key key;

        Mode keyMode;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public KeyAndMode(int tick, Key key, Mode keyMode) {
            this.tick = tick;
            this.key = key;
            this.keyMode = keyMode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// この転調パラメータが適用される tick ※絶対値
        /// </summary>
        public int Tick {
            get => tick; // NOTE: 後から変更できない
        }

        /// <summary>
        /// キー
        /// </summary>
        public Key Key {
            get => key; // NOTE: 後から変更できない
        }

        /// <summary>
        /// 旋法
        /// </summary>
        public Mode KeyMode {
            get => keyMode; // NOTE: 後から変更できない
        }
    }
}
