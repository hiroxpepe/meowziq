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
    /// IO related extension methods.
    /// </summary>
    internal static class Extensions {
        /// <summary>
        /// converts a string to a MemoryStream.
        /// </summary>
        internal static MemoryStream ToMemoryStream(this string source) {
            return new MemoryStream(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// compares two strings.
        /// </summary>
        internal static bool Is(this string source, string target) {
            return source == target;
        }
    }
}
