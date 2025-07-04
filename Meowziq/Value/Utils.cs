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

namespace Meowziq.Value {
    /// <summary>
    /// Provides utility methods for processing input values.
    /// </summary>
    /// <remarks>
    /// <item>All methods are static and stateless.</item>
    /// <item>Used for string preprocessing in value handling.</item>
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    internal static class Utils {
        /// <summary>
        /// Removes unnecessary characters '[', ']', and '|'.
        /// </summary>
        /// <param name="target">Target string to be filtered.</param>
        /// <returns>Filtered string with specified characters removed.</returns>
        /// <remarks>
        /// <item>Used for cleaning up input strings before further processing.</item>
        /// </remarks>
        internal static string Filter(string target) {
            return target.Replace(oldValue: "|", newValue: string.Empty).Replace(oldValue: "[", newValue: string.Empty).Replace(oldValue: "]", newValue: string.Empty); // removes unnecessary characters.
        }
    }
}
