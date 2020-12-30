
using System.Collections.Generic;

namespace Meowziq.Core {
    /// <summary>
    /// ソングとはキーとそのタイミングでの旋法を表現する
    ///     + バンドのコード譜のような概念
    /// メロディも知見である
    ///     + 多彩なフレーズを駆使するプレイヤーを育てていく仕組み
    /// </summary>
    public class Song {

        // 音楽理論を知らない人が曲を作ることを楽しめる仕組みを作る

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Key key; // キー ※ひとまず転調ではなく旋法の切り替えを実装する TODO: 転調キーを変えて再計算

        Mode mode; // キー全体の旋法

        List<Span> spanList; // このキーでの度数がどれくらいの長さどの旋法で演奏されるか

        List<Pattern> patternList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Song(Key key, Mode mode) {
            this.spanList = new List<Span>(); // TODO: 順番付き？
            this.patternList = new List<Pattern>();
            this.key = key;
            this.mode = mode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public Key Key {
            get => key;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void Add(Pattern pattern) {
            patternList.Add(pattern);
        }

        public void Add(Span span) {
            if (span.Mode == Mode.Undefined) {
                span.Mode = mode; // song の旋法を設定
            }
            spanList.Add(span);
        }

        public List<Span> GetAllSpan() {
            List<Span> _newSpanList = new List<Span>();
            foreach (var _pattern in patternList) {
                var _measList = _pattern.GetAllMeas();
                foreach (var _meas in _measList) {
                    var _spanList = _meas.GetAllSpan();
                    foreach (var _span in _spanList) {
                        if (_span.Mode == Mode.Undefined) {
                            _span.Mode = mode; // song の旋法を設定
                        }
                        _newSpanList.Add(_span);
                    }
                }
            }
            return _newSpanList;
        }

        /// <summary>
        /// TBA
        /// </summary>
        public Song Repeat(int count) {
            return this;
        }
    }
}
