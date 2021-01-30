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
            var _intTempo = 120;
            var _expected = new byte[3]{
                    Convert.ToByte("07", 16),
                    Convert.ToByte("A1", 16),
                    Convert.ToByte("20", 16)
                };
            var _result = Converter.ToByteTempo(_intTempo);
            CollectionAssert.AreEqual(_result, _expected);
        }

        /// <summary>
        /// BPM 140
        /// </summary>
        [TestMethod()]
        public void ToByteTempoTest2() {
            var _intTempo = 140;
            var _expected = new byte[3]{
                    Convert.ToByte("06", 16),
                    Convert.ToByte("8A", 16),
                    Convert.ToByte("1B", 16)
                };
            var _result = Converter.ToByteTempo(_intTempo);
            CollectionAssert.AreEqual(_result, _expected);
        }
    }
}