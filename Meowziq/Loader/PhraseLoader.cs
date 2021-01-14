
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Meowziq.Loader {
    /// <summary>
    /// Phrase のローダークラス
    /// </summary>
    public static class PhraseLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Phrase のリストを作成します
        /// </summary>
        public static List<Core.Phrase> Build(string targetPath) {
            // Core.Phrase のリストに変換
            return loadJson(targetPath).PhraseArray.Select(x => convertPhrase(x)).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Phrase convertPhrase(Phrase phrase) {
            var _phrase = new Core.Phrase();
            _phrase.Type = phrase.Type;
            _phrase.Name = phrase.Name;
            _phrase.Note = validateValue(phrase.Note); // TODO: バリデートの移動
            _phrase.Oct = phrase.Oct;
            _phrase.Chord = phrase.Chord;
            _phrase.Range = phrase.Range;
            _phrase.Pre = phrase.Pre;
            _phrase.Post = phrase.Post;
            if (phrase.Data != null) { // 複合データがある場合
                _phrase.Data.NoteArray = phrase.Data.NoteArray.Select(x => validateValue(x)).ToArray();
                _phrase.Data.OctArray = phrase.Data.OctArray;
                _phrase.Data.PreArray = phrase.Data.PreArray;
                _phrase.Data.PostArray = phrase.Data.PostArray;
                if (phrase.Data.InstArray != null) { // ドラム用音名データがある場合
                    _phrase.Data.PercussionArray = phrase.Data.InstArray.Select(x => Percussion.Enum.Parse(x)).ToArray();
                }
            }
            return _phrase;
        }

        /// <summary>
        /// FIXME: バリデーターは Loader ではなく Value クラスに移動する
        /// TODO: 使用可能な文字
        /// </summary>
        static string validateValue(string target) {
            if (target == null) {
                return target; // 値がなければそのまま返す FIXME:
            }
            // 拍のデータの数が4文字かどうか
            var _target1 = target;
            // 文字の置き換え
            _target1 = _target1.Replace("[", "|").Replace("]", "|");
            // 区切り文字で切り分ける
            var _array1 = _target1.Split('|')
                .Where(x => !string.IsNullOrWhiteSpace(x)) // 空文字以外
                .Where(x => x.Length != 4) // データが4文字ではない
                .ToArray();
            // そのデータがあれば例外を投げる
            if (_array1.Length != 0) {
                throw new FormatException("data count must be 4.");
            }
            // バリデーションOKなら元々の文字列を返す
            return target;
        }

        static Json loadJson(string targetPath) {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) {
                var _serializer = new DataContractJsonSerializer(typeof(Json));
                return (Json) _serializer.ReadObject(_stream);
            }
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
            [DataMember(Name = "data")]
            public Data Data {
                get; set;
            }
            [DataMember(Name = "note")]
            public string Note {
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
            [DataMember(Name = "note")]
            public string[] NoteArray {
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
