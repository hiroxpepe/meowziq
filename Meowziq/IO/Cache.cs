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
    /// Provides caching for JSON resources used in the application.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Cache {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// Holds the current resource data for this tick.
        /// </summary>
        static Resourse _current_resourse, _valid_resourse;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        /// <summary>
        /// Initializes static members of the <see cref="Cache"/> class.
        /// </summary>
        static Cache() {
            _current_resourse = new();
            _valid_resourse = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// Gets the contents of the JSON data read in this tick.
        /// </summary>
        public static Resourse Current { get => _current_resourse; }

        /// <summary>
        /// Gets the final contents of JSON data that passed all validations.
        /// </summary>
        public static Resourse Valid { get => _valid_resourse; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Reads JSON files as strings and stores them in the current resource.
        /// </summary>
        /// <param name="targetPath">The directory path containing the JSON files.</param>
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
        /// Updates the valid resource with the latest data that has passed validation.
        /// </summary>
        public static void Update() {
            _valid_resourse.Pattern = _current_resourse.Pattern;
            _valid_resourse.Song = _current_resourse.Song;
            _valid_resourse.Phrase = _current_resourse.Phrase;
            _valid_resourse.Player = _current_resourse.Player;
            _valid_resourse.Mixer = _current_resourse.Mixer;
        }

        /// <summary>
        /// Initializes the resources by clearing their contents.
        /// </summary>
        public static void Clear() {
            _current_resourse.Clear();
            _valid_resourse.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Holds the contents of JSON files as strings and provides memory streams for each.
        /// </summary>
        public class Resourse {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Properties [noun, adjective] 

            /// <summary>
            /// Gets a memory stream of the pattern JSON data.
            /// </summary>
            public Stream PatternStream { get => Pattern.ToMemoryStream(); }

            /// <summary>
            /// Gets a memory stream of the song JSON data.
            /// </summary>
            public Stream SongStream { get => Song.ToMemoryStream(); }

            /// <summary>
            /// Gets a memory stream of the phrase JSON data.
            /// </summary>
            public Stream PhraseStream { get => Phrase.ToMemoryStream(); }

            /// <summary>
            /// Gets a memory stream of the player JSON data.
            /// </summary>
            public Stream PlayerStream { get => Player.ToMemoryStream(); }

            /// <summary>
            /// Gets a memory stream of the mixer JSON data, or null if not available.
            /// </summary>
            public Stream MixerStream { get => Mixer is null ? null : Mixer.ToMemoryStream(); }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // internal Properties [noun, adjective] 

            /// <summary>
            /// Gets or sets the pattern JSON data as a string.
            /// </summary>
            internal string Pattern { get; set; }

            /// <summary>
            /// Gets or sets the song JSON data as a string.
            /// </summary>
            internal string Song { get; set; }

            /// <summary>
            /// Gets or sets the phrase JSON data as a string.
            /// </summary>
            internal string Phrase { get; set; }

            /// <summary>
            /// Gets or sets the player JSON data as a string.
            /// </summary>
            internal string Player { get; set; }

            /// <summary>
            /// Gets or sets the mixer JSON data as a string.
            /// </summary>
            internal string Mixer { get; set; }

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // internal Methods [verb]

            /// <summary>
            /// Clears all JSON data fields in this resource.
            /// </summary>
            internal void Clear() {
                Pattern = Song = Phrase = Player = Mixer = null;
            }
        }
    }
}
