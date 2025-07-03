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
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// The NLog logger instance for this class.
        /// </summary>
        static Logger _logger = LogManager.GetCurrentClassLogger();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Events [verb, verb phrase]

        public static event Changed? OnFatal;

        public static event Changed? OnError;

        public static event Changed? OnWarn;

        public static event Changed? OnInfo;

        public static event Changed? OnDebug;

        public static event Changed? OnTrace;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Logs a fatal error message.
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Fatal(string target) {
            LogEventInfo log_event = new(level: LogLevel.Fatal, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
            OnFatal?.Invoke(sender: null, e: new EvtArgs("Fatal") { Value = target });
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Error(string target) {
            LogEventInfo log_event = new(level: LogLevel.Error, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
            OnError?.Invoke(sender: null, e: new EvtArgs("Error") { Value = target });
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Warn(string target) {
            LogEventInfo log_event = new(level: LogLevel.Warn, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
            OnWarn?.Invoke(sender: null, e: new EvtArgs("Warn") { Value = target });
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Info(string target) {
            LogEventInfo log_event = new(level: LogLevel.Info, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
            OnInfo?.Invoke(sender: null, e: new EvtArgs("Info") { Value = target });
        }

        /// <summary>
        /// Logs a debug message (only in DEBUG builds).
        /// </summary>
        /// <param name="target">The message to log.</param>
        public static void Debug(string target) {
#if DEBUG
            LogEventInfo log_event = new(level: LogLevel.Debug, loggerName: _logger.Name, message: target);
            _logger.Log(wrapperType: typeof(Log), logEvent: log_event);
            OnDebug?.Invoke(sender: null, e: new EvtArgs("Debug") { Value = target });
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
            OnTrace?.Invoke(sender: null, e: new EvtArgs("Trace") { Value = target });
#endif
        }

        public static void ClearOnFatal() { OnFatal = null; }
        
        public static void ClearOnError() { OnError = null; }

        public static void ClearOnWarn() { OnWarn = null; }

        public static void ClearOnInfo() { OnInfo = null; }

        public static void ClearOnDebug() { OnDebug = null; }
        
        public static void ClearOnTrace() { OnTrace = null; }
    }
}