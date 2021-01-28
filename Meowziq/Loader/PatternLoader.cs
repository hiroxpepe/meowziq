
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Meowziq.Core;

namespace Meowziq.Loader {
    /// <summary>
    /// Pattern のローダークラス
    /// </summary>
    public static class PatternLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Pattern のリストを作成します
        ///     + ファイル読み込み
        /// </summary>
        public static List<Core.Pattern> Build(string targetPath) {
            // Core.Pattern のリストに変換
            return loadJson(targetPath).PatternArray.Select(x => convertPattern(x)).ToList();
        }

        /// <summary>
        /// Pattern のリストを作成します
        ///     + キャッシュした文字列
        /// </summary>
        public static List<Core.Pattern> Build(Stream target) {
            // Core.Pattern のリストに変換
            return loadJson(target).PatternArray.Select(x => convertPattern(x)).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Pattern convertPattern(Pattern pattern) {
            // Core.Pattern に変換
            return new Core.Pattern(pattern.Name, convertMeasList(pattern.Data));
        }

        static List<Meas> convertMeasList(string patternString) {
            // 小節に切り出す "[I:Aeo| | | ][IV| | | ][I| | | ][IV| | | ]"
            var _measStringArray = patternString.Replace("][", "@")  // まず "][" を "@" に置き換え
                .Split('@') // 小節で切り分ける
                .Select(x => x.Replace("[", "").Replace("]", "")).ToArray(); // 不要文字削除
            // Span リストに変換してから Meas を作成してリストに変換
            return _measStringArray.Select(x => new Meas(convertSpanList(x))).ToList();
        }

        static List<Span> convertSpanList(string measString) {
            // Span に切り出す "I:Aeo | | | "
            var _beatArray = measString.Split('|')
                .Select(x => x.Replace("     ", " ")) // 5空白を1空白に置き換え
                .Select(x => x.Replace("    ", " ")) // 4空白を1空白に置き換え
                .Select(x => x.Replace("   ", " ")) // 3空白を1空白に置き換え
                .Select(x => x.Replace("  ", " ")) // 1空白を1空白に置き換え
                .ToArray();

            // 1小節4拍しかないので決め打ち // FIXME: 足りないパターンがある [V|V|II|V] とか
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
            // TODO: // 2拍と1拍と1拍 "I | |V |I "

            // 1拍と3拍 "I |V | | "
            else if (!_beatArray[1].Equals(" ") && _beatArray[2].Equals(" ") && _beatArray[3].Equals(" ")) {
                _spanList.Add(convertSpan(1, _beatArray[0])); // 1拍目
                _spanList.Add(convertSpan(3, _beatArray[1])); // 2拍目
            }
            // 1拍と2拍と1拍 "I |V | |I |"
            else if (!_beatArray[1].Equals(" ") && _beatArray[2].Equals(" ") && !_beatArray[3].Equals(" ")) {
                _spanList.Add(convertSpan(1, _beatArray[0])); // 1拍目
                _spanList.Add(convertSpan(2, _beatArray[1])); // 2拍目
                _spanList.Add(convertSpan(1, _beatArray[3])); // 4拍目
            }
            // TODO: // 1拍と1拍と2拍 "I |V |I | |"

            // TODO: // 1拍と1拍と1拍と1拍 "I |V |I |V "
            return _spanList;
        }

        static Span convertSpan(int beat, string beatString) {
            var _part = beatString.Split(':'); // ':' で分割
            if (_part.Length == 1) {
                return new Span(beat, Degree.Enum.Parse(_part[0].Trim()));
            } else {
                return new Span(beat, Degree.Enum.Parse(_part[0].Trim()), Mode.Enum.Parse(_part[1].Trim())); // 旋法指定あり
            }
        }

        static Json loadJson(string targetPath) {
            using (var _stream = new FileStream(targetPath, FileMode.Open)) { // FIXME: ファイルが読めなかったとき
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
        class Json {
            [DataMember(Name = "pattern")]
            public Pattern[] PatternArray {
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
