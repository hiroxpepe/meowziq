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
    /// Provides static methods to inherit and merge Phrase data by replacing wildcard entries with base data.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>All methods are static and thread-safe.</description></item>
    /// <item><description>Used for phrase inheritance and data merging in Meowziq.</description></item>
    /// </list>
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Inheritor {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Replaces wildcard ('*') entries in the target Phrase data with values from the base Phrase data.
        /// </summary>
        /// <param name="target">Target Phrase object to be modified.</param>
        /// <param name="baze">Base Phrase object to inherit values from.</param>
        /// <returns>The modified target Phrase object with inherited values.</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>Modifies the target object in place.</description></item>
        /// <item><description>Wildcard replacement is performed for Note, Chord, Seque, Exp, and array fields.</description></item>
        /// </list>
        /// </remarks>
        /// <todo>
        /// Implement inheritance for the remaining Seque fields if needed.
        /// </todo>
        /// <author>h.adachi (STUDIO MeowToon)</author>
        public static Core.Phrase Apply(Core.Phrase target, Core.Phrase baze) {
            target.Data.Note.Text = applyString(target.Data.Note.Text, baze.Data.Note.Text);
            target.Data.Note.Oct = baze.Data.Note.Oct; // inherits the base.
            target.Data.Auto = baze.Data.Auto; // inherits the base.
            target.Data.Chord.Text = applyString(target.Data.Chord.Text, baze.Data.Chord.Text);
            target.Data.Chord.Range = baze.Data.Chord.Range; // inherits the base.
            // TODO: rest of the "Seque".
            target.Data.Seque.Text = applyString(target.Data.Seque.Text, baze.Data.Seque.Text);
            target.Data.Seque.Range = baze.Data.Seque.Range; // inherits the base.
            target.Data.Exp.Pre = applyString(target.Data.Exp.Pre, baze.Data.Exp.Pre);
            target.Data.Exp.Post = applyString(target.Data.Exp.Post, baze.Data.Exp.Post);
            target.Data.BeatArray = applyArray(target.Data.BeatArray, baze.Data.BeatArray);
            target.Data.NoteArray = applyArray(target.Data.NoteArray, baze.Data.NoteArray);
            target.Data.AutoArray = applyArray(target.Data.AutoArray, baze.Data.AutoArray);
            if (baze.Data.PercussionArray != null) {
                target.Data.PercussionArray = baze.Data.PercussionArray; // inherits the base.
            }
            if (target.Data.HasMulti) {
                target.Data.OctArray = baze.Data.OctArray; // inherits the base.
                target.Data.PreArray = applyArray(target.Data.PreArray, baze.Data.PreArray);
                target.Data.PostArray = applyArray(target.Data.PostArray, baze.Data.PostArray);
            }
            return target;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// Replaces all wildcard ('*') characters in the target string with corresponding characters from the base string.
        /// </summary>
        /// <param name="target">Target string to be processed.</param>
        /// <param name="baze">Base string to inherit values from.</param>
        /// <returns>Processed string with wildcards replaced by base values.</returns>
        /// <remarks>
        /// <item>Throws if the string lengths do not match.</item>
        /// </remarks>
        /// <exception cref="FormatException">Thrown if the target and base strings have different lengths.</exception>
        static string applyString(string target, string baze) {
            if (target is null || target.Equals(string.Empty)) { // Returns baze if the target is an empty string or null.
                return baze;
            }
            if (target.Count() != baze.Count()) { // Target and baze data should be the same number.
                throw new FormatException("inherited data count must be the same as the base.");
            }
            char[] result = target.Select(selector: (x, index) => x.Equals('*') ? baze.ToArray()[index] : x).ToArray();
            return new string(result);
        }

        /// <summary>
        /// Replaces all wildcard ('*') entries in the target string array with corresponding entries from the base string array.
        /// </summary>
        /// <param name="target">Target string array to be processed.</param>
        /// <param name="baze">Base string array to inherit values from.</param>
        /// <returns>Processed string array with wildcards replaced by base values.</returns>
        /// <remarks>
        /// <item>Throws if the array lengths do not match.</item>
        /// </remarks>
        /// <exception cref="FormatException">Thrown if the target and base arrays have different lengths.</exception>
        static string[] applyArray(string[] target, string[] baze) {
            if (target is null) { // returns baze if the target is null.
                return baze;
            }
            if (target.Count() != baze.Count()) { // Target and baze data should be the same number.
                throw new FormatException("inherited arrray count must be the same as the base.");
            }
            string[] result = target.Select(selector: (x, index) => applyString(target: x, baze: baze[index])).ToArray();
            return result;
        }
    }
}
