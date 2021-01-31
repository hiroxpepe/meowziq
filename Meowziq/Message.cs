
using System;
using System.Collections.Generic;
using System.Linq;
using Sanford.Multimedia.Midi;

using Meowziq.Core;

namespace Meowziq {
    /// <summary>
    /// SMF 出力用 Track オブジェクト達を保持するクラス
    /// </summary>
    public static class Multi {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Dictionary<int, Track> trackDict;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Multi() {
            trackDict = new Dictionary<int, Track>();
            Enumerable.Range(0, 15).ToList().ForEach(x => trackDict.Add(x, new Track()));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective]

        public static List<Track> List {
            get => trackDict.Select(x => x.Value).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static Track Get(int index) {
            return trackDict[index];
        }
    }

    /// <summary>
    /// Item クラス
    /// NOTE: Add された Value を一度だけ取り出す Dictionary
    /// </summary>
    public class Item<TValue> : Dictionary<int, List<TValue>> {

        HashSet<int> toAddHashSet; // 追加した判定

        HashSet<int> takeOutHashSet; // 取り出した判定

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Item() {
            toAddHashSet = new HashSet<int>(); // 追加した判定
            takeOutHashSet = new HashSet<int>(); // 取り出した判定
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// key の List に value を追加します
        /// </summary>
        public void Add(int key, TValue value) {
            if (!ContainsKey(key)) { // key がなければ
                var _newList = new List<TValue>(); // List を新規作成
                _newList.Add(value); // List に value を追加
                Add(key, _newList); // Item に新規追加
            } else {
                var _list = this[key];　// Item から key の List を取得
                _list.Add(value); // List に value を追加
            }
        }

        /// <summary>
        /// key の Value を返します
        /// </summary>
        public List<TValue> Get(int key) {
            if (!ContainsKey(key)) {
                return null; // key に存在しない場合は null を返す
            }
            return this[key]; // 初回なら key の value を返す
        }

        /// <summary>
        /// 1回だけ key の Value を返します
        /// </summary>
        public List<TValue> GetOnce(int key) {
            if (!takeOutHashSet.Add(key)) { // 取り出したことを判定する hashSet で false なら
                return null; // 既に1回取り出したので null を返す
            }
            if (!ContainsKey(key)) {
                return null; // key に存在しない場合は null を返す
            }
            return this[key]; // 初回なら key の value を返す // TODO: ソート： ドラム優先、メロ後：ソートパラメータが必要？
        }

        /// <summary>
        /// key の value を置き換えます
        /// </summary>
        public void SetBy(int key, List<TValue> value) {
            this.Remove(key);
            this.Add(key, value);
        }

        /// <summary>
        /// key と value を追加します
        /// TODO: 重複 key は？
        /// </summary>
        public new void Add(int key, List<TValue> value) {
            toAddHashSet.Add(key); // key を追加したフラグを追加
            base.Add(key, value);
        }

        /// <summary>
        /// key があれば true、なければ false を返します
        /// </summary>
        public new bool ContainsKey(int key) {
            return toAddHashSet.Contains(key); // HashSet.Contains() は高速
        }

        /// <summary>
        /// 内容を初期化します
        /// </summary>
        public new void Clear() {
            toAddHashSet.Clear();
            takeOutHashSet.Clear();
            base.Clear();
        }
    }

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
                load(tick); // TODO: load に失敗したらキャッシュを読む
            }
        }

        /// <summary>
        /// プログラムNo(音色)を ChannelMessage として適用します
        /// </summary>
        public static void ApplyProgramChange(int midiCh, int tick, int programNum ) {
            add(tick, new ChannelMessage(ChannelCommand.ProgramChange, midiCh, programNum, 127)); // プログラムチェンジ
        }

        /// <summary>
        /// ボリュームを ChannelMessage として適用します
        /// </summary>
        public static void ApplyVolume(int midiCh, int tick, int volume) {
            add(tick, new ChannelMessage(ChannelCommand.Controller, midiCh, 7, volume));
        }

        /// <summary>
        /// PAN (パン)を ChannelMessage として適用します
        /// </summary>
        public static void ApplyPan(int midiCh, int tick, Pan pan) {
            add(tick, new ChannelMessage(ChannelCommand.Controller, midiCh, 10, (int) pan));
        }

        /// <summary>
        /// ミュートを ChannelMessage として適用します
        /// </summary>
        public static void ApplyMute(int midiCh, int tick, bool mute) {
            if (!mute) {
                return;
            }
            add(tick, new ChannelMessage(ChannelCommand.Controller, midiCh, 7, 0));
        }

        /// <summary>
        /// Note を ChannelMessage として適用します
        /// </summary>
        public static void ApplyNote(int midiCh, Note note) {
            if (!flag) {
                if (note.HasPre) { // ノートが優先発音の場合
                    var _noteOffTick = note.Tick - Length.Of32beat.Int32(); // 念のため32分音符前に停止
                    if (Prime.AllNoteOffToAddArray[midiCh].Add(_noteOffTick)) { // MIDI ch 毎にこの tick のノート強制停止は一回のみ 
                        if (midiCh != 9) { // ドラム以外
                            add(_noteOffTick, new ChannelMessage(ChannelCommand.Controller, midiCh, 120));
                        }
                    }
                }
                add(note.Tick, new ChannelMessage(ChannelCommand.NoteOn, midiCh, note.Num, 127)); // ノートON
                add(note.Tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midiCh, note.Num, 0)); // ノートOFF ※この位置
            } else { // Second スタートで実行
                if (note.HasPre) { // ノートが優先発音の場合
                    var _noteOffTick = note.Tick - Length.Of32beat.Int32(); // 念のため32分音符前に停止
                    if (Second.AllNoteOffToAddArray[midiCh].Add(_noteOffTick)) { // MIDI ch 毎にこの tick のノート強制停止は一回のみ 
                        if (midiCh != 9) { // ドラム以外
                            add(_noteOffTick, new ChannelMessage(ChannelCommand.Controller, midiCh, 120));
                        }
                    }
                }
                add(note.Tick, new ChannelMessage(ChannelCommand.NoteOn, midiCh, note.Num, 127)); // ノートON
                add(note.Tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midiCh, note.Num, 0)); // ノートOFF ※この位置
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
