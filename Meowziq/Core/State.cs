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

using static Meowziq.Env;

namespace Meowziq.Core {
    /// <summary>
    /// state class
    /// </summary>
    /// <note>
    /// + 曲がどのような状況で演奏されているかを表す情報を保持する
    /// + 設定されて読み出させるだけこれを状態を変更する目的で使わない
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class State {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Const [nouns]

        const int TO_ONE_BASE = 1;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields [nouns, noun phrases]

        static int _tick, _tempo, _count_beat_length = 0;

        static bool _same_tick;

        static string _name, _copyright;

        static HashSet<int> _hashset;

        static Map<int, Item16beat> _item_map;

        static Map<int, Track> _track_map;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static State() {
            _hashset = new();
            _item_map = new();
            _track_map = new();
            _name = "Undefined"; // FIXME: when loading another song.
            _copyright = "Undefined"; // FIXME: when loading another song.
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Properties [noun, adjective] 

        /// <summary>
        /// provides the song tempo.
        /// </summary>
        public static int Tempo {
            get => _tempo; set => _tempo = value;
        }

        /// <summary>
        /// provides the current tick.
        /// </summary>
        public static int Tick {
            get => _tick;
            set {
                if (_tick == value) {
                    _same_tick = true;
                }
                else {
                    _same_tick = false;
                    _tick = value;
                }
            }
        }

        /// <summary>
        /// whether given the same tick.
        /// </summary>
        public static bool SameTick {
            get => _same_tick;
        }

        /// <summary>
        /// gets the position of the current beat.
        /// </summary>
        /// <note>
        /// shows as starting from 1 instead of starting from 0.
        /// </note>
        public static int Beat {
            get {
                return (_tick / 480) + TO_ONE_BASE - _count_beat_length;
            }
        }

        /// <summary>
        /// gets the position of current measures.
        /// </summary>
        public static int Meas {
            get {
                if (Beat <= 0) { return 0; }
                return (Beat - 1) / 4 + TO_ONE_BASE;
            }
        }

        /// <summary>
        /// provides the song name.
        /// </summary>
        public static string Name {
            get => _name; set => _name = value;
        }

        /// <summary>
        /// provides the song copyright.
        /// </summary>
        public static string Copyright {
            get => _copyright; set => _copyright = value;
        }

        /// <summary>
        /// sets the song tempo and name. 
        /// </summary>
        public static (int tempo, string name) TempoAndName {
            set {
                _name = value.name;
                _tempo = value.tempo;
            }
        }

        /// <summary>
        /// sets the beat length of the "count" pattern. 
        /// </summary>
        public static int CountBeatLength {
            set => _count_beat_length = value;
        }

        /// <summary>
        /// gets the hashset.
        /// </summary>
        public static HashSet<int> HashSet {
            get => _hashset;
        }

        /// <summary>
        /// gets the Item map.
        /// </summary>
        public static Map<int, Item16beat> ItemMap {
            get => _item_map;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool Contains {
            get => _hashset.Contains(_tick);
        }

        /// <summary>
        /// gets the current Item.
        /// </summary>
        /// <note>
        /// used in UI display.
        /// </note>
        public static Item16beat CurrentItem {
            get => _item_map[_tick];
        }

        /// <summary>
        /// gets the Track map.
        /// </summary>
        /// <note>
        /// used when converting the song to SMF.
        /// </note>
        public static Map<int, Track> TrackMap {
            get => _track_map;
        }

        /// <summary>
        /// gets the Track list.
        /// </summary>
        /// <note>
        /// used when converting the song to SMF.
        /// </note>
        public static List<Track> TrackList {
            get => _track_map.Select(x => x.Value).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <remarks>
        /// not used yet.
        /// </remarks>
        public static void IncrementTick() {
            _tick += TICK_RESOLUTION; // increments tick.
        }

        /// <summary>
        /// clears the list and the map data.
        /// </summary>
        public static void Clear() {
            _hashset.Clear();
            _item_map.Clear();
            _track_map.Clear();
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
