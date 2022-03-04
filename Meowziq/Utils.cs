
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meowziq {
    /// <summary>
    /// NOTE: TOPレベルであるべき ⇒ 実装パッケージを using しない、引数はプリミティブ型か Enum のみ
    /// MEMO: ここでは var で変数を定義しない
    /// </summary>
    public class Utils {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        static Random _random = new Random();

        ///////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// 引数の index 値の 16beat における長さを返します
        /// </summary>
        public static int To16beatLength(int index) {
            return Length.Of16beat.Int32() * index;
        }

        /// <summary>
        /// 引数の count 値(1拍)における 16beat の数を返します
        /// </summary>
        public static int To16beatCount(int beatCount) {
            return beatCount * 4; // 1拍における 16beat の数なので 4 を掛ける
        }

        /// <summary>
        /// 1～9の Phrase コード記法から Note No の配列を取得します
        ///     Span に旋法なし：キーの旋法と度数から旋法に対応したその度数の旋法を取得
        ///     Span に旋法あり：Span に設定した旋法を使用
        /// </summary>
        public static int[] ToNoteArray(Key key, Degree degree, Mode keyMode, Mode spanMode, int index) {
            int noteDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取得
            Mode mode = spanMode is Mode.Undefined ? modeSpanBy(degree, keyMode) : spanMode; // Span に旋法がなければ自動旋法
            int[] scale7 = scale7By(Key.Enum.Parse(noteDegree), mode); // そのルート音の旋法スケールを取得
            return noteArrayBy(index, scale7); // 旋法スケールから引数indexに対応したコード構成音の配列を返す
        }

        /// <summary>
        /// 1～7の Phrase ノート記法から Note No を取得します
        /// ※ "auto": 自動的に旋法を決定、Span の旋法両対応
        ///     Span に旋法なし：キーの旋法と度数から旋法に対応したその度数の旋法を取得
        ///     Span に旋法あり：Span に設定した旋法を使用
        /// ※ "note": キーの旋法を自動判定し、その旋法の ノート記法の対応 Note No を返す
        /// </summary>
        public static int ToNote(Key key, Degree degree, Mode keyMode, Mode spanMode, int index, bool autoNote = true) {
            int noteDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取得
            Mode mode = spanMode is Mode.Undefined ? modeSpanBy(degree, keyMode) : spanMode; // Span に旋法がなければ自動旋法
            int[] scale7 = null;
            if (autoNote) { // Span ベースの旋法(自動・設定)
                scale7 = scale7By(Key.Enum.Parse(noteDegree), mode); // そのルート音の旋法スケールを取得
            } else { // キーの旋法を自動判定してそちらから取得
                var modeCurrentKey = ToModeKey(key, degree, keyMode, mode);
                if (!modeCurrentKey.ToString().Equals("Undefined")) { // 旋法が判定出来れば
                    scale7 = scale7By(key, modeCurrentKey); // 判定した曲の旋法を取得
                }
            }
            return scale7[index - 1]; // 0基底のスケール配列から引数添え字のノートを返す
        }

        /// <summary>
        /// FIXME: テキスト設定からランダムアルぺジエーター
        /// </summary>
        public static int ToNoteRandom(Key key, Degree degree, Mode keyMode, Mode spanMode) {
            return noteOfRandom3By(key, degree, keyMode, spanMode);
        }

        /// <summary>
        /// 度数とキーの旋法から旋法に対応したその度数の旋法を返します
        /// </summary>
        public static Mode ToModeSpan(Degree degree, Mode keyMode) {
            return modeSpanBy(degree, keyMode);
        }

        /// <summary>
        /// Span の旋法から曲の旋法を判定する
        /// </summary>
        public static Mode ToModeKey(Key key, Degree degree, Mode keyMode, Mode spanMode) {
            // NOTE: 旋法チェンジ判定不能なものをはねる
            // MEMO: C キーにおける A の表示の仕方は後で考える
            Mode modeKeyMaybe = modeKeyMaybeBy(degree, spanMode); // span の度数と旋法からこの旋法と推測される
            int[] scaleKey = scale7By(key, modeKeyMaybe); // この旋法と曲キーで作成したスケールの構成音が同じになるはずである
            int noteDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取得
            int[] scaleSpan = scale7By(Key.Enum.Parse(noteDegree), spanMode); // span の度数と旋法でもスケールを作成してみる
            if (!compareScale(scaleKey, scaleSpan)) { // 比較して構成音は同じはず
                return Mode.Undefined; // 構成音が違う場合
            }
            return modeKeyMaybe; // TODO: これで正しいかテストを作成して確認
        }

        /// <summary>
        /// メジャーかマイナーかシンプルなコードネームを返します
        /// </summary>
        public static string ToSimpleCodeName(Key key, Degree degree, Mode keyMode, Mode spanMode, bool autoMode = true) {
            int noteDegree = noteRootBy(key, degree, keyMode); // 曲のキーの度数と旋法から度数のルート音を取得
            string codeBase = Key.Enum.Parse(noteDegree).ToString(); // コードネームの基本取得
            Mode mode;
            if (autoMode) { // 自動旋法取得
                mode = modeSpanBy(degree, keyMode);
            } else {
                mode = spanMode; // Spanの旋法適用
            }
            string majorOrMinerString = majorOrMiner(mode) ? "" : "m";
            return codeBase + majorOrMinerString;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// スケールの構成音を比較します
        /// </summary>
        static bool compareScale(int[] scale1, int[] scale2) {
            List<Key> list1 = scale1.Select(x => Key.Enum.Parse(x)).OrderBy(x => x).ToList();
            List<Key> list2 = scale2.Select(x => Key.Enum.Parse(x)).OrderBy(x => x).ToList();
            var result = list1.Where(x => !list2.Contains(x));
            if (result.Count() == 0) {
                return true; // 構成音が一致
            }
            return false; // 構成音が異なる
        }

        /// <summary>
        /// その旋法がメジャーかマイナーかbool値を返します
        /// </summary>
        static bool majorOrMiner(Mode mode) {
            switch (mode) {
                case Mode.Lyd:
                case Mode.Ion:
                case Mode.Mix:
                    return true;
                case Mode.Dor:
                case Mode.Aeo:
                case Mode.Phr:
                case Mode.Loc:
                    return false;
                default:
                    throw new ArgumentException("invalid mode.");
            }
        }

        /// <summary>
        /// 1～9のコード記法から Note No の配列を返します
        /// </summary>
        static int[] noteArrayBy(int index, int[] scale7) {
            // TODO: バリデート
            int[] note2 = new int[2]; // chord 構成音の2音
            int[] note3 = new int[3]; // chord 構成音の3音
            int[] note4 = new int[4]; // chord 構成音の4音
            switch (index) {
                case 3: // e.g. C, Cm, Cm(b5)
                    note3[0] = scale7[1 - 1];
                    note3[1] = scale7[3 - 1];
                    note3[2] = scale7[5 - 1];
                    return note3;
                case 4: // e.g. C(#4), Csus4, Csus4(-5)
                    note3[0] = scale7[1 - 1];
                    note3[1] = scale7[4 - 1];
                    note3[2] = scale7[5 - 1];
                    return note3;
                case 5: // e.g. C5(no3), C(-5,no3)
                    note2[0] = scale7[1 - 1];
                    note2[1] = scale7[5 - 1];
                    return note2;
                case 6: // e.g. C6, Cm6, Cm(b6), Cm(-5,b6)
                    note4[0] = scale7[1 - 1];
                    note4[1] = scale7[3 - 1];
                    note4[2] = scale7[5 - 1];
                    note4[3] = scale7[6 - 1];
                    return note4;
                case 7: // e.g. CM7, C7, Cm7, Cm7(-5)
                    note4[0] = scale7[1 - 1];
                    note4[1] = scale7[3 - 1];
                    note4[2] = scale7[5 - 1];
                    note4[3] = scale7[7 - 1];
                    return note4;
                // TODO: 8で2音オクターブ？
                case 9: // e.g. Cadd9, Cmadd9, Cm(b9), Cm(-5,b9)
                    note4[0] = scale7[1 - 1];
                    note4[1] = scale7[3 - 1];
                    note4[2] = scale7[5 - 1];
                    note4[3] = scale7[2 - 1] + 12; // 1オクターブ上: FIXME: Range の対応外
                    return note4;
                default:
                    throw new ArgumentException("invalid index.");
            }
        }

        static int noteRootBy(Key key, Degree degree, Mode keyMode) {
            key.Validate();
            degree.Validate();
            keyMode.Validate();
            int[] scale7 = scale7By(key, keyMode);
            return scale7[(int) degree];
        }

        static int[] noteArray3By(Key key, Degree degree, Mode keyMode) {
            key.Validate();
            degree.Validate();
            keyMode.Validate();
            int[] scale7 = scale7By(key, keyMode);
            int[] note3 = new int[3]; // コード構成音の3音を抽出
            switch (degree) {
                case Degree.I:
                    note3[0] = scale7[1 - 1];
                    note3[1] = scale7[3 - 1];
                    note3[2] = scale7[5 - 1];
                    break;
                case Degree.II:
                    note3[0] = scale7[2 - 1];
                    note3[1] = scale7[4 - 1];
                    note3[2] = scale7[6 - 1];
                    break;
                case Degree.III:
                    note3[0] = scale7[3 - 1];
                    note3[1] = scale7[5 - 1];
                    note3[2] = scale7[7 - 1];
                    break;
                case Degree.IV:
                    note3[0] = scale7[4 - 1];
                    note3[1] = scale7[6 - 1];
                    note3[2] = scale7[1 - 1];
                    break;
                case Degree.V:
                    note3[0] = scale7[5 - 1];
                    note3[1] = scale7[7 - 1];
                    note3[2] = scale7[2 - 1];
                    break;
                case Degree.VI:
                    note3[0] = scale7[6 - 1];
                    note3[1] = scale7[1 - 1];
                    note3[2] = scale7[3 - 1];
                    break;
                case Degree.VII:
                    note3[0] = scale7[7 - 1];
                    note3[1] = scale7[2 - 1];
                    note3[2] = scale7[4 - 1];
                    break;
            }
            return note3;
        }

        static int[] noteArray4By(Key key, Degree degree, Mode keyMode) {
            key.Validate();
            degree.Validate();
            keyMode.Validate();
            int[] scale7 = scale7By(key, keyMode);
            int[] note4 = new int[4]; // コード構成音の4音を抽出
            switch (degree) {
                case Degree.I:
                    note4[0] = scale7[1 - 1];
                    note4[1] = scale7[3 - 1];
                    note4[2] = scale7[5 - 1];
                    note4[3] = scale7[7 - 1];
                    break;
                case Degree.II:
                    note4[0] = scale7[2 - 1];
                    note4[1] = scale7[4 - 1];
                    note4[2] = scale7[6 - 1];
                    note4[3] = scale7[1 - 1];
                    break;
                case Degree.III:
                    note4[0] = scale7[3 - 1];
                    note4[1] = scale7[5 - 1];
                    note4[2] = scale7[7 - 1];
                    note4[3] = scale7[2 - 1];
                    break;
                case Degree.IV:
                    note4[0] = scale7[4 - 1];
                    note4[1] = scale7[6 - 1];
                    note4[2] = scale7[1 - 1];
                    note4[3] = scale7[3 - 1];
                    break;
                case Degree.V:
                    note4[0] = scale7[5 - 1];
                    note4[1] = scale7[7 - 1];
                    note4[2] = scale7[2 - 1];
                    note4[3] = scale7[4 - 1];
                    break;
                case Degree.VI:
                    note4[0] = scale7[6 - 1];
                    note4[1] = scale7[1 - 1];
                    note4[2] = scale7[3 - 1];
                    note4[3] = scale7[5 - 1];
                    break;
                case Degree.VII:
                    note4[0] = scale7[7 - 1];
                    note4[1] = scale7[2 - 1];
                    note4[2] = scale7[4 - 1];
                    note4[3] = scale7[6 - 1];
                    break;
            }
            return note4;
        }

        // TODO: 5音を取り出すメソッド：7モードのペンタトニック

        /// <summary>
        /// MEMO: 有効？ 何の目的で使用？
        /// </summary>
        static int[] noteArray7By(Key key, Degree degree, Mode keyMode) {
            key.Validate();
            degree.Validate();
            keyMode.Validate();
            int[] scale7 = scale7By(key, keyMode);
            int[] note7 = new int[7]; // コード構成音の7音を抽出
            switch (degree) {
                case Degree.I:
                    note7[0] = scale7[1 - 1];
                    note7[1] = scale7[3 - 1];
                    note7[2] = scale7[5 - 1];
                    note7[3] = scale7[7 - 1];
                    note7[4] = scale7[2 - 1];
                    note7[5] = scale7[4 - 1];
                    note7[6] = scale7[6 - 1];
                    break;
                case Degree.II:
                    note7[0] = scale7[2 - 1];
                    note7[1] = scale7[4 - 1];
                    note7[2] = scale7[6 - 1];
                    note7[3] = scale7[1 - 1];
                    note7[4] = scale7[3 - 1];
                    note7[5] = scale7[5 - 1];
                    note7[6] = scale7[7 - 1];
                    break;
                case Degree.III:
                    note7[0] = scale7[3 - 1];
                    note7[1] = scale7[5 - 1];
                    note7[2] = scale7[7 - 1];
                    note7[3] = scale7[2 - 1];
                    note7[4] = scale7[4 - 1];
                    note7[5] = scale7[6 - 1];
                    note7[6] = scale7[1 - 1];
                    break;
                case Degree.IV:
                    note7[0] = scale7[4 - 1];
                    note7[1] = scale7[6 - 1];
                    note7[2] = scale7[1 - 1];
                    note7[3] = scale7[3 - 1];
                    note7[4] = scale7[5 - 1];
                    note7[5] = scale7[7 - 1];
                    note7[6] = scale7[2 - 1];
                    break;
                case Degree.V:
                    note7[0] = scale7[5 - 1];
                    note7[1] = scale7[7 - 1];
                    note7[2] = scale7[2 - 1];
                    note7[3] = scale7[4 - 1];
                    note7[4] = scale7[6 - 1];
                    note7[5] = scale7[1 - 1];
                    note7[6] = scale7[3 - 1];
                    break;
                case Degree.VI:
                    note7[0] = scale7[6 - 1];
                    note7[1] = scale7[1 - 1];
                    note7[2] = scale7[3 - 1];
                    note7[3] = scale7[5 - 1];
                    note7[4] = scale7[7 - 1];
                    note7[5] = scale7[2 - 1];
                    note7[6] = scale7[4 - 1];
                    break;
                case Degree.VII:
                    note7[0] = scale7[7 - 1];
                    note7[1] = scale7[2 - 1];
                    note7[2] = scale7[4 - 1];
                    note7[3] = scale7[6 - 1];
                    note7[4] = scale7[1 - 1];
                    note7[5] = scale7[3 - 1];
                    note7[6] = scale7[5 - 1];
                    break;
            }
            return note7;
        }

        /// <summary>
        /// キーと旋法からモードスケールを返します
        /// </summary>
        static int[] scale7By(Key key, Mode keyMode) {
            key.Validate();
            keyMode.Validate();
            int noteRoot = (int) key;
            int[] scale7 = new int[7]; // 7音の旋法を作成
            switch (keyMode) {
                case Mode.Lyd:
                    scale7[0] = noteRoot; // 1
                    scale7[1] = noteRoot + 2; // 2
                    scale7[2] = noteRoot + 4; // 3
                    scale7[3] = noteRoot + 6; // #4
                    scale7[4] = noteRoot + 7; // 5
                    scale7[5] = noteRoot + 9; // 6
                    scale7[6] = noteRoot + 11; // 7
                    break;
                case Mode.Ion:
                    scale7[0] = noteRoot; // 1
                    scale7[1] = noteRoot + 2; // 2
                    scale7[2] = noteRoot + 4; // 3
                    scale7[3] = noteRoot + 5; // 4
                    scale7[4] = noteRoot + 7; // 5
                    scale7[5] = noteRoot + 9; // 6
                    scale7[6] = noteRoot + 11; // 7
                    break;
                case Mode.Mix:
                    scale7[0] = noteRoot; // 1
                    scale7[1] = noteRoot + 2; // 2
                    scale7[2] = noteRoot + 4; // 3
                    scale7[3] = noteRoot + 5; // 4
                    scale7[4] = noteRoot + 7; // 5
                    scale7[5] = noteRoot + 9; // 6
                    scale7[6] = noteRoot + 10; // b7
                    break;
                case Mode.Dor:
                    scale7[0] = noteRoot; // 1
                    scale7[1] = noteRoot + 2; // 2
                    scale7[2] = noteRoot + 3; // b3
                    scale7[3] = noteRoot + 5; // 4
                    scale7[4] = noteRoot + 7; // 5
                    scale7[5] = noteRoot + 9; // 6
                    scale7[6] = noteRoot + 10; // b7
                    break;
                case Mode.Aeo:
                    scale7[0] = noteRoot; // 1
                    scale7[1] = noteRoot + 2; // 2
                    scale7[2] = noteRoot + 3; // b3
                    scale7[3] = noteRoot + 5; // 4
                    scale7[4] = noteRoot + 7; // 5
                    scale7[5] = noteRoot + 8; // b6
                    scale7[6] = noteRoot + 10; // b7
                    break;
                case Mode.Phr:
                    scale7[0] = noteRoot; // 1
                    scale7[1] = noteRoot + 1; // b2
                    scale7[2] = noteRoot + 3; // b3
                    scale7[3] = noteRoot + 5; // 4
                    scale7[4] = noteRoot + 7; // 5
                    scale7[5] = noteRoot + 8; // b6
                    scale7[6] = noteRoot + 10; // b7
                    break;
                case Mode.Loc:
                    scale7[0] = noteRoot; // 1
                    scale7[1] = noteRoot + 1; // b2
                    scale7[2] = noteRoot + 3; // b3
                    scale7[3] = noteRoot + 5; // 4
                    scale7[4] = noteRoot + 6; // b5
                    scale7[5] = noteRoot + 8; // b6
                    scale7[6] = noteRoot + 10; // b7
                    break;
            }
            return scale7;
        }

        /// <summary>
        /// MEMO: ここで引いた Mode と元々の曲 key でスケールを作成してルート音が合っているか調べる
        /// TODO: ※仮 -- Span の度数と旋法でキーの旋法を返す
        /// TODO: C キーで Am が A になる時は？ ⇒ ※暫定: 後続でチェックして Mode.Undefined を返すようにした
        /// </summary>
        static Mode modeKeyMaybeBy(Degree degree, Mode spanMode) {
            spanMode.Validate();
            degree.Validate();
            if (spanMode is Mode.Lyd) {
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
            } else if (spanMode is Mode.Ion) {
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
            } else if (spanMode is Mode.Mix) {
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
            } else if (spanMode is Mode.Dor) {
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
            } else if (spanMode is Mode.Aeo) {
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
            } else if (spanMode is Mode.Phr) {
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
            } else if (spanMode is Mode.Loc) {
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
        /// 度数とキーの旋法から対応したその度数の旋法を返します
        /// </summary>
        static Mode modeSpanBy(Degree degree, Mode keyMode) {
            keyMode.Validate();
            degree.Validate();
            if (keyMode is Mode.Lyd) {
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
            } else if (keyMode is Mode.Ion) {
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
            } else if (keyMode is Mode.Mix) {
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
            } else if (keyMode is Mode.Dor) {
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
            } else if (keyMode is Mode.Aeo) {
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
            } else if (keyMode is Mode.Phr) {
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
            } else if (keyMode is Mode.Loc) {
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
        /// そのキーの旋法の度数の3和音構成音をランダムで返す
        /// </summary>
        static int noteOfRandom3By(Key key, Degree degree, Mode keyMode, Mode spanMode) {
            Mode mode = spanMode is Mode.Undefined ? modeSpanBy(degree, keyMode) : spanMode; // 自動旋法の場合度数とキーの旋法から対応したその度数の旋法を取得
            int[] scaleKey = scale7By(key, keyMode); // キーの旋法でスケールを取得
            int noteDegree = scaleKey[(int) degree]; // そのスケールの度数の音を取得
            int[] scale7 = scale7By(Key.Enum.Parse(noteDegree), mode); // その音のこの旋法でのスケールを取得
            int[] noteArray3 = noteArrayBy(3, scale7); // スケールから3和音を取得
            int random = Utils._random.Next(0, 3); // ランダム値作成: 0 から 2
            return noteArray3[random]; // そのスケールの3音のどれかを取得
        }

        // MEMO: 展開コードでキーボードの範囲(ゾーン)を指定するのはどうか
        // TODO: クローズドボイシングでルートのコードのソーンに追従する展開形を自動計算
    }
}
