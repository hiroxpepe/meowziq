
using NLog;

namespace Meowziq {
    /// <summary>
    /// ログ用ファサードクラス
    /// NOTE: NLog を使用
    /// </summary>
    public static class Log {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Logger _logger = LogManager.GetCurrentClassLogger();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static void Fatal(string target) {
            LogEventInfo eventInfo = new LogEventInfo(LogLevel.Fatal, _logger.Name, target);
            _logger.Log(typeof(Log), eventInfo);
        }

        public static void Error(string target) {
            LogEventInfo eventInfo = new LogEventInfo(LogLevel.Error, _logger.Name, target);
            _logger.Log(typeof(Log), eventInfo);
        }

        public static void Warn(string target) {
            LogEventInfo eventInfo = new LogEventInfo(LogLevel.Warn, _logger.Name, target);
            _logger.Log(typeof(Log), eventInfo);
        }

        public static void Info(string target) {
            LogEventInfo eventInfo = new LogEventInfo(LogLevel.Info, _logger.Name, target);
            _logger.Log(typeof(Log), eventInfo);
        }

        public static void Debug(string target) {
#if DEBUG
            LogEventInfo eventInfo = new LogEventInfo(LogLevel.Debug, _logger.Name, target);
            _logger.Log(typeof(Log), eventInfo);
#endif
        }

        public static void Trace(string target) {
#if DEBUG
            LogEventInfo eventInfo = new LogEventInfo(LogLevel.Trace, _logger.Name, target);
            _logger.Log(typeof(Log), eventInfo);
#endif
        }
    }
}
