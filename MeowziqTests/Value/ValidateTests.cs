using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Meowziq.Value.Tests {
    [TestClass()]
    public class ValidateTests {

        /// <summary>
        /// 正常データと判定
        /// </summary>
        [TestMethod()]
        public void PhraseValueTest1() {
            var result = new PrivateType(typeof(Validate)).InvokeStatic("PhraseValue", "[1111|----|5555|----]");
            Assert.AreEqual(result, "[1111|----|5555|----]");
        }

        /// <summary>
        /// 不正データと判定されたら例外が投げられる
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void vPhraseValueTest2() {
            new PrivateType(typeof(Validate)).InvokeStatic("PhraseValue", "[--1--|----|----|----]");
        }
    }
}