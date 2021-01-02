
using System.Collections.Generic;

namespace Meowziq.Core {
    /// <summary>
    /// プレイヤー
    /// + 曲(ソング)を知っている
    /// + 自分の知識でフレーズを繰り出す
    ///     + フレーズの引き出しが沢山あれば曲をこなしていける
    ///     + ヴァース、コーラスなど曲のパートに合わせて演奏を変化させてくる
    ///     + 他のプレイヤーの演奏を参考にしながら演奏を修正してくる
    /// + 音色変更やエフェクトのタイミングも知っている
    ///     + 演奏する楽器について責任を持つ
    /// + ライブラリは知りたくない
    /// </summary>
    public class Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Song song;

        int midiCh; // 暫定

        int programNum; // 暫定

        string type;

        protected List<Phrase> phraseList;

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

        public Instrument Program {
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
            // フレーズ読み込み
            //preBuild();

            // 音色変更
            message.Apply(midiCh, programNum); 
            
            // 全ての Pattern の Note を MIDI データ化する
            int _position = 0;
            foreach (Pattern _pattern in song.AllPattern) {
                // Player の全ての Phrase を処理
                // TODO: Player 内でどの Phrase で演るか判断が必要
                foreach (Phrase _phrase in phraseList) {
                    _phrase.BuildByPattern(_position, song.Key, _pattern);
                    List<Note> _noteList = _phrase.AllNote;
                    foreach (Note _note in _noteList) {
                        message.Apply(midiCh, _note); // message に適用
                    }
                    _noteList.Clear(); // 必要
                }
                int _tick = _pattern.BeatCount * 480;
                _position += _tick; // Pattern の長さ分ポジションを移動する
            }
        }
    }
}
