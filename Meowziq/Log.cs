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
    /// Provides a facade for logging using NLog.
    /// </summary>
    /// <remarks>
    /// Uses NLog for all logging operations.
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Log {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// The NLog logger instance for this class.
        /// </summary>
        static Logger _logger = LogManager.GetCurrentClassLogger();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Logs a fatal error message.
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Fatal(string target) {
            LogEventInfo log_event = new(level: LogLevel.Fatal, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Error(string target) {
            LogEventInfo log_event = new(level: LogLevel.Error, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Warn(string target) {
            LogEventInfo log_event = new(level: LogLevel.Warn, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Info(string target) {
            LogEventInfo log_event = new(level: LogLevel.Info, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
        }

        /// <summary>
        /// Logs a debug message (only in DEBUG builds).
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Debug(string target) {
#if DEBUG
            LogEventInfo log_event = new(level: LogLevel.Debug, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
#endif
        }

        /// <summary>
        /// Logs a trace message (only in DEBUG builds).
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Trace(string target) {
#if DEBUG
            LogEventInfo log_event = new(level: LogLevel.Trace, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
#endif
        }
    }
}
