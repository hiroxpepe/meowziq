
using Meowziq.Phrase;

namespace Meowziq.Player {
    public class DrumPlayer : Core.Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void preBuild() {
            phraseList.Add(new TextDrumPhrase());
        }

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
