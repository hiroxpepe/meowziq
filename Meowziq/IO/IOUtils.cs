﻿/*
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
    /// IO utils functions.
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class IOUtils {
        /// <summary>
        /// creates a directory if necessary.
        /// </summary>
        public static DirectoryInfo MakeDirectoryIfNecessary(string target) {
            string directory = Path.GetDirectoryName(target);
            bool exists = Directory.Exists(directory);
            if (!exists) {
                return Directory.CreateDirectory(directory);
            }
            return null;
        }
    }
}