
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

using System;

namespace Meowziq.Tests {
    [TestClass()]
    public class ExtensionsTests {

        [TestMethod()]
        public void HasValueTest() {
            string target1 = null;
            IsFalse(target1.HasValue());

            string target2 = "";
            IsFalse(target2.HasValue());

            string target3 = "hoge";
            IsTrue(target3.HasValue());
        }

        /// <summary>
        /// 正常系
        /// </summary>
        [TestMethod()]
        public void Int32Test1() {
            char target0 = '0';
            AreEqual(target0.Int32(), 0);
            char target1 = '1';
            AreEqual(target1.Int32(), 1);
            char target2 = '2';
            AreEqual(target2.Int32(), 2);
            char target3 = '3';
            AreEqual(target3.Int32(), 3);
            char target4 = '4';
            AreEqual(target4.Int32(), 4);
            char target5 = '5';
            AreEqual(target5.Int32(), 5);
            char target6 = '6';
            AreEqual(target6.Int32(), 6);
            char target7 = '7';
            AreEqual(target7.Int32(), 7);
            char target8 = '8';
            AreEqual(target8.Int32(), 8);
            char target9 = '9';
            AreEqual(target9.Int32(), 9);
        }

        /// <summary>
        /// 異常系
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Int32Test2() {
            char targetA = 'A';
            var result = targetA.Int32();
        }
    }
}
