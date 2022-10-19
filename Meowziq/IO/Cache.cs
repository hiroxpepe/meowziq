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

namespace Meowziq.IO {
    /// <summary>
    /// cache class
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Cache {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Resourse _current_resourse, _valid_resourse;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Cache() {
            _current_resourse = new();
            _valid_resourse = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// gets the contents of the json data read in this tick.
        /// </summary>
        public static Resourse Current {
            get => _current_resourse;
        }

        /// <summary>
        /// gets the final contents of json data that passed all validations.
        /// </summary>
        public static Resourse Valid {
            get => _valid_resourse;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// reads json files as strings.
        /// </summary>
        public static void Load(string targetPath) {
            using StreamReader stream1 = new($"{targetPath}/pattern.json");
            _current_resourse.Pattern = stream1.ReadToEnd();
            using StreamReader stream2 = new($"{targetPath}/song.json");
            _current_resourse.Song = stream2.ReadToEnd();
            using StreamReader stream3 = new($"{targetPath}/phrase.json");
            _current_resourse.Phrase = stream3.ReadToEnd();
            using StreamReader stream4 = new($"{targetPath}/player.json");
            _current_resourse.Player = stream4.ReadToEnd();
            if (File.Exists($"{targetPath}/mixer.json")) {
                using StreamReader stream5 = new($"{targetPath}/mixer.json");
                _current_resourse.Mixer = stream5.ReadToEnd();
            }
        }

        /// <summary>
        /// updates as the latest resourse that has passed validation.
        /// </summary>
        public static void Update() {
            _valid_resourse.Pattern = _current_resourse.Pattern;
            _valid_resourse.Song = _current_resourse.Song;
            _valid_resourse.Phrase = _current_resourse.Phrase;
            _valid_resourse.Player = _current_resourse.Player;
            _valid_resourse.Mixer = _current_resourse.Mixer;
        }

        /// <summary>
        /// initializes the resourses.
        /// </summary>
        public static void Clear() {
            _current_resourse.Clear();
            _valid_resourse.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// class for holding the contents of json files as strings.
        /// </summary>
        public class Resourse {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Properties [noun, adjective] 

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
                Pattern = Song = Phrase = Player = Mixer = null;
            }
        }
    }
}
