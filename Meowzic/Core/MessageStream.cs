
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

namespace Meowzic.Core {
    public class MessageStream {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        // Tick 毎の メッセージのリスト
        Dictionary<int, List<ChannelMessage>> item = new Dictionary<int, List<ChannelMessage>>();

        Song song; // 曲データへの参照

        //Hashtable hashtable; // tick 呼び出しの重複回避用

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public MessageStream(Song song) {
            this.song = song;
            build();
            int i = 0;
            //this.hashtable = new Hashtable();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void AddTo(int tick, ChannelMessage channelMessage) {
            if (!item.ContainsKey(tick)) {
                List<ChannelMessage> _newList = new List<ChannelMessage>();
                _newList.Add(channelMessage);
                item.Add(tick, _newList); // 新規追加
            } else {
                List<ChannelMessage> _list = item[tick];
                _list.Add(channelMessage); // 追加
            }
        }

        public List<ChannelMessage> GetBy(int tick) {
            tick = tick - 1; // -1 が必要

            List<ChannelMessage> _returnList;// = new List<ChannelMessage>();
            if (item.ContainsKey(tick)) { // その tick が存在するかどうか
                _returnList = item[tick];
                item.Remove(tick); // 重複回避の為、削除
                return _returnList;
            }
            return null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        // ヘルパーとして切り出す

        private void build() { // 動的にするには？
            List<Pattern> _patternList = song.GetPattern();

            // ここからがカスタムになるはず
            int _position = -480 * _patternList[0].Bar; // 最初の拍数で調節
            foreach (Pattern pattern in _patternList) {
                int _tick = pattern.Bar * 480; // 拍設定の頭でルート音を鳴らす場合
                _position += _tick;
                int _note = Utils.GetRootNote(song.Key, pattern.Chord, pattern.Mode);
                AddTo(_position, new ChannelMessage(ChannelCommand.NoteOn, 0, _note, 127)); // ノートON
                AddTo(_position + 480, new ChannelMessage(ChannelCommand.NoteOff, 0, _note, 0)); // ノートOFF
            }

        }

    }
}
