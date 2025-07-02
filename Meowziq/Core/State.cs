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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using static Meowziq.Env;

namespace Meowziq.Core {
    /// <summary>
    /// Represents the state of the song playback.
    /// </summary>
    /// <note>
    /// Holds information about how the song is being played.<br/>
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class State {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Const [nouns]

        const int TO_ONE_BASE = 1;
        const int BEAT_TO_MEAS = 4;

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
        /// Gets or sets the song tempo.
        /// </summary>
        public static int Tempo { get => _tempo; set => _tempo = value; }

        /// <summary>
        /// Gets or sets the current tick.
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
                    Repeat.IncrementTick();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the same tick is given.
        /// </summary>
        public static bool SameTick { get => _same_tick; }

        /// <summary>
        /// Gets the position of the current beat.
        /// </summary>
        /// <note>
        /// Shows as starting from 1 instead of starting from 0.<br/>
        /// </note>
        public static int Beat {
            get => (Repeat.Tick / NOTE_RESOLUTION) + TO_ONE_BASE - _count_beat_length;
        }

        /// <summary>
        /// Gets the position of the current measure.
        /// </summary>
        public static int Meas {
            get {
                if (Beat <= 0) { return 0; }
                return (Beat - 1) / BEAT_TO_MEAS + TO_ONE_BASE;
            }
        }

        /// <summary>
        /// Gets or sets the song name.
        /// </summary>
        public static string Name { get => _name; set => _name = value; }

        /// <summary>
        /// Gets or sets the song copyright.
        /// </summary>
        public static string Copyright { get => _copyright; set => _copyright = value; }

        /// <summary>
        /// Sets the song tempo and name.
        /// </summary>
        public static (int tempo, string name) TempoAndName {
            set { _name = value.name; _tempo = value.tempo; }
        }

        /// <summary>
        /// Gets or sets the beat length of the "count" pattern.
        /// </summary>
        public static int CountBeatLength { get => _count_beat_length; set => _count_beat_length = value; }

        /// <summary>
        /// Gets the hashset.
        /// </summary>
        public static HashSet<int> HashSet { get => _hashset; }

        /// <summary>
        /// Gets the Item map.
        /// </summary>
        public static Map<int, Item16beat> ItemMap { get => _item_map; }

        /// <summary>
        /// Gets a value indicating whether the Item of the current tick exists.
        /// </summary>
        public static bool Has { get => _hashset.Contains(Repeat.Tick); }

        /// <summary>
        /// Gets the current Item.
        /// </summary>
        /// <note>
        /// Used in UI display.<br/>
        /// </note>
        public static Item16beat CurrentItem { get => _item_map[Repeat.Tick]; }

        /// <summary>
        /// Gets the Track map.
        /// </summary>
        /// <note>
        /// Used when converting the song to SMF.<br/>
        /// </note>
        public static Map<int, Track> TrackMap { get => _track_map; }

        /// <summary>
        /// Gets the Track list.
        /// </summary>
        /// <note>
        /// Used when converting the song to SMF.<br/>
        /// </note>
        public static List<Track> TrackList { get => _track_map.Select(x => x.Value).ToList(); }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Clears the list and the map data.
        /// </summary>
        public static void Clear() {
            _hashset.Clear();
            _item_map.Clear();
            _track_map.Clear();
        }

        /// <summary>
        /// Initializes the tick.
        /// </summary>
        /// <note>
        /// Necessary when starting for _sameTick.<br/>
        /// </note>
        public static void InitTick() {
            _tick = -1;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Represents repeat information.
        /// </summary>
        public static class Repeat {

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // static Fields [nouns, noun phrases]

            static int _begin_meas, _end_meas, _begin_tick, _end_tick, _tick_counter = 0;

            static string _begin_pattern_name = string.Empty;

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // public static Properties [noun, adjective] 

            /// <summary>
            /// Sets the begin measure number of repeats.
            /// </summary>
            public static int BeginMeas {
                set {
                    _begin_meas = value;
                    _begin_tick = ((_begin_meas - 1) * NOTE_RESOLUTION * TIMES_TO_MEASURE) + (CountBeatLength * NOTE_RESOLUTION);
                }
            }

            /// <summary>
            /// Sets the end measure number of repeats.
            /// </summary>
            public static int EndMeas {
                set { 
                    _end_meas = value;
                    _end_tick = ((_end_meas - 1) * NOTE_RESOLUTION * TIMES_TO_MEASURE) + (CountBeatLength * NOTE_RESOLUTION);
                }
            }

            /// <summary>
            /// Gets the repeat tick.
            /// </summary>
            public static int Tick {
                get {
                    if (!has || (_begin_meas > meas && _end_meas < meas)) {
                        return _tick;
                    }
                    return _tick_counter;
                }
            }

            /// <summary>
            /// Gets the repeat begin tick.
            /// </summary>
            public static int BeginTick { get => _begin_tick; }

            /// <summary>
            /// Gets or sets the repeat begin pattern name.
            /// </summary>
            public static string BeginPatternName { get => _begin_pattern_name; set => _begin_pattern_name = value; }

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // public static Methods [verb]

            /// <summary>
            /// Increments the tick.
            /// </summary>
            public static void IncrementTick() {
                if (!has) { return; }
                if (_begin_meas > meas && _end_meas < meas) { return; }
                _tick_counter += TICK_INTERVAL;
                // resets when repeat length is reached.
                if (_tick_counter == _begin_tick + repeatTickLength ) {
                    _tick_counter = _begin_tick;
                }
            }

            /// <summary>
            /// Clears this tick state.
            /// </summary>
            public static void ClearTick() {
                _begin_tick = _end_tick = 0;
            }

            /// <summary>
            /// Resets the tick counter.
            /// </summary>
            public static void ResetTickCounter() {
                _tick_counter = _begin_tick - TICK_INTERVAL;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // private static Properties [noun, adjective]

            /// <summary>
            /// Gets a value indicating whether a repeat value exists.
            /// </summary>
            static bool has { get => _begin_meas >= 0 && _end_meas >= 0 && _begin_meas < _end_meas; }

            /// <summary>
            /// Gets the length of the tick in repeat.
            /// </summary>
            static int repeatTickLength { get => (_end_meas - _begin_meas) * NOTE_RESOLUTION * TIMES_TO_MEASURE; }

            /// <summary>
            /// Gets the position of the current beat.
            /// </summary>
            static int beat { get => (_tick / NOTE_RESOLUTION) + TO_ONE_BASE - _count_beat_length; }

            /// <summary>
            /// Gets the position of the current measure.
            /// </summary>
            static int meas {
                get {
                    if (beat <= 0) { return 0; }
                    return (beat - 1) / BEAT_TO_MEAS + TO_ONE_BASE;
                }
            }
        }

        /// <summary>
        /// Holds a piece of Track information for SMF output.
        /// </summary>
        public class Track {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int _midi_ch, _vol, _pan;

            string _name, _instrument;

            bool _mute;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective]

            /// <summary>
            /// Gets or sets the MIDI channel for this track.
            /// </summary>
            public int MidiCh { get => _midi_ch; set => _midi_ch = value; }

            /// <summary>
            /// Gets or sets the name of this track. Returns the name in title case.
            /// </summary>
            public string Name {
                get => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_name); 
                set => _name = value;
            }

            /// <summary>
            /// Gets or sets the instrument name for this track.
            /// </summary>
            public string Instrument { get => _instrument; set => _instrument = value; }

            /// <summary>
            /// Gets or sets the volume for this track.
            /// </summary>
            public int Vol { get => _vol; set => _vol = value; }

            /// <summary>
            /// Gets or sets the pan position for this track.
            /// </summary>
            public int Pan { get => _pan; set => _pan = value; }

            /// <summary>
            /// Gets or sets a value indicating whether this track is muted.
            /// </summary>
            public bool Mute { get => _mute; set => _mute = value; }
        }

        /// <summary>
        /// Represents information about what key, degree, and mode it is played in.
        /// </summary>
        /// <note>
        /// Created every 16 beats.<br/>
        /// </note>
        public class Item16beat {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int _tick;

            Key _key;

            Degree _degree;

            Mode _key_mode, _span_mode;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective]

            /// <summary>
            /// Gets or sets the tick position for this item.
            /// </summary>
            public int Tick { get => _tick; set => _tick = value; }

            /// <summary>
            /// Gets or sets the key for this item.
            /// </summary>
            public Key Key { get => _key; set => _key = value; }

            /// <summary>
            /// Gets or sets the degree for this item.
            /// </summary>
            public Degree Degree { get => _degree; set => _degree = value; }

            /// <summary>
            /// Gets or sets the key mode for this item.
            /// </summary>
            public Mode KeyMode { get => _key_mode; set => _key_mode = value; }

            /// <summary>
            /// Gets or sets the span mode for this item. Returns the key mode if undefined.
            /// </summary>
            public Mode SpanMode {
                get {
                    if (_span_mode == Mode.Undefined) { return _key_mode; }
                    return _span_mode;
                }
                set => _span_mode = value;
            }

            /// <summary>
            /// Gets a value indicating whether the span mode is set to automatic (undefined).
            /// </summary>
            public bool AutoMode {
                get {
                    if (_span_mode == Mode.Undefined) { return true; }
                    return false;
                }
            }
        }
    }
}
