
using System;
using System.Text.RegularExpressions;

namespace Meowziq {
    /// <summary>
    /// 共通拡張メソッド
    /// </summary>
    public static class Extensions {
        /// <summary>
        /// char 文字を数値に変換します
        /// </summary>
        public static int Int32(this char source) {
            if (!Regex.IsMatch(source.ToString(), @"^[0-9]+$")) { // 0～9 のみ有効 
                throw new FormatException("a char value must be 0～9.");
            }
            return int.Parse(source.ToString());
        }

        /// <summary>
        /// 文字列が null または 空文字("")ではない場合 TRUE を返します
        /// </summary>
        public static bool HasValue(this string source) {
            return !(source is null || source.Equals(""));
        }
    }
}
