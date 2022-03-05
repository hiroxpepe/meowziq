
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

using System;
using System.Reflection;

namespace MeowziqTest {
    [TestClass()]
    public class PrivateTypeTest {
        [TestMethod()]
        public void InvokeStaticTest1() {
            var result = new PrivateType(typeof(Hoge)).Invoke("getString1");
            AreEqual("hoge", result);
        }
        [TestMethod()]
        public void InvokeStaticTest2() {
            var result = new PrivateType(typeof(Hoge)).InvokeStatic("getString2");
            AreEqual("hoge", result);
        }
        [TestMethod()]
        public void InvokeStaticTest3() {
            var result = new PrivateType(typeof(Hoge)).Invoke("getString3", "piyo");
            AreEqual("piyo", result);
        }
        [TestMethod()]
        public void InvokeStaticTest4() {
            var result = new PrivateType(typeof(Hoge)).InvokeStatic("getString4", "piyo");
            AreEqual("piyo", result);
        }
        [TestMethod()]
        public void InvokeStaticTest5() {
            var result = new PrivateType(typeof(Hoge)).Invoke("addInt1", 2, 3);
            AreEqual(5, result);
        }
        [TestMethod()]
        public void InvokeStaticTest6() {
            var result = new PrivateType(typeof(Hoge)).InvokeStatic("addInt2", 2, 3);
            AreEqual(5, result);
        }

        /// <summary>
        /// a test class.
        /// </summary>
        class Hoge {
            string getString1() {
                return "hoge";
            }
            static string getString2() {
                return "hoge";
            }
            string getString3(string value) {
                return value;
            }
            static string getString4(string value) {
                return value;
            }
            int addInt1(int x, int y) {
                return x + y;
            }
            static int addInt2(int x, int y) {
                return x + y;
            }
        }
    }

    /// <summary>
    /// instead of PrivateType
    /// https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualstudio.testtools.unittesting.privatetype.-ctor?view=visualstudiosdk-2022#microsoft-visualstudio-testtools-unittesting-privatetype-ctor(system-type)
    /// </summary>
    public class PrivateType {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        readonly Type _type;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public PrivateType(Type type) {
            _type = type;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public object InvokeStatic(string methodName, params object[] args) {
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static;
            try {
                return _type.InvokeMember(methodName, bindingFlags, null, null, args);
            } catch (Exception ex) {
                throw ex.InnerException;
            }
        }

        public object Invoke(string methodName, params object[] args) {
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance;
            try {
                return _type.InvokeMember(methodName, bindingFlags, null, Activator.CreateInstance(_type), args);
            } catch (Exception ex) {
                throw ex.InnerException;
            }
        }
    }
}
