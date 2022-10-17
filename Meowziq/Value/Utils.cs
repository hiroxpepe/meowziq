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
    /// utils class for input values.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    internal static class Utils {
        /// <summary>
        /// removes unnecessary characters "[", "]", "|".
        /// </summary>
        internal static string Filter(string target) {
            return target.Replace("|", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty); // removes unnecessary characters.
        }
    }
}
