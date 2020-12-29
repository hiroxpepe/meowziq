
using Meowzic.Core;

namespace Meowzic.Player {
    public class BassPlayer : Core.Player {



        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public BassPlayer(Song song) : base(song) {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
