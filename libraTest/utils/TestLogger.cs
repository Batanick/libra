using System;
using libra.core.utils;
using NLog;

namespace libraTest.core
{
    public class TestLogger : IResourceLogger
    {
        private readonly Logger _logger;

        private TestLogger(Logger logger)
        {
            _logger = logger;
        }

        public static IResourceLogger Create(string loggerName)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
            var logger = LogManager.GetLogger(loggerName);
            return new TestLogger(logger);
        }

        public void LogInfo(string msg)
        {
            _logger.Info(msg);
        }

        public void LogWarn(string msg)
        {
            _logger.Warn(msg);
        }

        public void LogError(string msg)
        {
            _logger.Error(msg);
        }
    }
}