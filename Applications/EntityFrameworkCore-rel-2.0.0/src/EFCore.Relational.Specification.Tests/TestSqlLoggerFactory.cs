// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore
{
    public class TestSqlLoggerFactory : ILoggerFactory
    {
        private const string FileLineEnding = @"
";

        private static readonly string _newLine = Environment.NewLine;

        public void AssertBaseline(string[] expected, bool assertOrder = true)
        {
            var sqlStatements
                = _logger.SqlStatements
                    .Select(sql => sql.Replace(Environment.NewLine, FileLineEnding))
                    .ToList();

            try
            {
                if (assertOrder)
                {
                    for (var i = 0; i < expected.Length; i++)
                    {
                        Assert.Equal(expected[i], sqlStatements[i]);
                    }
                }
                else
                {
                    foreach (var expectedFragment in expected)
                    {
                        Assert.Contains(expectedFragment, sqlStatements);
                    }
                }
            }
            catch
            {
                var methodCallLine = Environment.StackTrace.Split(
                        new[] { Environment.NewLine },
                        StringSplitOptions.RemoveEmptyEntries)[4]
                    .Substring(6);

                var testName = methodCallLine.Substring(0, methodCallLine.IndexOf(')') + 1);
                var lineIndex = methodCallLine.LastIndexOf("line", StringComparison.Ordinal);
                var lineNumber = lineIndex > 0 ? methodCallLine.Substring(lineIndex) : "";

                const string indent = FileLineEnding + "                ";

                var currentDirectory = Directory.GetCurrentDirectory();
                var logFile = currentDirectory.Substring(
                                  0,
                                  currentDirectory.LastIndexOf("\\test\\", StringComparison.Ordinal) + 1)
                              + "QueryBaseline.cs";

                var testInfo = $"{testName + " : " + lineNumber}" + FileLineEnding;

                var newBaseLine = $@"            AssertSql(
                {string.Join("," + indent + "//" + indent, sqlStatements.Take(9).Select(sql => "@\"" + sql.Replace("\"", "\"\"") + "\""))});

";

                if (sqlStatements.Count > 9)
                {
                    newBaseLine += "Output truncated.";
                }

                _logger.TestOutputHelper?.WriteLine("---- New Baseline -------------------------------------------------------------------");
                _logger.TestOutputHelper?.WriteLine(newBaseLine);

                var contents = testInfo + newBaseLine + FileLineEnding + FileLineEnding;

                File.AppendAllText(logFile, contents);

                throw;
            }
        }

        private readonly Logger _logger = new Logger();

        public void Clear()
        {
            _logger.Clear();
        }

        public string Log => _logger.LogBuilder.ToString();

        public IReadOnlyList<string> SqlStatements => _logger.SqlStatements;

        public IReadOnlyList<string> Parameters => _logger.Parameters;

        public string Sql => string.Join(_newLine + _newLine, SqlStatements);

        public CancellationToken CancelQuery()
        {
            return _logger.CancelQuery();
        }

        public void SetTestOutputHelper(ITestOutputHelper testOutputHelper)
        {
            _logger.TestOutputHelper = testOutputHelper;
        }

        ILogger ILoggerFactory.CreateLogger(string categoryName) => _logger;

        void ILoggerFactory.AddProvider(ILoggerProvider provider) => throw new NotImplementedException();

        void IDisposable.Dispose()
        {
        }

        private sealed class Logger : ILogger
        {
            public IndentedStringBuilder LogBuilder { get; } = new IndentedStringBuilder();
            public List<string> SqlStatements { get; } = new List<string>();
            public List<string> Parameters { get; } = new List<string>();

            private CancellationTokenSource _cancellationTokenSource;

            public ITestOutputHelper TestOutputHelper { get; set; }

            private readonly object _sync = new object();

            public void Clear()
            {
                lock (_sync) // Guard against tests with explicit concurrency
                {
                    SqlStatements.Clear();
                    LogBuilder.Clear();
                    Parameters.Clear();

                    _cancellationTokenSource = null;
                }
            }

            public CancellationToken CancelQuery()
            {
                lock (_sync) // Guard against tests with explicit concurrency
                {
                    _cancellationTokenSource = new CancellationTokenSource();

                    return _cancellationTokenSource.Token;
                }
            }

            void ILogger.Log<TState>(
                LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var format = formatter(state, exception)?.Trim();

                lock (_sync) // Guard against tests with explicit concurrency
                {
                    if (format != null)
                    {
                        if (_cancellationTokenSource != null)
                        {
                            _cancellationTokenSource.Cancel();
                            _cancellationTokenSource = null;
                        }

                        if (eventId.Id == RelationalEventId.CommandExecuted.Id
                            || eventId.Id == RelationalEventId.CommandError.Id)
                        {
                            var structure = (IReadOnlyList<KeyValuePair<string, object>>)state;

                            var parameters = structure.Where(i => i.Key == "parameters").Select(i => (string)i.Value).First();
                            var commandText = structure.Where(i => i.Key == "commandText").Select(i => (string)i.Value).First();

                            if (!string.IsNullOrWhiteSpace(parameters))
                            {
                                Parameters.Add(parameters);
                                parameters = parameters.Replace(", ", _newLine) + _newLine + _newLine;
                            }

                            SqlStatements.Add(parameters + commandText);
                        }
                        else
                        {
                            LogBuilder.AppendLine(format);
                        }

                        if (eventId.Id != RelationalEventId.CommandExecuted.Id)
                        {
                            TestOutputHelper?.WriteLine(format + Environment.NewLine);
                        }
                    }
                }
            }

            bool ILogger.IsEnabled(LogLevel logLevel) => true;

            IDisposable ILogger.BeginScope<TState>(TState state)
            {
                lock (_sync) // Guard against tests with explicit concurrency
                {
                    return LogBuilder.Indent();
                }
            }
        }
    }
}
