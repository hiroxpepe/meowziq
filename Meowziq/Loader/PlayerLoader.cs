
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;

namespace Meowziq.Loader {
    /// <summary>
    /// Player のローダークラス
    /// </summary>
    public static class PlayerLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static List<Phrase> phraseList; // 誰に何を渡すか

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjectives] 

        public static List<Phrase> PhraseList {
            set => phraseList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Player のリストを作成します
        /// </summary>
        public static List<Core.Player> BuildPlayerList(string targetPath) {
            var _resultList = new List<Core.Player>();
            foreach (var _player in loadJson(targetPath).Player) {
                _resultList.Add(convertPlayer(_player)); // json のデータを変換
            }
            return _resultList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Player convertPlayer(Player player) {
            var _player = new Core.Player();
            _player.MidiCh = Utils.ToMidiChannel(player.Midi);
            if (int.Parse(player.Midi) == 9) { // FIXME: 9ch 以外のドラムを可能にする
                _player.DrumKit = Utils.ToDrumKit(player.Inst); // FIXME: 設定が違う場合
            } else {
                _player.Instrument = Utils.ToInstrument(player.Inst); // FIXME: 設定が違う場合
            }
            _player.Type = player.Type;
            // Player と Phrase の type が一致したら
            foreach (var _phrase in phraseList) {
                if (_player.Type.Equals(_phrase.Type)) {
                    _player.PhraseList.Add(_phrase); // Phrase フレーズを渡す
                }
            }
            return _player;
        }

        static PlayerData loadJson(string targetPath) {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) {
                var _serializer = new DataContractJsonSerializer(typeof(PlayerData));
                return (PlayerData) _serializer.ReadObject(_stream);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        // MEMO: 編集: JSON をクラスとして張り付ける
        [DataContract]
        public class PlayerData {
            [DataMember(Name = "player")]
            public Player[] Player {
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
