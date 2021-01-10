
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;

namespace Meowziq.Loader {
    /// <summary>
    /// Song のローダークラス
    /// </summary>
    public static class SongLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static List<Pattern> patternList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjectives] 

        public static List<Pattern> PatternList {
            set => patternList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Song を作成します
        /// </summary>
        public static Core.Song Build(string targetPath) {
            if (patternList == null) {
                throw new ArgumentException("need patternList.");
            }
            var _song = loadJson(targetPath).Song;
            return new Core.Song(
                _song.Name,
                Key.Enum.Parse(_song.Key),
                Mode.Enum.Parse(_song.Mode),
                _song.PatternArray.Select(x => searchPattern(x)).ToList() // 名前で探して Pattern オブジェクトをリスト化
            );
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Pattern searchPattern(string patternName) {
            try {
                return patternList.Where(x => x.Name.Equals(patternName)).First(); // MEMO: 名前が一致した最初の要素
            } catch (Exception e) {
                throw new ArgumentException("undefined pattern.");
            }
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
            [DataMember(Name = "song")]
            public Song Song {
                get; set;
            }
        }

        [DataContract]
        class Song {
            [DataMember(Name = "name")]
            public string Name {
                get; set;
            }
            [DataMember(Name = "tempo")]
            public string Tempo {
                get; set;
            }
            [DataMember(Name = "key")]
            public string Key {
                get; set;
            }
            [DataMember(Name = "mode")]
            public string Mode {
                get; set;
            }
            [DataMember(Name = "pattern")]
            public string[] PatternArray {
                get; set;
            }
        }
    }
}
