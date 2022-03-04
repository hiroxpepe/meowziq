
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Meowziq.IO.Tests {
    [TestClass()]
    public class ExtensionsTests {

        /// <summary>
        /// ※ C# の仕様で出来ない
        /// </summary>
        [TestMethod()]
        public void SetTest() {
            var text = "hoge";
            var target = "piyo";
            text.Set(target);
            AreEqual(text, "hoge");
        }

        /// <summary>
        /// ※ C# の仕様で出来ない
        /// </summary>
        [TestMethod()]
        public void ClearTest() {
            var text = "hoge";
            text.Clear();
            AreEqual(text, "hoge");
        }
    }
}
