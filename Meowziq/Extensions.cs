
using System;
using System.Text.RegularExpressions;

namespace Meowziq {
    /// <summary>
    /// common extension methods.
    /// </summary>
    public static class Extensions {
        /// <summary>
        /// converts a character to a number.
        /// </summary>
        public static int Int32(this char source) {
            if (!Regex.IsMatch(source.ToString(), @"^[0-9]+$")) { // only 0 to 9 are valid.
                throw new FormatException("a char value must be 0～9.");
            }
            return int.Parse(source.ToString());
        }

        /// <summary>
        /// returns true if the string is not a null or empty string.
        /// </summary>
        public static bool HasValue(this string source) {
            return !(source is null || source.Equals(""));
        }
    }
}
