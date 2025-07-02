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

using System.IO;
using System.Text;

namespace Meowziq.IO {
    /// <summary>
    /// Provides IO-related extension methods for string operations.
    /// </summary>
    internal static class Extensions {
        /// <summary>
        /// Converts the string to a UTF-8 encoded <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="source">The source string to convert.</param>
        /// <returns>A <see cref="MemoryStream"/> containing the UTF-8 bytes of the string.</returns>
        internal static MemoryStream ToMemoryStream(this string source) {
            return new MemoryStream(buffer: Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// Determines whether the source string is equal to the target string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="target">The string to compare with.</param>
        /// <returns><c>true</c> if the strings are equal; otherwise, <c>false</c>.</returns>
        internal static bool Is(this string source, string target) {
            return source == target;
        }
    }
}
