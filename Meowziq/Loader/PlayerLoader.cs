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
using static Meowziq.Env;

namespace Meowziq.Loader {
    /// <summary>
    /// Provides loader functionality for creating and managing Player objects.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class PlayerLoader<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// Stores the list of phrases used to create Player objects.
        /// </summary>
        static List<Phrase> _phrase_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// Sets the list of phrases used to create Player objects.
        /// </summary>
        public static List<Phrase> PhraseList { set => _phrase_list = value; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Creates a list of <see cref="Core.Player{T}"/> objects from the specified stream.
        /// </summary>
        /// <param name="target">The stream containing the player JSON data.</param>
        /// <returns>A list of <see cref="Core.Player{T}"/> objects.</returns>
        public static List<Core.Player<T>> Build(Stream target) {
            if (_phrase_list is null) { throw new ArgumentException("need _phrase_list."); }
            return loadJson(target).PlayerArray.Select(selector: x => convertPlayer(player: x)).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// Converts a <see cref="Player"/> object to a <see cref="Core.Player{T}"/> object.
        /// </summary>
        /// <param name="player">The player to convert.</param>
        /// <returns>The converted <see cref="Core.Player{T}"/> object.</returns>
        static Core.Player<T> convertPlayer(Player player) {
            Core.Player<T> new_player = new();
            new_player.MidiCh = MidiChannel.Enum.Parse(player.Midi);
            if (player.Inst is not null) { // Checks for presence of mixer.json.
                if (int.Parse(player.Midi) == MIDI_CH_DRUM) { // Enables drums other than 10ch.
                    new_player.DrumKit = DrumKit.Enum.Parse(player.Inst);
                } else {
                    new_player.Instrument = Instrument.Enum.Parse(player.Inst);
                }
            }
            new_player.Type = player.Type;
            new_player.PhraseList = _phrase_list.Where(predicate: x => x.Type.Equals(new_player.Type)).ToList(); // Matches the type of Player and Phrase.
            return new_player;
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
        /// Represents the root JSON object for player data.
        /// </summary>
        [DataContract]
        public class Json {
            /// <summary>
            /// Gets or sets the array of player data.
            /// </summary>
            [DataMember(Name = "player")]
            public Player[] PlayerArray { get; set; }
        }

        /// <summary>
        /// Represents a player entry in the JSON data.
        /// </summary>
        [DataContract]
        public class Player {
            /// <summary>
            /// Gets or sets the player type.
            /// </summary>
            [DataMember(Name = "type")]
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets the MIDI channel as a string.
            /// </summary>
            [DataMember(Name = "midi")]
            public string Midi { get; set; }

            /// <summary>
            /// Gets or sets the instrument name.
            /// </summary>
            [DataMember(Name = "inst")]
            public string Inst { get; set; }
        }
    }
}
