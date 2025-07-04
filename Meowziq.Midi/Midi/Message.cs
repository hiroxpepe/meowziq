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
    /// Provides static methods for handling MIDI messages using Sanford.Multimedia.Midi.
    /// </summary>
    /// <remarks>
    /// <item>Relies on abstraction instead of ChannelMessage.</item>
    /// <item>Manages per-tick ChannelMessages, program changes, volume, pan, mute, and note events.</item>
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Message {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// Stores the set of processed ticks.
        /// </summary>
        static HashSet<int> _hashset;

        /// <summary>
        /// Stores the internal flag for switching data.
        /// </summary>
        static bool _flag;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        /// <summary>
        /// Initializes static fields of the <see cref="Message"/> class.
        /// </summary>
        static Message() {
            _hashset = new();
            _flag = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Gets the list of ChannelMessages for the specified tick.
        /// </summary>
        /// <param name="tick">Tick value to retrieve messages for.</param>
        /// <returns>List of ChannelMessages for the tick, or null if not found.</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>Returns null if the list does not exist.</description></item>
        /// <item><description>Runs in "Prime" start.</description></item>
        /// </list>
        /// </remarks>
        public static List<ChannelMessage> GetBy(int tick) {
            if (_flag) {
                return Prime.Item.Get(key: tick); // TODO: checks for problems.
            } else {
                return Second.Item.Get(key: tick); // TODO: checks for problems.
            }
        }

        /// <summary>
        /// Determines whether the item with the specified tick exists.
        /// </summary>
        /// <param name="tick">Tick value to check for existence.</param>
        /// <returns>True if the item exists; otherwise, false.</returns>
        /// <remarks>
        /// <item>Runs in "Prime" start.</item>
        /// </remarks>
        public static bool Has(int tick) {
            if (_flag) {
                return Prime.Item.Select(selector: x => x.Key).Max() > tick;
            } else {
                return Second.Item.Select(selector: x => x.Key).Max() > tick;
            }
        }

        /// <summary>
        /// Changes internal data from the specified tick and loads cache if needed.
        /// </summary>
        /// <param name="tick">Tick value to apply changes from.</param>
        /// <param name="load">Action to load cache if needed.</param>
        /// <remarks>
        /// <item>Ignores the tick if already processed.</item>
        /// <item>Changes every time the tick accumulates to N beats.</item>
        /// <item>Loads cache if load fails.</item>
        /// </remarks>
        public static void ApplyTick(int tick, Action<int> load) {
            /// <remarks>
            /// Ignores the tick as it has already been processed.
            /// </remarks>
            if (!_hashset.Add(item: tick)) {
                //return; // TODO: checks for problems.
            }
            /// <remarks>
            /// Changes every time the tick accumulates to N beats.
            /// </remarks>
            /// <memo>
            /// Sets it to 4 to change every 1 measure. <br/>
            /// Also sets it to 1 to change every 1 beat. 
            /// </memo>
            if (tick % (Length.Of4beat.Int32() * LOAD_EVERY_BEAT) == 0) {
                change();
                /// <remarks>
                /// Loads cache if load fails.
                /// </remarks>
                load(tick);
            }
        }

        /// <summary>
        /// Applies the program number as a ChannelMessage.
        /// </summary>
        /// <param name="tick">Tick value to apply the program change at.</param>
        /// <param name="midi_ch">MIDI channel to apply the program change to.</param>
        /// <param name="program_num">Program number to set.</param>
        /// <remarks>
        /// <item>Program change.</item>
        /// </remarks>
        public static void ApplyProgramChange(int tick, int midi_ch, int program_num ) {
            add(tick: tick, message: new ChannelMessage(command: ChannelCommand.ProgramChange, midi_ch, data1: program_num, data2: 127));
        }

        /// <summary>
        /// Applies the volume as a ChannelMessage.
        /// </summary>
        /// <param name="tick">Tick value to apply the volume at.</param>
        /// <param name="midi_ch">MIDI channel to apply the volume to.</param>
        /// <param name="volume">Volume value to set.</param>
        public static void ApplyVolume(int tick, int midi_ch, int volume) {
            add(tick, message: new ChannelMessage(command: ChannelCommand.Controller, midi_ch, data1: 7, data2: volume));
        }

        /// <summary>
        /// Applies the pan as a ChannelMessage.
        /// </summary>
        /// <param name="tick">Tick value to apply the pan at.</param>
        /// <param name="midi_ch">MIDI channel to apply the pan to.</param>
        /// <param name="pan">Pan value to set.</param>
        public static void ApplyPan(int tick, int midi_ch, Pan pan) {
            add(tick, message: new ChannelMessage(command: ChannelCommand.Controller, midi_ch, data1: 10, data2: (int) pan));
        }

        /// <summary>
        /// Applies mute as a ChannelMessage.
        /// </summary>
        /// <param name="tick">Tick value to apply mute at.</param>
        /// <param name="midi_ch">MIDI channel to apply mute to.</param>
        /// <param name="mute">Indicates whether to mute (true) or not (false).</param>
        public static void ApplyMute(int tick, int midi_ch, bool mute) {
            if (!mute) {
                return;
            }
            add(tick, message: new ChannelMessage(command: ChannelCommand.Controller, midi_ch, data1: 7, data2: 0));
        }

        /// <summary>
        /// Applies the note as a ChannelMessage.
        /// </summary>
        /// <param name="tick">Tick value to apply the note at.</param>
        /// <param name="midi_ch">MIDI channel to apply the note to.</param>
        /// <param name="note">Note object to apply.</param>
        /// <remarks>
        /// <item>Runs in "Second" start.</item>
        /// </remarks>
        public static void ApplyNote(int tick, int midi_ch, Note note) {
            // Applies a note as ChannelMessage, handling forced note-off and drum channel exclusion.
            if (!_flag) {
                if (note.HasPre) { // Note has priority pronunciation.
                    int note_off_tick = tick - Length.Of32beat.Int32(); // Stops before the 32nd note as a precaution.
                    if (Prime.AllNoteOffToAddArray[midi_ch].Add(item: note_off_tick)) { // Forces stop for the note of tick only once per MIDI channel.
                        if (midi_ch != 9) { // Excludes the drum MIDI channel.
                            add(tick: note_off_tick, message: new ChannelMessage(command: ChannelCommand.Controller, midi_ch, data1: 120));
                        }
                    }
                }
                add(tick, message: new ChannelMessage(command: ChannelCommand.NoteOn, midi_ch, data1: note.Num, data2: note.Velo)); // MIDI note on.
                add(tick + note.Gate, message: new ChannelMessage(command: ChannelCommand.NoteOff, midi_ch, data1: note.Num, data2: 0)); // MIDI note off.
            } else {
                if (note.HasPre) { // Note has priority pronunciation.
                    int note_off_tick = tick - Length.Of32beat.Int32(); // Stops before the 32nd note as a precaution.
                    if (Second.AllNoteOffToAddArray[midi_ch].Add(item: note_off_tick)) { // Forces stop for the note of tick only once per MIDI channel.
                        if (midi_ch != 9) { // Excludes the drum MIDI channel.
                            add(tick: note_off_tick, message: new ChannelMessage(command: ChannelCommand.Controller, midi_ch, data1: 120));
                        }
                    }
                }
                add(tick, message: new ChannelMessage(command: ChannelCommand.NoteOn, midi_ch, data1: note.Num, data2: note.Velo)); // MIDI note on.
                add(tick + note.Gate, message: new ChannelMessage(command: ChannelCommand.NoteOff, midi_ch, data1: note.Num, data2: 0)); // MIDI note off.
            }
        }

        /// <summary>
        /// Initializes the state and clears all internal data.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>Clears all internal states.</description></item>
        /// <item><description>Resets the flag.</description></item>
        /// </list>
        /// </remarks>
        public static void Clear() {
            _hashset.Clear();
            Prime.Clear();
            Second.Clear();
            State.Clear();
            State.InitTick(); // Necessary when starting for State.SameTick.
            _flag = true;
        }

        /// <summary>
        /// Inverts the internal flag.
        /// </summary>
        public static void Invert() {
            _flag = !_flag;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// Adds a ChannelMessage to the list for the specified tick.
        /// </summary>
        /// <param name="tick">Tick value to add the message at.</param>
        /// <param name="message">ChannelMessage to add.</param>
        /// <remarks>
        /// <item>Runs in "Second" start.</item>
        /// </remarks>
        static void add(int tick, ChannelMessage message) {
            // Adds the ChannelMessage to the appropriate list depending on the internal flag.
            if (!_flag) {
                Prime.Item.Add(key: tick, value: message);
            } else {
                Second.Item.Add(key: tick, value: message);
            }
        }

        /// <summary>
        /// Switches internal data and clears the corresponding data.
        /// </summary>
        static void change() {
            // Inverts the internal flag and clears the corresponding data.
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

        /// <summary>
        /// Provides the primary storage for per-tick ChannelMessages and note-off management.
        /// </summary>
        /// <remarks>
        /// <item>Used for main MIDI message storage and forced note-off management.</item>
        /// </remarks>
        static class Prime {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            /// <summary>
            /// Stores the list of messages per tick.
            /// </summary>
            static Item<ChannelMessage> _item = new();

            /// <summary>
            /// Stores the array for note forced stop per MIDI track.
            /// </summary>
            static HashSet<int>[] _all_note_off_to_add_array = new HashSet<int>[MIDI_TRACK_COUNT];

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            /// <summary>
            /// Initializes static fields of the <see cref="Prime"/> class.
            /// </summary>
            static Prime() {
                // Initializes each element of the array for note forced stop.
                _all_note_off_to_add_array = _all_note_off_to_add_array.Select(x => new HashSet<int>()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            /// <summary>
            /// Gets or sets the item list of messages per tick.
            /// </summary>
            public static Item<ChannelMessage> Item { get => _item; set => _item = value; }

            /// <summary>
            /// Gets or sets the array for note forced stop per MIDI track.
            /// </summary>
            public static HashSet<int>[] AllNoteOffToAddArray { get => _all_note_off_to_add_array; set => _all_note_off_to_add_array = value; }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            /// <summary>
            /// Clears the item list and all note off arrays.
            /// </summary>
            public static void Clear() {
                _item.Clear();
                _all_note_off_to_add_array.ToList().ForEach(action: x => x.Clear());
            }
        }

        /// <summary>
        /// Provides the secondary storage for per-tick ChannelMessages and note-off management.
        /// </summary>
        /// <remarks>
        /// <item>Used for alternate MIDI message storage and forced note-off management.</item>
        /// </remarks>
        static class Second {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            /// <summary>
            /// Stores the list of messages per tick.
            /// </summary>
            static Item<ChannelMessage> _item = new();

            /// <summary>
            /// Stores the array for note forced stop per MIDI track.
            /// </summary>
            static HashSet<int>[] _all_note_off_to_add_array = new HashSet<int>[MIDI_TRACK_COUNT];

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            /// <summary>
            /// Initializes static fields of the <see cref="Second"/> class.
            /// </summary>
            static Second() {
                // Initializes each element of the array for note forced stop.
                _all_note_off_to_add_array = _all_note_off_to_add_array.Select(x => new HashSet<int>()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            /// <summary>
            /// Gets or sets the item list of messages per tick.
            /// </summary>
            public static Item<ChannelMessage> Item { get => _item; set => _item = value; }

            /// <summary>
            /// Gets or sets the array for note forced stop per MIDI track.
            /// </summary>
            public static HashSet<int>[] AllNoteOffToAddArray { get => _all_note_off_to_add_array; set => _all_note_off_to_add_array = value; }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            /// <summary>
            /// Clears the item list and all note off arrays.
            /// </summary>
            public static void Clear() {
                _item.Clear();
                _all_note_off_to_add_array.ToList().ForEach(action: x => x.Clear());
            }
        }
    }
}
