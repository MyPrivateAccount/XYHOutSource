// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     In-memory specific extension methods for <see cref="DbContextOptionsBuilder" />.
    /// </summary>
    public static class InMemoryDbContextOptionsExtensions
    {
        private const string LegacySharedName = "___Shared_Database___";
		
        /// <summary>
        ///     Configures the context to connect to an in-memory database.
        ///     The in-memory database is shared anywhere the same name is used.
        /// </summary>
        /// <typeparam name="TContext"> The type of context being configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder<TContext> UseInMemoryDatabase<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [NotNull] string databaseName,
            [CanBeNull] Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseInMemoryDatabase(
                (DbContextOptionsBuilder)optionsBuilder, databaseName, inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a named in-memory database.
        ///     The in-memory database is shared anywhere the same name is used.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="databaseName">
        ///     The name of the in-memory database. This allows the scope of the in-memory database to be controlled
        ///     independently of the context. The in-memory database is shared anywhere the same name is used.
        /// </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        public static DbContextOptionsBuilder UseInMemoryDatabase(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string databaseName,
            [CanBeNull] Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(databaseName, nameof(databaseName));

            var extension = optionsBuilder.Options.FindExtension<InMemoryOptionsExtension>()
                ?? new InMemoryOptionsExtension();

            extension = extension.WithStoreName(databaseName);

            ConfigureWarnings(optionsBuilder);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            inMemoryOptionsAction?.Invoke(new InMemoryDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to the legacy shared in-memory database.
        ///     This method is obsolete. Use <see cref="UseInMemoryDatabase{TContext}(DbContextOptionsBuilder{TContext},Action{InMemoryDbContextOptionsBuilder})" /> instead.
        ///     The database will be shared anywhere the same name is used.
        /// </summary>
        /// <typeparam name="TContext"> The type of context being configured. </typeparam>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        [Obsolete("Use UseInMemoryDatabase(string) instead. The database will be shared anywhere the same name is used.")]
        public static DbContextOptionsBuilder<TContext> UseInMemoryDatabase<TContext>(
            [NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder,
            [CanBeNull] Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
            where TContext : DbContext
            => optionsBuilder.UseInMemoryDatabase(LegacySharedName, inMemoryOptionsAction);

        /// <summary>
        ///     Configures the context to connect to the legacy shared in-memory database.
        ///     This method is obsolete. Use <see cref="UseInMemoryDatabase(DbContextOptionsBuilder,Action{InMemoryDbContextOptionsBuilder})" /> instead.
        ///     The database will be shared anywhere the same name is used.
        /// </summary>
        /// <param name="optionsBuilder"> The builder being used to configure the context. </param>
        /// <param name="inMemoryOptionsAction">An optional action to allow additional in-memory specific configuration.</param>
        /// <returns> The options builder so that further configuration can be chained. </returns>
        [Obsolete("Use UseInMemoryDatabase(string) instead. The database will be shared anywhere the same name is used.")]
        public static DbContextOptionsBuilder UseInMemoryDatabase(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [CanBeNull] Action<InMemoryDbContextOptionsBuilder> inMemoryOptionsAction = null)
            => optionsBuilder.UseInMemoryDatabase(LegacySharedName, inMemoryOptionsAction);

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            // Set warnings defaults
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                  ?? new CoreOptionsExtension();

            coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
                coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
                    InMemoryEventId.TransactionIgnoredWarning, WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }
}
