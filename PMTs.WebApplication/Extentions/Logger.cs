using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Extentions
{
    public class Loggers
    {
        public static ILog log { get; set; }

        public void setConfig(string plateCode, string UserName)
        {
            GlobalContext.Properties["PlantCode"] = plateCode;
            GlobalContext.Properties["UserName"] = UserName;
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            log = LogManager.GetLogger(typeof(Program));
            //log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public static void Info(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            log.Info(formatLog(filePath, memberName, lineNumber, message));
        }

        public static void Debug(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            log.Debug(formatLog(filePath, memberName, lineNumber, message));
        }

        public static void Error(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string filePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            log.Error(formatLog(filePath, memberName, lineNumber, message));
        }

        private static string formatLog(string filePath, string memberName, int lineNumber, string msg)
        {
            string[] words = filePath.Split('\\');
            string className = words[words.Length - 1].Replace(".cs", ".");
            return className + memberName + "(" + lineNumber.ToString() + ") => " + msg;
        }



    }
}
