
using System;
using System.Collections.Generic;
using System.Linq;
using Sanford.Multimedia.Midi;

using Meowziq.Core; // depends on Meowziq.Core.Note

namespace Meowziq.Midi {
    /// <summary>
    /// message class using Sanford.Multimedia.Midi
    /// </summary>
    /// <fixme>
    /// relies on abstraction instead of ChannelMessage.
    /// </fixme>
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
        /// <remarks>
        /// returns null if the list does not exist.<br/>
        /// run in "Prime" start.
        /// </remarks>
        public static List<ChannelMessage> GetBy(int tick) {
            if (_flag) {
                return Prime.Item.GetOnce(tick);
            } else {
                return Second.Item.GetOnce(tick);
            }
        }

        /// <summary>
        /// whether has the item with the argument tick.
        /// </summary>
        /// <remarks>
        /// run in "Prime" start.
        /// </remarks>
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
            if (!_hashset.Add(tick)) {
                return;
            }
            /// <remarks>
            /// change every time the tick accumulates to 2 beats. 
            /// </remarks>
            /// <memo>
            /// set it to 4 can change every 1 bar. <br/>
            /// also set it to 1 can change every 1 beat. 
            /// </memo>
            if (tick % (Length.Of4beat.Int32() * 2) == 0) {
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
        /// <remarks>
        /// program change.
        /// </remarks> 
        public static void ApplyProgramChange(int tick, int midi_ch, int program_num ) {
            add(tick, new ChannelMessage(ChannelCommand.ProgramChange, midi_ch, program_num, 127));
        }

        /// <summary>
        /// applies volume as ChannelMessage.
        /// </summary>
        public static void ApplyVolume(int tick, int midi_ch, int volume) {
            add(tick, new ChannelMessage(ChannelCommand.Controller, midi_ch, 7, volume));
        }

        /// <summary>
        /// applies pan as ChannelMessage.
        /// </summary>
        public static void ApplyPan(int tick, int midi_ch, Pan pan) {
            add(tick, new ChannelMessage(ChannelCommand.Controller, midi_ch, 10, (int) pan));
        }

        /// <summary>
        /// applies mute as ChannelMessage.
        /// </summary>
        public static void ApplyMute(int tick, int midi_ch, bool mute) {
            if (!mute) {
                return;
            }
            add(tick, new ChannelMessage(ChannelCommand.Controller, midi_ch, 7, 0));
        }

        /// <summary>
        /// applies note as ChannelMessage.
        /// </summary>
        /// <remarks>
        /// run in "Second" start.
        /// </remarks>
        public static void ApplyNote(int tick, int midi_ch, Note note) { // TODO: tick を使用する
            if (!_flag) {
                if (note.HasPre) { // ノートが優先発音の場合
                    var note_off_tick = tick - Length.Of32beat.Int32(); // 念のため32分音符前に停止
                    if (Prime.AllNoteOffToAddArray[midi_ch].Add(note_off_tick)) { // MIDI ch 毎にこの tick のノート強制停止は一回のみ 
                        if (midi_ch != 9) { // exclude the drum midi channel.
                            add(note_off_tick, new ChannelMessage(ChannelCommand.Controller, midi_ch, 120));
                        }
                    }
                }
                add(tick, new ChannelMessage(ChannelCommand.NoteOn, midi_ch, note.Num, note.Velo)); // midi note on.
                add(tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midi_ch, note.Num, 0)); // midi note off.
            } else {
                if (note.HasPre) { // ノートが優先発音の場合
                    var note_off_tick = tick - Length.Of32beat.Int32(); // 念のため32分音符前に停止
                    if (Second.AllNoteOffToAddArray[midi_ch].Add(note_off_tick)) { // MIDI ch 毎にこの tick のノート強制停止は一回のみ 
                        if (midi_ch != 9) { // exclude the drum midi channel.
                            add(note_off_tick, new ChannelMessage(ChannelCommand.Controller, midi_ch, 120));
                        }
                    }
                }
                add(tick, new ChannelMessage(ChannelCommand.NoteOn, midi_ch, note.Num, note.Velo)); // midi note on.
                add(tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midi_ch, note.Num, 0)); // midi note off.
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
        /// adds a channel_message to the list.
        /// </summary>
        /// <remarks>
        /// run in "Second" start.
        /// </remarks>
        static void add(int tick, ChannelMessage channel_message) {
            if (!_flag) {
                Prime.Item.Add(tick, channel_message);
            } else {
                Second.Item.Add(tick, channel_message);
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
            static HashSet<int>[] _all_note_off_to_add_array = new HashSet<int>[16];

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Prime() {
                _all_note_off_to_add_array = _all_note_off_to_add_array.Select(x => x = new HashSet<int>()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            public static Item<ChannelMessage> Item {
                get => _item;
                set => _item = value;
            }

            public static HashSet<int>[] AllNoteOffToAddArray {
                get => _all_note_off_to_add_array;
                set => _all_note_off_to_add_array = value;
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
            static HashSet<int>[] _all_note_off_to_add_array = new HashSet<int>[16];

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Second() {
                _all_note_off_to_add_array = _all_note_off_to_add_array.Select(x => x = new HashSet<int>()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            public static Item<ChannelMessage> Item {
                get => _item;
                set => _item = value;
            }

            public static HashSet<int>[] AllNoteOffToAddArray {
                get => _all_note_off_to_add_array;
                set => _all_note_off_to_add_array = value;
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
