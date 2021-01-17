
using System;
using System.Collections.Generic;
using System.Linq;
using Sanford.Multimedia.Midi;

using Meowziq.Core;

namespace Meowziq {
    /// <summary>
    /// Sanford.Multimedia.Midi を使用した Message クラス
    /// </summary>
    public static class Message {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static HashSet<int> hashSet = new HashSet<int>();

        static bool flag;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static　Constructor

        static Message() {
            flag = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// 引数の tick の ChannelMessage のリストを返します
        /// </summary>
        public static List<ChannelMessage> GetBy(int tick) {
            if (flag) { // Prime スタートで実行
                if (!Prime.HashSet.Add(tick)) {
                    return null; // 既に処理した tick なので無視する
                }
                if (Prime.Item.ContainsKey(tick)) { // その tick が存在するかどうか
                    return Prime.Item[tick];
                }
            } else {
                if (!Second.HashSet.Add(tick)) {
                    return null; // 既に処理した tick なので無視する
                }
                if (Second.Item.ContainsKey(tick)) { // その tick が存在するかどうか
                    return Second.Item[tick];
                }
            }
            return null;
        }

        /// <summary>
        /// 引数 index 値のアイテムを持つかどうかを返します
        /// </summary>
        public static bool HasNext(int idx) {
            if (flag) { // Prime スタートで実行
                return Prime.Item.Count() > idx;
            } else {
                return Second.Item.Count() > idx;
            }
        }

        /// <summary>
        /// 引数の tick を起点にして切り替え処理を行います
        /// </summary>
        public static void Apply(int tick, Action<int> load) {
            if (!hashSet.Add(tick)) {
                return; // 既に処理した tick なので無視する
            }
            // tick が1小節ごとに切り替え
            if (tick % (Length.Of4beat.Int32() * 4) == 0) {
                change();
                load(tick);
            }
        }

        /// <summary>
        /// プログラムNo(音色)を ChannelMessage として適用します TODO: バンクセレクト
        /// </summary>
        public static void Apply(int midiCh, int tick, int programNum ) {
            add(tick, new ChannelMessage(ChannelCommand.ProgramChange, midiCh, programNum, 127)); // プログラムチェンジ
        }

        /// <summary>
        /// Note を ChannelMessage として適用します
        /// </summary>
        public static void Apply(int midiCh, Note note) {
            if (!flag) {
                if (note.StopPre) { // ノートが優先発音の場合
                    var _noteOffTick = note.Tick - Length.Of32beat.Int32(); // 念のため32分音符前に停止
                    if (Prime.AllNoteOffHashsetArray[midiCh].Add(_noteOffTick)) { // MIDI ch 毎にこの tick のノート強制停止は一回のみ 
                        if (midiCh != 9) { // ドラム以外
                            add(_noteOffTick, new ChannelMessage(ChannelCommand.Controller, midiCh, 120));
                        }
                    }
                }
                add(note.Tick, new ChannelMessage(ChannelCommand.NoteOn, midiCh, note.Num, 127)); // ノートON
                add(note.Tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midiCh, note.Num, 0)); // ノートOFF
            } else { // Second スタートで実行
                if (note.StopPre) { // ノートが優先発音の場合
                    var _noteOffTick = note.Tick - Length.Of32beat.Int32(); // 念のため32分音符前に停止
                    if (Second.AllNoteOffHashsetArray[midiCh].Add(_noteOffTick)) { // MIDI ch 毎にこの tick のノート強制停止は一回のみ 
                        if (midiCh != 9) { // ドラム以外
                            add(_noteOffTick, new ChannelMessage(ChannelCommand.Controller, midiCh, 120));
                        }
                    }
                }
                add(note.Tick, new ChannelMessage(ChannelCommand.NoteOn, midiCh, note.Num, 127)); // ノートON
                add(note.Tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midiCh, note.Num, 0)); // ノートOFF
            }
        }

        /// <summary>
        /// 状態を初期化します
        /// </summary>
        public static void Clear() {
            hashSet.Clear();
            Prime.Clear();
            Second.Clear();
            Info.Clear();
            flag = true;
        }

        /// <summary>
        /// 内部フラグを反転します
        /// </summary>
        public static void Invert() {
            flag = !flag;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// ChannelMessage をリストに追加します
        /// </summary>
        static void add(int tick, ChannelMessage channelMessage) {
            if (!flag) {
                if (!Prime.Item.ContainsKey(tick)) {
                    var _newList = new List<ChannelMessage>();
                    _newList.Add(channelMessage);
                    Prime.Item.Add(tick, _newList); // 新規追加
                } else {
                    var _list = Prime.Item[tick];
                    _list.Add(channelMessage); // 追加
                }
            } else { // Second スタートで実行
                if (!Second.Item.ContainsKey(tick)) {
                    var _newList = new List<ChannelMessage>();
                    _newList.Add(channelMessage);
                    Second.Item.Add(tick, _newList); // 新規追加
                } else {
                    var _list = Second.Item[tick];
                    _list.Add(channelMessage); // 追加
                }
            }
        }

        /// <summary>
        /// 内部のデータを切り替えます
        /// </summary>
        static void change() {
            flag = !flag;
            if (flag) {
                Second.Clear();
                Info.Clear();
            } else {
                Prime.Clear();
                Info.Clear();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        static class Prime {

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            static Dictionary<int, List<ChannelMessage>> item = new Dictionary<int, List<ChannelMessage>>(); // Tick 毎の メッセージのリスト

            static HashSet<int> hashSet = new HashSet<int>(); // ※Dictionary.ContainsKey() が遅いのでその対策

            static HashSet<int>[] allNoteOffHashsetArray = new HashSet<int>[16]; // ノート強制停止用配列

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Prime() {
                allNoteOffHashsetArray = allNoteOffHashsetArray.Select(x => x = new HashSet<int>()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjectives] 

            public static Dictionary<int, List<ChannelMessage>> Item {
                get => item;
                set => item = value;
            }

            public static HashSet<int> HashSet {
                get => hashSet;
                set => hashSet = value;
            }

            public static HashSet<int>[] AllNoteOffHashsetArray {
                get => allNoteOffHashsetArray;
                set => allNoteOffHashsetArray = value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            public static void Clear() {
                item.Clear();
                hashSet.Clear();
                allNoteOffHashsetArray.ToList().ForEach(x => x.Clear());
            }
        }

        static class Second {

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            static Dictionary<int, List<ChannelMessage>> item = new Dictionary<int, List<ChannelMessage>>(); // Tick 毎の メッセージのリスト

            static HashSet<int> hashSet = new HashSet<int>(); // ※Dictionary.ContainsKey() が遅いのでその対策

            static HashSet<int>[] allNoteOffHashsetArray = new HashSet<int>[16]; // ノート強制停止用配列

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Second() {
                allNoteOffHashsetArray = allNoteOffHashsetArray.Select(x => x = new HashSet<int>()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjectives] 

            public static Dictionary<int, List<ChannelMessage>> Item {
                get => item;
                set => item = value;
            }

            public static HashSet<int> HashSet {
                get => hashSet;
                set => hashSet = value;
            }

            public static HashSet<int>[] AllNoteOffHashsetArray {
                get => allNoteOffHashsetArray;
                set => allNoteOffHashsetArray = value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            public static void Clear() {
                item.Clear();
                hashSet.Clear();
                allNoteOffHashsetArray.ToList().ForEach(x => x.Clear());
            }
        }
    }
}
