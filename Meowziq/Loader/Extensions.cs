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

using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Loader {
    /// <summary>
    /// Provides loader-related extension methods for string operations.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    internal static class Extensions {
        /// <summary>
        /// Gets the total beat length represented by the string.
        /// </summary>
        /// <param name="source">The source string representing measures.</param>
        /// <returns>The total number of beats.</returns>
        internal static int GetBeatLength(this string source) {
            const int MEAS_TO_BEAT = 4;
            int length = GetMeasStringArray(source: source).Length;
            return length * MEAS_TO_BEAT;
        }

        /// <summary>
        /// Splits the measure string into an array of measure strings.
        /// </summary>
        /// <param name="source">The source string representing measures.</param>
        /// <returns>An array of measure strings.</returns>
        internal static string[] GetMeasStringArray(this string source) {
            return getMeasEnumerable(target: source).ToArray();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// Splits the measure string into its individual measure components.
        /// </summary>
        /// <param name="target">The target string to split.</param>
        /// <returns>An enumerable collection of measure strings.</returns>
        static IEnumerable<string> getMeasEnumerable(string target) {
            return target.Replace(oldValue: "][", newValue: "@").Split(separator: '@').Select(
                selector: x => x.Replace(oldValue: "[", newValue: string.Empty).Replace(oldValue: "]", newValue: string.Empty)
            );
        }
    }
}
