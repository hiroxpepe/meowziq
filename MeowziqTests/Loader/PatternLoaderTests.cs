
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meowziq.Loader.Tests {
    [TestClass()]
    public class PatternLoaderTests {

        /// <summary>
        /// デバッグで使用
        /// </summary>
        [TestMethod()]
        public void BuildTest() {
            var _list = PatternLoader.Build(@"./data/pattern.json");
        }
    }
}