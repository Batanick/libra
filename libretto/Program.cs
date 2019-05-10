using System;
using NLog;

namespace libretto
{
    class Program
    {
        static void Main(string[] args)
        {
            InitLogger();
            Console.WriteLine("Hello World!");
        }

        private static void InitLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
        }
    }
}