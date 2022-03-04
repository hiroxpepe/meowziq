
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
    public static class PatternLoader {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Pattern のリストを作成します
        /// </summary>
        public static List<Core.Pattern> Build(Stream target) {
            return loadJson(target).PatternArray.Select(x => convertPattern(x)).ToList(); // Core.Pattern のリストに変換
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Core.Pattern convertPattern(Pattern pattern) {
            return new Core.Pattern(pattern.Name, convertMeasList(pattern.Data)); // Core.Pattern に変換
        }

        static List<Meas> convertMeasList(string patternString) {
            // 旋法の省略記述を変換
            patternString = interpretModePrefix(patternString);
            // 小節に切り出す "[I:Aeo| | | ][IV| | | ][I| | | ][IV| | | ]"
            var measStringArray = patternString.Replace("][", "@")  // まず "][" を "@" に置き換え
                .Split('@') // 小節で切り分ける
                .Select(x => x.Replace("[", "").Replace("]", "")).ToArray(); // 不要文字削除
            // Span リストに変換してから Meas を作成してリストに変換
            return measStringArray.Select(x => new Meas(convertSpanList(x))).ToList();
        }

        /// <summary>
        /// 旋法の省略記法を解釈します
        /// </summary>
        static string interpretModePrefix(string target) {
            target = target.Replace(":l", ":Lyd");
            target = target.Replace(":i", ":Ion");
            target = target.Replace(":m", ":Mix");
            target = target.Replace(":d", ":Dor");
            target = target.Replace(":a", ":Aeo");
            target = target.Replace(":p", ":Phr");
            target = target.Replace(":o", ":Loc");
            return target;
        }

        static List<Span> convertSpanList(string measString) {
            // Span に切り出す "I:Aeo | | | "
            var beatArray = measString.Split('|')
                .Select(x => x.Replace("     ", " ")) // 5空白を1空白に置き換え
                .Select(x => x.Replace("    ", " ")) // 4空白を1空白に置き換え
                .Select(x => x.Replace("   ", " ")) // 3空白を1空白に置き換え
                .Select(x => x.Replace("  ", " ")) // 1空白を1空白に置き換え
                .ToArray();

            // 1小節4拍しかないので決め打ち // FIXME: 足りないパターンがある [V|V|II|V] とか
            var spanList = new List<Span>();
            // 4拍全部 "I | | | "
            if (beatArray[1].Equals(" ") && beatArray[2].Equals(" ") && beatArray[3].Equals(" ")) {
                spanList.Add(convertSpan(4, beatArray[0])); // 1拍目
            }
            // 3拍と1拍 "I | | |V "
            else if (beatArray[1].Equals(" ") && beatArray[2].Equals(" ") && !beatArray[3].Equals(" ")) {
                spanList.Add(convertSpan(3, beatArray[0])); // 1拍目
                spanList.Add(convertSpan(1, beatArray[3])); // 4拍目
            }
            // 2拍と2拍 "I | |V | "
            else if (beatArray[1].Equals(" ") && !beatArray[2].Equals(" ") && beatArray[3].Equals(" ")) {
                spanList.Add(convertSpan(2, beatArray[0])); // 1拍目
                spanList.Add(convertSpan(2, beatArray[2])); // 3拍目
            }
            // TODO: // 2拍と1拍と1拍 "I | |V |I "

            // 1拍と3拍 "I |V | | "
            else if (!beatArray[1].Equals(" ") && beatArray[2].Equals(" ") && beatArray[3].Equals(" ")) {
                spanList.Add(convertSpan(1, beatArray[0])); // 1拍目
                spanList.Add(convertSpan(3, beatArray[1])); // 2拍目
            }
            // 1拍と2拍と1拍 "I |V | |I |"
            else if (!beatArray[1].Equals(" ") && beatArray[2].Equals(" ") && !beatArray[3].Equals(" ")) {
                spanList.Add(convertSpan(1, beatArray[0])); // 1拍目
                spanList.Add(convertSpan(2, beatArray[1])); // 2拍目
                spanList.Add(convertSpan(1, beatArray[3])); // 4拍目
            }
            // TODO: // 1拍と1拍と2拍 "I |V |I | |"

            // TODO: // 1拍と1拍と1拍と1拍 "I |V |I |V "
            return spanList;
        }

        static Span convertSpan(int beat, string beatString) {
            var part = beatString.Split(':'); // ':' で分割
            if (part.Length == 1) {
                return new Span(beat, Degree.Enum.Parse(part[0].Trim()));
            } else {
                return new Span(beat, Degree.Enum.Parse(part[0].Trim()), Mode.Enum.Parse(part[1].Trim())); // 旋法指定あり
            }
        }

        static Json loadJson(Stream target) {
            var serializer = new DataContractJsonSerializer(typeof(Json));
            return (Json) serializer.ReadObject(target);
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
