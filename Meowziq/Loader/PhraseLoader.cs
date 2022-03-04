﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Value;

namespace Meowziq.Loader {
    /// <summary>
    /// Phrase のローダークラス
    /// </summary>
    public static class PhraseLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Phrase のリストを作成します
        /// NOTE: Core.Phrase のリストに変換します
        /// NOTE: "base": 指定がある場合 Phrase を継承します
        /// </summary>
        public static List<Core.Phrase> Build(Stream target) {
            var list = loadJson(target).PhraseArray.Select(x => convertPhrase(x)).ToList();
            return list.Select(x => x = !(x.Base is null) ? Inheritor.Apply(x, searchBasePhrase(x.Type, x.Base, list)) : x)
                .ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Phrase convertPhrase(Phrase phrase) {
            var newPhrase = new Core.Phrase();
            newPhrase.Type = phrase.Type;
            newPhrase.Name = phrase.Name;
            newPhrase.Base = phrase.Base;
            if (phrase.Note != null) { // キーの旋法を自動判定するモード
                newPhrase.Data.Note.Text = phrase.Note;
            } else if (phrase.Auto != null) { // Spanの旋法を自動判定するモード
                newPhrase.Data.Note.Text = phrase.Auto;
                newPhrase.Data.Auto = true;
            }
            newPhrase.Data.Note.Oct = phrase.Oct;
            newPhrase.Data.Chord.Text = phrase.Chord;
            newPhrase.Data.Seque.Text = phrase.Gete;
            newPhrase.Range = phrase.Range;
            newPhrase.Data.Exp.Pre = phrase.Pre;
            newPhrase.Data.Exp.Post = phrase.Post;
            if (phrase.Data != null) { // 複合データがある場合
                newPhrase.Data.BeatArray = phrase.Data.BeatArray;
                newPhrase.Data.NoteArray = phrase.Data.NoteArray;
                newPhrase.Data.AutoArray = phrase.Data.AutoArray;
                newPhrase.Data.OctArray = phrase.Data.OctArray;
                newPhrase.Data.PreArray = phrase.Data.PreArray;
                newPhrase.Data.PostArray = phrase.Data.PostArray;
                if (phrase.Data.InstArray != null) { // ドラム用音名データがある場合
                    newPhrase.Data.PercussionArray = phrase.Data.InstArray.Select(x => Percussion.Enum.Parse(x)).ToArray();
                }
            }
            return newPhrase;
        }

        static Core.Phrase searchBasePhrase(string phraseType, string phraseName, List<Core.Phrase> list) {
            try {
                return list.Where(x => x.Type.Equals(phraseType) && x.Name.Equals(phraseName)).First();
            } catch {
                throw new ArgumentException("undefined pattern.");
            }
        }

        static Json loadJson(Stream target) {
            var serializer = new DataContractJsonSerializer(typeof(Json));
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
