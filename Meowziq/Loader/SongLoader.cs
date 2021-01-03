
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

        string targetPath; // song.json への PATH 文字列

        SongData songData;

        List<Core.Pattern> patternList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public SongLoader(string targetPath) {
            this.targetPath = targetPath;
            this.songData = new SongData();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public List<Core.Pattern> PatternList {
            set => patternList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Song を作成します
        /// </summary>
        public Core.Song BuildSong() {
            loadJson(); // json のデータをオブジェクトにデシリアライズ
            var _song = new Core.Song(
                Utils.ToKey(songData.Song.Key),
                Utils.ToMode(songData.Song.Mode)
            );
            _song.Name = songData.Song.Name; // FIXME: コンストラクタに含める
            foreach (var _patternName in songData.Song.Pattern) {
                _song.Add(searchPattern(_patternName)); // Pattern 追加
            }
            return _song;
        }

        /// <summary>
        /// Song の名前だけを返します
        /// </summary>
        public string GetSongName() {
            loadJson(); // json のデータをオブジェクトにデシリアライズ
            return songData.Song.Name;
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
