
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

        [TestMethod()]
        public void ToByteTextTest() {
            var _text = "test";
            var _result = Converter.ToByteText(_text);
        }
    }
}