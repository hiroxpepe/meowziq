
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meowziq.Loader {
    /// <summary>
    /// テスト用拡張メソッド
    /// </summary>
    public static class Extensions {
        public static string validateValue(this PhraseLoader clazz, string target) {
            return (string) new PrivateObject(clazz).Invoke("validateValue", target);
        }
    }
}
