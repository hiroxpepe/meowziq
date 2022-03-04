
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

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
            var result = new PrivateType(typeof(Inheritor)).InvokeStatic("applyString", "[****|****|****|x-x-]", "[xxxx|xxxx|xxxx|xxxx]");
            AreEqual(result, "[xxxx|xxxx|xxxx|x-x-]");
        }

        /// <summary>
        /// target が "" の場合 baze のデータを返す
        /// </summary>
        [TestMethod()]
        public void applyStringTest3() {
            var result = new PrivateType(typeof(Inheritor)).InvokeStatic("applyString", "", "[xxxx|xxxx|xxxx|xxxx]");
            AreEqual(result, "[xxxx|xxxx|xxxx|xxxx]");
        }

        /// <summary>
        /// 配列数が異なる場合は例外が投げられる
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void applyArrrayTest1() {
            var target = new string[] {
                "[****|****|****|x-x-]"
            };
            var baze = new string[] {
                "[****|****|****|x-x-]",
                "[****|****|****|-xx-]"
            };
            new PrivateType(typeof(Inheritor)).InvokeStatic("applyArray", target, baze);
        }

        /// <summary>
        /// 正常データ変換
        /// </summary>
        [TestMethod()]
        public void applyArrrayTest2() {
            var target = new string[] {
                "[****|****|****|x-x-]",
                "[x-x-|****|****|****]"
            };
            var baze = new string[] {
                "[xxxx|xxxx|xxxx|xxxx]",
                "[xxxx|xxxx|xxxx|xxxx]"
            };
            var expected = new string[] {
                "[xxxx|xxxx|xxxx|x-x-]",
                "[x-x-|xxxx|xxxx|xxxx]"
            };
            var result = (string[]) new PrivateType(typeof(Inheritor)).InvokeStatic("applyArray", target, baze);
            CollectionAssert.AreEqual(result, expected);
        }

        /// <summary>
        /// 正常データ変換
        /// </summary>
        [TestMethod()]
        public void ApplyTest() {
            var target = new Core.Phrase();
            target.Data.Note.Text   = "[****|1**3|****|****][****|****|>>1>|--3-]";
            var baze = new Core.Phrase();
            baze.Data.Note.Text     = "[7>--|4>>5|>>7>|>>5>][7>--|4>>5|>>7>|>>5>]";
            var expected = new Core.Phrase();
            expected.Data.Note.Text = "[7>--|1>>3|>>7>|>>5>][7>--|4>>5|>>1>|--3-]";
            var result = Inheritor.Apply(target, baze);
            AreEqual(result.Data.Note.Text, expected.Data.Note.Text);
        }
    }
}
