﻿
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meowziq {
    /// <summary>
    /// NOTE: TOPレベルであるべき ⇒ 実装パッケージを using しない
    /// </summary>
    public class Utils {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        static Random random = new Random();

        ///////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// 1～9の Phrase コード記法から Note No の配列を取得します
        /// ※自動的に旋法を決定する場合
        /// </summary>
        public static int[] GetNoteArrayByAutoMode(Key key, Degree degree, Mode keyMode, int index) {
            int _noteOfDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取得
            Mode _modeOfDegree = modeBy(degree, keyMode); // キーの旋法と度数から旋法に対応したその度数の旋法を取得
            int[] _scale7 = scale7By(Key.Enum.Parse(_noteOfDegree), _modeOfDegree); // そのルート音の旋法スケールを取得
            return noteArryBy(index, _scale7); // 旋法スケールから引数indexに対応したコード構成音の配列を返す
        }

        /// <summary>
        /// 1～7の Phrase コード記法から Note No の配列を取得します
        /// ※Span の旋法を適用する場合
        /// </summary>
        public static int[] GetNoteArrayBySpanMode(Key key, Degree degree, Mode keyMode, Mode spanMode, int index) {
            int _noteOfDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取得
            int[] _scale7 = scale7By(Key.Enum.Parse(_noteOfDegree), spanMode); // そのルート音の旋法スケールを取得※Span に設定した旋法を使用
            return noteArryBy(index, _scale7); // 旋法スケールから引数indexに対応したコード構成音の配列を返す
        }

        /// <summary>
        /// 1～7の Phrase ノート記法から Note No を取得します
        /// ※自動的に旋法を決定する場合
        /// </summary>
        public static int GetNoteByAutoMode(Key key, Degree degree, Mode keyMode, int index) {
            int _noteOfDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取得
            Mode _modeOfDegree = modeBy(degree, keyMode); // キーの旋法と度数から旋法に対応したその度数の旋法を取得
            int[] _scale7 = scale7By(Key.Enum.Parse(_noteOfDegree), _modeOfDegree); // そのルート音の旋法スケールを取得
            return _scale7[index - 1]; // 0基底のスケール配列から引数添え字のノートを返す
        }

        /// <summary>
        /// 1～7の Phrase ノート記法から Note No を取得します
        /// ※Span の旋法を適用する場合
        /// </summary>
        public static int GetNoteBySpanMode(Key key, Degree degree, Mode keyMode, Mode spanMode, int index) {
            int _noteOfDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取得
            int[] _scale7 = scale7By(Key.Enum.Parse(_noteOfDegree), spanMode); // そのルート音の旋法スケールを取得※Span に設定した旋法を使用
            return _scale7[index - 1]; // 0基底のスケール配列から引数添え字のノートを返す
        }

        /// <summary>
        /// FIXME: テキスト設定からランダムアルぺジエーター
        /// </summary>
        public static int GetNoteAsRandom(Key key, Degree degree, Mode keyMode, Mode spanMode, bool autoMode) {
            return noteOfRandomByModeScaleOf3(key, degree, keyMode, spanMode, autoMode);
        }

        /// <summary>
        /// 度数とキーの旋法から旋法に対応したその度数の旋法を返します
        /// </summary>
        public static Mode GetModeBy(Degree degree, Mode keyMode) {
            return modeBy(degree, keyMode);
        }

        /// <summary>
        /// TBA: 暫定: span の旋法から曲の旋法を判定する
        /// </summary>
        public static Mode GeyModeKeyBy(Key key, Degree degree, Mode keyMode, Mode spanMode) {
            // TODO: まず旋法チェンジ判定不能なものをはねる
            // MEMO: C における A の表示の仕方は後で考える
            // MEMO: 曲中に転調させる方法と合わせて考える
            Mode _modeMaybeKey = modeKeyBy(degree, spanMode); // span の度数と旋法からこの旋法と推測される
            int[] _scaleOfKey = scale7By(key, _modeMaybeKey); // この旋法と曲キーで作成したスケールの構成音が同じになるはずである
            int _noteOfDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取得
            int[] _scaleOfSpan = scale7By(Key.Enum.Parse(_noteOfDegree), spanMode); // span の度数と旋法でもスケールを作成してみる
            if (!compareScale(_scaleOfKey, _scaleOfSpan)) { // 比較して構成音は同じはず
                return Mode.Undefined; // 構成音が違う場合
            }
            return _modeMaybeKey; // TODO: これで正しいかテストを作成して確認
        }

        /// <summary>
        /// メジャーかマイナーかシンプルなコードネームを返します
        /// </summary>
        public static string GetSimpleCodeName(Key key, Degree degree, Mode keyMode, Mode spanMode, bool autoMode = true) {
            int _noteOfDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取
            string _codeBase = Key.Enum.Parse(_noteOfDegree).ToString(); // コードネームの基本取得
            Mode _mode;
            if (autoMode) { // 自動旋法取得
                _mode = modeBy(degree, keyMode);
            } else {
                _mode = spanMode; // Spanの旋法適用
            }
            string _3rdString = majorOrMinerString(_mode);
            return _codeBase + _3rdString;
        }

        public static string Filter(string target) {
            return target.Replace("|", "").Replace("[", "").Replace("]", ""); // 不要文字削除
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// スケールの構成音を比較します
        /// </summary>
        static bool compareScale(int[] scale1, int[] scale2) {
            List<Key> _keyList1 = scale1.Select(x => Key.Enum.Parse(x)).OrderBy(x => x).ToList();
            List<Key> _keyList2 = scale2.Select(x => Key.Enum.Parse(x)).OrderBy(x => x).ToList();
            var _result = _keyList1.Where(x => !_keyList2.Contains(x));
            if (_result.Count() == 0) {
                return true; // 構成音が一致
            }
            return false;
        }

        /// <summary>
        /// その旋法のコードがメジャーかマイナーか付加する文字列を返します
        /// </summary>
        static string majorOrMinerString(Mode _mode) {
            switch (_mode) {
                case Mode.Lyd:
                case Mode.Ion:
                case Mode.Mix:
                    return ""; // TODO: "M"
                case Mode.Dor:
                case Mode.Aeo:
                case Mode.Phr:
                case Mode.Loc:
                    return "m";
                default:
                    throw new ArgumentException("invalid mode.");
            }
        }

        /// <summary>
        /// 1～9のコード記法から Note No の配列を返します
        /// </summary>
        static int[] noteArryBy(int index, int[] scale7) {
            // TODO: バリデート
            int[] _note2 = new int[2]; // chord 構成音の2音
            int[] _note3 = new int[3]; // chord 構成音の3音
            int[] _note4 = new int[4]; // chord 構成音の4音
            switch (index) {
                case 3: // e.g. C, Cm, Cm(b5)
                    _note3[0] = scale7[1 - 1];
                    _note3[1] = scale7[3 - 1];
                    _note3[2] = scale7[5 - 1];
                    return _note3;
                case 4: // e.g. C(#4), Csus4, Csus4(-5)
                    _note3[0] = scale7[1 - 1];
                    _note3[1] = scale7[4 - 1];
                    _note3[2] = scale7[5 - 1];
                    return _note3;
                case 5: // e.g. C5(no3), C(-5,no3)
                    _note2[0] = scale7[1 - 1];
                    _note2[1] = scale7[5 - 1];
                    return _note2;
                case 6: // e.g. C6, Cm6, Cm(b6), Cm(-5,b6)
                    _note4[0] = scale7[1 - 1];
                    _note4[1] = scale7[3 - 1];
                    _note4[2] = scale7[5 - 1];
                    _note4[3] = scale7[6 - 1];
                    return _note4;
                case 7: // e.g. CM7, C7, Cm7, Cm7(-5)
                    _note4[0] = scale7[1 - 1];
                    _note4[1] = scale7[3 - 1];
                    _note4[2] = scale7[5 - 1];
                    _note4[3] = scale7[7 - 1];
                    return _note4;
                case 9: // e.g. Cadd9, Cmadd9, Cm(b9), Cm(-5,b9)
                    _note4[0] = scale7[1 - 1];
                    _note4[1] = scale7[3 - 1];
                    _note4[2] = scale7[5 - 1];
                    _note4[3] = scale7[2 - 1] + 12; // 1オクターブ上: FIXME: Range の対応外
                    return _note4;
                default:
                    throw new ArgumentException("invalid index.");
            }
        }

        static int noteRootBy(Key key, Degree degree, Mode keyMode) {
            key.Validate();
            degree.Validate();
            keyMode.Validate();
            int[] _scale7 = scale7By(key, keyMode);
            return _scale7[(int) degree];
        }

        static int[] note3By(Key key, Degree degree, Mode keyMode) {
            key.Validate();
            degree.Validate();
            keyMode.Validate();
            int[] _scale7 = scale7By(key, keyMode);
            int[] _note3 = new int[3]; // コード構成音の3音を抽出
            switch (degree) {
                case Degree.I:
                    _note3[0] = _scale7[1 - 1];
                    _note3[1] = _scale7[3 - 1];
                    _note3[2] = _scale7[5 - 1];
                    break;
                case Degree.II:
                    _note3[0] = _scale7[2 - 1];
                    _note3[1] = _scale7[4 - 1];
                    _note3[2] = _scale7[6 - 1];
                    break;
                case Degree.III:
                    _note3[0] = _scale7[3 - 1];
                    _note3[1] = _scale7[5 - 1];
                    _note3[2] = _scale7[7 - 1];
                    break;
                case Degree.IV:
                    _note3[0] = _scale7[4 - 1];
                    _note3[1] = _scale7[6 - 1];
                    _note3[2] = _scale7[1 - 1];
                    break;
                case Degree.V:
                    _note3[0] = _scale7[5 - 1];
                    _note3[1] = _scale7[7 - 1];
                    _note3[2] = _scale7[2 - 1];
                    break;
                case Degree.VI:
                    _note3[0] = _scale7[6 - 1];
                    _note3[1] = _scale7[1 - 1];
                    _note3[2] = _scale7[3 - 1];
                    break;
                case Degree.VII:
                    _note3[0] = _scale7[7 - 1];
                    _note3[1] = _scale7[2 - 1];
                    _note3[2] = _scale7[4 - 1];
                    break;
            }
            return _note3;
        }

        static int[] note4By(Key key, Degree degree, Mode keyMode) {
            key.Validate();
            degree.Validate();
            keyMode.Validate();
            int[] _scale7 = scale7By(key, keyMode);
            int[] _note4 = new int[4]; // コード構成音の4音を抽出
            switch (degree) {
                case Degree.I:
                    _note4[0] = _scale7[1 - 1];
                    _note4[1] = _scale7[3 - 1];
                    _note4[2] = _scale7[5 - 1];
                    _note4[3] = _scale7[7 - 1];
                    break;
                case Degree.II:
                    _note4[0] = _scale7[2 - 1];
                    _note4[1] = _scale7[4 - 1];
                    _note4[2] = _scale7[6 - 1];
                    _note4[3] = _scale7[1 - 1];
                    break;
                case Degree.III:
                    _note4[0] = _scale7[3 - 1];
                    _note4[1] = _scale7[5 - 1];
                    _note4[2] = _scale7[7 - 1];
                    _note4[3] = _scale7[2 - 1];
                    break;
                case Degree.IV:
                    _note4[0] = _scale7[4 - 1];
                    _note4[1] = _scale7[6 - 1];
                    _note4[2] = _scale7[1 - 1];
                    _note4[3] = _scale7[3 - 1];
                    break;
                case Degree.V:
                    _note4[0] = _scale7[5 - 1];
                    _note4[1] = _scale7[7 - 1];
                    _note4[2] = _scale7[2 - 1];
                    _note4[3] = _scale7[4 - 1];
                    break;
                case Degree.VI:
                    _note4[0] = _scale7[6 - 1];
                    _note4[1] = _scale7[1 - 1];
                    _note4[2] = _scale7[3 - 1];
                    _note4[3] = _scale7[5 - 1];
                    break;
                case Degree.VII:
                    _note4[0] = _scale7[7 - 1];
                    _note4[1] = _scale7[2 - 1];
                    _note4[2] = _scale7[4 - 1];
                    _note4[3] = _scale7[6 - 1];
                    break;
            }
            return _note4;
        }

        // TODO: 5音を取り出すメソッド：7モードのペンタトニック

        /// <summary>
        /// MEMO: 有効？
        /// </summary>
        static int[] note7By(Key key, Degree degree, Mode keyMode) {
            key.Validate();
            degree.Validate();
            keyMode.Validate();
            int[] _scale7 = scale7By(key, keyMode);
            int[] _note7 = new int[7]; // コード構成音の7音を抽出
            switch (degree) {
                case Degree.I:
                    _note7[0] = _scale7[1 - 1];
                    _note7[1] = _scale7[3 - 1];
                    _note7[2] = _scale7[5 - 1];
                    _note7[3] = _scale7[7 - 1];
                    _note7[4] = _scale7[2 - 1];
                    _note7[5] = _scale7[4 - 1];
                    _note7[6] = _scale7[6 - 1];
                    break;
                case Degree.II:
                    _note7[0] = _scale7[2 - 1];
                    _note7[1] = _scale7[4 - 1];
                    _note7[2] = _scale7[6 - 1];
                    _note7[3] = _scale7[1 - 1];
                    _note7[4] = _scale7[3 - 1];
                    _note7[5] = _scale7[5 - 1];
                    _note7[6] = _scale7[7 - 1];
                    break;
                case Degree.III:
                    _note7[0] = _scale7[3 - 1];
                    _note7[1] = _scale7[5 - 1];
                    _note7[2] = _scale7[7 - 1];
                    _note7[3] = _scale7[2 - 1];
                    _note7[4] = _scale7[4 - 1];
                    _note7[5] = _scale7[6 - 1];
                    _note7[6] = _scale7[1 - 1];
                    break;
                case Degree.IV:
                    _note7[0] = _scale7[4 - 1];
                    _note7[1] = _scale7[6 - 1];
                    _note7[2] = _scale7[1 - 1];
                    _note7[3] = _scale7[3 - 1];
                    _note7[4] = _scale7[5 - 1];
                    _note7[5] = _scale7[7 - 1];
                    _note7[6] = _scale7[2 - 1];
                    break;
                case Degree.V:
                    _note7[0] = _scale7[5 - 1];
                    _note7[1] = _scale7[7 - 1];
                    _note7[2] = _scale7[2 - 1];
                    _note7[3] = _scale7[4 - 1];
                    _note7[4] = _scale7[6 - 1];
                    _note7[5] = _scale7[1 - 1];
                    _note7[6] = _scale7[3 - 1];
                    break;
                case Degree.VI:
                    _note7[0] = _scale7[6 - 1];
                    _note7[1] = _scale7[1 - 1];
                    _note7[2] = _scale7[3 - 1];
                    _note7[3] = _scale7[5 - 1];
                    _note7[4] = _scale7[7 - 1];
                    _note7[5] = _scale7[2 - 1];
                    _note7[6] = _scale7[4 - 1];
                    break;
                case Degree.VII:
                    _note7[0] = _scale7[7 - 1];
                    _note7[1] = _scale7[2 - 1];
                    _note7[2] = _scale7[4 - 1];
                    _note7[3] = _scale7[6 - 1];
                    _note7[4] = _scale7[1 - 1];
                    _note7[5] = _scale7[3 - 1];
                    _note7[6] = _scale7[5 - 1];
                    break;
            }
            return _note7;
        }

        /// <summary>
        /// キーと旋法からモードスケールを返します
        /// </summary>
        static int[] scale7By(Key key, Mode keyMode) {
            key.Validate();
            keyMode.Validate();
            int _noteRoot = (int) key;
            int[] _scale7 = new int[7]; // 7音の旋法を作成
            switch (keyMode) {
                case Mode.Lyd:
                    _scale7[0] = _noteRoot; // 1
                    _scale7[1] = _noteRoot + 2; // 2
                    _scale7[2] = _noteRoot + 4; // 3
                    _scale7[3] = _noteRoot + 6; // #4
                    _scale7[4] = _noteRoot + 7; // 5
                    _scale7[5] = _noteRoot + 9; // 6
                    _scale7[6] = _noteRoot + 11; // 7
                    break;
                case Mode.Ion:
                    _scale7[0] = _noteRoot; // 1
                    _scale7[1] = _noteRoot + 2; // 2
                    _scale7[2] = _noteRoot + 4; // 3
                    _scale7[3] = _noteRoot + 5; // 4
                    _scale7[4] = _noteRoot + 7; // 5
                    _scale7[5] = _noteRoot + 9; // 6
                    _scale7[6] = _noteRoot + 11; // 7
                    break;
                case Mode.Mix:
                    _scale7[0] = _noteRoot; // 1
                    _scale7[1] = _noteRoot + 2; // 2
                    _scale7[2] = _noteRoot + 4; // 3
                    _scale7[3] = _noteRoot + 5; // 4
                    _scale7[4] = _noteRoot + 7; // 5
                    _scale7[5] = _noteRoot + 9; // 6
                    _scale7[6] = _noteRoot + 10; // b7
                    break;
                case Mode.Dor:
                    _scale7[0] = _noteRoot; // 1
                    _scale7[1] = _noteRoot + 2; // 2
                    _scale7[2] = _noteRoot + 3; // b3
                    _scale7[3] = _noteRoot + 5; // 4
                    _scale7[4] = _noteRoot + 7; // 5
                    _scale7[5] = _noteRoot + 9; // 6
                    _scale7[6] = _noteRoot + 10; // b7
                    break;
                case Mode.Aeo:
                    _scale7[0] = _noteRoot; // 1
                    _scale7[1] = _noteRoot + 2; // 2
                    _scale7[2] = _noteRoot + 3; // b3
                    _scale7[3] = _noteRoot + 5; // 4
                    _scale7[4] = _noteRoot + 7; // 5
                    _scale7[5] = _noteRoot + 8; // b6
                    _scale7[6] = _noteRoot + 10; // b7
                    break;
                case Mode.Phr:
                    _scale7[0] = _noteRoot; // 1
                    _scale7[1] = _noteRoot + 1; // b2
                    _scale7[2] = _noteRoot + 3; // b3
                    _scale7[3] = _noteRoot + 5; // 4
                    _scale7[4] = _noteRoot + 7; // 5
                    _scale7[5] = _noteRoot + 8; // b6
                    _scale7[6] = _noteRoot + 10; // b7
                    break;
                case Mode.Loc:
                    _scale7[0] = _noteRoot; // 1
                    _scale7[1] = _noteRoot + 1; // b2
                    _scale7[2] = _noteRoot + 3; // b3
                    _scale7[3] = _noteRoot + 5; // 4
                    _scale7[4] = _noteRoot + 6; // b5
                    _scale7[5] = _noteRoot + 8; // b6
                    _scale7[6] = _noteRoot + 10; // b7
                    break;
            }
            return _scale7;
        }

        /// <summary>
        /// MEMO: ここで引いた Mode と元々の曲 key でスケールを作成してルート音が合っているか調べる
        /// TODO: 仮：span の度数と旋法でキーの旋法を返す TODO: C キーで Am が A になる時は？
        /// </summary>
        static Mode modeKeyBy(Degree degree, Mode spanMode) {
            spanMode.Validate();
            degree.Validate();
            if (spanMode == Mode.Lyd) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Lyd;
                    case Degree.II:
                        return Mode.Phr;
                    case Degree.III:
                        return Mode.Dor;
                    case Degree.IV:
                        return Mode.Ion;
                    case Degree.V:
                        return Mode.Loc;
                    case Degree.VI:
                        return Mode.Aeo;
                    case Degree.VII:
                        return Mode.Mix;
                }
            } else if (spanMode == Mode.Ion) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Ion;
                    case Degree.II:
                        return Mode.Loc;
                    case Degree.III:
                        return Mode.Aeo;
                    case Degree.IV:
                        return Mode.Mix;
                    case Degree.V:
                        return Mode.Lyd;
                    case Degree.VI:
                        return Mode.Phr;
                    case Degree.VII:
                        return Mode.Dor;
                }
            } else if (spanMode == Mode.Mix) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Mix;
                    case Degree.II:
                        return Mode.Lyd;
                    case Degree.III:
                        return Mode.Phr;
                    case Degree.IV:
                        return Mode.Dor;
                    case Degree.V:
                        return Mode.Ion;
                    case Degree.VI:
                        return Mode.Loc;
                    case Degree.VII:
                        return Mode.Aeo;
                }
            } else if (spanMode == Mode.Dor) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Dor;
                    case Degree.II:
                        return Mode.Ion;
                    case Degree.III:
                        return Mode.Loc;
                    case Degree.IV:
                        return Mode.Aeo;
                    case Degree.V:
                        return Mode.Mix;
                    case Degree.VI:
                        return Mode.Lyd;
                    case Degree.VII:
                        return Mode.Phr;
                }
            } else if (spanMode == Mode.Aeo) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Aeo;
                    case Degree.II:
                        return Mode.Mix;
                    case Degree.III:
                        return Mode.Lyd;
                    case Degree.IV:
                        return Mode.Phr;
                    case Degree.V:
                        return Mode.Dor;
                    case Degree.VI:
                        return Mode.Ion;
                    case Degree.VII:
                        return Mode.Loc;
                }
            } else if (spanMode == Mode.Phr) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Phr;
                    case Degree.II:
                        return Mode.Dor;
                    case Degree.III:
                        return Mode.Ion;
                    case Degree.IV:
                        return Mode.Loc;
                    case Degree.V:
                        return Mode.Aeo;
                    case Degree.VI:
                        return Mode.Mix;
                    case Degree.VII:
                        return Mode.Lyd;
                }
            } else if (spanMode == Mode.Loc) {
                switch (degree) {
                    case Degree.I:
                        return Mode.Loc;
                    case Degree.II:
                        return Mode.Aeo;
                    case Degree.III:
                        return Mode.Mix;
                    case Degree.IV:
                        return Mode.Lyd;
                    case Degree.V:
                        return Mode.Phr;
                    case Degree.VI:
                        return Mode.Dor;
                    case Degree.VII:
                        return Mode.Ion;
                }
            }
            throw new ArgumentException("not key or degree.");
        }

        /// <summary>
        /// 度数とキーの旋法から旋法に対応したその度数の旋法を返します
        /// </summary>
        static Mode modeBy(Degree degree, Mode keyMode) {
            keyMode.Validate();
            degree.Validate();
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
            throw new ArgumentException("not key or degree.");
        }

        /// <summary>
        /// 度数関係なくそのキーの旋法の構成音をランダムで返す
        /// </summary>
        static int noteOfRandomByModeScale(Key key, Mode keyMode) {
            int[] _scale = scale7By(key, keyMode);
            int _random = random.Next(0, 7);
            return _scale[_random]; // 0 から 6
        }

        /// <summary>
        /// そのキーの旋法の度数の構成音をランダムで返す
        /// </summary>
        static int noteOfRandomByModeScaleOf3(Key key, Degree degree, Mode keyMode, Mode spanMode, bool autoMode = true) {
            int[] _scale;
            if (autoMode) {
                _scale = note3By(key, degree, keyMode); // 3音
            } else {
                _scale = note3By(key, degree, spanMode); // 3音
            }
            int _random = random.Next(0, 3);
            return _scale[_random]; // 0 から 2
        }

        // MEMO: 展開コードでキーボードの範囲(ゾーン)を指定するのはどうか
        // TODO: クローズドボイシングでルートのコードのソーンに追従する展開形を自動計算
    }
}
