
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using Meowziq.IO;

namespace Meowziq.Loader.Tests {
    [TestClass()]
    public class PatternLoaderTests {

        /// <summary>
        /// デバッグで使用
        /// </summary>
        [TestMethod()]
        public void BuildTest() {
            using (var _stream = new StreamReader($"./data/pattern.json")) {
                var _list = PatternLoader.Build(_stream.ReadToEnd().ToMemoryStream());
            }
        }
    }
}