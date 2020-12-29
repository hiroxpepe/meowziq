
using Meowzic.Core;
using Meowzic.Phrase;

namespace Meowzic.Player {
    public class DrumPlayer : Core.Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public DrumPlayer(Song song, int midiCh, int programNum) : base(song, midiCh, programNum) {
            this.phraseList.Add(new TextPhrase());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
