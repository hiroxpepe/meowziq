
using Meowziq.Phrase;

namespace Meowziq.Player {
    public class BassPlayer : Core.Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void preBuild() {
            this.phraseList.Add(new TextBassPhrase());
        }

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
