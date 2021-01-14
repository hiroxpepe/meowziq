﻿
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

        static bool flag;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static　Constructor

        static Message() {
            flag = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        static bool IsPrime { // TODO: 必要？
            get => flag;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static void Apply(int tick) { // MEMO: tick が 2回くる
            if (tick == 0) {
                return; // 初回は無視:必要
            }
            // tick が1小節ごとに切り替え // TODO 外に出す か専用の HashSet
            if (tick % (Tick.Of4beat.Int32() * 4) == 0) {
                flag = change();
            }
        }

        /// <summary>
        /// 引数の tick の ChannelMessage のリストを返します
        /// </summary>
        public static List<ChannelMessage> GetBy(int tick) {
            if (flag) {
                //tick = tick - 1; // -1 が必要
                if (!Prime.HashSet.Add(tick)) {
                    return null; // 既に処理した tick なので無視する
                }
                if (Prime.Item.ContainsKey(tick)) { // その tick が存在するかどうか
                    return Prime.Item[tick];
                }
            } else {
                //tick = tick - 1; // -1 が必要
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
        /// Note を ChannelMessage として適用します
        /// </summary>
        public static void Apply(int midiCh, Note note) {
            if (flag) {
                if (note.StopPre) { // ノートが優先発音の場合
                    var _noteOffTick = note.Tick - Tick.Of32beat.Int32(); // 念のため32分音符前に停止
                    if (Prime.AllNoteOffHashsetArray[midiCh].Add(_noteOffTick)) { // MIDI ch 毎にこの tick のノート強制停止は一回のみ 
                        if (midiCh != 9) { // ドラム以外
                            add(_noteOffTick, new ChannelMessage(ChannelCommand.Controller, midiCh, 120));
                        }
                    }
                }
                add(note.Tick, new ChannelMessage(ChannelCommand.NoteOn, midiCh, note.Num, 127)); // ノートON
                add(note.Tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midiCh, note.Num, 0)); // ノートOFF
            } else {
                if (note.StopPre) { // ノートが優先発音の場合
                    var _noteOffTick = note.Tick - Tick.Of32beat.Int32(); // 念のため32分音符前に停止
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
        /// プログラムNo(音色)を ChannelMessage として適用します
        /// TODO: バンクセレクト
        /// </summary>
        public static void Apply(int midiCh, int programNum) {
            if (flag) {
                add(0, new ChannelMessage(ChannelCommand.ProgramChange, midiCh, programNum, 127)); // プログラムチェンジ
            } else {
                add(0, new ChannelMessage(ChannelCommand.ProgramChange, midiCh, programNum, 127)); // プログラムチェンジ
            }
        }

        /// <summary>
        /// 状態をリセットします
        /// </summary>
        public static void Reset() {
            if (flag) {
                Prime.Item.Clear();
                Prime.HashSet.Clear();
                Prime.AllNoteOffHashsetArray.ToList().ForEach(x => x.Clear());
            } else {
                Second.Item.Clear();
                Second.HashSet.Clear();
                Second.AllNoteOffHashsetArray.ToList().ForEach(x => x.Clear());
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// ChannelMessage をリストに追加します
        /// </summary>
        static void add(int tick, ChannelMessage channelMessage) {
            if (flag) {
                if (!Prime.Item.ContainsKey(tick)) {
                    var _newList = new List<ChannelMessage>();
                    _newList.Add(channelMessage);
                    Prime.Item.Add(tick, _newList); // 新規追加
                } else {
                    var _list = Prime.Item[tick];
                    _list.Add(channelMessage); // 追加
                }
            } else {
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
        static bool change() {
            if (flag == true) {
                return false;
            } else {
                return true;
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
        }
    }

    /// <summary>
    /// 曲がどのような状況で演奏されているかを表す情報を保持するクラス
    /// NOTE: 設定されて読み出させるだけ、これを状態を変更する目的で使わない
    /// TODO: 記述するファイル移動
    /// </summary>
    public static class Info {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static　Constructor

        static Info() {
            HashSet = new HashSet<int>(); // ※Dictionary.ContainsKey() が遅いのでその対策
            ItemDictionary = new Dictionary<int, Item>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjectives] 

        public static HashSet<int> HashSet {
            get; set;
        }

        public static Dictionary<int, Item> ItemDictionary {
            get; set;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static void Reset() {
            HashSet.Clear();
            ItemDictionary.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// どのようなキー、度数、旋法で演奏されているかを表す情報
        /// </summary>
        public class Item {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjectives] 

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
                get; set;
            }
        }
    }
}
