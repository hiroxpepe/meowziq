
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Meowziq.Loader;

namespace Meowziq.Loader.Tests {
    [TestClass()]
    public class PhraseLoaderTests {

        // validateValue

        [TestMethod()]
        public void validateValueTest1() {
            var obj = new PhraseLoader("");
            var result = obj.validateValue("[1111|----|5555|----]"); // 正常データ
            Assert.AreEqual(result, true);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void validateValueTest2() {
            var obj = new PhraseLoader("");
            obj.validateValue("[--1--|----|----|----]"); // 例外をテスト
        }

        // BeatCount

        [TestMethod()]
        public void BeatCountTest1() {
            var obj = new PhraseLoader("[1111|----|5555|----]");
            var count = obj.BeatCount;
            Assert.AreEqual(count, 4);
        }


    }
}
