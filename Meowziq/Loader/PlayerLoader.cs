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
    /// loader class for Player object.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class PlayerLoader<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static List<Phrase> _phrase_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static List<Phrase> PhraseList {
            set => _phrase_list = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// creates a list of players.
        /// </summary>
        public static List<Core.Player<T>> Build(Stream target) {
            if (_phrase_list is null) {
                throw new ArgumentException("need _phrase_list.");
            }
            /// <remarks>
            /// converts to a list of Core.Player.
            /// </remarks>
            return loadJson(target).PlayerArray.Select(x => convertPlayer(x)).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Player<T> convertPlayer(Player player) {
            Core.Player<T> new_player = new();
            new_player.MidiCh = MidiChannel.Enum.Parse(player.Midi);
            if (int.Parse(player.Midi) is 9) { // FIXME: 10ch 以外のドラムを可能にする
                new_player.DrumKit = DrumKit.Enum.Parse(player.Inst); // FIXME: 設定が違う場合
            } else {
                new_player.Instrument = Instrument.Enum.Parse(player.Inst); // FIXME: 設定が違う場合
            }
            new_player.Type = player.Type;
            new_player.PhraseList = _phrase_list.Where(x => x.Type.Equals(new_player.Type)).ToList(); // Player と Phrase の type が一致したら
            return new_player;
        }

        static Json loadJson(Stream target) {
            DataContractJsonSerializer serializer = new(typeof(Json));
            return (Json) serializer.ReadObject(target);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        [DataContract]
        public class Json {
            [DataMember(Name = "player")]
            public Player[] PlayerArray {
                get; set;
            }
        }

        [DataContract]
        public class Player {
            [DataMember(Name = "type")]
            public string Type {
                get; set;
            }
            [DataMember(Name = "midi")]
            public string Midi {
                get; set;
            }
            [DataMember(Name = "inst")]
            public string Inst {
                get; set;
            }
        }
    }
}
