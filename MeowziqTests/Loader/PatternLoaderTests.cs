
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

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
            using (var stream = new StreamReader($"./data/pattern.json")) {
                var list = PatternLoader.Build(stream.ReadToEnd().ToMemoryStream());
            }
        }
    }
}
