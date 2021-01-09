
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;

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
            return loadJson(targetPath).Phrase.Select(x => convertPhrase(x)).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Phrase convertPhrase(Phrase phrase) {
            var _phrase = new Core.Phrase();
            _phrase.Type = phrase.Type;
            _phrase.Name = phrase.Name;
            _phrase.NoteText = validateValue(phrase.Note);
            if (phrase.Data != null) { // 複合データがある場合
                _phrase.Data.NoteTextArray = phrase.Data.Note.Select(x => validateValue(x)).ToArray();
                _phrase.Data.NoteOctArray = phrase.Data.Oct;
                if (phrase.Data.Inst != null) { // ドラム用音名データがある場合
                    _phrase.Data.PercussionArray = phrase.Data.Inst.Select(x => Percussion.Enum.Parse(x)).ToArray();
                }
            }
            return _phrase;
        }

        /// <summary>
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
            public Phrase[] Phrase {
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
        }

        [DataContract]
        class Data {
            [DataMember(Name = "inst")]
            public string[] Inst {
                get; set;
            }
            [DataMember(Name = "note")]
            public string[] Note {
                get; set;
            }
            [DataMember(Name = "oct")]
            public int[] Oct {
                get; set;
            }
        }
    }
}
