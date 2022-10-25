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
    /// class for inheriting Phrase data.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Inheritor {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// replaces '*' in the target data with the baze data.
        /// </summary>
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
        /// if the target data is '*', replace it with the baze data.
        /// </summary>
        static string applyString(string target, string baze) {
            if (target is null || target.Equals(string.Empty)) { // returns baze if the target is an empty string or null.
                return baze;
            }
            if (target.Count() != baze.Count()) { // target and baze data should be the same number.
                throw new FormatException("inherited data count must be the same as the base.");
            }
            char[] result = target.Select(selector: (x, index) => x.Equals('*') ? baze.ToArray()[index] : x).ToArray();
            return new string(result);
        }

        /// <summary>
        /// if the target data in the string array is '*', replace it with the baze data in the string array.
        /// </summary>
        static string[] applyArray(string[] target, string[] baze) {
            if (target is null) { // returns baze if the target is null.
                return baze;
            }
            if (target.Count() != baze.Count()) { // target and baze data should be the same number.
                throw new FormatException("inherited arrray count must be the same as the base.");
            }
            string[] result = target.Select(selector: (x, index) => applyString(target: x, baze: baze[index])).ToArray();
            return result;
        }
    }
}
