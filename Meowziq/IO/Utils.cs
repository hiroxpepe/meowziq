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

namespace Meowziq.IO {
    /// <summary>
    /// Provides IO utility functions.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    internal static class Utils {
        /// <summary>
        /// Creates a directory for the specified path if it does not already exist.
        /// </summary>
        /// <param name="target">The file or directory path to check.</param>
        /// <returns>The created <see cref="DirectoryInfo"/> if a new directory was created; otherwise, <c>null</c>.</returns>
        internal static DirectoryInfo MakeDirectoryIfNecessary(string target) {
            string directory = Path.GetDirectoryName(target);
            bool exists = Directory.Exists(path: directory);
            if (!exists) {
                return Directory.CreateDirectory(path: directory);
            }
            return null;
        }
    }
}
