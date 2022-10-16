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

using NLog;

namespace Meowziq {
    /// <summary>
    /// facade class for logging.
    /// </summary>
    /// <note>
    /// use NLog.
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Log {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Logger _logger = LogManager.GetCurrentClassLogger();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static void Fatal(string target) {
            LogEventInfo event_info = new(LogLevel.Fatal, _logger.Name, target);
            _logger.Log(typeof(Log), event_info);
        }

        public static void Error(string target) {
            LogEventInfo event_info = new(LogLevel.Error, _logger.Name, target);
            _logger.Log(typeof(Log), event_info);
        }

        public static void Warn(string target) {
            LogEventInfo event_info = new(LogLevel.Warn, _logger.Name, target);
            _logger.Log(typeof(Log), event_info);
        }

        public static void Info(string target) {
            LogEventInfo event_info = new(LogLevel.Info, _logger.Name, target);
            _logger.Log(typeof(Log), event_info);
        }

        public static void Debug(string target) {
#if DEBUG
            LogEventInfo event_info = new(LogLevel.Debug, _logger.Name, target);
            _logger.Log(typeof(Log), event_info);
#endif
        }

        public static void Trace(string target) {
#if DEBUG
            LogEventInfo event_info = new(LogLevel.Trace, _logger.Name, target);
            _logger.Log(typeof(Log), event_info);
#endif
        }
    }
}
