
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

using Meowziq.Core;

namespace Meowziq {
    /// <summary>
    /// Sanford.Multimedia.Midi を使用した Message クラス
    /// </summary>
    public static class Message {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Dictionary<int, List<ChannelMessage>> item = new Dictionary<int, List<ChannelMessage>>(); // Tick 毎の メッセージのリスト

        static HashSet<int> hashset = new HashSet<int>(); // カーソル役

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static List<ChannelMessage> GetBy(int tick) {
            tick = tick - 1; // -1 が必要
            if (!hashset.Add(tick)) {
                return null; // 既に処理した tick
            }
            if (item.ContainsKey(tick)) { // その tick が存在するかどうか
                return item[tick];
            }
            return null;
        }

        public static void Apply(int midiCh, Note note) {
            if (note.StopPre) { // このchのノート強制停止
                add(note.Tick - (Tick.Of32beat.Int32() ), new ChannelMessage(ChannelCommand.Controller, midiCh, 120));
            }
            add(note.Tick, new ChannelMessage(ChannelCommand.NoteOn, midiCh, note.Num, 127)); // ノートON
            add(note.Tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midiCh, note.Num, 0)); // ノートOFF
        }

        /// <summary>
        /// TODO: バンクセレクト
        /// </summary>
        public static void Apply(int midiCh, int programNum) {
            add(0, new ChannelMessage(ChannelCommand.ProgramChange, midiCh, programNum, 127)); // プログラムチェンジ
        }

        /// <summary>
        /// カーソルクリア
        /// </summary>
        public static void Reset() {
            item.Clear();
            hashset.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static void add(int tick, ChannelMessage channelMessage) {
            if (!item.ContainsKey(tick)) {
                var _newList = new List<ChannelMessage>();
                _newList.Add(channelMessage);
                item.Add(tick, _newList); // 新規追加
            } else {
                var _list = item[tick];
                _list.Add(channelMessage); // 追加
            }
        }
    }
}
