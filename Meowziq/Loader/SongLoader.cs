
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
        public static Core.Song BuildSong(string targetPath) {
            var _songData = loadJson(targetPath);
            var _song = new Core.Song(
                _songData.Song.Name,
                Key.Extension.Parse(_songData.Song.Key), //Utils.ToKey(_songData.Song.Key),
                Utils.ToMode(_songData.Song.Mode)
            );
            foreach (var _patternName in _songData.Song.Pattern) {
                _song.Add(searchPattern(_patternName)); // Pattern 追加
            }
            return _song;
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

        static SongData loadJson(string targetPath) {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) {
                var _serializer = new DataContractJsonSerializer(typeof(SongData));
                return (SongData) _serializer.ReadObject(_stream);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        // MEMO: 編集: JSON をクラスとして張り付ける
        [DataContract]
        class SongData {
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
            public string[] Pattern {
                get; set;
            }
        }
    }
}
