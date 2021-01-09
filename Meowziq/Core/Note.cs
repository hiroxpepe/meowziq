
namespace Meowziq.Core {
    /// <summary>
    /// ChannelMessage に変換される
    /// </summary>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int tick; // TICK ※絶対値

        int num; // NOTE番号

        int gate; // NOTE長さ

        int velo; // NOTE強さ

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Note(int tick, int num, int gate, int velo) {
            this.tick = tick;
            this.num = num;
            this.gate = gate;
            this.velo = velo;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public int Tick {
            get => tick;
        }

        public int Num {
            get => num;
        }

        public int Gate {
            get => gate;
        }

        public int Velo {
            get => velo;
        }
    }

    /// <summary>
    /// Note の配列を持つ
    /// </summary>
    public class Chord {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Note[] noteArray;

        // キーのコードと自分を比較して自動展開
    }
}
