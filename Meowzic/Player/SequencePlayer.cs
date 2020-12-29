
using Meowzic.Core;
using Meowzic.Phrase;

namespace Meowzic.Player {
    public class SequencePlayer : Core.Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public SequencePlayer(Song song, int midiCh, int programNum) : base(song, midiCh, programNum) {
            this.phraseList.Add(new RandomSequencePhrase());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
