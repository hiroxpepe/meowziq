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
using System.Globalization;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// ステイト クラス
    /// </summary>
    /// <note>
    /// + 曲がどのような状況で演奏されているかを表す情報を保持する
    /// + 設定されて読み出させるだけこれを状態を変更する目的で使わないで下さい
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class State {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        static string _name;

        static string _copyright;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static State() {
            HashSet = new(); // ※Dictionary.ContainsKey() が遅いのでその対策
            ItemMap = new();
            TrackMap = new();
            _name = "Undefined"; // TODO: 別の曲を読み込んだ時
            _copyright = "Undefined"; // TODO: 別の曲を読み込んだ時
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Properties [noun, adjective] 

        public static int Tempo {
            get; set;
        }

        public static int Beat {
            get; set;
        }

        public static int Meas {
            get; set;
        }

        public static string Name {
            get => _name;
            set => _name = value;
        }

        public static string Copyright {
            get => _copyright;
            set => _copyright = value;
        }

        public static (int tempo, string name) TempoAndName {
            set {
                Name = value.name;
                Tempo = value.tempo;
            }
        }

        public static HashSet<int> HashSet {
            get; set;
        }

        public static Map<int, Item16beat> ItemMap {
            get; set;
        }

        public static Map<int, Track> TrackMap {
            get; set;
        }

        public static List<Track> TrackList {
            get => TrackMap.Select(x => x.Value).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static void Clear() {
            Beat = 0;
            Meas = 0;
            HashSet.Clear();
            ItemMap.Clear();
            TrackMap.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// SMF 出力用のトラック情報を保持
        /// </summary>
        public class Track {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            string _name;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective]

            public int MidiCh {
                get; set;
            }

            public string Name {
                get => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_name); 
                set => _name = value;
            }

            public string Instrument {
                get; set;
            }

            public int Vol {
                get; set;
            }

            public int Pan {
                get; set;
            }

            public bool Mute {
                get; set;
            }
        }

        /// <summary>
        /// どのようなキー、度数、旋法で演奏されているかを表す情報
        /// NOTE: 16beat 毎に作成される
        /// </summary>
        public class Item16beat {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            string _span_mode; // TODO: Mode 型に変更

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective]

            public int Tick {
                get; set;
            }

            public string Key {
                get; set;
            }

            public string Degree {
                get; set;
            }

            public string KeyMode {
                get; set;
            }

            public string SpanMode {
                get {
                    if (_span_mode.Equals("Undefined")) {
                        return KeyMode;
                    }
                    return _span_mode;
                } 
                set => _span_mode = value;
            }

            public bool AutoMode {
                get {
                    if (_span_mode.Equals("Undefined")) {
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}
