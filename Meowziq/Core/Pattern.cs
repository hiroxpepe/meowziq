
using System.Collections.Generic;

namespace Meowziq.Core {
    /// <summary>
    /// パターンを表すクラス
    ///     + 通常数小節単位で構成される
    /// </summary>
    public class Pattern {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        List<Meas> measList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// コンストラクタだけが作成する唯一の方法
        /// </summary>
        public Pattern(List<Meas> measList) {
            if (measList.Count > 12) {
                throw new System.ArgumentException("measure counts are until 12."); // 1パターンは12小節まで
            }
            this.measList = measList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public List<Meas> GetAllMeas() {
            return measList;
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
            int _beatCount = 0;
            foreach (var _span in spanList) {
                _beatCount += _span.Beat; // 拍を集計する
            }
            if (_beatCount < 4 || _beatCount > 4) {
                // FIXME: 3拍子とかは？
                throw new System.ArgumentException("beat counts needs 4."); // 1小節に足りない or 超過している
            }
            this.spanList = spanList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public List<Span> GetAllSpan() {
            return spanList;
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
    }
}
