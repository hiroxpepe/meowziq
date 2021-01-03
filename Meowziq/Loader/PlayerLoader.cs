
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

        string targetPath; // player.json への PATH 文字列

        PlayerData playerData;

        List<Core.Phrase> phraseList; // 誰に何を渡すか

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public PlayerLoader(string targetPath) {
            this.targetPath = targetPath;
            this.playerData = new PlayerData();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public List<Core.Phrase> PhraseList {
            set => phraseList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Player のリストを作成します
        /// </summary>
        public List<Core.Player> BuildPlayerList() {
            loadJson(); // json のデータをオブジェクトにデシリアライズ
            List<Core.Player> _resultList = new List<Core.Player>();
            foreach (var _playerDao in playerData.Player) {
                _resultList.Add(convertPlayer(_playerDao)); // json のデータを変換
            }
            return _resultList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        Core.Player convertPlayer(Player playerDao) {
            var _player = new Core.Player();
            _player.MidiCh = Utils.ToMidiChannel(playerDao.Midi);
            if (playerDao.Inst != null) {
                _player.Program = Utils.ToInstrument(playerDao.Inst);
            }
            _player.Type = playerDao.Type;
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
