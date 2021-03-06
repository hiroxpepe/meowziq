﻿
using System;
using System.Linq;

namespace Meowziq.Value {
    /// <summary>
    /// NOTE: 入力値バリデーション クラス
    /// </summary>
    public static class Validater {
        /// <summary>
        /// TODO: 使用可能な文字の判定
        /// </summary>
        public static string PhraseValue(string target) {
            if (target is null) {
                return target; // 値がなければそのまま返す FIXME:
            }
            // 拍のデータの数が4文字かどうか
            var _target1 = target;
            _target1 = _target1.Replace("[", "|").Replace("]", "|"); // 文字の置き換え
            var _array1 = _target1.Split('|') // 区切り文字で切り分ける
                .Where(x => !string.IsNullOrWhiteSpace(x)) // 空文字以外が対象で
                .Where(x => x.Length != 4) // そのデータが4文字ではないものを抽出
                .ToArray();
            if (_array1.Length != 0) { // そのデータがあれば例外を投げる
                throw new FormatException("a beat data count must be 4.");
            }
            return target; // バリデーションOKなら元々の文字列を返す
        }
    }

    /// <summary>
    /// NOTE: 入力値用の Utils クラス
    /// </summary>
    internal class Utils {
        /// <summary>
        /// 不要文字 "[", "]", "|", を削除します
        /// </summary>
        internal static string Filter(string target) {
            return target.Replace("|", "").Replace("[", "").Replace("]", ""); // 不要文字削除
        }
    }

    /// <summary>
    /// 変換用クラス
    /// TODO: 拡張メソッドで十分？
    /// </summary>
    public static class Converter {
        /// <summary>
        /// 数値 BPM を SMF 用のTEMPO情報に変換します
        ///     BPM = 120 (1分あたり四分音符が120個) の場合、
        ///     四分音符の長さは 60 x 10の6乗 / 120 = 500,000 (μsec)
        ///     これを16進にすると 0x07A120 ⇒ 3バイトで表現すると "07", "A1", "20"
        /// </summary>
        public static byte[] ToByteTempo(int tempo) {
            var _double = 60 * Math.Pow(10, 6) / tempo;
            var _hex = int.Parse(Math.Round(_double).ToString()).ToString("X6"); // 16進数6桁変換
            var _charArray = _hex.ToCharArray();
            return new byte[3]{ // 3byte で返す
                Convert.ToByte(_charArray[0].ToString() + _charArray[1].ToString(), 16),
                Convert.ToByte(_charArray[2].ToString() + _charArray[3].ToString(), 16),
                Convert.ToByte(_charArray[4].ToString() + _charArray[5].ToString(), 16)
            };
        }

        /// <summary>
        /// 文字列を byte 配列に変換します
        /// </summary>
        public static byte[] ToByteArray(string target) {
            return target.ToCharArray().Select(x => Convert.ToByte(x)).ToArray();
        }
    }
}
