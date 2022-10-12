/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// Pattern クラス
    ///     + Meas オブジェクトのリストを管理します
    /// @author h.adachi
    /// </summary>
    public class Pattern {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _name;

        List<Meas> _meas_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// NOTE: コンストラクタだけが作成する唯一の方法
        /// </summary>
        public Pattern(string name, List<Meas> meas_list) {
            if (meas_list.Count > Env.MeasMax.Int32()) {
                throw new ArgumentException($"measure counts are until {Env.MeasMax.Int32()}."); // 1パターンは12小節まで
            }
            _name = name;
            _meas_list = meas_list;
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
            get => _meas_list.Count * 4;
        }

        /// <summary>
        /// Meas のリストを返す
        /// </summary>
        public List<Meas> AllMeas {
            get => _meas_list;
        }

        /// <summary>
        /// Span のリストを返す
        /// </summary>
        public List<Span> AllSpan {
            get => _meas_list.SelectMany(x => x.AllSpan).ToList();
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

        List<Span> _span_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// コンストラクタだけが作成する唯一の方法
        /// </summary>
        public Meas(List<Span> spanList) {
            var total_beat_count = 0;
            spanList.ForEach(x => total_beat_count += x.Beat); // 拍を集計する
            if (total_beat_count < 4 || total_beat_count > 4) {
                // FIXME: 3拍子とかは？
                throw new ArgumentException("beat counts needs 4."); // 1小節に足りない or 超過している
            }
            // Span を分解して1拍毎に追加する
            _span_list = new();
            spanList.ForEach(x => {
                Enumerable.Range(0, x.Beat).ToList().ForEach(
                    _x => _span_list.Add(new Span(1, x.Degree, x.SpanMode))
                );
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Span のリストを返す
        /// </summary>
        public List<Span> AllSpan {
            get => _span_list;
        }
    }

    /// <summary>
    /// Span クラス
    ///     + 調性が続く期間(拍)を表すクラス
    /// </summary>
    /// <remarks>
    /// 1～7の数値をどのキースケールの音に変換するのか<br/>
    /// </remarks>
    public class Span {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int _beat; // 拍数

        Key _key; // 曲のキー

        Degree _degree; // キーに対する度数

        Mode _key_mode; // キーの旋法

        Mode _span_mode; // この期間の旋法

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Span(int beat, Degree degree) {
            _beat = beat;
            _key = Key.Undefined;
            _degree = degree;
            _key_mode = Mode.Undefined;
            _span_mode = Mode.Undefined;
        }

        public Span(int beat, Degree degree, Mode span_mode) {
            _beat = beat;
            _key = Key.Undefined;
            _degree = degree;
            _key_mode = Mode.Undefined;
            _span_mode = span_mode;
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
            get => _key_mode;
            set => _key_mode = value;
        }

        public Mode SpanMode {
            get => _span_mode; // 一度設定した spanMode は変更しない
        }
    }
}
