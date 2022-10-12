/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.IO;
using System.Text;

namespace Meowziq.IO {
    /// <summary>
    /// キャッシュ クラス
    /// @author h.adachi
    /// </summary>
    public static class Cache {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Resourse _current; // この tick で読み込まれた json データの内容を保持

        static Resourse _valid; // 全てのバリデーションを通過した json データの内容を保持

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Cache() {
            _current = new();
            _valid = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static Resourse Current {
            get => _current;
        }

        public static Resourse Valid {
            get => _valid;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// json ファイルを文字列として読み込みます
        /// </summary>
        public static void Load(string targetPath) {
            using var stream1 = new StreamReader($"{targetPath}/pattern.json");
            _current.Pattern = stream1.ReadToEnd();
            using var stream2 = new StreamReader($"{targetPath}/song.json");
            _current.Song = stream2.ReadToEnd();
            using var stream3 = new StreamReader($"{targetPath}/phrase.json");
            _current.Phrase = stream3.ReadToEnd();
            using var stream4 = new StreamReader($"{targetPath}/player.json");
            _current.Player = stream4.ReadToEnd();
            if (File.Exists($"{targetPath}/mixer.json")) {
                using var stream5 = new StreamReader($"{targetPath}/mixer.json");
                _current.Mixer = stream5.ReadToEnd();
            }
        }

        /// <summary>
        /// バリデーションが通過した最新の内容として更新します
        /// </summary>
        public static void Update() {
            _valid.Pattern = _current.Pattern;
            _valid.Song = _current.Song;
            _valid.Phrase = _current.Phrase;
            _valid.Player = _current.Player;
            _valid.Mixer = _current.Mixer;
        }

        /// <summary>
        /// 内容を初期化します
        /// </summary>
        public static void Clear() {
            _current.Clear();
            _valid.Clear();
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
