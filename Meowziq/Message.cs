
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

using Meowziq.Core;

namespace Meowziq {
    /// <summary>
    /// Sanford.Multimedia.Midi を使用した Message クラス
    /// </summary>
    public class Message {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Dictionary<int, List<ChannelMessage>> item; // Tick 毎の メッセージのリスト

        HashSet<int> hashset; // カーソル役

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Message() {
            item = new Dictionary<int, List<ChannelMessage>>();
            hashset = new HashSet<int>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public List<ChannelMessage> GetBy(int tick) {
            tick = tick - 1; // -1 が必要
            if (!hashset.Add(tick)) {
                return null; // 既に処理した tick
            }
            if (item.ContainsKey(tick)) { // その tick が存在するかどうか
                return item[tick];
            }
            return null;
        }

        public void Apply(int midiCh, Note note) {
            add(note.Tick, new ChannelMessage(ChannelCommand.NoteOn, midiCh, note.Num, 127)); // ノートON
            add(note.Tick + note.Gate, new ChannelMessage(ChannelCommand.NoteOff, midiCh, note.Num, 0)); // ノートOFF
        }

        /// <summary>
        /// TODO: バンクセレクト
        /// </summary>
        public void Apply(int midiCh, int programNum) {
            add(0, new ChannelMessage(ChannelCommand.ProgramChange, midiCh, programNum, 127)); // プログラムチェンジ
        }

        /// <summary>
        /// カーソルクリア
        /// </summary>
        public void Reset() {
            hashset.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void add(int tick, ChannelMessage channelMessage) {
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
