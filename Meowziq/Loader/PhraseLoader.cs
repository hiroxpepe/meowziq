
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
        ///     + キャッシュした文字列
        /// </summary>
        public static List<Core.Phrase> Build(Stream target) {
            var _list = loadJson(target).PhraseArray.Select(x => convertPhrase(x)).ToList();
            return _list.Select(x => x = !(x.Base is null) ? Inheritor.Apply(x, searchBasePhrase(x.Type, x.Base, _list)) : x)
                .ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Phrase convertPhrase(Phrase phrase) {
            var _phrase = new Core.Phrase();
            _phrase.Type = phrase.Type;
            _phrase.Name = phrase.Name;
            _phrase.Base = phrase.Base;
            if (phrase.Note != null) {
                _phrase.Data.Note.Text = phrase.Note;
            } else if (phrase.Auto != null) {
                _phrase.Data.Note.Text = phrase.Auto;
                _phrase.Data.Auto = true;
            }
            _phrase.Data.Note.Oct = phrase.Oct;
            _phrase.Data.Chord.Text = phrase.Chord;
            _phrase.Range = phrase.Range;
            _phrase.Data.Exp.Pre = phrase.Pre;
            _phrase.Data.Exp.Post = phrase.Post;
            if (phrase.Data != null) { // 複合データがある場合
                _phrase.Data.BeatArray = phrase.Data.BeatArray;
                _phrase.Data.NoteArray = phrase.Data.NoteArray;
                _phrase.Data.AutoArray = phrase.Data.AutoArray;
                _phrase.Data.OctArray = phrase.Data.OctArray;
                _phrase.Data.PreArray = phrase.Data.PreArray;
                _phrase.Data.PostArray = phrase.Data.PostArray;
                if (phrase.Data.InstArray != null) { // ドラム用音名データがある場合
                    _phrase.Data.PercussionArray = phrase.Data.InstArray.Select(x => Percussion.Enum.Parse(x)).ToArray();
                }
            }
            return _phrase;
        }

        static Core.Phrase searchBasePhrase(string phraseType, string phraseName, List<Core.Phrase> list) {
            try {
                return list.Where(x => x.Type.Equals(phraseType) && x.Name.Equals(phraseName)).First();
            } catch {
                throw new ArgumentException("undefined pattern.");
            }
        }

        static Json loadJson(Stream target) {
            var _serializer = new DataContractJsonSerializer(typeof(Json));
            return (Json) _serializer.ReadObject(target);
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
