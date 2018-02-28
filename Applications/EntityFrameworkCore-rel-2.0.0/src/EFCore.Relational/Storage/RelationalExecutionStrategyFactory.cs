// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage
{
    /// <summary>
    ///     Factory for creating <see cref="IExecutionStrategy" /> instances for use with relational
    ///     database providers.
    /// </summary>
    public class RelationalExecutionStrategyFactory : IExecutionStrategyFactory
    {
        private readonly Func<ExecutionStrategyDependencies, IExecutionStrategy> _createExecutionStrategy;

        /// <summary>
        ///     Creates a new instance of this class with the given service dependencies.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
        public RelationalExecutionStrategyFactory([NotNull] ExecutionStrategyDependencies dependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;

            var configuredFactory = dependencies.Options == null
                ? null
                : RelationalOptionsExtension.Extract(dependencies.Options)?.ExecutionStrategyFactory;

            _createExecutionStrategy = configuredFactory ?? CreateDefaultStrategy;
        }

        /// <summary>
        ///     Parameter object containing service dependencies.
        /// </summary>
        protected virtual ExecutionStrategyDependencies Dependencies { get; }

        /// <summary>
        ///     Creates or returns a cached instance of the default <see cref="IExecutionStrategy" /> for the
        ///     current database provider.
        /// </summary>
        protected virtual IExecutionStrategy CreateDefaultStrategy([NotNull] ExecutionStrategyDependencies dependencies)
            => new NoopExecutionStrategy(Dependencies);

        /// <summary>
        ///     Creates an <see cref="IExecutionStrategy" /> for the current database provider.
        /// </summary>
        public virtual IExecutionStrategy Create() => _createExecutionStrategy(Dependencies);
    }
}
