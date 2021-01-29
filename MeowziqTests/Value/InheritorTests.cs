
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Meowziq.Value.Tests {
    [TestClass()]
    public class InheritorTests {

        /// <summary>
        /// データ数が異なる場合は例外が投げられる
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void applyStringTest1() {
            new PrivateType(typeof(Inheritor)).InvokeStatic("applyString", "[****|****|****|x-x-*]", "[xxxx|xxxx|xxxx|xxxx]");
        }

        /// <summary>
        /// 正常データ変換
        /// </summary>
        [TestMethod()]
        public void applyStringTest2() {
            var _result = new PrivateType(typeof(Inheritor)).InvokeStatic("applyString", "[****|****|****|x-x-]", "[xxxx|xxxx|xxxx|xxxx]");
            Assert.AreEqual(_result, "[xxxx|xxxx|xxxx|x-x-]");
        }

        /// <summary>
        /// target が "" の場合 baze のデータを返す
        /// </summary>
        [TestMethod()]
        public void applyStringTest3() {
            var _result = new PrivateType(typeof(Inheritor)).InvokeStatic("applyString", "", "[xxxx|xxxx|xxxx|xxxx]");
            Assert.AreEqual(_result, "[xxxx|xxxx|xxxx|xxxx]");
        }

        /// <summary>
        /// 配列数が異なる場合は例外が投げられる
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void applyArrrayTest1() {
            var _target = new string[] {
                "[****|****|****|x-x-]"
            };
            var _base = new string[] {
                "[****|****|****|x-x-]",
                "[****|****|****|-xx-]"
            };
            new PrivateType(typeof(Inheritor)).InvokeStatic("applyArray", _target, _base);
        }

        /// <summary>
        /// 正常データ変換
        /// </summary>
        [TestMethod()]
        public void applyArrrayTest2() {
            var _target = new string[] {
                "[****|****|****|x-x-]",
                "[x-x-|****|****|****]"
            };
            var _base = new string[] {
                "[xxxx|xxxx|xxxx|xxxx]",
                "[xxxx|xxxx|xxxx|xxxx]"
            };
            var _expected = new string[] {
                "[xxxx|xxxx|xxxx|x-x-]",
                "[x-x-|xxxx|xxxx|xxxx]"
            };
            var _result = (string[]) new PrivateType(typeof(Inheritor)).InvokeStatic("applyArray", _target, _base);
            CollectionAssert.AreEqual(_result, _expected);
        }

        /// <summary>
        /// 正常データ変換
        /// </summary>
        [TestMethod()]
        public void ApplyTest() {
            var _target = new Data();
            _target.Note.Text   = "[****|1**3|****|****][****|****|>>1>|--3-]";
            var _baze = new Data();
            _baze.Note.Text     = "[7>--|4>>5|>>7>|>>5>][7>--|4>>5|>>7>|>>5>]";
            var _expected = new Data();
            _expected.Note.Text = "[7>--|1>>3|>>7>|>>5>][7>--|4>>5|>>1>|--3-]";
            var _result = Inheritor.Apply(_target, _baze);
            Assert.AreEqual(_result.Note.Text, _expected.Note.Text);
        }
    }
}
