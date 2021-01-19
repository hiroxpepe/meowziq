
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meowziq.Loader.Tests {
    [TestClass()]
    public class PhraseLoaderTests {

        // validateValue

        [TestMethod()]
        public void validateValueTest1() {
            var result = new PrivateType(typeof(PhraseLoader)).InvokeStatic("validateValue", "[1111|----|5555|----]"); // 正常データ
            Assert.AreEqual(result, "[1111|----|5555|----]");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void validateValueTest2() {
            new PrivateType(typeof(PhraseLoader)).InvokeStatic("validateValue", "[--1--|----|----|----]"); // 例外をテスト
        }
    }
}
