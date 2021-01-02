
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;

namespace Meowziq.Loader {
    /// <summary>
    /// Player のローダークラス
    /// </summary>
    public class PlayerLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string targetPath; // player.jsonc への PATH 文字列

        PlayerData playerData;

        List<Core.Phrase> phraseList; // 誰に何を渡すか

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public List<Core.Phrase> PhraseList {
            set => phraseList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public PlayerLoader(string targetPath) {
            this.targetPath = targetPath;
            playerData = new PlayerData();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Player のリストを作成します
        /// </summary>
        public List<Core.Player> BuildPlayerList() {
            loadJson(); // json のデータをオブジェクトにデシリアライズ
            List<Core.Player> _resultList = new List<Core.Player>();
            foreach (var _playerDao in playerData.player) {
                _resultList.Add(convertPlayer(_playerDao)); // json のデータを変換
            }
            return _resultList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        Core.Player convertPlayer(Player playerDao) {
            var _player = new Core.Player();
            _player.MidiCh = Utils.ToMidiChannel(playerDao.midi);
            if (playerDao.inst != null) {
                _player.Program = Utils.ToInstrument(playerDao.inst);
            }
            _player.Type = playerDao.type;
            // Player と Phrase の type が一致したら
            foreach (var _phrase in phraseList) {
                if (_player.Type.Equals(_phrase.Type)) {
                    _player.PhraseList.Add(_phrase); // Phrase フレーズを渡す
                }
            }
            return _player;
        }

        void loadJson() {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) {
                var _serializer = new DataContractJsonSerializer(typeof(PlayerData));
                playerData = (PlayerData) _serializer.ReadObject(_stream);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        // MEMO: 編集: JSON をクラスとして張り付ける
        [DataContract]
        public class PlayerData {
            [DataMember]
            public Player[] player {
                get; set;
            }
        }

        [DataContract]
        public class Player {
            [DataMember]
            public string type {
                get; set;
            }
            [DataMember]
            public string midi {
                get; set;
            }
            [DataMember]
            public string inst {
                get; set;
            }
        }
    }
}
