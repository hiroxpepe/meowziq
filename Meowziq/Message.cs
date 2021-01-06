
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

        // Tick 毎の メッセージのリスト TODO: チャンネル
        Dictionary<int, List<ChannelMessage>> item = new Dictionary<int, List<ChannelMessage>>();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Message() {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public List<ChannelMessage> GetBy(int tick) {
            tick = tick - 1; // -1 が必要
            List<ChannelMessage> _returnList;
            if (item.ContainsKey(tick)) { // その tick が存在するかどうか
                _returnList = item[tick];
                item.Remove(tick); // 重複回避の為、削除
                return _returnList;
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

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void add(int tick, ChannelMessage channelMessage) {
            if (!item.ContainsKey(tick)) {
                List<ChannelMessage> _newList = new List<ChannelMessage>();
                _newList.Add(channelMessage);
                item.Add(tick, _newList); // 新規追加
            } else {
                List<ChannelMessage> _list = item[tick];
                _list.Add(channelMessage); // 追加
            }
        }
    }
}
