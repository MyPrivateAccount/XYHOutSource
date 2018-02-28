// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage
{
    /// <summary>
    ///     <para>
    ///         Service dependencies parameter class for <see cref="RelationalDatabase" />
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
    public sealed class RelationalDatabaseDependencies
    {
        /// <summary>
        ///     <para>
        ///         Creates the service dependencies parameter object for a <see cref="RelationalDatabase" />.
        ///     </para>
        ///     <para>
        ///         Do not call this constructor directly from either provider or application code as it may change 
        ///         as new dependencies are added. Instead, use this type in your constructor so that an instance 
        ///         will be created and injected automatically by the dependency injection container. To create 
        ///         an instance with some dependent services replaced, first resolve the object from the dependency 
        ///         injection container, then replace selected services using the 'With...' methods. Do not call 
        ///         the constructor at any point in this process.
        ///     </para>
        /// </summary>
        /// <param name="batchPreparer"> The <see cref="ICommandBatchPreparer" /> to be used. </param>
        /// <param name="batchExecutor"> The <see cref="IBatchExecutor" /> to be used. </param>
        /// <param name="connection"> The <see cref="IRelationalConnection" /> to be used. </param>
        public RelationalDatabaseDependencies(
            [NotNull] ICommandBatchPreparer batchPreparer,
            [NotNull] IBatchExecutor batchExecutor,
            [NotNull] IRelationalConnection connection)
        {
            Check.NotNull(batchPreparer, nameof(batchPreparer));
            Check.NotNull(batchExecutor, nameof(batchExecutor));
            Check.NotNull(connection, nameof(connection));

            BatchPreparer = batchPreparer;
            BatchExecutor = batchExecutor;
            Connection = connection;
        }

        /// <summary>
        ///     The <see cref="ICommandBatchPreparer" /> to be used.
        /// </summary>
        public ICommandBatchPreparer BatchPreparer { get; }

        /// <summary>
        ///     The <see cref="IBatchExecutor" /> to be used.
        /// </summary>
        public IBatchExecutor BatchExecutor { get; }

        /// <summary>
        ///     The <see cref="IRelationalConnection" /> to be used.
        /// </summary>
        public IRelationalConnection Connection { get; }

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="batchPreparer">
        ///     A replacement for the current dependency of this type.
        /// </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public RelationalDatabaseDependencies With([NotNull] ICommandBatchPreparer batchPreparer)
            => new RelationalDatabaseDependencies(batchPreparer, BatchExecutor, Connection);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="batchExecutor">
        ///     A replacement for the current dependency of this type.
        /// </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public RelationalDatabaseDependencies With([NotNull] IBatchExecutor batchExecutor)
            => new RelationalDatabaseDependencies(BatchPreparer, batchExecutor, Connection);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="connection">
        ///     A replacement for the current dependency of this type.
        /// </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public RelationalDatabaseDependencies With([NotNull] IRelationalConnection connection)
            => new RelationalDatabaseDependencies(BatchPreparer, BatchExecutor, connection);
    }
}
