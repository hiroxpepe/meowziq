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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meowziq.Core;

namespace Meowziq.Loader {
    /// <summary>
    /// loader class for pattern.
    /// </summary>
    /// <author>
    /// h.adachi (STUDIO MeowToon)
    /// </author>
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

        static List<Meas> convertMeasList(string pattern_string) {
            // 旋法の省略記述を変換
            pattern_string = interpretModePrefix(pattern_string);
            // 小節に切り出す "[I:Aeo| | | ][IV| | | ][I| | | ][IV| | | ]"
            var meas_string_array = pattern_string.Replace("][", "@")  // まず "][" を "@" に置き換え
                .Split('@') // 小節で切り分ける
                .Select(x => x.Replace("[", string.Empty).Replace("]", string.Empty)).ToArray(); // 不要文字削除
            // Span リストに変換してから Meas を作成してリストに変換
            return meas_string_array.Select(x => new Meas(convertSpanList(x))).ToList();
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

        static List<Span> convertSpanList(string meas_string) {
            // Span に切り出す "I:Aeo | | | "
            var beat_array = meas_string.Split('|')
                .Select(x => x.Replace("     ", " ")) // 5空白を1空白に置き換え
                .Select(x => x.Replace("    ", " ")) // 4空白を1空白に置き換え
                .Select(x => x.Replace("   ", " ")) // 3空白を1空白に置き換え
                .Select(x => x.Replace("  ", " ")) // 1空白を1空白に置き換え
                .ToArray();

            // 1小節4拍しかないので決め打ち // FIXME: 足りないパターンがある [V|V|II|V] とか
            List<Span> span_list = new();
            // 4拍全部 "I | | | "
            if (beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(4, beat_array[0])); // 1拍目
            }
            // 3拍と1拍 "I | | |V "
            else if (beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && !beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(3, beat_array[0])); // 1拍目
                span_list.Add(convertSpan(1, beat_array[3])); // 4拍目
            }
            // 2拍と2拍 "I | |V | "
            else if (beat_array[1].Equals(" ") && !beat_array[2].Equals(" ") && beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(2, beat_array[0])); // 1拍目
                span_list.Add(convertSpan(2, beat_array[2])); // 3拍目
            }
            // TODO: // 2拍と1拍と1拍 "I | |V |I "

            // 1拍と3拍 "I |V | | "
            else if (!beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(1, beat_array[0])); // 1拍目
                span_list.Add(convertSpan(3, beat_array[1])); // 2拍目
            }
            // 1拍と2拍と1拍 "I |V | |I |"
            else if (!beat_array[1].Equals(" ") && beat_array[2].Equals(" ") && !beat_array[3].Equals(" ")) {
                span_list.Add(convertSpan(1, beat_array[0])); // 1拍目
                span_list.Add(convertSpan(2, beat_array[1])); // 2拍目
                span_list.Add(convertSpan(1, beat_array[3])); // 4拍目
            }
            // TODO: // 1拍と1拍と2拍 "I |V |I | |"

            // TODO: // 1拍と1拍と1拍と1拍 "I |V |I |V "
            return span_list;
        }

        static Span convertSpan(int beat, string beat_string) {
            var part = beat_string.Split(':'); // ':' で分割
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
