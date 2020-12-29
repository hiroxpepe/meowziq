﻿
using Meowzic.Core;
using Meowzic.Phrase;

namespace Meowzic.Player {
    public class BassPlayer : Core.Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public BassPlayer(Song song, int midiCh, int programNum) : base(song, midiCh, programNum) {
            this.phraseList.Add(new RootBass8BeatPhrase());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
