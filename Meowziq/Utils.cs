/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Meowziq {
    /// <summary>
    /// definition of util functions.
    /// </summary>
    /// <note>
    /// + this should be top level. <br/>
    /// + do not do using the implementation package. <br/>
    /// + arguments must be primitive types or enums only. <br/>
    /// </note>
    /// <memo>
    /// do not define variables with var here.
    /// </memo>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Utils {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        static Random _random = new Random();

        ///////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// returns the length in 16beats for the index value of the argument.
        /// </summary>
        public static int To16beatLength(int index) {
            return Length.Of16beat.Int32() * index;
        }

        /// <summary>
        /// returns the length16beats in the count value (one beat) of the argument.
        /// </summary>
        public static int To16beatCount(int beat_count) {
            return beat_count * 4; // multiplies by 4 because it is the number of 16beats in one beat.
        }

        /// <summary>
        /// 1～9の Phrase コード記法から Note No の配列を取得します
        ///     Span に旋法なし：キーの旋法と度数から旋法に対応したその度数の旋法を取得
        ///     Span に旋法あり：Span に設定した旋法を使用
        /// </summary>
        public static int[] ToNoteArray(Key key, Degree degree, Mode key_mode, Mode span_mode, int index) {
            int degree_note = rootNoteBy(key: key, degree: degree, key_mode: key_mode); // 曲のキーの度数と旋法から度数のルート音を取得
            Mode mode = span_mode is Mode.Undefined ? spanModeBy(degree: degree, key_mode: key_mode) : span_mode; // Span に旋法がなければ自動旋法
            int[] scale7 = scale7By(key: Key.Enum.Parse(degree_note), key_mode: mode); // そのルート音の旋法スケールを取得
            return noteArrayBy(index: index, scale7: scale7); // 旋法スケールから引数indexに対応したコード構成音の配列を返す
        }

        /// <summary>
        /// 1～7の Phrase ノート記法から Note No を取得します
        /// ※ "auto": 自動的に旋法を決定、Span の旋法両対応
        ///     Span に旋法なし：キーの旋法と度数から旋法に対応したその度数の旋法を取得
        ///     Span に旋法あり：Span に設定した旋法を使用
        /// ※ "note": キーの旋法を自動判定し、その旋法の ノート記法の対応 Note No を返す
        /// </summary>
        public static int ToNote(Key key, Degree degree, Mode key_mode, Mode span_mode, int index, bool auto_note = true) {
            int degree_note = rootNoteBy(key: key, degree: degree, key_mode: key_mode); // 曲のキーの度数と旋法から度数のルート音を取得
            Mode mode = span_mode is Mode.Undefined ? spanModeBy(degree: degree, key_mode: key_mode) : span_mode; // Span に旋法がなければ自動旋法
            int[] scale7 = null;
            if (auto_note) { // Span ベースの旋法(自動・設定)
                scale7 = scale7By(key: Key.Enum.Parse(degree_note), key_mode: mode); // そのルート音の旋法スケールを取得
            } else { // キーの旋法を自動判定してそちらから取得
                Mode key_mode_current = ToKeyMode(key: key, degree: degree, key_mode: key_mode, span_mode: mode);
                if (key_mode_current is not Mode.Undefined) { // 旋法が判定出来れば
                    scale7 = scale7By(key: key, key_mode: key_mode_current); // 判定した曲の旋法を取得
                }
            }
            return scale7[index - 1]; // 0基底のスケール配列から引数添え字のノートを返す
        }

        /// <summary>
        /// gets random note from parameters.
        /// </summary>
        /// <fixme>
        /// FIXME: this
        /// </fixme>
        public static int ToRandomNote(Key key, Degree degree, Mode key_mode, Mode span_mode) {
            return noteOfRandom3By(key: key, degree: degree, key_mode: key_mode, span_mode: span_mode);
        }

        /// <summary>
        /// 度数とキーの旋法から旋法に対応したその度数の旋法を返します
        /// </summary>
        public static Mode ToSpanMode(Degree degree, Mode key_mode) {
            return spanModeBy(degree: degree, key_mode: key_mode);
        }

        /// <summary>
        /// Span の旋法から曲の旋法を判定する
        /// </summary>
        public static Mode ToKeyMode(Key key, Degree degree, Mode key_mode, Mode span_mode) {
            // NOTE: 旋法チェンジ判定不能なものをはねる
            // MEMO: C キーにおける A の表示の仕方は後で考える
            Mode key_mode_maybe = keyModeMaybeBy(degree: degree, span_mode: span_mode); // span の度数と旋法からこの旋法と推測される
            int[] key_scale = scale7By(key: key, key_mode: key_mode_maybe); // この旋法と曲キーで作成したスケールの構成音が同じになるはずである
            int degree_note = rootNoteBy(key: key, degree: degree, key_mode: key_mode); // 曲のキーの度数と旋法から度数のルート音を取得
            int[] span_scale = scale7By(key: Key.Enum.Parse(degree_note), key_mode: span_mode); // span の度数と旋法でもスケールを作成してみる
            if (!compareScale(scale1: key_scale, scale2: span_scale)) { // 比較して構成音は同じはず
                return Mode.Undefined; // 構成音が違う場合
            }
            return key_mode_maybe; // TODO: これで正しいかテストを作成して確認
        }

        /// <summary>
        /// メジャーかマイナーかシンプルなコードネームを返します
        /// </summary>
        public static string ToSimpleCodeName(Key key, Degree degree, Mode key_mode, Mode span_mode, bool auto_mode = true) {
            int degree_note = rootNoteBy(key: key, degree: degree, key_mode: key_mode); // 曲のキーの度数と旋法から度数のルート音を取得
            string code_base = Key.Enum.Parse(degree_note).ToString(); // コードネームの基本取得
            Mode mode;
            if (auto_mode) { // 旋法を取得する
                mode = spanModeBy(degree: degree, key_mode: key_mode);
            } else {
                mode = span_mode; // Spanの旋法をそのまま適用
            }
            string major_or_miner_string = majorOrMiner(mode: mode) ? string.Empty : "m";
            return code_base + major_or_miner_string;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// compares scale notes.
        /// </summary>
        static bool compareScale(int[] scale1, int[] scale2) {
            List<Key> list1 = scale1.Select(x => Key.Enum.Parse(x)).OrderBy(x => x).ToList();
            List<Key> list2 = scale2.Select(x => Key.Enum.Parse(x)).OrderBy(x => x).ToList();
            var result = list1.Where(x => !list2.Contains(x));
            if (result.Count() == 0) {
                return true; // scale notes match.
            }
            return false; // scale notes not match.
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
            // TODO: validate
            int[] note2 = new int[2]; // extracts two notes as chord notes from scale notes.
            int[] note3 = new int[3]; // extracts three notes as chord notes from scale notes.
            int[] note4 = new int[4]; // extracts four notes as chord notes from scale notes.
            switch (index) {
                // FIXME: index = 2.
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
                // TODO: two octaves in 8?
                case 9: // e.g. Cadd9, Cmadd9, Cm(b9), Cm(-5,b9)
                    note4[0] = scale7[1 - 1];
                    note4[1] = scale7[3 - 1];
                    note4[2] = scale7[5 - 1];
                    note4[3] = scale7[2 - 1] + 12; // 1 octave up. // FIXME: Considers not supported Range.
                    return note4;
                default:
                    throw new ArgumentException("invalid index or scale7.");
            }
        }

        /// <summary>
        /// returns root note number from the key, the degree, and the mode of the song.
        /// </summary>
        static int rootNoteBy(Key key, Degree degree, Mode key_mode) {
            key.Validate();
            degree.Validate();
            key_mode.Validate();
            int[] scale7 = scale7By(key: key, key_mode: key_mode);
            return scale7[(int) degree];
        }

        /// <summary>
        /// コード構成音の3音を抽出
        /// </summary>
        static int[] noteArray3By(Key key, Degree degree, Mode key_mode) {
            key.Validate();
            degree.Validate();
            key_mode.Validate();
            int[] scale7 = scale7By(key: key, key_mode: key_mode);
            int[] note3 = new int[3];
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

        /// <summary>
        /// コード構成音の4音を抽出
        /// </summary>
        static int[] noteArray4By(Key key, Degree degree, Mode key_mode) {
            key.Validate();
            degree.Validate();
            key_mode.Validate();
            int[] scale7 = scale7By(key: key, key_mode: key_mode);
            int[] note4 = new int[4];
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

        // TODO: コード構成音の5音を取り出すメソッド：7モードのペンタトニック

        /// <summary>
        /// コード構成音の7音を抽出
        /// </summary>
        static int[] noteArray7By(Key key, Degree degree, Mode key_mode) {
            key.Validate();
            degree.Validate();
            key_mode.Validate();
            int[] scale7 = scale7By(key: key, key_mode: key_mode);
            int[] note7 = new int[7];
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
        /// returns a modal scale notes number from the key and the mode.
        /// </summary>
        static int[] scale7By(Key key, Mode key_mode) {
            key.Validate();
            key_mode.Validate();
            int root_note = (int) key;
            /// <note>
            /// creates a mode scale consisting of seven notes.
            /// </note>
            int[] scale7 = new int[7];
            switch (key_mode) {
                /// <note>
                /// lydian mode scale.
                /// </note>
                case Mode.Lyd:
                    scale7[0] = root_note; // 1
                    scale7[1] = root_note + 2; // 2
                    scale7[2] = root_note + 4; // 3
                    scale7[3] = root_note + 6; // #4
                    scale7[4] = root_note + 7; // 5
                    scale7[5] = root_note + 9; // 6
                    scale7[6] = root_note + 11; // 7
                    break;
                /// <note>
                /// ionian mode scale.
                /// </note>
                case Mode.Ion:
                    scale7[0] = root_note; // 1
                    scale7[1] = root_note + 2; // 2
                    scale7[2] = root_note + 4; // 3
                    scale7[3] = root_note + 5; // 4
                    scale7[4] = root_note + 7; // 5
                    scale7[5] = root_note + 9; // 6
                    scale7[6] = root_note + 11; // 7
                    break;
                /// <note>
                /// mixolydian mode scale.
                /// </note>
                case Mode.Mix:
                    scale7[0] = root_note; // 1
                    scale7[1] = root_note + 2; // 2
                    scale7[2] = root_note + 4; // 3
                    scale7[3] = root_note + 5; // 4
                    scale7[4] = root_note + 7; // 5
                    scale7[5] = root_note + 9; // 6
                    scale7[6] = root_note + 10; // b7
                    break;
                /// <note>
                /// dorian mode scale.
                /// </note>
                case Mode.Dor:
                    scale7[0] = root_note; // 1
                    scale7[1] = root_note + 2; // 2
                    scale7[2] = root_note + 3; // b3
                    scale7[3] = root_note + 5; // 4
                    scale7[4] = root_note + 7; // 5
                    scale7[5] = root_note + 9; // 6
                    scale7[6] = root_note + 10; // b7
                    break;
                /// <note>
                /// aeolian mode scale.
                /// </note>
                case Mode.Aeo:
                    scale7[0] = root_note; // 1
                    scale7[1] = root_note + 2; // 2
                    scale7[2] = root_note + 3; // b3
                    scale7[3] = root_note + 5; // 4
                    scale7[4] = root_note + 7; // 5
                    scale7[5] = root_note + 8; // b6
                    scale7[6] = root_note + 10; // b7
                    break;
                /// <note>
                /// phrygian mode scale.
                /// </note>
                case Mode.Phr:
                    scale7[0] = root_note; // 1
                    scale7[1] = root_note + 1; // b2
                    scale7[2] = root_note + 3; // b3
                    scale7[3] = root_note + 5; // 4
                    scale7[4] = root_note + 7; // 5
                    scale7[5] = root_note + 8; // b6
                    scale7[6] = root_note + 10; // b7
                    break;
                /// <note>
                /// locrian mode scale.
                /// </note>
                case Mode.Loc:
                    scale7[0] = root_note; // 1
                    scale7[1] = root_note + 1; // b2
                    scale7[2] = root_note + 3; // b3
                    scale7[3] = root_note + 5; // 4
                    scale7[4] = root_note + 6; // b5
                    scale7[5] = root_note + 8; // b6
                    scale7[6] = root_note + 10; // b7
                    break;
            }
            return scale7;
        }

        /// <summary>
        /// MEMO: ここで引いた Mode と元々の曲 key でスケールを作成してルート音が合っているか調べる
        /// TODO: ※仮 -- Span の度数と旋法でキーの旋法を返す
        /// TODO: C キーで Am が A になる時は？ ⇒ ※暫定: 後続でチェックして Mode.Undefined を返すようにした
        /// </summary>
        static Mode keyModeMaybeBy(Degree degree, Mode span_mode) {
            span_mode.Validate();
            degree.Validate();
            if (span_mode is Mode.Lyd) {
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
            } else if (span_mode is Mode.Ion) {
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
            } else if (span_mode is Mode.Mix) {
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
            } else if (span_mode is Mode.Dor) {
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
            } else if (span_mode is Mode.Aeo) {
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
            } else if (span_mode is Mode.Phr) {
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
            } else if (span_mode is Mode.Loc) {
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
            throw new ArgumentException("invalid degree or span_mode.");
        }

        /// <summary>
        /// 度数とキーの旋法から対応したその度数の旋法を返します
        /// </summary>
        static Mode spanModeBy(Degree degree, Mode key_mode) {
            key_mode.Validate();
            degree.Validate();
            if (key_mode is Mode.Lyd) {
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
            } else if (key_mode is Mode.Ion) {
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
            } else if (key_mode is Mode.Mix) {
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
            } else if (key_mode is Mode.Dor) {
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
            } else if (key_mode is Mode.Aeo) {
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
            } else if (key_mode is Mode.Phr) {
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
            } else if (key_mode is Mode.Loc) {
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
            throw new ArgumentException("invalid degree or key_mode.");
        }

        /// <summary>
        /// そのキーの旋法の度数の3和音構成音をランダムで返す
        /// </summary>
        static int noteOfRandom3By(Key key, Degree degree, Mode key_mode, Mode span_mode) {
            Mode mode = span_mode is Mode.Undefined ? spanModeBy(degree: degree, key_mode: key_mode) : span_mode; // 自動旋法の場合度数とキーの旋法から対応したその度数の旋法を取得
            int[] key_scale = scale7By(key: key, key_mode: key_mode); // キーの旋法でスケールを取得
            int degree_note = key_scale[(int) degree]; // そのスケールの度数の音を取得
            int[] scale7 = scale7By(key: Key.Enum.Parse(degree_note), key_mode: mode); // その音のこの旋法でのスケールを取得
            int[] note_array3 = noteArrayBy(index: 3, scale7: scale7); // スケールから3和音を取得
            int random = _random.Next(minValue: 0, maxValue: 3); // ランダム値作成: 0 から 2 // FIXME:
            return note_array3[random]; // そのスケールの3音のどれかを取得
        }

        // MEMO: 展開コードでキーボードの範囲(ゾーン)を指定するのはどうか
        // TODO: クローズドボイシングでルートのコードのソーンに追従する展開形を自動計算
    }
}
