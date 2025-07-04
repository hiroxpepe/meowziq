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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;

namespace Meowziq.Loader {
    /// <summary>
    /// Provides methods for loading and converting Song objects from JSON data.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class SongLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// Holds the list of patterns used for song construction.
        /// </summary>
        static List<Pattern> _pattern_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// Sets the pattern list for song construction.
        /// </summary>
        public static List<Pattern> PatternList { set => _pattern_list = value; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Creates a <see cref="Core.Song"/> object from the specified stream.
        /// </summary>
        /// <param name="target">The stream containing the song JSON data.</param>
        /// <returns>The constructed <see cref="Core.Song"/> object.</returns>
        public static Core.Song Build(Stream target) {
            if (_pattern_list is null) { throw new ArgumentException("need _pattern_list."); }
            Song song = loadJson(target).Song;
            List<Core.Section> section_list = song.Section.Select(selector: x =>
                new Core.Section(
                    key: Key.Enum.Parse(x.Key),
                    key_mode: Mode.Enum.Parse(x.Mode),
                    pattern_list: x.PatternArray.Select(selector: _x => searchPattern(pattern_name: _x)).ToList()
            )).ToList();
            return new Core.Song(
                name: song.Name,
                tempo: int.Parse(song.Tempo),
                section_list: section_list
            );
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// Searches for a <see cref="Pattern"/> object by name in the pattern list.
        /// </summary>
        /// <param name="pattern_name">The name of the pattern to search for.</param>
        /// <returns>The found <see cref="Pattern"/> object.</returns>
        /// <exception cref="ArgumentException">Thrown if the pattern is not found.</exception>
        static Pattern searchPattern(string pattern_name) {
            try {
                return _pattern_list.Where(predicate: x => x.Name.Equals(pattern_name)).First(); // First element with matching name.
            } catch {
                throw new ArgumentException("undefined pattern.");
            }
        }

        /// <summary>
        /// Reads a .json file and deserializes it to a <see cref="Json"/> object.
        /// </summary>
        /// <param name="target">The stream containing the JSON data.</param>
        /// <returns>The deserialized <see cref="Json"/> object.</returns>
        static Json loadJson(Stream target) {
            DataContractJsonSerializer serializer = new(type: typeof(Json));
            return (Json) serializer.ReadObject(stream: target);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Represents the root JSON object for song data.
        /// </summary>
        [DataContract]
        class Json {
            /// <summary>
            /// Gets or sets the song data.
            /// </summary>
            [DataMember(Name = "song")]
            public Song Song { get; set; }
        }

        /// <summary>
        /// Represents a song entry in the JSON data.
        /// </summary>
        [DataContract]
        class Song {
            /// <summary>
            /// Gets or sets the song name.
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the song tempo as a string.
            /// </summary>
            [DataMember(Name = "tempo")]
            public string Tempo { get; set; }

            /// <summary>
            /// Gets or sets the array of song sections.
            /// </summary>
            [DataMember(Name = "section")]
            public Section[] Section { get; set; }
        }

        /// <summary>
        /// Represents a section entry in the song JSON data.
        /// </summary>
        [DataContract]
        class Section {
            /// <summary>
            /// Gets or sets the key for the section.
            /// </summary>
            [DataMember(Name = "key")]
            public string Key { get; set; }

            /// <summary>
            /// Gets or sets the mode for the section.
            /// </summary>
            [DataMember(Name = "mode")]
            public string Mode { get; set; }

            /// <summary>
            /// Gets or sets the array of pattern names for the section.
            /// </summary>
            [DataMember(Name = "pattern")]
            public string[] PatternArray { get; set; }
        }
    }
}
