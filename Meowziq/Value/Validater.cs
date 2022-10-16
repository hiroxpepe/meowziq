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
    /// validation class for input values.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Validater {
        /// <summary>
        /// checks the letters of the phrase
        /// </summary>
        public static string PhraseValue(string target) {
            if (target is null) {
                return target; // no value, return as is.
            }
            // whether the number of one beat data is 4 characters.
            string target1 = target;
            target1 = target1.Replace("[", "|").Replace("]", "|"); // replaces the character. 
            string[] array1 = target1.Split('|') // separates by delimiter.
                .Where(predicate: x => !string.IsNullOrWhiteSpace(x)) // non empty characters are targeted.
                .Where(predicate: x => x.Length != 4) // extracts the data is not 4 characters.
                .ToArray();
            if (array1.Length != 0) { // throws an exception if the data exists.
                throw new FormatException("a beat data count must be 4.");
            }
            return target; // returns the original string if validation is ok.
        }
    }

    /// <summary>
    /// utils class for input values.
    /// </summary>
    internal class Utils {
        /// <summary>
        /// removes unnecessary characters "[", "]", "|".
        /// </summary>
        internal static string Filter(string target) {
            return target.Replace("|", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty); // removes unnecessary characters.
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
        /// converts numeric BPM to tempo information for SMF.
        /// </summary>
        /// <summary_jp>
        /// BPM = 120 (1分あたり四分音符が120個) の場合、<br/>
        /// 四分音符の長さは 60 x 10 の6乗 / 120 = 500,000 (μsec) <br/>
        /// これを16進にすると 0x07A120 <br/>
        /// 3バイトで表現すると "07", "A1", "20"
        /// </summary_jp>
        public static byte[] ToByteTempo(int tempo) {
            double double_value = 60 * Math.Pow(10, 6) / tempo;
            string hex = int.Parse(Math.Round(double_value).ToString()).ToString("X6"); // hexadecimal 6-digit conversion.
            char[] char_array = hex.ToCharArray();
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
