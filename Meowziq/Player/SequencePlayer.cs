
using Meowziq.Phrase;

namespace Meowziq.Player {
    public class SequencePlayer : Core.Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void preBuild() {
            this.phraseList.Add(new RandomSequencePhrase());
        }

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
