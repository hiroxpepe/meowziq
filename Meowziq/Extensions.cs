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
using System.Text.RegularExpressions;

namespace Meowziq {
    /// <summary>
    /// Provides common extension methods for character and string operations.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Extensions {
        /// <summary>
        /// Converts a numeric character to its integer value.
        /// </summary>
        /// <param name="source">The character to convert (must be '0'-'9').</param>
        /// <returns>The integer value of the character.</returns>
        /// <exception cref="FormatException">Thrown if the character is not a digit.</exception>
        public static int Int32(this char source) {
            if (!Regex.IsMatch(input: source.ToString(), pattern: @"^[0-9]+$")) { // Only 0 to 9 are valid.
                throw new FormatException("a char value must be 0～9.");
            }
            return int.Parse(s: source.ToString());
        }

        /// <summary>
        /// Determines whether the string is not null or empty.
        /// </summary>
        /// <param name="source">The string to check.</param>
        /// <returns><c>true</c> if the string is not null or empty; otherwise, <c>false</c>.</returns>
        public static bool HasValue(this string source) {
            return !(source is null || source.Equals(string.Empty));
        }
    }
}
