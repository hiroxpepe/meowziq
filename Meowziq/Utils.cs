
using System;

namespace Meowziq {

    public class Utils {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        static Random random = new Random();

        ///////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// 1～7の Phrase ノート記法から Note No を取得します
        /// ※自動的に旋法を決定する場合
        /// </summary>
        public static int GetNoteWithAutoMode(Key key, Degree degree, Mode keyMode, int index) {
            // 曲のキーの度数と旋法から度数のルート音を取得
            int _rootOfDegree = noteRoot(key, degree, keyMode); // FIXME: 曲の旋法？
            // キーの旋法と度数から旋法に対応したその度数の旋法を取得
            Mode _autoMode = modeForDegree(keyMode, degree);
            // そのルート音の旋法スケールを取得
            int[] _modeScale = modeScale(toKey(_rootOfDegree), _autoMode);
            return _modeScale[index - 1]; // 0基底
        }

        /// <summary>
        /// 1～7の Phrase ノート記法から Note No を取得します
        /// ※Span の旋法を適用する場合
        /// </summary>
        public static int GetNoteWithSpanMode(Key key, Degree degree, Mode keyMode, Mode spanMode, int index) {
            // 曲のキーの度数と旋法から度数のルート音を取得
            int _rootOfDegree = noteRoot(key, degree, keyMode); // FIXME: 曲の旋法？
            // そのルート音の旋法スケールを取得※Span に設定した旋法を使用
            int[] _modeScale = modeScale(toKey(_rootOfDegree), spanMode);
            return _modeScale[index - 1]; // 0基底
        }

        /// <summary>
        /// FIXME: テキスト設定からランダムアルぺジエーター
        /// </summary>
        public static int GetRandomNote(Key key, Degree degree, Mode mode) {
            return randomArpeggioFromModeScaleOf3Note(key, degree, mode);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static int noteRoot(Key key, Degree degree, Mode mode) {
            int[] _modeScale = modeScale(key, mode);
            return _modeScale[(int) degree];
        }

        static int[] note3(Key key, Degree degree, Mode mode) {
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

        static int[] note4(Key key, Degree degree, Mode mode) {
            int[] _modeScale = modeScale(key, mode);
            int[] _note4 = new int[4]; // コード構成音の4音を抽出
            switch (degree) {
                case Degree.I:
                    _note4[0] = _modeScale[1 - 1];
                    _note4[1] = _modeScale[3 - 1];
                    _note4[2] = _modeScale[5 - 1];
                    _note4[3] = _modeScale[7 - 1];
                    break;
                case Degree.II:
                    _note4[0] = _modeScale[2 - 1];
                    _note4[1] = _modeScale[4 - 1];
                    _note4[2] = _modeScale[6 - 1];
                    _note4[3] = _modeScale[1 - 1];
                    break;
                case Degree.III:
                    _note4[0] = _modeScale[3 - 1];
                    _note4[1] = _modeScale[5 - 1];
                    _note4[2] = _modeScale[7 - 1];
                    _note4[3] = _modeScale[2 - 1];
                    break;
                case Degree.IV:
                    _note4[0] = _modeScale[4 - 1];
                    _note4[1] = _modeScale[6 - 1];
                    _note4[2] = _modeScale[1 - 1];
                    _note4[3] = _modeScale[3 - 1];
                    break;
                case Degree.V:
                    _note4[0] = _modeScale[5 - 1];
                    _note4[1] = _modeScale[7 - 1];
                    _note4[2] = _modeScale[2 - 1];
                    _note4[3] = _modeScale[4 - 1];
                    break;
                case Degree.VI:
                    _note4[0] = _modeScale[6 - 1];
                    _note4[1] = _modeScale[1 - 1];
                    _note4[2] = _modeScale[3 - 1];
                    _note4[3] = _modeScale[5 - 1];
                    break;
                case Degree.VII:
                    _note4[0] = _modeScale[7 - 1];
                    _note4[1] = _modeScale[2 - 1];
                    _note4[2] = _modeScale[4 - 1];
                    _note4[3] = _modeScale[6 - 1];
                    break;
                default:
                    break;
            }
            return _note4;
        }

        // TODO: 5音を取り出すメソッド
        // TODO: 7モードのペンタトニック

        /// <summary>
        /// MEMO: 有効？
        /// </summary>
        static int[] note7(Key key, Degree degree, Mode mode) {
            int[] _modeScale = modeScale(key, mode);
            int[] _note7 = new int[7]; // コード構成音の7音を抽出
            switch (degree) {
                case Degree.I:
                    _note7[0] = _modeScale[1 - 1];
                    _note7[1] = _modeScale[3 - 1];
                    _note7[2] = _modeScale[5 - 1];
                    _note7[3] = _modeScale[7 - 1];
                    _note7[4] = _modeScale[2 - 1];
                    _note7[5] = _modeScale[4 - 1];
                    _note7[6] = _modeScale[6 - 1];
                    break;
                case Degree.II:
                    _note7[0] = _modeScale[2 - 1];
                    _note7[1] = _modeScale[4 - 1];
                    _note7[2] = _modeScale[6 - 1];
                    _note7[3] = _modeScale[1 - 1];
                    _note7[4] = _modeScale[3 - 1];
                    _note7[5] = _modeScale[5 - 1];
                    _note7[6] = _modeScale[7 - 1];
                    break;
                case Degree.III:
                    _note7[0] = _modeScale[3 - 1];
                    _note7[1] = _modeScale[5 - 1];
                    _note7[2] = _modeScale[7 - 1];
                    _note7[3] = _modeScale[2 - 1];
                    _note7[4] = _modeScale[4 - 1];
                    _note7[5] = _modeScale[6 - 1];
                    _note7[6] = _modeScale[1 - 1];
                    break;
                case Degree.IV:
                    _note7[0] = _modeScale[4 - 1];
                    _note7[1] = _modeScale[6 - 1];
                    _note7[2] = _modeScale[1 - 1];
                    _note7[3] = _modeScale[3 - 1];
                    _note7[4] = _modeScale[5 - 1];
                    _note7[5] = _modeScale[7 - 1];
                    _note7[6] = _modeScale[2 - 1];
                    break;
                case Degree.V:
                    _note7[0] = _modeScale[5 - 1];
                    _note7[1] = _modeScale[7 - 1];
                    _note7[2] = _modeScale[2 - 1];
                    _note7[3] = _modeScale[4 - 1];
                    _note7[4] = _modeScale[6 - 1];
                    _note7[5] = _modeScale[1 - 1];
                    _note7[6] = _modeScale[3 - 1];
                    break;
                case Degree.VI:
                    _note7[0] = _modeScale[6 - 1];
                    _note7[1] = _modeScale[1 - 1];
                    _note7[2] = _modeScale[3 - 1];
                    _note7[3] = _modeScale[5 - 1];
                    _note7[4] = _modeScale[7 - 1];
                    _note7[5] = _modeScale[2 - 1];
                    _note7[6] = _modeScale[4 - 1];
                    break;
                case Degree.VII:
                    _note7[0] = _modeScale[7 - 1];
                    _note7[1] = _modeScale[2 - 1];
                    _note7[2] = _modeScale[4 - 1];
                    _note7[3] = _modeScale[6 - 1];
                    _note7[4] = _modeScale[1 - 1];
                    _note7[5] = _modeScale[3 - 1];
                    _note7[6] = _modeScale[5 - 1];
                    break;
                default:
                    break;
            }
            return _note7;
        }

        /// <summary>
        /// キーと旋法からモードスケールを返します
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

        /// <summary>
        /// キーの旋法と度数から旋法に対応したその度数の旋法を返します
        /// </summary>
        static Mode modeForDegree(Mode keyMode, Degree degree) {
            if (keyMode == Mode.Lyd) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Lyd;
                    case Degree.II:
                        return Mode.Mix;
                    case Degree.III:
                        return Mode.Aeo;
                    case Degree.IV:
                        return Mode.Loc;
                    case Degree.V:
                        return Mode.Ion;
                    case Degree.VI:
                        return Mode.Dor;
                    case Degree.VII:
                        return Mode.Phr;
                }
            } else if (keyMode == Mode.Ion) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Ion;
                    case Degree.II:
                        return Mode.Dor;
                    case Degree.III:
                        return Mode.Phr;
                    case Degree.IV:
                        return Mode.Lyd;
                    case Degree.V:
                        return Mode.Mix;
                    case Degree.VI:
                        return Mode.Aeo;
                    case Degree.VII:
                        return Mode.Loc;
                }
            } else if (keyMode == Mode.Mix) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Mix;
                    case Degree.II:
                        return Mode.Aeo;
                    case Degree.III:
                        return Mode.Loc;
                    case Degree.IV:
                        return Mode.Ion;
                    case Degree.V:
                        return Mode.Dor;
                    case Degree.VI:
                        return Mode.Phr;
                    case Degree.VII:
                        return Mode.Lyd;
                }
            } else if (keyMode == Mode.Dor) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Dor;
                    case Degree.II:
                        return Mode.Phr;
                    case Degree.III:
                        return Mode.Lyd;
                    case Degree.IV:
                        return Mode.Mix;
                    case Degree.V:
                        return Mode.Aeo;
                    case Degree.VI:
                        return Mode.Loc;
                    case Degree.VII:
                        return Mode.Ion;
                }
            } else if (keyMode == Mode.Aeo) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Aeo;
                    case Degree.II:
                        return Mode.Loc;
                    case Degree.III:
                        return Mode.Ion;
                    case Degree.IV:
                        return Mode.Dor;
                    case Degree.V:
                        return Mode.Phr;
                    case Degree.VI:
                        return Mode.Lyd;
                    case Degree.VII:
                        return Mode.Mix;
                }
            } else if (keyMode == Mode.Phr) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Phr;
                    case Degree.II:
                        return Mode.Lyd;
                    case Degree.III:
                        return Mode.Mix;
                    case Degree.IV:
                        return Mode.Aeo;
                    case Degree.V:
                        return Mode.Loc;
                    case Degree.VI:
                        return Mode.Ion;
                    case Degree.VII:
                        return Mode.Dor;
                }
            } else if (keyMode == Mode.Loc) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Loc;
                    case Degree.II:
                        return Mode.Ion;
                    case Degree.III:
                        return Mode.Dor;
                    case Degree.IV:
                        return Mode.Phr;
                    case Degree.V:
                        return Mode.Lyd;
                    case Degree.VI:
                        return Mode.Mix;
                    case Degree.VII:
                        return Mode.Aeo;
                }
            }
            return Mode.Undefined;
        }

        /// <summary>
        /// コード関係なく、そのキー、モードのスケール ノートを返す
        /// </summary>
        static int randomArpeggioFromModeScale(Key key, Mode mode) {
            int[] _scale = modeScale(key, mode);
            int _ret = random.Next(0, 7);
            return _scale[_ret]; // 0 から 6
        }

        /// <summary>
        /// そのキーのルートコードのモードスケール ノートを返す
        /// </summary>
        static int randomArpeggioFromModeScaleOf3Note(Key key, Degree degree, Mode mode) {
            int _ret;
            int[] _scale = note3(key, degree, mode); // 3音
            _ret = random.Next(0, 3);
            return _scale[_ret]; // 0 から 2
        }

        // MEMO: 展開コードでキーボードの範囲(ゾーン)を指定するのはどうか
        // TODO: クローズドボイシングでルートのコードのソーンに追従する展開形を自動計算

        static Key toKey(int note) { // TODO: 拡張メソッド
            if (note > 75) {
                note -= 12; // オクターブ調節
            }
            switch (note) {
                case 75:
                    return Key.Eb;
                case 74:
                    return Key.D;
                case 73:
                    return Key.Db;
                case 72:
                    return Key.C;
                case 71:
                    return Key.B;
                case 70:
                    return Key.Bb;
                case 69:
                    return Key.A;
                case 68:
                    return Key.Ab;
                case 67:
                    return Key.G;
                case 66:
                    return Key.Gb;
                case 65:
                    return Key.F;
                case 64:
                    return Key.E; // MEMO: E を最低音とする
                default:
                    return Key.Undefined;
            }
        }
    }
}
