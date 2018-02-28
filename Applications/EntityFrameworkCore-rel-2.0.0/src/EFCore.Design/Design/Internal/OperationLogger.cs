﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class OperationLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IOperationReporter _reporter;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public OperationLogger([NotNull] string categoryName, [NotNull] IOperationReporter reporter)
        {
            _categoryName = categoryName;
            _reporter = reporter;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool IsEnabled(LogLevel logLevel)
            => true;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IDisposable BeginScope<TState>(TState state)
            => null;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            // Only show SQL when verbose
            if (_categoryName == DbLoggerCategory.Database.Command.Name
                && eventId.Id == RelationalEventId.CommandExecuted.Id)
            {
                logLevel = LogLevel.Debug;
            }

            var message = GetMessage(state, exception, formatter);
            switch (logLevel)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    _reporter.WriteError(message);
                    break;

                case LogLevel.Warning:
                    _reporter.WriteWarning(message);
                    break;

                case LogLevel.Information:
                    _reporter.WriteInformation(message);
                    break;

                default:
                    _reporter.WriteVerbose(message);
                    break;
            }
        }

        private string GetMessage<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var builder = new StringBuilder();
            if (formatter != null)
            {
                builder.Append(formatter(state, exception));
            }
            else if (state != null)
            {
                builder.Append(state);

                if (exception != null)
                {
                    builder
                        .AppendLine()
                        .Append(exception);
                }
            }

            return builder.ToString();
        }
    }
}
