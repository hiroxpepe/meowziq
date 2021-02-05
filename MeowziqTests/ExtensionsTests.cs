
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Meowziq.Tests {
    [TestClass()]
    public class ExtensionsTests {

        [TestMethod()]
        public void HasValueTest() {
            string _target1 = null;
            Assert.IsFalse(_target1.HasValue());

            string _target2 = "";
            Assert.IsFalse(_target2.HasValue());

            string _target3 = "hoge";
            Assert.IsTrue(_target3.HasValue());
        }

        /// <summary>
        /// 正常系
        /// </summary>
        [TestMethod()]
        public void Int32Test1() {
            char _target0 = '0';
            Assert.AreEqual(_target0.Int32(), 0);
            char _target1 = '1';
            Assert.AreEqual(_target1.Int32(), 1);
            char _target2 = '2';
            Assert.AreEqual(_target2.Int32(), 2);
            char _target3 = '3';
            Assert.AreEqual(_target3.Int32(), 3);
            char _target4 = '4';
            Assert.AreEqual(_target4.Int32(), 4);
            char _target5 = '5';
            Assert.AreEqual(_target5.Int32(), 5);
            char _target6 = '6';
            Assert.AreEqual(_target6.Int32(), 6);
            char _target7 = '7';
            Assert.AreEqual(_target7.Int32(), 7);
            char _target8 = '8';
            Assert.AreEqual(_target8.Int32(), 8);
            char _target9 = '9';
            Assert.AreEqual(_target9.Int32(), 9);
        }

        /// <summary>
        /// 異常系
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Int32Test2() {
            char _targetA = 'A';
            var _result = _targetA.Int32();
        }
    }
}