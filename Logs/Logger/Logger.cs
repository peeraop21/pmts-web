using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace PMTs.Logs.Logger
{
    public static class Logger
    {
        private static ILog _log;

        public static void Log(string message)
        {
            EnsureLogger();

            _log.Info("Test log: {message}");
            // _log.Error($"Error Log");
        }

        public static void Info(string AppCaller, string factoryCode, string Controller, string router, string message)
        {
            EnsureLogger();
            _log.Info(AppCaller + "[" + factoryCode + "] " + Controller + ":" + router + " > " + message);
        }

        public static void Fatal(string message)
        {
            EnsureLogger();

            _log.Fatal("log:" + message);
            // _log.Error($"Error Log");
        }

        public static void Error(string AppCaller, string factoryCode, string Controller, string router, string message)
        {
            EnsureLogger();

            _log.Error(AppCaller + "[" + factoryCode + "] " + Controller + ":" + router + " > " + message);
            // _log.Error($"Error Log");
        }


        public static void Debug(string message)
        {
            EnsureLogger();

            _log.Debug("log:" + message);
            // _log.Error($"Error Log");
        }

        private static void EnsureLogger()
        {
            if (_log != null) return;

            var assembly = Assembly.GetEntryAssembly();
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var configFile = GetConfigFile();

            // Configure Log4Net
            XmlConfigurator.Configure(logRepository, configFile);
            _log = LogManager.GetLogger(assembly, assembly.ManifestModule.Name.Replace(".dll", "").Replace(".", " "));
        }

        private static FileInfo GetConfigFile()
        {
            FileInfo configFile = null;

            // Search config file
            var configFileNames = new[] { "Config/log4net.config", "log4net.config" };

            foreach (var configFileName in configFileNames)
            {
                configFile = new FileInfo(configFileName);

                if (configFile.Exists) break;
            }

            if (configFile == null || !configFile.Exists) throw new NullReferenceException("Log4net config file not found.");

            return configFile;
        }
    }
}
