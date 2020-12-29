
using System;

namespace Meowzic.Core {
    public class Utils {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        static Random random = new Random();

        ///////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static int GetRootNote(Key key) {
            return (int) key;
        }

        public static int GetRootNote(Key key, Degree degree, Mode mode) {
            return chordRootNote(key, degree, mode);
        }

        public static int GetNote(Key key, Mode mode, Arpeggio arpeggio) {
            return arpeggioAsModeScale(key, mode, arpeggio);
        }

        public static int GetNote(Key key, Degree degree, Mode mode, Arpeggio arpeggio) {
            return arpeggioAsModeScaleIn3(key, degree, mode, arpeggio);
        }

        public static int GetNote(Key key, Degree degree, Mode mode, Arpeggio arpeggio, int beatCount) {
            return arpeggioAsModeScaleIn3(key, degree, mode, arpeggio, beatCount);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// コード関係なく、そのキー、モードのスケール ノートを返す
        /// </summary>
        static int arpeggioAsModeScale(Key key, Mode mode, Arpeggio arpeggio) {
            int[] _scale = modeScale(key, mode);
            if (arpeggio == Arpeggio.Random) {
                int _ret = random.Next(0, 7);
                return _scale[_ret]; // 0 から 6
            }
            // TODO: Up, Down
            return 0;
        }

        // TODO: 5音を取り出すメソッド

        /// <summary>
        /// そのキーのルートコードのモードスケール ノートを返す
        /// </summary>
        static int arpeggioAsModeScaleIn3(Key key, Degree degree, Mode mode, Arpeggio arpeggio, int beatCount = 0) {
            int _ret;
            int[] _scale = chordNote3(key, degree, mode); // 3音
            switch (arpeggio) {
                case Arpeggio.Random:
                    _ret = random.Next(0, 3);
                    return _scale[_ret]; // 0 から 2
                case Arpeggio.Up:
                    _ret = upNoteIn3(beatCount); // 3音から音階上昇
                    return _scale[_ret];
                default:
                    break;
            }
            // TODO: Up, Down
            return 0;
        }

        static int upNoteIn3(int beatCount) {
            if (beatCount > 15) {
                beatCount = beatCount % 16;
            }
            switch (beatCount) {
                case 0:
                case 3:
                case 6:
                case 9:
                case 12:
                case 15:
                    return 0;
                case 1:
                case 4:
                case 7:
                case 10:
                case 13:
                    return 1;
                case 2:
                case 5:
                case 8:
                case 11:
                case 14:
                    return 2;
                default:
                    break;
            }
            return 0;
        }

        static int chordRootNote(Key key, Degree degree, Mode mode) {
            int[] _scale = modeScale(key, mode);
            return _scale[(int) degree];
        }

        static int[] chordNote3(Key key, Degree degree, Mode mode) {
            int[] _modeScale = modeScale(key, mode);
            int[] _note3 = new int[3]; // コード構成音の3音を抽出
            switch (degree) {
                case Degree.I:
                    _note3[0] = _modeScale[1 - 1];
                    _note3[1] = _modeScale[3 - 1];
                    _note3[2] = _modeScale[5 - 1];
                    break;
                case Degree.II:
                    _note3[0] = _modeScale[2 - 1];
                    _note3[1] = _modeScale[4 - 1];
                    _note3[2] = _modeScale[6 - 1];
                    break;
                case Degree.III:
                    _note3[0] = _modeScale[3 - 1];
                    _note3[1] = _modeScale[5 - 1];
                    _note3[2] = _modeScale[7 - 1];
                    break;
                case Degree.IV:
                    _note3[0] = _modeScale[4 - 1];
                    _note3[1] = _modeScale[6 - 1];
                    _note3[2] = _modeScale[1 - 1];
                    break;
                case Degree.V:
                    _note3[0] = _modeScale[5 - 1];
                    _note3[1] = _modeScale[7 - 1];
                    _note3[2] = _modeScale[2 - 1];
                    break;
                case Degree.VI:
                    _note3[0] = _modeScale[6 - 1];
                    _note3[1] = _modeScale[1 - 1];
                    _note3[2] = _modeScale[3 - 1];
                    break;
                case Degree.VII:
                    _note3[0] = _modeScale[7 - 1];
                    _note3[1] = _modeScale[2 - 1];
                    _note3[2] = _modeScale[4 - 1];
                    break;
                default:
                    break;
            }
            return _note3;
        }

        static int[] chordNote4(Key key, Degree degree, Mode mode) {
            int[] _modeScale = modeScale(key, mode);
            int[] _codeNote4 = new int[4]; // コード構成音の4音を抽出
            switch (degree) {
                case Degree.I:
                    _codeNote4[0] = _modeScale[1 - 1];
                    _codeNote4[1] = _modeScale[3 - 1];
                    _codeNote4[2] = _modeScale[5 - 1];
                    _codeNote4[3] = _modeScale[7 - 1];
                    break;
                case Degree.II:
                    _codeNote4[0] = _modeScale[2 - 1];
                    _codeNote4[1] = _modeScale[4 - 1];
                    _codeNote4[2] = _modeScale[6 - 1];
                    _codeNote4[3] = _modeScale[1 - 1];
                    break;
                case Degree.III:
                    _codeNote4[0] = _modeScale[3 - 1];
                    _codeNote4[1] = _modeScale[5 - 1];
                    _codeNote4[2] = _modeScale[7 - 1];
                    _codeNote4[3] = _modeScale[2 - 1];
                    break;
                case Degree.IV:
                    _codeNote4[0] = _modeScale[4 - 1];
                    _codeNote4[1] = _modeScale[6 - 1];
                    _codeNote4[2] = _modeScale[1 - 1];
                    _codeNote4[3] = _modeScale[3 - 1];
                    break;
                case Degree.V:
                    _codeNote4[0] = _modeScale[5 - 1];
                    _codeNote4[1] = _modeScale[7 - 1];
                    _codeNote4[2] = _modeScale[2 - 1];
                    _codeNote4[3] = _modeScale[4 - 1];
                    break;
                case Degree.VI:
                    _codeNote4[0] = _modeScale[6 - 1];
                    _codeNote4[1] = _modeScale[1 - 1];
                    _codeNote4[2] = _modeScale[3 - 1];
                    _codeNote4[3] = _modeScale[5 - 1];
                    break;
                case Degree.VII:
                    _codeNote4[0] = _modeScale[7 - 1];
                    _codeNote4[1] = _modeScale[2 - 1];
                    _codeNote4[2] = _modeScale[4 - 1];
                    _codeNote4[3] = _modeScale[6 - 1];
                    break;
                default:
                    break;
            }
            return _codeNote4;
        }

        // 7モードのペンタトニック

        /// <summary>
        /// 該当キーのモードスケールを返します。
        /// </summary>
        static int[] modeScale(Key key, Mode mode) {
            int _root = (int) key;
            int[] _scale = new int[7]; // 7音を作成
            switch (mode) {
                case Mode.Lyd:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 4; // III
                    _scale[3] = _root + 6; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 11; // VII
                    break;
                case Mode.Ion:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 4; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 11; // VII
                    break;
                case Mode.Mix:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 4; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Dor:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Aeo:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 8; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Phr:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 1; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 8; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Loc:
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
        Lyd = 0,
        Ion = 1,
        Mix = 2,
        Dor = 3,
        Aeo = 4,
        Phr = 5,
        Loc = 6,
        Undefined = 99,
    }

    public enum Degree {
        I = 0,
        II = 1,
        III = 2,
        IV = 3,
        V = 4,
        VI = 5,
        VII = 6,
    }

    public enum Arpeggio {
        Up,
        Down,
        Random,
    }
}
