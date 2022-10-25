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

namespace Meowziq.Value {
    /// <summary>
    /// validation class for input values.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Validater {
        /// <summary>
        /// checks the letters of the phrase
        /// </summary>
        public static string PhraseValue(string target) {
            if (target is null) {
                return target; // no value, return as is.
            }
            // whether the number of one beat data is 4 characters.
            string target1 = target;
            target1 = target1.Replace("[", "|").Replace("]", "|"); // replaces the character. 
            string[] array1 = target1.Split('|') // separates by delimiter.
                .Where(predicate: x => !string.IsNullOrWhiteSpace(value: x)) // non empty characters are targeted.
                .Where(predicate: x => x.Length != 4) // extracts the data is not 4 characters.
                .ToArray();
            if (array1.Length is not 0) { // throws an exception if the data exists.
                throw new FormatException("a beat data count must be 4.");
            }
            return target; // returns the original string if validation is ok.
        }
    }
}
