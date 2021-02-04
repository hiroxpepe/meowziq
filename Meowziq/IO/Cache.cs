
using System.IO;
using System.Text;

namespace Meowziq.IO {
    /// <summary>
    /// キャッシュ クラス
    /// </summary>
    public static class Cache {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Resourse current; // この tick で読み込まれた json データの内容を保持

        static Resourse valid; // 全てのバリデーションを通過した json データの内容を保持

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Cache() {
            current = new Resourse();
            valid = new Resourse();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static Resourse Current {
            get => current;
        }

        public static Resourse Valid {
            get => valid;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// json ファイルを文字列として読み込みます
        /// </summary>
        public static void Load(string targetPath) {
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
            if (File.Exists($"{targetPath}/mixer.json")) {
                using (var _stream = new StreamReader($"{targetPath}/mixer.json")) {
                    current.Mixer = _stream.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// バリデーションが通過した最新の内容として更新します
        /// </summary>
        public static void Update() {
            valid.Pattern = current.Pattern;
            valid.Song = current.Song;
            valid.Phrase = current.Phrase;
            valid.Player = current.Player;
            valid.Mixer = current.Mixer;
        }

        /// <summary>
        /// 内容を初期化します
        /// </summary>
        public static void Clear() {
            current.Clear();
            valid.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// json ファイルの内容保持用 クラス
        /// </summary>
        public class Resourse {

            public Stream PatternStream {
                get => Pattern.ToMemoryStream();
            }

            public Stream SongStream {
                get => Song.ToMemoryStream();
            }

            public Stream PhraseStream {
                get => Phrase.ToMemoryStream();
            }

            public Stream PlayerStream {
                get => Player.ToMemoryStream();
            }

            public Stream MixerStream {
                get => Mixer is null ? null : Mixer.ToMemoryStream();
            }

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

            internal string Mixer {
                get; set;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // internal Methods [verb]

            internal void Clear() {
                Pattern = null;
                Song = null;
                Phrase = null;
                Player = null;
                Mixer = null;
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

        public static bool Is(this string source, string target) { // is でいい？
            return source == target;
        }

        public static void Set(/*ref*/ this string source, string target) {
            source = target; // FIXME: 値渡しなのでこれでは無理 ⇒ C# の仕様で出来ない
        }

        public static void Clear(/*ref*/ this string source) {
            source = ""; // FIXME: 値渡しなのでこれでは無理 ⇒ C# の仕様で出来ない
        }
    }
}
