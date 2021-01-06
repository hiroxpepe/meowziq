
using System.Collections.Generic;

namespace Meowziq.Core {
    /// <summary>
    /// Song クラス
    ///     + Pattern のオブジェクトのリストを管理する
    /// </summary>
    public class Song {

        // 音楽理論を知らない人が曲を作ることを楽しめる仕組みを作る

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Key key; // キー ※ひとまず転調ではなく旋法の切り替えを実装する TODO: 転調キーを変えて再計算

        Mode keyMode; // キー全体の旋法

        string name;

        List<Pattern> patternList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Song(string name, Key key, Mode keyMode) {
            this.name = name;
            this.key = key;
            this.keyMode = keyMode;
            this.patternList = new List<Pattern>();
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
        // public Methods [verb]

        public void Add(Pattern pattern) {
            foreach (var _meas in pattern.AllMeas) {
                foreach (var _span in _meas.AllSpan) {
                    if (_span.Mode == Mode.Undefined) {
                        _span.Mode = keyMode; // 設定がない場合 song の旋法を設定
                    }
                    _span.KeyMode = keyMode; // こちらはもれなく設定 // FIXME: 曲の旋法を変える時
                }
            }
            patternList.Add(pattern);
        }
    }
}
