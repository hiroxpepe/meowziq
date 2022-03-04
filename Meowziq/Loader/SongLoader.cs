
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

        static List<Pattern> _patternList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static List<Pattern> PatternList {
            set => _patternList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Song を作成します
        /// </summary>
        public static Core.Song Build(Stream target) {
            if (_patternList is null) {
                throw new ArgumentException("need patternList.");
            }
            var song = loadJson(target).Song;
            var sectionList = song.Section.Select(x =>
                new Core.Section(
                    Key.Enum.Parse(x.Key),
                    Mode.Enum.Parse(x.Mode),
                    x.PatternArray.Select(_x => searchPattern(_x)).ToList()
            )).ToList();
            return new Core.Song(
                song.Name,
                int.Parse(song.Tempo),
                sectionList
            );
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Pattern searchPattern(string patternName) {
            try {
                return _patternList.Where(x => x.Name.Equals(patternName)).First(); // MEMO: 名前が一致した最初の要素
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
