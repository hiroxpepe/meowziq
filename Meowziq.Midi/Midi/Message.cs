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
using System.Linq;
using Sanford.Multimedia.Midi;

using Meowziq.Core; // depends on Meowziq.Core.Note
using static Meowziq.Env;

namespace Meowziq.Midi {
    /// <summary>
    /// message class using Sanford.Multimedia.Midi
    /// </summary>
    /// <fixme>
    /// + relies on abstraction instead of ChannelMessage. <br/>
    /// </fixme>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Message {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static HashSet<int> _hashset;

        static bool _flag;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Message() {
            _hashset = new();
            _flag = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// returns the list of ChannelMessages for the argument tick.
        /// </summary>
        /// <note>
        /// + returns null if the list does not exist. <br/>
        /// + run in "Prime" start. <br/>
        /// </note>
        public static List<ChannelMessage> GetBy(int tick) {
            if (_flag) {
                return Prime.Item.Get(key: tick); // TODO: checks for problems.
            } else {
                return Second.Item.Get(key: tick); // TODO: checks for problems.
            }
        }

        /// <summary>
        /// whether has the item with the argument tick.
        /// </summary>
        /// <note>
        /// + run in "Prime" start. <br/>
        /// </note>
        public static bool Has(int tick) {
            if (_flag) {
                return Prime.Item.Select(x => x.Key).Max() > tick;
            } else {
                return Second.Item.Select(x => x.Key).Max() > tick;
            }
        }

        /// <summary>
        /// changes internal data from the argument tick.
        /// </summary>
        public static void ApplyTick(int tick, Action<int> load) {
            /// <remarks>
            /// ignore the tick as it has already been processed.
            /// </remarks>
            if (!_hashset.Add(item: tick)) {
                //return; // TODO: checks for problems.
            }
            /// <remarks>
            /// change every time the tick accumulates to N beats. 
            /// </remarks>
            /// <memo>
            /// set it to 4 can change every 1 measure. <br/>
            /// also set it to 1 can change every 1 beat. 
            /// </memo>
            if (tick % (Length.Of4beat.Int32() * LOAD_EVERY_BEAT) == 0) {
                change();
                /// <remarks>
                /// if load fails Read cache.
                /// </remarks>
                load(tick);
            }
        }

        /// <summary>
        /// applies program_num as ChannelMessage.
        /// </summary>
        /// <note>
        /// + program change. <br/>
        /// </note> 
        public static void ApplyProgramChange(int tick, int midi_ch, int program_num ) {
            add(tick: tick, message: new ChannelMessage(ChannelCommand.ProgramChange, midi_ch, program_num, 127));
        }

        /// <summary>
        /// applies volume as ChannelMessage.
        /// </summary>
        public static void ApplyVolume(int tick, int midi_ch, int volume) {
            add(tick: tick, message: new ChannelMessage(ChannelCommand.Controller, midi_ch, 7, volume));
        }

        /// <summary>
        /// applies pan as ChannelMessage.
        /// </summary>
        public static void ApplyPan(int tick, int midi_ch, Pan pan) {
            add(tick: tick, message: new ChannelMessage(ChannelCommand.Controller, midi_ch, 10, (int) pan));
        }

        /// <summary>
        /// applies mute as ChannelMessage.
        /// </summary>
        public static void ApplyMute(int tick, int midi_ch, bool mute) {
            if (!mute) {
                return;
            }
            add(tick: tick, message: new ChannelMessage(ChannelCommand.Controller, midi_ch, 7, 0));
        }

        /// <summary>
        /// applies note as ChannelMessage.
        /// </summary>
        /// <note>
        /// + run in "Second" start. <br/>
        /// </note>
        public static void ApplyNote(int tick, int midi_ch, Note note) {
            if (!_flag) {
                if (note.HasPre) { // note has priority pronunciation,
                    int note_off_tick = tick - Length.Of32beat.Int32(); // just in case, stop before the 32nd note.
                    if (Prime.AllNoteOffToAddArray[midi_ch].Add(note_off_tick)) { // forced stop for the note of tick only once per midi ch.
                        if (midi_ch != 9) { // exclude the drum midi channel.
                            add(tick: note_off_tick, message: new ChannelMessage(ChannelCommand.Controller, midi_ch, 120));
                        }
                    }
                }
                add(tick: tick, message: new ChannelMessage(ChannelCommand.NoteOn, midi_ch, note.Num, note.Velo)); // midi note on.
                add(tick: tick + note.Gate, message: new ChannelMessage(ChannelCommand.NoteOff, midi_ch, note.Num, 0)); // midi note off.
            } else {
                if (note.HasPre) { // note has priority pronunciation,
                    int note_off_tick = tick - Length.Of32beat.Int32(); // just in case, stop before the 32nd note.
                    if (Second.AllNoteOffToAddArray[midi_ch].Add(note_off_tick)) { // forced stop for the note of tick only once per midi ch.
                        if (midi_ch != 9) { // exclude the drum midi channel.
                            add(tick: note_off_tick, message: new ChannelMessage(ChannelCommand.Controller, midi_ch, 120));
                        }
                    }
                }
                add(tick: tick, message: new ChannelMessage(ChannelCommand.NoteOn, midi_ch, note.Num, note.Velo)); // midi note on.
                add(tick: tick + note.Gate, message: new ChannelMessage(ChannelCommand.NoteOff, midi_ch, note.Num, 0)); // midi note off.
            }
        }

        /// <summary>
        /// initializes the state.
        /// </summary>
        public static void Clear() {
            _hashset.Clear();
            Prime.Clear();
            Second.Clear();
            State.Clear();
            _flag = true;
        }

        /// <summary>
        /// inverts the internal flag.
        /// </summary>
        public static void Invert() {
            _flag = !_flag;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// adds a ChannelMessage to the list.
        /// </summary>
        /// <note>
        /// + run in "Second" start. <br/>
        /// </note>
        static void add(int tick, ChannelMessage message) {
            if (!_flag) {
                Prime.Item.Add(tick, message);
            } else {
                Second.Item.Add(tick, message);
            }
        }

        /// <summary>
        /// switches internal data.
        /// </summary>
        static void change() {
            _flag = !_flag;
            if (_flag) {
                Second.Clear();
                State.Clear();
            } else {
                Prime.Clear();
                State.Clear();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        static class Prime {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            /// <summary>
            /// list of messages per tick.
            /// </summary>
            static Item<ChannelMessage> _item = new();

            /// <summary>
            /// array for note forced stop.
            /// </summary>
            static HashSet<int>[] _all_note_off_to_add_array = new HashSet<int>[MIDI_TRACK_COUNT];

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Prime() {
                _all_note_off_to_add_array = _all_note_off_to_add_array.Select(selector: x => x = new()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            public static Item<ChannelMessage> Item {
                get => _item; set => _item = value;
            }

            public static HashSet<int>[] AllNoteOffToAddArray {
                get => _all_note_off_to_add_array; set => _all_note_off_to_add_array = value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            public static void Clear() {
                _item.Clear();
                _all_note_off_to_add_array.ToList().ForEach(x => x.Clear());
            }
        }

        static class Second {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            /// <summary>
            /// list of messages per tick.
            /// </summary>
            static Item<ChannelMessage> _item = new();

            /// <summary>
            /// array for note forced stop.
            /// </summary>
            static HashSet<int>[] _all_note_off_to_add_array = new HashSet<int>[MIDI_TRACK_COUNT];

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Second() {
                _all_note_off_to_add_array = _all_note_off_to_add_array.Select(selector: x => x = new()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            public static Item<ChannelMessage> Item {
                get => _item; set => _item = value;
            }

            public static HashSet<int>[] AllNoteOffToAddArray {
                get => _all_note_off_to_add_array; set => _all_note_off_to_add_array = value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            public static void Clear() {
                _item.Clear();
                _all_note_off_to_add_array.ToList().ForEach(x => x.Clear());
            }
        }
    }
}
