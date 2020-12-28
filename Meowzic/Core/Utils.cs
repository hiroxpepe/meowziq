
namespace Meowzic.Core {
    public class Utils {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public static int GetRootNote(Key key) {
            return (int) key;
        }

        public static int GetRootNote(Key key, Chord code, Mode mode) {
            return GetChordRootNote(key, code, mode);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        private static int GetChordRootNote(Key key, Chord code, Mode mode) {
            int[] _scale = CreateScale(key, mode);
            return _scale[(int) code];
        }

        private static int[] CreateScale(Key key, Mode mode) {
            int _root = (int) key;
            int[] _scale = new int[7]; // 7音を作成
            switch (mode) {
                case Mode.Lydian:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 4; // III
                    _scale[3] = _root + 6; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 11; // VII
                    break;
                case Mode.Ionian:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 4; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 11; // VII
                    break;
                case Mode.Mixolydian:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 4; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Dorian:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Aeolian:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 8; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Phrygian:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 1; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 8; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Locrian:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 1; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 6; // V
                    _scale[5] = _root + 8; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                default:
                    break;
            }
            return _scale;
        }
    }

    public enum PatternType {
        Intro,
        Verse,
        Bridge,
        Chorus,
        Hook,
        Solo,
        Interlude,
        Outro,
    }

    public enum Key {
        E = 64,
        F = 65,
        Gb = 66,
        G = 67,
        Ab = 68,
        A = 69,
        Bb = 70,
        B = 71,
        C = 72,
        Db = 73,
        D = 74,
        Eb = 74,
    }

    public enum Mode {
        Undefined,
        Lydian,
        Ionian,
        Mixolydian,
        Dorian,
        Aeolian,
        Phrygian,
        Locrian,
    }

    public enum Chord {
        I = 0,
        II = 1,
        III = 2,
        IV = 3,
        V = 4,
        VI = 5,
        VII = 6,
    }
}
