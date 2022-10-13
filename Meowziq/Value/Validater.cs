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
using System.Linq;

namespace Meowziq.Value {
    /// <summary>
    /// 入力値バリデーション クラス
    /// </summary>
    /// <author>
    /// h.adachi (STUDIO MeowToon)
    /// </author>
    public static class Validater {
        /// <summary>
        /// TODO: 使用可能な文字の判定
        /// </summary>
        public static string PhraseValue(string target) {
            if (target is null) {
                return target; // 値がなければそのまま返す FIXME:
            }
            // 拍のデータの数が4文字かどうか
            var target1 = target;
            target1 = target1.Replace("[", "|").Replace("]", "|"); // 文字の置き換え
            var array1 = target1.Split('|') // 区切り文字で切り分ける
                .Where(x => !string.IsNullOrWhiteSpace(x)) // 空文字以外が対象で
                .Where(x => x.Length != 4) // そのデータが4文字ではないものを抽出
                .ToArray();
            if (array1.Length != 0) { // そのデータがあれば例外を投げる
                throw new FormatException("a beat data count must be 4.");
            }
            return target; // バリデーションOKなら元々の文字列を返す
        }
    }

    /// <summary>
    /// NOTE: 入力値用の Utils クラス
    /// </summary>
    internal class Utils {
        /// <summary>
        /// 不要文字 "[", "]", "|", を削除します
        /// </summary>
        internal static string Filter(string target) {
            return target.Replace("|", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty); // 不要文字削除
        }
    }

    /// <summary>
    /// conversion class.
    /// </summary>
    /// <todo>
    /// are extension methods enough?
    /// </todo>
    public static class Converter {
        /// <summary>
        /// converts numeric BPM to tempo information for smf.
        /// </summary>
        /// <summary_jp>
        /// BPM = 120 (1分あたり四分音符が120個) の場合、<br/>
        /// 四分音符の長さは 60 x 10の6乗 / 120 = 500,000 (μsec) <br/>
        /// これを16進にすると 0x07A120 <br/>
        /// 3バイトで表現すると "07", "A1", "20"
        /// </summary_jp>
        public static byte[] ToByteTempo(int tempo) {
            var double_value = 60 * Math.Pow(10, 6) / tempo;
            var hex = int.Parse(Math.Round(double_value).ToString()).ToString("X6"); // hexadecimal 6-digit conversion.
            var char_array = hex.ToCharArray();
            return new byte[3]{ // returns in 3 bytes.
                Convert.ToByte(char_array[0].ToString() + char_array[1].ToString(), 16),
                Convert.ToByte(char_array[2].ToString() + char_array[3].ToString(), 16),
                Convert.ToByte(char_array[4].ToString() + char_array[5].ToString(), 16)
            };
        }

        /// <summary>
        /// converts string to byte array.
        /// </summary>
        public static byte[] ToByteArray(string target) {
            return target.ToCharArray().Select(x => Convert.ToByte(x)).ToArray();
        }
    }
}
