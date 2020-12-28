
namespace Meowzic.Core {
    public class Pattern {
        // コードトラックが必要？

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int bar; // 小節数

        Chord chord; // コード

        Mode mode; // 旋法

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Pattern(int bar, Chord chord) {
            this.bar = bar;
            this.chord = chord;
            this.mode = Mode.Undefined;
        }

        public Pattern(int bar, Chord chord, Mode mode) {
            this.bar = bar;
            this.chord = chord;
            this.mode = mode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public int Bar {
            get => bar;
        }

        public Chord Chord {
            get => chord;
        }

        public Mode Mode {
            get => mode;
            set => mode = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

    }

}
