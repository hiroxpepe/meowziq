
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;

namespace Meowziq.Loader {
    /// <summary>
    /// Pattern のローダークラス
    /// </summary>
    public class PatternLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string targetPath; // pattern.json への PATH 文字列

        PatternData patternData;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public PatternLoader(string targetPath) {
            this.targetPath = targetPath;
            this.patternData = new PatternData();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Pattern のリストを作成します
        /// </summary>
        public List<Core.Pattern> BuildPatternList() {
            loadJson(); // json のデータをオブジェクトにデシリアライズ
            var _resultList = new List<Core.Pattern>();
            foreach (var _pattern in patternData.Pattern) {
                _resultList.Add(convertPattern(_pattern)); // json のデータを変換
            }
            return _resultList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        Core.Pattern convertPattern(Pattern pattern) {
            var _measList = convertMeasList(pattern.Data);
            var _pattern = new Core.Pattern(pattern.Name, _measList);
            return _pattern;
        }

        List<Meas> convertMeasList(string patternString) {
            // 小節に切り出す "[I:Aeo| | | ][IV| | | ][I| | | ][IV| | | ]"
            var _measStringArray = patternString.Replace("][", "@")  // まず "][" を "@" に置き換え
                .Split('@') // 小節で切り分ける
                .Select(x => x.Replace("[","").Replace("]","")).ToArray(); // 不要文字削除
            // Meas リストに変換
            var _measList = new List<Meas>();
            foreach (var _measString in _measStringArray) {
                // Span リストに変換
                var _spanList = convertSpanList(_measString);
                // Meas を作成して追加
                _measList.Add(new Meas(_spanList));
            }
            return _measList;
        }

        List<Span> convertSpanList(string measString) {
            // Span に切り出す "I:Aeo | | | "
            var _beatArray = measString.Split('|')
                .Select(x => x.Replace("     ", " ")) // 5空白を1空白に置き換え
                .Select(x => x.Replace("    ", " ")) // 4空白を1空白に置き換え
                .Select(x => x.Replace("   ", " ")) // 3空白を1空白に置き換え
                .Select(x => x.Replace("  ", " ")) // 1空白を1空白に置き換え
                .ToArray();

            // 1小節4拍しかないので決め打ち
            var _spanList = new List<Span>();
            // 4拍全部 "I | | | "
            if (_beatArray[1].Equals(" ") && _beatArray[2].Equals(" ") && _beatArray[3].Equals(" ")) {
                _spanList.Add(convertSpan(4, _beatArray[0])); // 1拍目
            }
            // 3拍と1拍 "I | | |V "
            else if (_beatArray[1].Equals(" ") && _beatArray[2].Equals(" ") && !_beatArray[3].Equals(" ")) {
                _spanList.Add(convertSpan(3, _beatArray[0])); // 1拍目
                _spanList.Add(convertSpan(1, _beatArray[3])); // 4拍目
            }
            // 2拍と2拍 "I | |V | "
            else if (_beatArray[1].Equals(" ") && !_beatArray[2].Equals(" ") && _beatArray[3].Equals(" ")) {
                _spanList.Add(convertSpan(2, _beatArray[0])); // 1拍目
                _spanList.Add(convertSpan(2, _beatArray[2])); // 3拍目
            }
            // 1拍と3拍 "I |V | | "
            else if (!_beatArray[1].Equals(" ") && _beatArray[2].Equals(" ") && _beatArray[3].Equals(" ")) {
                _spanList.Add(convertSpan(1, _beatArray[0])); // 1拍目
                _spanList.Add(convertSpan(3, _beatArray[1])); // 2拍目
            }
            return _spanList;
        }

        Span convertSpan(int beat, string beatString) {
            var _part = beatString.Split(':'); // ':' で分割
            if (_part.Length == 1) {
                return new Span(beat, Utils.ToDegree(_part[0].Trim()));
            } else {
                return new Span(beat, Utils.ToDegree(_part[0].Trim()), Utils.ToMode(_part[1].Trim())); // 旋法指定あり
            }
        }

        void loadJson() {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) {
                var _serializer = new DataContractJsonSerializer(typeof(PatternData));
                patternData = (PatternData) _serializer.ReadObject(_stream);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        // MEMO: 編集: JSON をクラスとして張り付ける
        [DataContract]
        class PatternData {
            [DataMember(Name = "pattern")]
            public Pattern[] Pattern {
                get; set;
            }
        }

        [DataContract]
        class Pattern {
            [DataMember(Name = "name")]
            public string Name {
                get; set;
            }
            [DataMember(Name = "data")]
            public string Data {
                get; set;
            }
        }
    }
}
