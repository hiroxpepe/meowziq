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
    /// loader class for Player object.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class PlayerLoader<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static List<Phrase> _phrase_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static List<Phrase> PhraseList { set => _phrase_list = value; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// creates a list of Core.Player objects.
        /// </summary>
        public static List<Core.Player<T>> Build(Stream target) {
            if (_phrase_list is null) { throw new ArgumentException("need _phrase_list."); }
            return loadJson(target).PlayerArray.Select(selector: x => convertPlayer(player: x)).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// converts a Player object to a Core.Player object.
        /// </summary>
        static Core.Player<T> convertPlayer(Player player) {
            Core.Player<T> new_player = new();
            new_player.MidiCh = MidiChannel.Enum.Parse(player.Midi);
            if (player.Inst is not null) { // FIXME: checks for presence of mixer.json.
                if (int.Parse(player.Midi) == MIDI_CH_DRUM) { // FIXME: enables drums other than 10ch.
                    new_player.DrumKit = DrumKit.Enum.Parse(player.Inst);
                } else {
                    new_player.Instrument = Instrument.Enum.Parse(player.Inst);
                }
            }
            new_player.Type = player.Type;
            new_player.PhraseList = _phrase_list.Where(predicate: x => x.Type.Equals(new_player.Type)).ToList(); // matched the type of Player and Phrase. 
            return new_player;
        }

        /// <summary>
        /// reads a .json file to the JSON object.
        /// </summary>
        static Json loadJson(Stream target) {
            DataContractJsonSerializer serializer = new(type: typeof(Json));
            return (Json) serializer.ReadObject(stream: target);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        [DataContract]
        public class Json {
            [DataMember(Name = "player")]
            public Player[] PlayerArray { get; set; }
        }

        [DataContract]
        public class Player {
            [DataMember(Name = "type")]
            public string Type { get; set; }
            [DataMember(Name = "midi")]
            public string Midi { get; set; }
            [DataMember(Name = "inst")]
            public string Inst { get; set; }
        }
    }
}
