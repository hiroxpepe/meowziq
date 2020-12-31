
using System.Collections.Generic;

namespace Meowziq.Core {
    /// <summary>
    /// パターンを表すクラス
    ///     + 通常数小節単位で構成される
    /// </summary>
    public class Pattern {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string name;

        List<Meas> measList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// コンストラクタだけが作成する唯一の方法
        /// </summary>
        public Pattern(string name, List<Meas> measList) {
            if (measList.Count > 16) {
                throw new System.ArgumentException("measure counts are until 16."); // 1パターンは16小節まで
            }
            this.name = name;
            this.measList = measList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        // TODO: 位置を返すカーソルが必要？

        public string Name {
            get => name;
        }

        /// <summary>
        /// パターンの拍数
        /// </summary>
        public int BeatCount {
            // FIXME: 1小節を4拍として計算
            get => measList.Count * 4;
        }

        public List<Meas> AllMeas {
            get => measList;
        }

        public List<Span> AllSpan {
            get {
                List<Span> _spanList = new List<Span>();
                foreach (var _meas in measList) {
                    foreach (var _span in _meas.AllSpan) {
                        _spanList.Add(_span);
                    }
                }
                return _spanList;
            }
        }
    }

    /// <summary>
    /// 小節を表すクラス
    /// </summary>
    public class Meas {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        List<Span> spanList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// コンストラクタだけが作成する唯一の方法
        /// </summary>
        public Meas(List<Span> spanList) {
            int _totalBeatCount = 0;
            foreach (var _span in spanList) {
                _totalBeatCount += _span.Beat; // 拍を集計する
            }
            if (_totalBeatCount < 4 || _totalBeatCount > 4) {
                // FIXME: 3拍子とかは？
                throw new System.ArgumentException("beat counts needs 4."); // 1小節に足りない or 超過している
            }
            // Span を分解して個別する
            this.spanList = new List<Span>();
            foreach (var _span in spanList) {
                var _beatCount = _span.Beat;
                for (var _i = 0; _i < _beatCount; _i++) {
                    this.spanList.Add(new Span(1, _span.Degree, _span.Mode));
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public List<Span> AllSpan {
            get => spanList;
        }
    }

    /// <summary>
    /// 旋法が続く期間を表すクラス
    /// </summary>
    public class Span {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int beat; // 拍数

        Degree degree; // キーに対する度数

        Mode mode; // 旋法

        Mode keyMode; // キー(曲)の旋法

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Span(int beat, Degree degree) {
            this.beat = beat;
            this.degree = degree;
            this.mode = Mode.Undefined;
        }

        public Span(int beat, Degree degree, Mode mode) {
            this.beat = beat;
            this.degree = degree;
            this.mode = mode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public int Beat {
            get => beat;
        }

        public Degree Degree {
            get => degree;
        }

        public Mode Mode {
            get => mode;
            set => mode = value;
        }

        public Mode KeyMode {
            get => keyMode;
            set => keyMode = value;
        }
    }
}
