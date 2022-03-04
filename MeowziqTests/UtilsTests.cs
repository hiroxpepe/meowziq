
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Meowziq.Tests {
    [TestClass()]
    public class UtilsTests {

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:I ⇒ Cコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonI() {
            var note = Utils.ToNoteRandom(Key.C, Degree.I, Mode.Ion, Mode.Undefined);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.C).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.E).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.G).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:II ⇒ Dmコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonII() {
            var note = Utils.ToNoteRandom(Key.C, Degree.II, Mode.Ion, Mode.Undefined);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.D).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.F).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.A).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:III ⇒ Emコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonIII() {
            var note = Utils.ToNoteRandom(Key.C, Degree.III, Mode.Ion, Mode.Undefined);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.E).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.G).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.B).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:IV ⇒ Fコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonIV() {
            var note = Utils.ToNoteRandom(Key.C, Degree.IV, Mode.Ion, Mode.Undefined);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.F).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.A).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.C).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:V ⇒ Gコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonV() {
            var note = Utils.ToNoteRandom(Key.C, Degree.V, Mode.Ion, Mode.Undefined);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.G).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.B).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.D).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:VI ⇒ Amコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonVI() {
            var note = Utils.ToNoteRandom(Key.C, Degree.VI, Mode.Ion, Mode.Undefined);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.A).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.C).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.E).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Ion, spanMode:Undefined, Degree:VII ⇒ Bm-5コード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestKeyModeCIonVII() {
            var note = Utils.ToNoteRandom(Key.C, Degree.VII, Mode.Ion, Mode.Undefined);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.B).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.D).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.F).ToString())
            );
        }

        // ============================================================================================

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:I ⇒ Cコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonI() {
            var note = Utils.ToNoteRandom(Key.C, Degree.I, Mode.Aeo, Mode.Ion);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.C).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.E).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.G).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:II ⇒ Dコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonII() {
            var note = Utils.ToNoteRandom(Key.C, Degree.II, Mode.Aeo, Mode.Ion);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.D).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.Gb).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.A).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:III ⇒ Ebコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonIII() {
            var note = Utils.ToNoteRandom(Key.C, Degree.III, Mode.Aeo, Mode.Ion);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.Eb).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.G).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.Bb).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:IV ⇒ Fコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonIV() {
            var note = Utils.ToNoteRandom(Key.C, Degree.IV, Mode.Aeo, Mode.Ion);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.F).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.A).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.C).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:V ⇒ Gコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonV() {
            var note = Utils.ToNoteRandom(Key.C, Degree.V, Mode.Aeo, Mode.Ion);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.G).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.B).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.D).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:VI ⇒ Abコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonVI() {
            var note = Utils.ToNoteRandom(Key.C, Degree.VI, Mode.Aeo, Mode.Ion);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.Ab).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.C).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.Eb).ToString())
            );
        }

        /// <summary>
        /// Cキー: keyMode:Aeo, spanMode:Ion, Degree:VII ⇒ Bbコード
        /// </summary>
        [TestMethod()]
        public void ToNoteRandomTestSpanModeCIonVII() {
            var note = Utils.ToNoteRandom(Key.C, Degree.VII, Mode.Aeo, Mode.Ion);
            var noteToKey = Key.Enum.Parse(note);
            Log.Info($"note: {noteToKey}");
            IsTrue(
                ((int) noteToKey).ToString().Contains(((int) Key.Bb).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.D).ToString()) ||
                ((int) noteToKey).ToString().Contains(((int) Key.F).ToString())
            );
        }
    }
}
