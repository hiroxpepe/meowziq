
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
        // static Properties [noun, adjective] 

        public static List<Pattern> PatternList {
            set => patternList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Song を作成します
        /// </summary>
        public static Core.Song Build(Stream target) {
            if (patternList is null) {
                throw new ArgumentException("need patternList.");
            }
            var _song = loadJson(target).Song;
            var _sectionList = _song.Section.Select(x =>
                new Core.Section(
                    Key.Enum.Parse(x.Key),
                    Mode.Enum.Parse(x.Mode),
                    x.PatternArray.Select(_x => searchPattern(_x)).ToList()
            )).ToList();
            return new Core.Song(
                _song.Name,
                int.Parse(_song.Tempo),
                _sectionList
            );
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Pattern searchPattern(string patternName) {
            try {
                return patternList.Where(x => x.Name.Equals(patternName)).First(); // MEMO: 名前が一致した最初の要素
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
            [DataMember(Name = "section")]
            public Section[] Section {
                get; set;
            }
        }

        [DataContract]
        class Section {
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
