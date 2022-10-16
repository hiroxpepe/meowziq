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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Value;

namespace Meowziq.Loader {
    /// <summary>
    /// loader class for Phrase object.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class PhraseLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Phrase のリストを作成します
        /// NOTE: Core.Phrase のリストに変換します
        /// NOTE: "base": 指定がある場合 Phrase を継承します
        /// </summary>
        public static List<Core.Phrase> Build(Stream target) {
            List<Core.Phrase> list = loadJson(target).PhraseArray.Select(x => convertPhrase(x)).ToList();
            return list.Select(x => x = !(x.Base is null) ? Inheritor.Apply(x, searchBasePhrase(x.Type, x.Base, list)) : x).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Phrase convertPhrase(Phrase phrase) {
            Core.Phrase new_phrase = new();
            new_phrase.Type = phrase.Type;
            new_phrase.Name = phrase.Name;
            new_phrase.Base = phrase.Base;
            if (phrase.Note != null) { // キーの旋法を自動判定するモード
                new_phrase.Data.Note.Text = phrase.Note;
            } else if (phrase.Auto != null) { // Spanの旋法を自動判定するモード
                new_phrase.Data.Note.Text = phrase.Auto;
                new_phrase.Data.Auto = true;
            }
            new_phrase.Data.Note.Oct = phrase.Oct;
            new_phrase.Data.Chord.Text = phrase.Chord;
            new_phrase.Data.Seque.Text = phrase.Gete;
            new_phrase.Range = phrase.Range;
            new_phrase.Data.Exp.Pre = phrase.Pre;
            new_phrase.Data.Exp.Post = phrase.Post;
            if (phrase.Data != null) { // 複合データがある場合
                new_phrase.Data.BeatArray = phrase.Data.BeatArray;
                new_phrase.Data.NoteArray = phrase.Data.NoteArray;
                new_phrase.Data.AutoArray = phrase.Data.AutoArray;
                new_phrase.Data.OctArray = phrase.Data.OctArray;
                new_phrase.Data.PreArray = phrase.Data.PreArray;
                new_phrase.Data.PostArray = phrase.Data.PostArray;
                if (phrase.Data.InstArray != null) { // ドラム用音名データがある場合
                    new_phrase.Data.PercussionArray = phrase.Data.InstArray.Select(x => Percussion.Enum.Parse(x)).ToArray();
                }
            }
            return new_phrase;
        }

        static Core.Phrase searchBasePhrase(string phrase_type, string phrase_name, List<Core.Phrase> list) {
            try {
                return list.Where(x => x.Type.Equals(phrase_type) && x.Name.Equals(phrase_name)).First();
            } catch {
                throw new ArgumentException("undefined pattern.");
            }
        }

        static Json loadJson(Stream target) {
            DataContractJsonSerializer serializer = new(typeof(Json));
            return (Json) serializer.ReadObject(target);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        [DataContract]
        class Json {
            [DataMember(Name = "phrase")]
            public Phrase[] PhraseArray {
                get; set;
            }
        }

        [DataContract]
        class Phrase {
            [DataMember(Name = "type")]
            public string Type {
                get; set;
            }
            [DataMember(Name = "name")]
            public string Name {
                get; set;
            }
            [DataMember(Name = "base")]
            public string Base {
                get; set;
            }
            [DataMember(Name = "data")]
            public Data Data {
                get; set;
            }
            [DataMember(Name = "note")]
            public string Note {
                get; set;
            }
            [DataMember(Name = "auto")]
            public string Auto {
                get; set;
            }
            [DataMember(Name = "oct")]
            public int Oct {
                get; set;
            }
            [DataMember(Name = "chord")]
            public string Chord {
                get; set;
            }
            [DataMember(Name = "gete")]
            public string Gete {
                get; set;
            }
            [DataMember(Name = "range")]
            public string Range {
                get; set;
            }
            [DataMember(Name = "pre")]
            public string Pre {
                get; set;
            }
            [DataMember(Name = "post")]
            public string Post {
                get; set;
            }
        }

        [DataContract]
        class Data {
            [DataMember(Name = "inst")]
            public string[] InstArray {
                get; set;
            }
            [DataMember(Name = "beat")]
            public string[] BeatArray {
                get; set;
            }
            [DataMember(Name = "note")]
            public string[] NoteArray {
                get; set;
            }
            [DataMember(Name = "auto")]
            public string[] AutoArray {
                get; set;
            }
            [DataMember(Name = "oct")]
            public int[] OctArray {
                get; set;
            }
            [DataMember(Name = "pre")]
            public string[] PreArray {
                get; set;
            }
            [DataMember(Name = "post")]
            public string[] PostArray {
                get; set;
            }
        }
    }
}
