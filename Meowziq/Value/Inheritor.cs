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
    /// Phrase のデータを継承する為のクラス
    /// </summary>
    /// <author>
    /// h.adachi (STUDIO MeowToon)
    /// </author>
    public static class Inheritor {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// target のデータが '*' の箇所を baze のデータで置き換えます
        /// </summary>
        public static Core.Phrase Apply(Core.Phrase target, Core.Phrase baze) {
            target.Data.Note.Text = applyString(target.Data.Note.Text, baze.Data.Note.Text);
            target.Data.Note.Oct = baze.Data.Note.Oct; // baze 継承
            target.Data.Auto = baze.Data.Auto; // baze 継承
            target.Data.Chord.Text = applyString(target.Data.Chord.Text, baze.Data.Chord.Text);
            target.Data.Chord.Range = baze.Data.Chord.Range; // baze 継承
            // TODO: Seque 記述のこり
            target.Data.Seque.Text = applyString(target.Data.Seque.Text, baze.Data.Seque.Text);
            target.Data.Seque.Range = baze.Data.Seque.Range; // baze 継承
            target.Data.Exp.Pre = applyString(target.Data.Exp.Pre, baze.Data.Exp.Pre);
            target.Data.Exp.Post = applyString(target.Data.Exp.Post, baze.Data.Exp.Post);
            target.Data.BeatArray = applyArray(target.Data.BeatArray, baze.Data.BeatArray);
            target.Data.NoteArray = applyArray(target.Data.NoteArray, baze.Data.NoteArray);
            target.Data.AutoArray = applyArray(target.Data.AutoArray, baze.Data.AutoArray);
            if (baze.Data.PercussionArray != null) {
                target.Data.PercussionArray = baze.Data.PercussionArray; // baze 継承
            }
            if (target.Data.HasMulti) {
                target.Data.OctArray = baze.Data.OctArray; // baze 継承
                target.Data.PreArray = applyArray(target.Data.PreArray, baze.Data.PreArray);
                target.Data.PostArray = applyArray(target.Data.PostArray, baze.Data.PostArray);
            }
            return target;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// target のデータが '*' なら baze のデータと置き換えます
        /// </summary>
        static string applyString(string target, string baze) {
            if (target is null || target.Equals(string.Empty)) { // target が空文字 or null なら baze を返す
                return baze;
            }
            if (target.Count() != baze.Count()) { // target と baze のデータは同じ数
                throw new FormatException("inherited data count must be the same as the base.");
            }
            var result = target.Select((x, idx) => x.Equals('*') ? baze.ToArray()[idx] : x).ToArray();
            return new string(result);
        }

        /// <summary>
        /// target の string 配列の中のデータが '*' なら、baze の string 配列のデータと置き換えます
        /// </summary>
        static string[] applyArray(string[] target, string[] baze) {
            if (target is null) { // target が null なら baze を返す
                return baze;
            }
            if (target.Count() != baze.Count()) { // target と baze のデータは同じ数
                throw new FormatException("inherited arrray count must be the same as the base.");
            }
            var result = target.Select((x, idx) => applyString(x, baze[idx])).ToArray();
            return result;
        }
    }
}
