
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

using System;

using Meowziq.Value;

namespace MeowziqTest.Value {
    [TestClass()]
    public class ValidaterTests {

        /// <summary>
        /// 正常データと判定
        /// </summary>
        [TestMethod()]
        public void PhraseValueTest1() {
            var result = Validater.PhraseValue("[1111|----|5555|----]");
            AreEqual(result, "[1111|----|5555|----]");
        }

        /// <summary>
        /// 不正データと判定されたら例外が投げられる
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void PhraseValueTest2() {
            Validater.PhraseValue("[--1--|----|----|----]");
        }
    }
}
