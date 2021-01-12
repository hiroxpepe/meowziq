
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// Pattern クラス
    ///     + Meas オブジェクトのリストを管理します
    /// </summary>
    public class Pattern {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string name;

        List<Meas> measList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// NOTE: コンストラクタだけが作成する唯一の方法
        /// </summary>
        public Pattern(string name, List<Meas> measList) {
            if (measList.Count > 16) {
                throw new ArgumentException("measure counts are until 16."); // 1パターンは16小節まで
            }
            this.name = name;
            this.measList = measList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public string Name {
            get => name;
        }

        /// <summary>
        /// Pattern の拍数を返す
        /// </summary>
        public int BeatCount {
            // FIXME: 1小節を4拍として計算
            get => measList.Count * 4;
        }

        /// <summary>
        /// Meas のリストを返す
        /// </summary>
        public List<Meas> AllMeas {
            get => measList;
        }

        /// <summary>
        /// Span のリストを返す
        /// </summary>
        public List<Span> AllSpan {
            get => measList.SelectMany(x => x.AllSpan).Select(x => x).ToList();
        }
    }

    /// <summary>
    /// Meas クラス
    ///     + 小節を表すクラス
    ///     + Span オブジェクトのリストを管理する
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
            var _totalBeatCount = 0;
            spanList.ForEach(x => _totalBeatCount += x.Beat); // 拍を集計する
            if (_totalBeatCount < 4 || _totalBeatCount > 4) {
                // FIXME: 3拍子とかは？
                throw new ArgumentException("beat counts needs 4."); // 1小節に足りない or 超過している
            }
            // Span を分解して個別にする
            this.spanList = new List<Span>();
            spanList.ForEach(x => {
                for (var _idx = 0; _idx < x.Beat; _idx++) {
                    this.spanList.Add(new Span(1, x.Degree, x.Mode));
                }
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        /// <summary>
        /// Span のリストを返す
        /// </summary>
        public List<Span> AllSpan {
            get => spanList;
        }
    }

    /// <summary>
    /// Span クラス
    ///     + 旋法が続く期間を表すクラス
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
            this.keyMode = Mode.Undefined;
        }

        public Span(int beat, Degree degree, Mode mode) {
            this.beat = beat;
            this.degree = degree;
            this.mode = mode;
            this.keyMode = Mode.Undefined;
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
