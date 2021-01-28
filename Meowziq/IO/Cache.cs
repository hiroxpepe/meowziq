﻿
using System.IO;
using System.Text;

namespace Meowziq.IO {
    /// <summary>
    /// キャッシュ クラス
    /// </summary>
    public class Cache {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        DataSet current; // この tick で読み込まれた json データの内容を保持

        DataSet valid; // 全てのバリデーションを通過した json データの内容を保持

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Cache() {
            this.current = new DataSet();
            this.valid = new DataSet();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public Stream CurrentPattern {
            get => current.Pattern.ToMemoryStream();
        }

        public Stream CurrentSong {
            get => current.Song.ToMemoryStream();
        }

        public Stream CurrentPhrase {
            get => current.Phrase.ToMemoryStream();
        }

        public Stream CurrentPlayer {
            get => current.Player.ToMemoryStream();
        }

        public Stream ValidPattern {
            get => valid.Pattern.ToMemoryStream();
        }

        public Stream ValidSong {
            get => valid.Song.ToMemoryStream();
        }

        public Stream ValidPhrase {
            get => valid.Phrase.ToMemoryStream();
        }

        public Stream ValidPlayer {
            get => valid.Player.ToMemoryStream();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// json ファイルを文字列として読み込みます
        /// </summary>
        public void Load(string targetPath) {
            using (var _stream = new StreamReader($"{targetPath}/pattern.json")) {
                current.Pattern = _stream.ReadToEnd();
            }
            using (var _stream = new StreamReader($"{targetPath}/song.json")) {
                current.Song = _stream.ReadToEnd();
            }
            using (var _stream = new StreamReader($"{targetPath}/phrase.json")) {
                current.Phrase = _stream.ReadToEnd();
            }
            using (var _stream = new StreamReader($"{targetPath}/player.json")) {
                current.Player = _stream.ReadToEnd();
            }
        }

        /// <summary>
        /// バリデーションが通過した最新の内容として更新します
        /// </summary>
        public void Update() {
            valid.Pattern = current.Pattern;
            valid.Song = current.Song;
            valid.Phrase = current.Phrase;
            valid.Player = current.Player;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// json ファイルの内容保持用 クラス
        /// </summary>
        class DataSet {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // internal Properties [noun, adjective] 

            internal string Pattern {
                get; set;
            }

            internal string Song {
                get; set;
            }

            internal string Phrase {
                get; set;
            }

            internal string Player {
                get; set;
            }
        }
    }

    /// <summary>
    /// IO系拡張メソッド
    /// </summary>
    public static class Extensions {

        public static MemoryStream ToMemoryStream(this string source) {
            return new MemoryStream(Encoding.UTF8.GetBytes(source));
        }

        public static bool Is(this string source, string target) {
            return source == target;
        }

        public static void Set(this string source, string target) {
            source = target;
        }

        public static void Clear(this string source) {
            source = "";
        }
    }
}