/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Windows.Forms;

namespace Meowziq.Win64 {
    /// <summary>
    /// Provides the main entry point and application context for Meowziq.Win64.
    /// </summary>
    /// <remarks>
    /// <item>This static class is responsible for application startup and context initialization.</item>
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    static class Program {
        /// <summary>
        /// Starts the Meowziq.Win64 application.
        /// </summary>
        /// <remarks>
        /// <item>Enables visual styles, sets compatible text rendering, and launches the main form.</item>
        /// </remarks>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
