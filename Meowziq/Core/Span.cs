
namespace Meowziq.Core {
    /// <summary>
    /// コードトラック
    /// </summary>
    public class Span {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int beat; // 拍数

        Degree degree; // コード ? この言い方はおかしいかも 度数と旋法で決まるはず

        Mode mode; // 旋法

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Span(int beat, Degree degree) {
            this.beat = beat;
            this.degree = degree;
            this.mode = Mode.Undefined;
        }

        public Span(int beat, Degree degree, Mode mode) {
            this.beat = beat;
            this.degree = degree;
            this.mode = mode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public int Beat {
            get => beat;
        }

        public Degree Degree {
            get => degree;
        }

        public Mode Mode {
            get => mode;
            set => mode = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]
    }
}
