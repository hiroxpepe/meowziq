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
    /// loader class for Song object.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class SongLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static List<Pattern> _pattern_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static List<Pattern> PatternList { set => _pattern_list = value; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// creates a Core.Song object.
        /// </summary>
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
        /// searches the Pattern object.
        /// </summary>
        static Pattern searchPattern(string pattern_name) {
            try {
                return _pattern_list.Where(predicate: x => x.Name.Equals(pattern_name)).First(); // first element with matching name.
            } catch {
                throw new ArgumentException("undefined pattern.");
            }
        }

        /// <summary>
        /// reads a .json file to the JSON object.
        /// </summary>
        static Json loadJson(Stream target) {
            DataContractJsonSerializer serializer = new(typeof(Json));
            return (Json) serializer.ReadObject(target);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        [DataContract]
        class Json {
            [DataMember(Name = "song")]
            public Song Song { get; set; }
        }

        [DataContract]
        class Song {
            [DataMember(Name = "name")]
            public string Name { get; set; }
            [DataMember(Name = "tempo")]
            public string Tempo { get; set; }
            [DataMember(Name = "section")]
            public Section[] Section { get; set; }
        }

        [DataContract]
        class Section {
            [DataMember(Name = "key")]
            public string Key { get; set; }
            [DataMember(Name = "mode")]
            public string Mode { get; set; }
            [DataMember(Name = "pattern")]
            public string[] PatternArray { get; set; }
        }
    }
}
