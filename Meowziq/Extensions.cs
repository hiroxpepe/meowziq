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
using System.Text.RegularExpressions;

namespace Meowziq {
    /// <summary>
    /// common extension methods.
    /// </summary>
    /// <author>
    /// h.adachi (STUDIO MeowToon)
    /// </author>
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
            return !(source is null || source.Equals(string.Empty));
        }
    }
}
