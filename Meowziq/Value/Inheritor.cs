
using System;
using System.Linq;

namespace Meowziq.Value {
    /// <summary>
    /// Phrase のデータを継承する為のクラス
    /// </summary>
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
            target.Data.Chord.Range = target.Data.Chord.Range; // baze 継承
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
            if (target is null || target.Equals("")) { // target が空文字 or null なら baze を返す
                return baze;
            }
            if (target.Count() != baze.Count()) { // target と baze のデータは同じ数
                throw new FormatException("inherited data count must be the same as the base.");
            }
            var _result = target.Select((x, idx) => x.Equals('*') ? baze.ToArray()[idx] : x).ToArray();
            return new string(_result);
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
            var _result = target.Select((x, idx) => applyString(x, baze[idx])).ToArray();
            return _result;
        }
    }
}
