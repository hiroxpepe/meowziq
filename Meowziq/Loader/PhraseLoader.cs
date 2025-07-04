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

using Meowziq.Value;

namespace Meowziq.Loader {
    /// <summary>
    /// Provides methods for loading and converting Phrase objects from JSON data.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class PhraseLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Creates a list of <see cref="Core.Phrase"/> objects from the specified stream.
        /// Inherits a Phrase if "base" is given.
        /// </summary>
        /// <param name="target">The stream containing the phrase JSON data.</param>
        /// <returns>A list of <see cref="Core.Phrase"/> objects.</returns>
        public static List<Core.Phrase> Build(Stream target) {
            List<Core.Phrase> list = loadJson(target).PhraseArray.Select(selector: x => convertPhrase(phrase: x)).ToList();
            return list.Select(selector: x => x = x.Base is not null ? 
                Inheritor.Apply(target: x, baze: searchBasePhrase(phrase_type: x.Type, phrase_name: x.Base, list: list)) : x
            ).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// Converts a <see cref="Phrase"/> object to a <see cref="Core.Phrase"/> object.
        /// </summary>
        /// <param name="phrase">The phrase to convert.</param>
        /// <returns>The converted <see cref="Core.Phrase"/> object.</returns>
        static Core.Phrase convertPhrase(Phrase phrase) {
            Core.Phrase new_phrase = new();
            new_phrase.Type = phrase.Type;
            new_phrase.Name = phrase.Name;
            new_phrase.Base = phrase.Base;
            if (phrase.Note is not null) { // Determines from the key mode.
                new_phrase.Data.Note.Text = phrase.Note;
            } else if (phrase.Auto is not null) { // Determines from the span mode.
                new_phrase.Data.Note.Text = phrase.Auto;
                new_phrase.Data.Auto = true;
            }
            new_phrase.Data.Note.Oct = phrase.Oct;
            new_phrase.Data.Chord.Text = phrase.Chord;
            new_phrase.Data.Seque.Text = phrase.Gate;
            new_phrase.Range = phrase.Range;
            new_phrase.Data.Exp.Pre = phrase.Pre;
            new_phrase.Data.Exp.Post = phrase.Post;
            if (phrase.Data is not null) { // Has compound data.
                new_phrase.Data.BeatArray = phrase.Data.BeatArray;
                new_phrase.Data.NoteArray = phrase.Data.NoteArray;
                new_phrase.Data.AutoArray = phrase.Data.AutoArray;
                new_phrase.Data.OctArray = phrase.Data.OctArray;
                new_phrase.Data.PreArray = phrase.Data.PreArray;
                new_phrase.Data.PostArray = phrase.Data.PostArray;
                if (phrase.Data.InstArray is not null) { // Has an instrument name array for drums.
                    new_phrase.Data.PercussionArray = phrase.Data.InstArray.Select(selector: x => Percussion.Enum.Parse(x)).ToArray();
                }
            }
            return new_phrase;
        }

        /// <summary>
        /// Searches for the base <see cref="Core.Phrase"/> Object by type and name.
        /// </summary>
        /// <param name="phrase_type">The type of the phrase.</param>
        /// <param name="phrase_name">The name of the base phrase.</param>
        /// <param name="list">The list of phrases to search.</param>
        /// <returns>The found base <see cref="Core.Phrase"/> object.</returns>
        /// <exception cref="ArgumentException">Thrown if the base phrase is not found.</exception>
        static Core.Phrase searchBasePhrase(string phrase_type, string phrase_name, List<Core.Phrase> list) {
            try {
                return list.Where(predicate: x => x.Type.Equals(phrase_type) && x.Name.Equals(phrase_name)).First();
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
        /// Represents the root JSON object for phrase data.
        /// </summary>
        [DataContract]
        class Json {
            /// <summary>
            /// Gets or sets the array of phrase data.
            /// </summary>
            [DataMember(Name = "phrase")]
            public Phrase[] PhraseArray { get; set; }
        }

        /// <summary>
        /// Represents a phrase entry in the JSON data.
        /// </summary>
        [DataContract]
        class Phrase {
            /// <summary>
            /// Gets or sets the phrase type.
            /// </summary>
            [DataMember(Name = "type")]
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets the phrase name.
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the base phrase name for inheritance.
            /// </summary>
            [DataMember(Name = "base")]
            public string Base { get; set; }

            /// <summary>
            /// Gets or sets the phrase data object.
            /// </summary>
            [DataMember(Name = "data")]
            public Data Data { get; set; }

            /// <summary>
            /// Gets or sets the note string.
            /// </summary>
            [DataMember(Name = "note")]
            public string Note { get; set; }

            /// <summary>
            /// Gets or sets the auto string.
            /// </summary>
            [DataMember(Name = "auto")]
            public string Auto { get; set; }

            /// <summary>
            /// Gets or sets the octave value.
            /// </summary>
            [DataMember(Name = "oct")]
            public int Oct { get; set; }

            /// <summary>
            /// Gets or sets the chord string.
            /// </summary>
            [DataMember(Name = "chord")]
            public string Chord { get; set; }

            /// <summary>
            /// Gets or sets the gate string.
            /// </summary>
            [DataMember(Name = "gate")]
            public string Gate { get; set; }

            /// <summary>
            /// Gets or sets the range string.
            /// </summary>
            [DataMember(Name = "range")]
            public string Range { get; set; }

            /// <summary>
            /// Gets or sets the pre-expression string.
            /// </summary>
            [DataMember(Name = "pre")]
            public string Pre { get; set; }

            /// <summary>
            /// Gets or sets the post-expression string.
            /// </summary>
            [DataMember(Name = "post")]
            public string Post { get; set; }
        }

        /// <summary>
        /// Represents the data section of a phrase entry in the JSON data.
        /// </summary>
        [DataContract]
        class Data {
            /// <summary>
            /// Gets or sets the instrument name array for drums.
            /// </summary>
            [DataMember(Name = "inst")]
            public string[] InstArray { get; set; }

            /// <summary>
            /// Gets or sets the beat array.
            /// </summary>
            [DataMember(Name = "beat")]
            public string[] BeatArray { get; set; }

            /// <summary>
            /// Gets or sets the note array.
            /// </summary>
            [DataMember(Name = "note")]
            public string[] NoteArray { get; set; }

            /// <summary>
            /// Gets or sets the auto array.
            /// </summary>
            [DataMember(Name = "auto")]
            public string[] AutoArray { get; set; }

            /// <summary>
            /// Gets or sets the octave array.
            /// </summary>
            [DataMember(Name = "oct")]
            public int[] OctArray { get; set; }

            /// <summary>
            /// Gets or sets the pre-expression array.
            /// </summary>
            [DataMember(Name = "pre")]
            public string[] PreArray { get; set; }

            /// <summary>
            /// Gets or sets the post-expression array.
            /// </summary>
            [DataMember(Name = "post")]
            public string[] PostArray { get; set; }
        }
    }
}
