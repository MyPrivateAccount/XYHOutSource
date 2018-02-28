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

namespace DesktopPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
          

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
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


            return WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://*:{configuration["Port"]}")
                .UseStartup<Startup>()
                .Build();
        }
    }
}
