
using Meowziq.Phrase;

namespace Meowziq.Player {
    public class PadPlayer : Core.Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void preBuild() {
            phraseList.Add(new TextPadPhrase());
        }

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
