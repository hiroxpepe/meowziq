
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meowziq.IO.Tests {
    [TestClass()]
    public class ExtensionsTests {

        /// <summary>
        /// ※ C# の仕様で出来ない
        /// </summary>
        [TestMethod()]
        public void SetTest() {
            var _text = "hoge";
            var _target = "piyo";
            _text.Set(_target);
            Assert.AreEqual(_text, "hoge");
        }

        /// <summary>
        /// ※ C# の仕様で出来ない
        /// </summary>
        [TestMethod()]
        public void ClearTest() {
            var _text = "hoge";
            _text.Clear();
            Assert.AreEqual(_text, "hoge");
        }
    }
}