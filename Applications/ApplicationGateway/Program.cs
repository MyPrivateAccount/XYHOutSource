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

            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://*:{configuration["Port"]}")
                .Build();
            host.Run();

            //BuildWebHost(args).Run();
        }
    }
}
