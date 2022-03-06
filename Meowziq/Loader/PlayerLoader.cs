
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;

namespace Meowziq.Loader {
    /// <summary>
    /// Player のローダークラス
    /// </summary>
    public static class PlayerLoader<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static List<Phrase> _phraseList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static List<Phrase> PhraseList {
            set => _phraseList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Player のリストを作成します
        /// </summary>
        public static List<Core.Player<T>> Build(Stream target) {
            if (_phraseList is null) {
                throw new ArgumentException("need phraseList.");
            }
            return loadJson(target).PlayerArray.Select(x => convertPlayer(x)).ToList(); // Core.Player のリストに変換
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Player<T> convertPlayer(Player player) {
            Core.Player<T> newPlayer = new();
            newPlayer.MidiCh = MidiChannel.Enum.Parse(player.Midi);
            if (int.Parse(player.Midi) is 9) { // FIXME: 10ch 以外のドラムを可能にする
                newPlayer.DrumKit = DrumKit.Enum.Parse(player.Inst); // FIXME: 設定が違う場合
            } else {
                newPlayer.Instrument = Instrument.Enum.Parse(player.Inst); // FIXME: 設定が違う場合
            }
            newPlayer.Type = player.Type;
            newPlayer.PhraseList = _phraseList.Where(x => x.Type.Equals(newPlayer.Type)).ToList(); // Player と Phrase の type が一致したら
            return newPlayer;
        }

        static Json loadJson(Stream target) {
            var serializer = new DataContractJsonSerializer(typeof(Json));
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
