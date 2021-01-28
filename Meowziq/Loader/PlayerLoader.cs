
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
    public static class PlayerLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static List<Phrase> phraseList; // 誰に何を渡すか

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static List<Phrase> PhraseList {
            set => phraseList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Player のリストを作成します
        ///     + ファイル読み込み
        /// </summary>
        public static List<Core.Player> Build(string targetPath) {
            if (phraseList == null) {
                throw new ArgumentException("need phraseList.");
            }
            // Core.Player のリストに変換
            return loadJson(targetPath).PlayerArray.Select(x => convertPlayer(x)).ToList();
        }

        /// <summary>
        /// Player のリストを作成します
        ///     + キャッシュした文字列
        /// </summary>
        public static List<Core.Player> Build(Stream target) {
            if (phraseList == null) {
                throw new ArgumentException("need phraseList.");
            }
            // Core.Player のリストに変換
            return loadJson(target).PlayerArray.Select(x => convertPlayer(x)).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Player convertPlayer(Player player) {
            var _player = new Core.Player();
            _player.MidiCh = MidiChannel.Enum.Parse(player.Midi);
            if (int.Parse(player.Midi) == 9) { // FIXME: 10ch 以外のドラムを可能にする
                _player.DrumKit = DrumKit.Enum.Parse(player.Inst); // FIXME: 設定が違う場合
            } else {
                _player.Instrument = Instrument.Enum.Parse(player.Inst); // FIXME: 設定が違う場合
            }
            _player.Type = player.Type;
            _player.PhraseList = phraseList.Where(x => x.Type.Equals(_player.Type)).ToList(); // Player と Phrase の type が一致したら
            return _player;
        }

        static Json loadJson(string targetPath) {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) {
                var _serializer = new DataContractJsonSerializer(typeof(Json));
                return (Json) _serializer.ReadObject(_stream);
            }
        }

        static Json loadJson(Stream target) {
            var _serializer = new DataContractJsonSerializer(typeof(Json));
            return (Json) _serializer.ReadObject(target);
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
