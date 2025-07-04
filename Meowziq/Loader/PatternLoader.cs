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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;
using static Meowziq.Env;

namespace Meowziq.Loader {
    /// <summary>
    /// Provides methods for loading and converting Pattern objects from JSON data.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class PatternLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Creates a list of <see cref="Core.Pattern"/> objects from the specified stream.
        /// </summary>
        /// <param name="target">The stream containing the pattern JSON data.</param>
        /// <returns>A list of <see cref="Core.Pattern"/> objects.</returns>
        public static List<Core.Pattern> Build(Stream target) {
            return loadJson(target).PatternArray.Select(selector: x => convertPattern(pattern: x)).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// Converts a <see cref="Pattern"/> object to a <see cref="Core.Pattern"/> object.
        /// </summary>
        /// <param name="pattern">The pattern to convert.</param>
        /// <returns>The converted <see cref="Core.Pattern"/> object.</returns>
        static Core.Pattern convertPattern(Pattern pattern) {
            getCountBeatLength(pattern);
            return new Core.Pattern(name: pattern.Name, meas_list: convertMeasList(pattern_string: pattern.Data));
        }

        /// <summary>
        /// Converts a pattern string to a list of <see cref="Meas"/> objects.
        /// </summary>
        /// <param name="pattern_string">The pattern string to convert.</param>
        /// <returns>A list of <see cref="Meas"/> objects.</returns>
        static List<Meas> convertMeasList(string pattern_string) {
            pattern_string = interpretModePrefix(pattern_string);
            return pattern_string.GetMeasStringArray().Select(selector: x => new Meas(convertSpanList(meas_string: x))).ToList();
        }

        /// <summary>
        /// Interprets church mode abbreviations in the pattern string.
        /// </summary>
        /// <param name="target">The pattern string to interpret.</param>
        /// <returns>The pattern string with mode abbreviations replaced.</returns>
        static string interpretModePrefix(string target) {
            target = target.Replace(oldValue: ":l", newValue: ":Lyd");
            target = target.Replace(oldValue: ":i", newValue: ":Ion");
            target = target.Replace(oldValue: ":m", newValue: ":Mix");
            target = target.Replace(oldValue: ":d", newValue: ":Dor");
            target = target.Replace(oldValue: ":a", newValue: ":Aeo");
            target = target.Replace(oldValue: ":p", newValue: ":Phr");
            target = target.Replace(oldValue: ":o", newValue: ":Loc");
            return target;
        }

        /// <summary>
        /// Converts a measure string to a list of <see cref="Span"/> objects.
        /// </summary>
        /// <param name="meas_string">The measure string to convert.</param>
        /// <returns>A list of <see cref="Span"/> objects.</returns>
        static List<Span> convertSpanList(string meas_string) {
            // splits a meas string into beats.
            string[] beat_array = meas_string.Split('|')
                .Select(selector: x => x.Replace(oldValue: "     ", newValue: " ")) // replaces 5 spaces with 1 space.
                .Select(selector: x => x.Replace(oldValue: "    ", newValue: " ")) // replaces 4 spaces with 1 space.
                .Select(selector: x => x.Replace(oldValue: "   ", newValue: " ")) // replaces 3 spaces with 1 space.
                .Select(selector: x => x.Replace(oldValue: "  ", newValue: " ")) // replaces 2 spaces with 1 space.
                .ToArray();
            // creates a list of Span objects.
            List<Span> span_list = new();
            // all 4 beats : "I | | | "
            if (beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && beat_array[3].Equals(" ")) {
                span_list.Add(item: convertSpan(beat: 4, beat_string: beat_array[0])); // 1st beat.
            }
            // 3 beats and 1 beat : "I | | |V "
            else if (beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && !beat_array[3].Equals(" ")) {
                span_list.Add(item: convertSpan(beat: 3, beat_string: beat_array[0])); // 1st beat.
                span_list.Add(item: convertSpan(beat: 1, beat_string: beat_array[3])); // 4th beat.
            }
            // 2 beats and 2 beats : "I | |V | "
            else if (beat_array[1].Equals(" ") && !beat_array[2].Equals(" ") && beat_array[3].Equals(" ")) {
                span_list.Add(item: convertSpan(beat: 2, beat_string: beat_array[0])); // 1st beat.
                span_list.Add(item: convertSpan(beat: 2, beat_string: beat_array[2])); // 3rd beat.
            }
            // 1 beat and 3 beats : "I |V | | "
            else if (!beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && beat_array[3].Equals(" ")) {
                span_list.Add(item: convertSpan(beat: 1, beat_string: beat_array[0])); // 1st beat.
                span_list.Add(item: convertSpan(beat: 3, beat_string: beat_array[1])); // 2nd beat.
            }
            // 1 beat and 2 beats and 1 beat : "I |V | |I |"
            else if (!beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && !beat_array[3].Equals(" ")) {
                span_list.Add(item: convertSpan(beat: 1, beat_string: beat_array[0])); // 1st beat.
                span_list.Add(item: convertSpan(beat: 2, beat_string: beat_array[1])); // 2nd beat.
                span_list.Add(item: convertSpan(beat: 1, beat_string: beat_array[3])); // 4th beat.
            }
            // FIXME: 2 beats and 1 beat and 1 beat.           : "I | |V |I "
            // FIXME: 1 beat and 1 beat and 2 beats.           : "I |V |I | |"
            // FIXME: 1 beat and 1 beat and 1 beat and 1 beat. : "I |V |I |V "
            return span_list;
        }

        /// <summary>
        /// Converts a beat string and number of beats to a <see cref="Span"/> object.
        /// </summary>
        /// <param name="beat">The number of beats.</param>
        /// <param name="beat_string">The beat string to convert.</param>
        /// <returns>The converted <see cref="Span"/> object.</returns>
        static Span convertSpan(int beat, string beat_string) {
            string[] part = beat_string.Split(separator: ':');
            if (part.Length == 1) {
                return new Span(beat: beat, degree: Degree.Enum.Parse(part[0].Trim()));
            }
            // a span has church mode specified.
            else {
                return new Span(beat: beat, degree: Degree.Enum.Parse(part[0].Trim()), span_mode: Mode.Enum.Parse(part[1].Trim()));
            }
        }

        /// <summary>
        /// Gets the length of the beat for the "count" pattern and updates the state.
        /// </summary>
        /// <param name="pattern">The pattern to check.</param>
        static void getCountBeatLength(Pattern pattern) {
            if (pattern.Name.Equals(COUNT_PATTERN)) {
                State.CountBeatLength = pattern.Data.GetBeatLength();
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
        /// Represents the root JSON object for pattern data.
        /// </summary>
        [DataContract]
        class Json {
            /// <summary>
            /// Gets or sets the array of pattern data.
            /// </summary>
            [DataMember(Name = "pattern")]
            public Pattern[] PatternArray { get; set; }
        }

        /// <summary>
        /// Represents a pattern entry in the JSON data.
        /// </summary>
        [DataContract]
        class Pattern {
            /// <summary>
            /// Gets or sets the pattern name.
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the pattern data string.
            /// </summary>
            [DataMember(Name = "data")]
            public string Data { get; set; }
        }
    }
}
