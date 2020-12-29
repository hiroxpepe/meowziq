
using Meowzic.Core;

namespace Meowzic.Phrase {

    public class RandomSequencePhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public RandomSequencePhrase() {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuild(int position, Key key, Span span) {
            int _16BeatCount = span.Beat * 4; // 16ビート
            for (int i = 0; i < _16BeatCount; i++) {
                int _note = Utils.GetNote(key, span.Degree, span.Mode, Arpeggio.Random, i); // 16の倍数
                Add(new Note(position + (120 * i), _note, 60, 127)); // gate 短め
            }
        }
    }
}
