// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore
{
    public class TestLoggerFactory : ILoggerFactory
    {
        public static ITestOutputHelper TestOutputHelper;

        private readonly TestLogger _logger = new TestLogger();

        private class TestLogger : ILogger
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                => TestOutputHelper?.WriteLine(formatter(state, exception));

            public bool IsEnabled(LogLevel logLevel) => TestOutputHelper != null;

            public IDisposable BeginScope<TState>(TState state) => new NullDisposable();

            private class NullDisposable : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }

        public ILogger CreateLogger(string categoryName) => _logger;

        public void AddProvider(ILoggerProvider provider)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
