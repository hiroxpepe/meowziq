
using System.Collections.Generic;
using Sanford.Multimedia.Midi;

using Meowzic.Core;

namespace Meowzic {
    /// <summary>
    /// Sanford.Multimedia.Midi を使用した Message クラス
    /// </summary>
    public class Message {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        // Tick 毎の メッセージのリスト TODO: チャンネル
        Dictionary<int, List<ChannelMessage>> item = new Dictionary<int, List<ChannelMessage>>();

        Song song; // 曲データへの参照 TODO: 外だし

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Message(Song song) {
            this.song = song;
            build();
            int i = 0;
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

        public void Apply(Note note) {
            // ノートON
            // ノートOFF
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        public void add(int tick, ChannelMessage channelMessage) {
            if (!item.ContainsKey(tick)) {
                List<ChannelMessage> _newList = new List<ChannelMessage>();
                _newList.Add(channelMessage);
                item.Add(tick, _newList); // 新規追加
            } else {
                List<ChannelMessage> _list = item[tick];
                _list.Add(channelMessage); // 追加
            }
        }

        // ヘルパーとして切り出す

        void build() {
            int _position = 0; //-480 * 4; // 1小節ぶん
            List<Part> _partList = song.GetAllPart();
            foreach (Part _part in _partList) {

                // ここでパターン(コード)にどのフレーズを対応させていくか
                    // フレーズ割り当てが動的に出来る
                // midiチャンネルがプレイヤー(音色)
                // var bass = new Player();

                beat8OnBase(_position, _part);
                beat16Arpeggio2(_position, _part, Arpeggio.Up);

                int _tick = _part.Beat * 480;
                _position += _tick; // パターンの長さ分ポジションを移動する
            }
        }

        void beat4OnRoot(int position, Part part) {
            int _4BeatCount = part.Beat; // 4ビート
            int _note = Utils.GetRootNote(song.Key, part.Degree, part.Mode);
            for (int i = 0; i < _4BeatCount; i++) {
                add(position + (480 * i), new ChannelMessage(ChannelCommand.NoteOn, 0, _note, 127)); // ノートON
                add(position + (480 * i) + 480, new ChannelMessage(ChannelCommand.NoteOff, 0, _note, 0)); // ノートOFF
            }
        }

        void beat8OnRoot(int position, Part part) {
            int _8BeatCount = part.Beat * 2; // 8ビート
            int _note = Utils.GetRootNote(song.Key, part.Degree, part.Mode);
            for (int i = 0; i < _8BeatCount; i++) {
                add(position + (240 * i), new ChannelMessage(ChannelCommand.NoteOn, 0, _note, 127)); // ノートON
                add(position + (240 * i) + 240, new ChannelMessage(ChannelCommand.NoteOff, 0, _note, 0)); // ノートOFF
            }
        }

        void beat8OnBase(int position, Part part) {
            int _8BeatCount = part.Beat * 2; // 8ビート
            int _note = Utils.GetRootNote(song.Key, part.Degree, part.Mode);
            for (int i = 0; i < _8BeatCount; i++) {
                add(position + (240 * i), new ChannelMessage(ChannelCommand.NoteOn, 0, (_note - 24), 127)); // ノートON
                add(position + (240 * i) + 240, new ChannelMessage(ChannelCommand.NoteOff, 0, (_note - 24), 0)); // ノートOFF
            }
        }

        void beat16OnRoot(int position, Part part) {
            int _16BeatCount = part.Beat * 4; // 16ビート
            int _note = Utils.GetRootNote(song.Key, part.Degree, part.Mode);
            for (int i = 0; i < _16BeatCount; i++) {
                add(position + (120 * i), new ChannelMessage(ChannelCommand.NoteOn, 0, _note, 127)); // ノートON
                add(position + (120 * i) + 120, new ChannelMessage(ChannelCommand.NoteOff, 0, _note, 0)); // ノートOFF
            }
        }

        /// <summary>
        /// キーのモードスケール ※あまり有効ではない
        /// </summary>
        void beat16Arpeggio1(int position, Part part, Arpeggio arpeggio) {
            int _16BeatCount = part.Beat * 4; // 16ビート
            for (int i = 0; i < _16BeatCount; i++) {
                int _note = Utils.GetNote(song.Key, part.Mode, arpeggio);
                add(position + (120 * i), new ChannelMessage(ChannelCommand.NoteOn, 0, _note, 127)); // ノートON
                add(position + (120 * i) + 120, new ChannelMessage(ChannelCommand.NoteOff, 0, _note, 0)); // ノートOFF
            }
        }

        void beat16Arpeggio2(int position, Part part, Arpeggio arpeggio) {
            int _16BeatCount = part.Beat * 4; // 16ビート
            for (int i = 0; i < _16BeatCount; i++) {
                int _note = Utils.GetNote(song.Key, part.Degree, part.Mode, arpeggio, i); // 16の倍数
                add(position + (120 * i), new ChannelMessage(ChannelCommand.NoteOn, 0, _note, 127)); // ノートON
                add(position + (120 * i) + 120, new ChannelMessage(ChannelCommand.NoteOff, 0, _note, 0)); // ノートOFF
            }
        }

    }
}
