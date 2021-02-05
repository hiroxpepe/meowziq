
using System;
using System.Collections.Generic;
using System.Linq;
using Sanford.Multimedia.Midi;

using Meowziq.Core; // Core.Note に依存

namespace Meowziq.Midi {
    /// <summary>
    /// Sanford.Multimedia.Midi を使用した Message クラス
    /// </summary>
    public static class Message {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static HashSet<int> hashSet = new HashSet<int>();

        static bool flag;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Message() {
            flag = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// 引数の tick の ChannelMessage のリストを返します
        /// NOTE: 引数 tick の ChannelMessage のリストが存在しなければ null を返します
        /// </summary>
        public static List<ChannelMessage> GetBy(int tick) {
            if (flag) { // Prime スタートで実行
                return Prime.Item.GetOnce(tick);
            } else {
                return Second.Item.GetOnce(tick);
            }
        }

        /// <summary>
        /// 引数 tick のアイテムを持つかどうかを返します
        /// </summary>
        public static bool Has(int tick) {
            if (flag) { // Prime スタートで実行
                return Prime.Item.Select(x => x.Key).Max() > tick;
            } else {
                return Second.Item.Select(x => x.Key).Max() > tick;
            }
        }

        /// <summary>
        /// 引数の tick を起点にして切り替え処理を行います
        /// </summary>
        public static void ApplyTick(int tick, Action<int> load) {
            if (!hashSet.Add(tick)) {
                return; // 既に処理した tick なので無視する
            }
            // tick が2拍ごとに切り替え MEMO: * 4 にすれば 1小節毎、* 1 にすれば 1拍毎に切り替えれる
            if (tick % (Length.Of4beat.Int32() * 2) == 0) {
                change();
                load(tick); // load に失敗したらキャッシュを読む
            }
        }

        /// <summary>
        /// プログラムNo(音色)を ChannelMessage として適用します
        /// </summary>
        public static void ApplyProgramChange(int tick, int midiCh, int programNum ) {
            add(tick, new ChannelMessage(ChannelCommand.ProgramChange, midiCh, programNum, 127)); // プログラムチェンジ
        }

        /// <summary>
        /// ボリュームを ChannelMessage として適用します
        /// </summary>
        public static void ApplyVolume(int tick, int midiCh, int volume) {
            add(tick, new ChannelMessage(ChannelCommand.Controller, midiCh, 7, volume));
        }

        /// <summary>
        /// PAN (パン)を ChannelMessage として適用します
        /// </summary>
        public static void ApplyPan(int tick, int midiCh, Pan pan) {
            add(tick, new ChannelMessage(ChannelCommand.Controller, midiCh, 10, (int) pan));
        }

        /// <summary>
        /// ミュートを ChannelMessage として適用します
        /// </summary>
        public static void ApplyMute(int tick, int midiCh, bool mute) {
            if (!mute) {
                return;
            }
            add(tick, new ChannelMessage(ChannelCommand.Controller, midiCh, 7, 0));
        }

        /// <summary>
        /// Note を ChannelMessage として適用します
        /// </summary>
        public static void ApplyNote(int tick, int midiCh, Note note) { // TODO: tick を使用する
            if (!flag) {
                if (note.HasPre) { // ノートが優先発音の場合
                    var _noteOffTick = tick - Length.Of32beat.Int32(); // 念のため32分音符前に停止
                    if (Prime.AllNoteOffToAddArray[midiCh].Add(_noteOffTick)) { // MIDI ch 毎にこの tick のノート強制停止は一回のみ 
                        if (midiCh != 9) { // ドラム以外
                            add(_noteOffTick, new ChannelMessage(ChannelCommand.Controller, midiCh, 120));
                        }
                    }
                }
                add(tick, new ChannelMessage(ChannelCommand.NoteOn, midiCh, note.Num, note.Velo/*104*/)); // ノートON
                add(tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midiCh, note.Num, 0)); // ノートOFF ※この位置
            } else { // Second スタートで実行
                if (note.HasPre) { // ノートが優先発音の場合
                    var _noteOffTick = tick - Length.Of32beat.Int32(); // 念のため32分音符前に停止
                    if (Second.AllNoteOffToAddArray[midiCh].Add(_noteOffTick)) { // MIDI ch 毎にこの tick のノート強制停止は一回のみ 
                        if (midiCh != 9) { // ドラム以外
                            add(_noteOffTick, new ChannelMessage(ChannelCommand.Controller, midiCh, 120));
                        }
                    }
                }
                add(tick, new ChannelMessage(ChannelCommand.NoteOn, midiCh, note.Num, note.Velo/*104*/)); // ノートON
                add(tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midiCh, note.Num, 0)); // ノートOFF ※この位置
            }
        }

        /// <summary>
        /// 状態を初期化します
        /// </summary>
        public static void Clear() {
            hashSet.Clear();
            Prime.Clear();
            Second.Clear();
            State.Clear();
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
                Prime.Item.Add(tick, channelMessage);
            } else { // Second スタートで実行
                Second.Item.Add(tick, channelMessage);
            }
        }

        /// <summary>
        /// 内部のデータを切り替えます
        /// </summary>
        static void change() {
            flag = !flag;
            if (flag) {
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

            static Item<ChannelMessage> item = new Item<ChannelMessage>(); // Tick 毎の メッセージのリスト

            static HashSet<int>[] allNoteOffToAddArray = new HashSet<int>[16]; // ノート強制停止用配列

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Prime() {
                allNoteOffToAddArray = allNoteOffToAddArray.Select(x => x = new HashSet<int>()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            public static Item<ChannelMessage> Item {
                get => item;
                set => item = value;
            }

            public static HashSet<int>[] AllNoteOffToAddArray {
                get => allNoteOffToAddArray;
                set => allNoteOffToAddArray = value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            public static void Clear() {
                item.Clear();
                allNoteOffToAddArray.ToList().ForEach(x => x.Clear());
            }
        }

        static class Second {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            static Item<ChannelMessage> item = new Item<ChannelMessage>(); // Tick 毎の メッセージのリスト

            static HashSet<int>[] allNoteOffToAddArray = new HashSet<int>[16]; // ノート強制停止用配列

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Second() {
                allNoteOffToAddArray = allNoteOffToAddArray.Select(x => x = new HashSet<int>()).ToArray();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            public static Item<ChannelMessage> Item {
                get => item;
                set => item = value;
            }

            public static HashSet<int>[] AllNoteOffToAddArray {
                get => allNoteOffToAddArray;
                set => allNoteOffToAddArray = value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            public static void Clear() {
                item.Clear();
                allNoteOffToAddArray.ToList().ForEach(x => x.Clear());
            }
        }
    }
}
