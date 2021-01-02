using Meowziq.Loader;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meowziq.Loader.Tests {
    [TestClass()]
    public class PatternLoaderTests {

        // BuildPatternList

        [TestMethod()]
        public void GetPatternListTest() {
            var obj = new PatternLoader(@"./data/pattern.jsonc");
            var list = obj.BuildPatternList(); // デバッグ用
        }
    }
}