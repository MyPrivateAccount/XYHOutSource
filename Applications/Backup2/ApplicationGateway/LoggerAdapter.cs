using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationGateway
{
    public class XYHLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new LoggerAdapter(categoryName);
        }

        public void Dispose()
        {
           
        }
    }

    class LoggerAdapter : ILogger
    {
        private XYH.Core.Log.ILogger _logger = null;
        
        public LoggerAdapter(string name)
        {
            _logger = XYH.Core.Log.LoggerManager.GetLogger(name ?? "global");
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            XYH.Core.Log.LogLevels _logLevels = XYH.Core.Log.LogLevels.Trace;
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logLevels = XYH.Core.Log.LogLevels.Trace;
                    break;
                case LogLevel.Critical:
                    _logLevels = XYH.Core.Log.LogLevels.Fatal;
                    break;
                case LogLevel.Debug:
                    _logLevels = XYH.Core.Log.LogLevels.Debug;
                    break;
                case LogLevel.Error:
                    _logLevels = XYH.Core.Log.LogLevels.Error;
                    break;
                case LogLevel.Information:
                    _logLevels = XYH.Core.Log.LogLevels.Info;
                    break;
                case LogLevel.Warning:
                    _logLevels = XYH.Core.Log.LogLevels.Warn;
                    break;
                default:
                    _logLevels = XYH.Core.Log.LogLevels.Trace;
                    break;

            }

            _logger.Log(_logLevels, "{0} {1}\r\n{2}", eventId.Id, eventId.Name??"", formatter(state, exception));

        }
    }
}
