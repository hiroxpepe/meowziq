using System;
using System.Windows.Forms;

namespace Meowziq.View {
    static class Program {
        /// <summary>
        /// the main entry point of the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
