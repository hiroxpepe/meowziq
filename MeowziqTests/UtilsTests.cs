
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Meowziq.Tests {
    [TestClass()]
    public class UtilsTests {

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:I ⇒ Cコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonI() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.I, Mode.Ion, Mode.Undefined);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.C).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.E).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.G).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:II ⇒ Dmコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonII() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.II, Mode.Ion, Mode.Undefined);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.D).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.F).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.A).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:III ⇒ Emコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonIII() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.III, Mode.Ion, Mode.Undefined);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.E).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.G).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.B).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:IV ⇒ Fコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonIV() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.IV, Mode.Ion, Mode.Undefined);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.F).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.A).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.C).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:V ⇒ Gコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonV() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.V, Mode.Ion, Mode.Undefined);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.G).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.B).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.D).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:VI ⇒ Amコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonVI() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.VI, Mode.Ion, Mode.Undefined);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.A).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.C).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.E).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:VII ⇒ Bm-5コード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonVII() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.VII, Mode.Ion, Mode.Undefined);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.B).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.D).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.F).ToString())
            );
        }

        // ============================================================================================

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:I ⇒ Cコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonI() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.I, Mode.Aeo, Mode.Ion);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.C).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.E).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.G).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:II ⇒ Dコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonII() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.II, Mode.Aeo, Mode.Ion);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.D).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.Gb).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.A).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:III ⇒ Ebコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonIII() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.III, Mode.Aeo, Mode.Ion);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.Eb).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.G).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.Bb).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:IV ⇒ Fコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonIV() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.IV, Mode.Aeo, Mode.Ion);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.F).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.A).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.C).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:V ⇒ Gコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonV() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.V, Mode.Aeo, Mode.Ion);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.G).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.B).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.D).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:VI ⇒ Abコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonVI() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.VI, Mode.Aeo, Mode.Ion);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.Ab).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.C).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.Eb).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:VII ⇒ Bbコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonVII() {
            var _note = Utils.ToNoteRandom(Key.C, Degree.VII, Mode.Aeo, Mode.Ion);
            var _noteToKey = Key.Enum.Parse(_note);
            Log.Info($"note: {_noteToKey}");
            Assert.IsTrue(
                ((int) _noteToKey).ToString().Contains(((int) Key.Bb).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.D).ToString()) ||
                ((int) _noteToKey).ToString().Contains(((int) Key.F).ToString())
            );
        }
    }
}