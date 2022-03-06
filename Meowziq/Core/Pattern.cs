
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

        string _name;

        List<Meas> _measList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// NOTE: コンストラクタだけが作成する唯一の方法
        /// </summary>
        public Pattern(string name, List<Meas> measList) {
            if (measList.Count > Env.MeasMax.Int32()) {
                throw new ArgumentException($"measure counts are until {Env.MeasMax.Int32()}."); // 1パターンは12小節まで
            }
            _name = name;
            _measList = measList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Name {
            get => _name;
        }

        /// <summary>
        /// Pattern の拍数を返す
        /// </summary>
        public int BeatCount {
            // FIXME: 1小節を4拍として計算
            get => _measList.Count * 4;
        }

        /// <summary>
        /// Meas のリストを返す
        /// </summary>
        public List<Meas> AllMeas {
            get => _measList;
        }

        /// <summary>
        /// Span のリストを返す
        /// </summary>
        public List<Span> AllSpan {
            get => _measList.SelectMany(x => x.AllSpan).ToList();
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

        List<Span> _spanList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// コンストラクタだけが作成する唯一の方法
        /// </summary>
        public Meas(List<Span> spanList) {
            var totalBeatCount = 0;
            spanList.ForEach(x => totalBeatCount += x.Beat); // 拍を集計する
            if (totalBeatCount < 4 || totalBeatCount > 4) {
                // FIXME: 3拍子とかは？
                throw new ArgumentException("beat counts needs 4."); // 1小節に足りない or 超過している
            }
            // Span を分解して1拍毎に追加する
            _spanList = new();
            spanList.ForEach(x => {
                Enumerable.Range(0, x.Beat).ToList().ForEach(
                    _x => _spanList.Add(new Span(1, x.Degree, x.SpanMode))
                );
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Span のリストを返す
        /// </summary>
        public List<Span> AllSpan {
            get => _spanList;
        }
    }

    /// <summary>
    /// Span クラス
    ///     + 旋法が続く期間を表すクラス
    /// </summary>
    public class Span {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int _beat; // 拍数

        Key _key; // 曲のキー

        Degree _degree; // キーに対する度数

        Mode _keyMode; // キーの旋法

        Mode _spanMode; // この期間の旋法

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Span(int beat, Degree degree) {
            _beat = beat;
            _key = Key.Undefined;
            _degree = degree;
            _keyMode = Mode.Undefined;
            _spanMode = Mode.Undefined;
        }

        public Span(int beat, Degree degree, Mode spanMode) {
            _beat = beat;
            _key = Key.Undefined;
            _degree = degree;
            _keyMode = Mode.Undefined;
            _spanMode = spanMode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public int Beat {
            get => _beat;
        }

        public Degree Degree {
            get => _degree;
        }

        public Key Key {
            get => _key;
            set => _key = value;
        }

        public Mode KeyMode {
            get => _keyMode;
            set => _keyMode = value;
        }

        public Mode SpanMode {
            get => _spanMode; // 一度設定した spanMode は変更しない
        }
    }
}
