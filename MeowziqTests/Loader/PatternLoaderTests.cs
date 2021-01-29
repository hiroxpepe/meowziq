
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meowziq.Loader.Tests {
    [TestClass()]
    public class PatternLoaderTests {

        // BuildPatternList

        [TestMethod()]
        public void BuildTest() {
            var _list = PatternLoader.Build(@"./data/pattern.json"); // デバッグ用
        }
    }
}