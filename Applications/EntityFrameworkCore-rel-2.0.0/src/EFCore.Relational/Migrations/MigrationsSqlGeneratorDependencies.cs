// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Migrations
{
    /// <summary>
    ///     <para>
    ///         Service dependencies parameter class for <see cref="MigrationsSqlGenerator" />
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    ///     <para>
    ///         Do not construct instances of this class directly from either provider or application code as the
    ///         constructor signature may change as new dependencies are added. Instead, use this type in
    ///         your constructor so that an instance will be created and injected automatically by the
    ///         dependency injection container. To create an instance with some dependent services replaced,
    ///         first resolve the object from the dependency injection container, then replace selected
    ///         services using the 'With...' methods. Do not call the constructor at any point in this process.
    ///     </para>
    /// </summary>
    public sealed class MigrationsSqlGeneratorDependencies
    {
        /// <summary>
        ///     <para>
        ///         Creates the service dependencies parameter object for a <see cref="MigrationsSqlGenerator" />.
        ///     </para>
        ///     <para>
        ///         Do not call this constructor directly from either provider or application code as it may change
        ///         as new dependencies are added. Instead, use this type in your constructor so that an instance
        ///         will be created and injected automatically by the dependency injection container. To create
        ///         an instance with some dependent services replaced, first resolve the object from the dependency
        ///         injection container, then replace selected services using the 'With...' methods. Do not call
        ///         the constructor at any point in this process.
        ///     </para>
        ///     <para>
        ///         This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///         directly from your code. This API may change or be removed in future releases.
        ///     </para>
        /// </summary>
        /// <param name="commandBuilderFactory"> The command builder factory. </param>
        /// <param name="sqlGenerationHelper"> Helpers for SQL generation. </param>
        /// <param name="typeMapper"> The type mapper being used. </param>
        public MigrationsSqlGeneratorDependencies(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper,
            [NotNull] IRelationalTypeMapper typeMapper)
        {
            Check.NotNull(commandBuilderFactory, nameof(commandBuilderFactory));
            Check.NotNull(sqlGenerationHelper, nameof(sqlGenerationHelper));
            Check.NotNull(typeMapper, nameof(typeMapper));

            CommandBuilderFactory = commandBuilderFactory;
            SqlGenerationHelper = sqlGenerationHelper;
            TypeMapper = typeMapper;
        }

        /// <summary>
        ///     The command builder factory.
        /// </summary>
        public IRelationalCommandBuilderFactory CommandBuilderFactory { get; }

        /// <summary>
        ///     Helpers for SQL generation.
        /// </summary>
        public ISqlGenerationHelper SqlGenerationHelper { get; }

        /// <summary>
        ///     The type mapper being used.
        /// </summary>
        public IRelationalTypeMapper TypeMapper { get; }

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="commandBuilderFactory"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public MigrationsSqlGeneratorDependencies With([NotNull] IRelationalCommandBuilderFactory commandBuilderFactory)
            => new MigrationsSqlGeneratorDependencies(
                commandBuilderFactory,
                SqlGenerationHelper,
                TypeMapper);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="sqlGenerationHelper"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public MigrationsSqlGeneratorDependencies With([NotNull] ISqlGenerationHelper sqlGenerationHelper)
            => new MigrationsSqlGeneratorDependencies(
                CommandBuilderFactory,
                sqlGenerationHelper,
                TypeMapper);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="typeMapper"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public MigrationsSqlGeneratorDependencies With([NotNull] IRelationalTypeMapper typeMapper)
            => new MigrationsSqlGeneratorDependencies(
                CommandBuilderFactory,
                SqlGenerationHelper,
                typeMapper);
    }
}
