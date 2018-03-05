using AuthorizationCenter.Managers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using XYH.Core.Log;

namespace AuthorizationCenter
{
    public static class Program
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

            XYH.Core.Log.LogLevels logLevel = XYH.Core.Log.LogLevels.Info;
            int maxDays = 7;
            var logConfig = configuration.GetSection("Log");
            string maxFileSize = "10MB";
            if (logConfig != null)
            {

                Enum.TryParse<XYH.Core.Log.LogLevels>(logConfig["Level"] ?? "", out logLevel);
                int.TryParse(logConfig["SaveDays"], out maxDays);
                maxFileSize = logConfig["MaxFileSize"];
                if (string.IsNullOrEmpty(maxFileSize))
                {
                    maxFileSize = "10MB";
                }
            }

            string logFolder = System.IO.Path.Combine(AppContext.BaseDirectory, "Logs");
            XYH.Core.Log.LoggerManager.InitLogger(new XYH.Core.Log.LogConfig()
            {
                LogBaseDir = logFolder,
                MaxFileSize = maxFileSize,
                LogLevels = logLevel,
                IsAsync = true,
                LogFileTemplate = XYH.Core.Log.LogFileTemplates.PerDayDirAndLogger,
                LogContentTemplate = XYH.Core.Log.LogLayoutTemplates.SimpleLayout,
            });
            LoggerManager.SetLoggerAboveLevels(logLevel);
            LoggerManager.StartClear(maxDays, logFolder, XYH.Core.Log.LoggerManager.GetLogger("clear"));

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
            if(CrashLogger!=null)
            {
                CrashLogger.Fatal("崩溃：\r\n{0}", e.ExceptionObject.ToString());
            }
        }
    }
}
