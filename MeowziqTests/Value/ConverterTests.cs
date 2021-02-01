
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Meowziq.Value.Tests {
    [TestClass()]
    public class ConverterTests {

        /// <summary>
        /// BPM 120
        /// </summary>
        [TestMethod()]
        public void ToByteTempoTest1() {
            var _tempo = 120;
            var _expected = new byte[3]{
                    Convert.ToByte("07", 16),
                    Convert.ToByte("A1", 16),
                    Convert.ToByte("20", 16)
                };
            var _result = Converter.ToByteTempo(_tempo);
            CollectionAssert.AreEqual(_result, _expected);
        }

        /// <summary>
        /// BPM 140
        /// </summary>
        [TestMethod()]
        public void ToByteTempoTest2() {
            var _tempo = 140;
            var _expected = new byte[3]{
                    Convert.ToByte("06", 16),
                    Convert.ToByte("8A", 16),
                    Convert.ToByte("1B", 16)
                };
            var _result = Converter.ToByteTempo(_tempo);
            CollectionAssert.AreEqual(_result, _expected);
        }

        /// <summary>
        /// 文字列を byte 配列で返す
        /// </summary>
        [TestMethod()]
        public void ToByteArrayTest1() {
            var _text = "Test";
            var _expected = new byte[4]{
                    Convert.ToByte("54", 16),
                    Convert.ToByte("65", 16),
                    Convert.ToByte("73", 16),
                    Convert.ToByte("74", 16)
                };
            var _result = Converter.ToByteArray(_text);
            _result.ToList().ForEach(x => {
                var _tmp = string.Format("{0:X2}", x);  // 確認用
            });
            CollectionAssert.AreEqual(_result, _expected);
        }
    }
}