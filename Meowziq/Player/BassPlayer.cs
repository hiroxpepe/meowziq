
using Meowziq.Core;
using Meowziq.Phrase;

namespace Meowziq.Player {
    public class BassPlayer : Core.Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public BassPlayer(Song song, int midiCh, int programNum) : base(song, midiCh, programNum) {
            this.phraseList.Add(new RootBass8BeatPhrase());
        }

        public BassPlayer(Song song, MidiChannel midiCh, Instrument programNum) : base(song, midiCh, programNum) {
            this.phraseList.Add(new RootBass8BeatPhrase());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onPlay() {
            throw new System.NotImplementedException();
        }
    }
}
