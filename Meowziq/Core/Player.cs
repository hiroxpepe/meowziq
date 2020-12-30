
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
    abstract public class Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Song song;

        int midiCh; // 暫定

        int programNum; // 暫定

        protected List<Phrase> phraseList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Player(Song song, int midiCh, int programNum) {
            this.song = song;
            this.midiCh = midiCh;
            this.programNum = programNum;
            this.phraseList = new List<Phrase>();
        }

        public Player(Song song, MidiChannel midiCh, Instrument programNum) {
            this.song = song;
            this.midiCh = (int) midiCh;
            this.programNum = (int) programNum;
            this.phraseList = new List<Phrase>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        // 全ての Phrase を Build する
        public void Build(Message message) {
            message.Apply(midiCh, programNum); // 音色変更
            int _position = 0;
            List<Span> _spanList = song.GetAllSpan();
            foreach (Span _span in _spanList) {
                foreach (Phrase _phrase in phraseList) {
                    _phrase.Build(_position, song.Key, _span);
                    List<Note> _noteList = _phrase.GetAllNote();
                    foreach (Note _note in _noteList) {
                        message.Apply(midiCh, _note); // message に適用
                    }
                    _noteList.Clear();
                }
                int _tick = _span.Beat * 480;
                _position += _tick; // スパンの長さ分ポジションを移動する
            }
        }

        /// <summary>
        /// TBA
        /// </summary>
        public void Play() {
            onPlay();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        abstract protected void onPlay();
    }
}
