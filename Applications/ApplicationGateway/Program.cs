using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XYH.Core.Log;

namespace ApplicationGateway
{
    public class Program
    {
        private static XYH.Core.Log.ILogger ExceptionLogger = null;
        private static XYH.Core.Log.ILogger CrashLogger = null;
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            LogLevels logLevel = LogLevels.Info;
            int maxDays = 7;
            var logConfig = configuration.GetSection("Log");
            string maxFileSize = "10MB";
            if (logConfig != null)
            {

                Enum.TryParse(logConfig["Level"] ?? "", out logLevel);
                int.TryParse(logConfig["SaveDays"], out maxDays);
                maxFileSize = logConfig["MaxFileSize"];
                if (string.IsNullOrEmpty(maxFileSize))
                {
                    maxFileSize = "10MB";
                }
            }
            string logFolder = Path.Combine(AppContext.BaseDirectory, "Logs");
            LoggerManager.InitLogger(new LogConfig()
            {
                LogBaseDir = logFolder,
                MaxFileSize = maxFileSize,
                LogLevels = logLevel,
                IsAsync = true,
                LogFileTemplate = LogFileTemplates.PerDayDirAndLogger,
                LogContentTemplate = LogLayoutTemplates.SimpleLayout,
            });
            LoggerManager.SetLoggerAboveLevels(logLevel);
            LoggerManager.StartClear(maxDays, logFolder, LoggerManager.GetLogger("clear"));

            ExceptionLogger = XYH.Core.Log.LoggerManager.GetLogger("Exception");
            CrashLogger = XYH.Core.Log.LoggerManager.GetLogger("Crash");

            //全局异常日志
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://*:{configuration["Port"]}")
                .Build();
            host.Run();

            //BuildWebHost(args).Run();
        }


        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            if (ExceptionLogger != null)
            {
                ExceptionLogger.Error("{0}", e.Exception.ToString());
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (CrashLogger != null)
            {
                CrashLogger.Fatal("崩溃：\r\n{0}", e.ExceptionObject.ToString());
            }
        }


    }
}
