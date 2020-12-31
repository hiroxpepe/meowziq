
using Meowziq.Core;
using Meowziq.Phrase;

namespace Meowziq.Player {
    public class DrumPlayer : Core.Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public DrumPlayer(Song song, int midiCh, int programNum) : base(song, midiCh, programNum) {
            this.phraseList.Add(new TextDrumPhrase());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
