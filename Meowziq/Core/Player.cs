
using System.Collections.Generic;

namespace Meowziq.Core {
    /// <summary>
    /// Player クラス
    ///     + Phrase オブジェクトのリストを管理
    ///     + MIDIノートを生成する
    /// </summary>
    public class Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Song song;

        int midiCh;

        int programNum;

        string type;

        List<Phrase> phraseList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Player() {
            phraseList = new List<Phrase>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public Song Song {
            set => song = value;
        }

        public MidiChannel MidiCh {
            set => midiCh = (int) value;
        }

        public Instrument Instrument {
            set => programNum = (int) value;
        }

        public DrumKit DrumKit {
            set => programNum = (int) value;
        }

        public string Type {
            get => type;
            set => type = value;
        }

        public List<Phrase> PhraseList {
            get => phraseList;
            set => phraseList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// MIDI ノートを生成します
        /// </summary>
        public void Build(Message message) {
            // 音色変更
            message.Apply(midiCh, programNum); 
            
            // 全ての Pattern の Note を MIDI データ化する
            var _position = 0;
            // MEMO: リアルタイム演奏を考える場合、1小節前に全て決まっている必要がある
            foreach (Pattern _pattern in song.AllPattern) {
                foreach (Phrase _phrase in phraseList) {
                    _phrase.Build(_position, song.Key, _pattern); // Note データを作成
                    var _noteList = _phrase.AllNote;
                    foreach (Note _note in _noteList) {
                        message.Apply(midiCh, _note); // message に適用
                    }
                    _noteList.Clear(); // 必要
                }
                var _tick = _pattern.BeatCount * 480;
                _position += _tick; // Pattern の長さ分ポジションを移動する
            }
        }
    }
}
