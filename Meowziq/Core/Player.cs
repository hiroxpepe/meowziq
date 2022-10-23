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
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// player class
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Player<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _instrument_name, _type;

        int _midi_ch, _program_num;

        Song _song;

        List<Phrase> _phrase_list;

        Track _track;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Player() {
            _phrase_list = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// gets the player type.
        /// </summary>
        public string Type {
            get => _type;
            set {
                _type = value;
                _track = new(type: _type);
            }
        }

        /// <summary>
        /// sets the MidiChannel value.
        /// </summary>
        public MidiChannel MidiCh { set => _midi_ch = (int) value; }

        /// <summary>
        /// sets the Instrument value.
        /// </summary>
        public Instrument Instrument {
            set {
                _program_num = (int) value;
                _instrument_name = value.ToString();
            }
        }

        /// <summary>
        /// sets the DrumKit value.
        /// </summary>
        public DrumKit DrumKit {
            set {
                _program_num = (int) value;
                _instrument_name = value.ToString();
            }
        }

        /// <summary>
        /// sets the Song object.
        /// </summary>
        public Song Song { set => _song = value; }

        /// <summary>
        /// provides all phrase lists.
        /// </summary>
        public List<Phrase> PhraseList {
            get => _phrase_list;
            set { 
                _phrase_list = value;
                _phrase_list.ForEach(action: x => x.NoteItem = _track.NoteItem);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// builds Phrase objects.
        /// </summary>
        /// <note>
        /// + used to SMF output, called only once. <br/>
        /// </note>
        public void Build(int tick, bool smf = false) {
            // sets the tempo and song name.
            State.TempoAndName = (_song.Tempo, _song.Name); // FIXME: looks like all players have it set up?

            /// <summary>
            /// adds initial parameters for Mixer object.
            /// </summary>
            /// <note>
            /// sets instrument name in player.json.
            /// </note>
            if (tick is 0) {
                string first_pattern_name = _song.AllSection.SelectMany(x => x.AllPattern).ToList().Where(x => x.Name is not "count").First().Name;
                Mixer<T>.ApplyVaule(tick: 0, midi_ch: _midi_ch, type: Type, name: first_pattern_name, program_num: _program_num);
            }

            Locate locate = new(tick, smf);
            HashSet<string> processed_pattern_name_hashset = new();
            foreach (Section section in _song.AllSection) {
                foreach (Pattern pattern in section.AllPattern) {
                    locate.BeatCount = pattern.BeatCount;
                    pattern.AllMeas.SelectMany(selector: x => x.AllSpan).ToList().ForEach(
                        action: x => { x.Key = section.Key; x.KeyMode = section.KeyMode; } // initializes Span objects.
                    );
                    if (locate.ChangedPattarn) { // matched the playing tick and the data processing tick.
                        Log.Trace($"pattarn changed. tick: {tick} {pattern.Name} {_type}");
                    }
                    foreach (Phrase phrase in _phrase_list.Where(predicate: x => x.Name.Equals(pattern.Name))) {
                        if (smf) { // builds all phrases.
                            phrase.Build(tick: locate.HeadTick, pattern: pattern);
                        } else if (locate.NeedPhraseBuild) { // needs to build a Phrase object.
                            phrase.Build(tick: locate.HeadTick, pattern: pattern); // creates Note objects.
                            processed_pattern_name_hashset.Add(pattern.Name); // holds processed pattern names.
                            Log.Info($"Build: tick: {tick} head: {locate.HeadTick} to end: {locate.endTick} {pattern.Name} {_type}");
                            if (locate.HeadTick == State.Repeat.BeginTick && State.Repeat.BeginTick is not 0) {
                                State.Repeat.BeginPatternName = pattern.Name; // holds the pattern name of repeat begins.
                            }
                        }
                        string begin_pattern_name = State.Repeat.BeginPatternName;
                        if (pattern.Name == begin_pattern_name && processed_pattern_name_hashset.Add(begin_pattern_name) && State.Repeat.BeginTick is not 0) {
                            Pattern repeat_begin_pattern = section.AllPattern.Where(predicate: x => x.Name == State.Repeat.BeginPatternName).First();
                            phrase.Build(tick: State.Repeat.BeginTick, pattern: repeat_begin_pattern); // builds the pattern of repeat begins.
                            Log.Info($"■■■ Build: tick: {State.Repeat.BeginTick} {pattern.Name} {_type}");
                        }
                        Mixer<T>.ApplyVaule(locate.HeadTick, _midi_ch, Type, pattern.Name, _program_num); // applies value changes to Mixer: to set it before Note.
                    }
                    locate.PreviousPatternName = pattern.Name; // holds as the previous phrase name.
                    locate.NextHeadTick();
                }
            }
            /// <summary>
            /// applies Note objects.
            /// </summary>
            _track.NoteItem.SelectMany(selector: x => x.Value).ToList().ForEach(action: x => Mixer<T>.ApplyNote(tick: x.Tick, midi_ch: _midi_ch, note: x));
            /// <summary>
            /// information for SMF output, called only once.
            /// </summary>
            if (smf) {
                State.TrackMap.Add(key: _midi_ch, value: new State.Track(){ MidiCh = _midi_ch, Name = _type, Instrument = _instrument_name });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// holds state about the start tick end tick maximum tick of the Pattern.
        /// </summary>
        class Locate {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            /// <summary>
            /// current tick: absolute value.
            /// </summary>
            int _current_tick;

            /// <summary>
            /// head tick of the Pattern in processing: absolute value.
            /// </summary>
            int _head_tick;

            /// <summary>
            /// length of the Pattern in processing.
            /// </summary>
            int _length;

            /// <summary>
            /// maximum expected length of the Pattern in processing.
            /// </summary>
            int _max;

            /// <summary>
            /// name of the Pattern that last processed.
            /// </summary>
            string _previous_name;

            /// <summary>
            /// whether in SMF export mode.
            /// </summary>
            bool _smf;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Locate(int tick, bool smf = false) {
                _current_tick = tick;
                _head_tick = 0;
                _length = 0;
                _max = tick + Length.Of4beat.Int32() * 4 * Measure.Max.Int32(); // maximum pattern length.
                _previous_name = string.Empty;
                _smf = smf;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective] 

            /// <summary>
            /// provides the head tick of the Pattern in processing.
            /// </summary>
            public int HeadTick { get => _head_tick; set => _head_tick = value; }

            /// <summary>
            /// sets the number of beats in the Pattern in processing.
            /// </summary>
            public int BeatCount { set => _length = value * Length.Of4beat.Int32(); }

            /// <summary>
            /// provides the previous phrase name.
            /// </summary>
            public string PreviousPatternName { get => _previous_name; set => _previous_name = value; }

            /// <summary>
            /// whether the pattern has changed.
            /// </summary>
            public bool ChangedPattarn { get => _current_tick == HeadTick && !_smf; }

            /// <summary>
            /// whether a Phrase build is needed.
            /// </summary>
            public bool NeedPhraseBuild { get => _current_tick <= endTick && beforeMax; }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            /// <summary>
            /// moves the head tick by the length of this Pattern.
            /// </summary>
            public void NextHeadTick() {
                _head_tick += _length; // moves the Pattern start tick by the length of the Pattern.
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Properties [noun, adjective] 

            /// <summary>
            /// gets the end tick of the Pattern in processing.
            /// </summary>
            public int endTick {  get => _head_tick + _length; }

            /// <summary>
            /// whether before the maximum value.
            /// </summary>
            public bool beforeMax { get => _head_tick < _max; }
        }
    }
}
