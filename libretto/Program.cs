using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using libretto.libretto;
using Mono.Options;
using NLog;

namespace libretto
{
    public class Program
    {
        private static Logger _log = LogManager.GetLogger(nameof(Program));

        public static void Main(string[] args)
        {
            InitLogger();
            
            bool showHelp = false;
            var dir = "./";

            var p = new OptionSet
            {
                {
                    "d|dir=", "Directory to save resource schemas.",
                    d => dir = d
                },
                {
                    "h|help", "show this message and exit",
                    v => showHelp = v != null
                }
            };

            if (showHelp)
            {
                ShowHelp(p);
                return;
            }

            try
            {
                var extra = p.Parse(args);

                var assemblies = new List<Assembly>();
                foreach (var path in extra)
                {
                    _log.Info($"Loading assembly: {path}");
                    var assembly = Assembly.LoadFrom(path);
                    assemblies.Add(assembly);
                }

                AnalyseAssembly(assemblies);
            }
            catch (Exception e)
            {
                Console.Write("libretto: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `libretto --help' for more information.");
            }
        }

        private static void AnalyseAssembly(List<Assembly> assemblies)
        {
            _log.Info($"Analysing:");
            var classAnalyser = new ClassAnalyser();
            var resourceTypes = classAnalyser.Process(assemblies);
            _log.Info($"Finished: total {resourceTypes.Count} resource types");
        }

        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: libretto [OPTIONS]+ ASSEMBLY_PATH");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
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