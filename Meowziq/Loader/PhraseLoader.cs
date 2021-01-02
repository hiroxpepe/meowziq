
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;

namespace Meowziq.Loader {
    /// <summary>
    /// Phrase のローダークラス
    /// </summary>
    public class PhraseLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string targetPath; // phrase.jsonc への PATH 文字列

        PhraseData phraseData;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public PhraseLoader(string targetPath) {
            //validateValue(target); // TODO: バリデート実行
            this.targetPath = targetPath;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Phrase のリストを作成します
        /// </summary>
        public List<Core.Phrase> BuildPhraseList() {
            loadJson(); // json のデータをオブジェクトにデシリアライズ
            List<Core.Phrase> _resultList = new List<Core.Phrase>();
            foreach (var _phraseDao in phraseData.phrase) {
                _resultList.Add(convertPhrase(_phraseDao)); // json のデータを変換
            }
            return _resultList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        Core.Phrase convertPhrase(Phrase phraseDao) {
            var _phrase = new Core.Phrase();
            _phrase.Type = phraseDao.type;
            _phrase.Name = phraseDao.name;
            _phrase.NoteText = validateValue(phraseDao.note);
            if (phraseDao.data != null) { // 複合データがある場合
                _phrase.DataValue.NoteTextArray = phraseDao.data.note
                    .Select(x => validateValue(x)).ToArray();
                if (phraseDao.data.inst != null) { // ドラム用音名データがある場合
                    Percussion[] _percussionArray = new Percussion[phraseDao.data.inst.Length];
                    var _i = 0;
                    foreach (var _inst in phraseDao.data.inst) {
                        _percussionArray[_i++] = Utils.ToPercussion(_inst);
                    }
                    _phrase.DataValue.PercussionArray = _percussionArray;
                }
            }
            return _phrase;
        }

        /// <summary>
        /// TODO: 使用可能な文字
        /// </summary>
        string validateValue(string target) {
            if (target == null) {
                return target; // 値がなければそのまま返す FIXME:
            }
            // 拍のデータの数が4文字かどうか
            var _target1 = target;
            // 文字の置き換え
            _target1 = _target1.Replace("[", "|").Replace("]", "|");
            // 区切り文字で切り分ける
            var _array1 = _target1.Split('|')
                .Where(x => !string.IsNullOrWhiteSpace(x)) // 空文字以外
                .Where(x => x.Length != 4) // データが4文字ではない
                .ToArray();
            // そのデータがあれば例外を投げる
            if (_array1.Length != 0) {
                throw new System.FormatException("data count must be 4.");
            }
            // バリデーションOKなら元々の文字列を返す
            return target;
        }

        void loadJson() {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) {
                var _serializer = new DataContractJsonSerializer(typeof(PhraseData));
                phraseData = (PhraseData) _serializer.ReadObject(_stream);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        // MEMO: 編集: JSON をクラスとして張り付ける
        [DataContract]
        class PhraseData {
            [DataMember]
            public Phrase[] phrase {
                get; set;
            }
        }

        [DataContract]
        class Phrase {
            [DataMember]
            public string type {
                get; set;
            }
            [DataMember]
            public string name {
                get; set;
            }
            [DataMember]
            public Data data {
                get; set;
            }
            [DataMember]
            public string note {
                get; set;
            }
        }

        [DataContract]
        class Data {
            [DataMember]
            public string[] inst {
                get; set;
            }
            [DataMember]
            public string[] note {
                get; set;
            }
        }
    }
}
