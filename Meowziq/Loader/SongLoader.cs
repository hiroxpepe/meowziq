
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
    public class SongLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string targetPath; // song.jsonc への PATH 文字列

        SongData songData;

        List<Core.Pattern> patternList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public List<Core.Pattern> PatternList {
            set => patternList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public SongLoader(string targetPath) {
            this.targetPath = targetPath;
            songData = new SongData();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Song を作成します
        /// </summary>
        public Core.Song BuildSong() {
            loadJson(); // json のデータをオブジェクトにデシリアライズ
            var _song = new Core.Song(
                Utils.ToKey(songData.song.key),
                Utils.ToMode(songData.song.mode)
            );
            foreach (var _patternName in songData.song.pattern) {
                _song.Add(searchPattern(_patternName)); // Pattern 追加
            }
            return _song;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        Core.Pattern searchPattern(string patternName) {
            try {
                return patternList.Where(x => x.Name.Equals(patternName)).First(); // MEMO: 名前が一致した最初の要素
            } catch (System.Exception e) {
                throw new System.ArgumentException("undefined pattern.");
            }
        }

        void loadJson() {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) {
                var _serializer = new DataContractJsonSerializer(typeof(SongData));
                songData = (SongData) _serializer.ReadObject(_stream);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        // MEMO: 編集: JSON をクラスとして張り付ける
        [DataContract]
        class SongData {
            [DataMember]
            public Song song {
                get; set;
            }
        }

        [DataContract]
        class Song {
            [DataMember]
            public string name {
                get; set;
            }
            [DataMember]
            public string tempo {
                get; set;
            }
            [DataMember]
            public string key {
                get; set;
            }
            [DataMember]
            public string mode {
                get; set;
            }
            [DataMember]
            public string[] pattern {
                get; set;
            }
        }
    }
}
