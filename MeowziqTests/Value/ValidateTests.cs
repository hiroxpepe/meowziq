using Microsoft.VisualStudio.TestTools.UnitTesting;
using Meowziq.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowziq.Value.Tests {
    [TestClass()]
    public class ValidateTests {

        [TestMethod()]
        public void PhraseValueTest1() {
            var result = new PrivateType(typeof(Validate)).InvokeStatic("PhraseValue", "[1111|----|5555|----]"); // 正常データ
            Assert.AreEqual(result, "[1111|----|5555|----]");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void vPhraseValueTest2() {
            new PrivateType(typeof(Validate)).InvokeStatic("PhraseValue", "[--1--|----|----|----]"); // 例外をテスト
        }
    }
}