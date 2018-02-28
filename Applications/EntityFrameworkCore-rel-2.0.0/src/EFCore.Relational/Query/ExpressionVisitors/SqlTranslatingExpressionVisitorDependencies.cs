// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    /// <summary>
    ///     <para>
    ///         Service dependencies parameter class for <see cref="SqlTranslatingExpressionVisitor" />
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
    public sealed class SqlTranslatingExpressionVisitorDependencies
    {
        /// <summary>
        ///     <para>
        ///         Creates the service dependencies parameter object for a <see cref="SqlTranslatingExpressionVisitor" />.
        ///     </para>
        ///     <para>
        ///         This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///         directly from your code. This API may change or be removed in future releases.
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
        /// <param name="compositeExpressionFragmentTranslator"> The composite expression fragment translator. </param>
        /// <param name="methodCallTranslator"> The method call translator. </param>
        /// <param name="memberTranslator"> The member translator. </param>
        /// <param name="relationalTypeMapper"> The relational type mapper. </param>
        public SqlTranslatingExpressionVisitorDependencies(
            [NotNull] IExpressionFragmentTranslator compositeExpressionFragmentTranslator,
            [NotNull] ICompositeMethodCallTranslator methodCallTranslator,
            [NotNull] IMemberTranslator memberTranslator,
            [NotNull] IRelationalTypeMapper relationalTypeMapper)
        {
            Check.NotNull(compositeExpressionFragmentTranslator, nameof(compositeExpressionFragmentTranslator));
            Check.NotNull(methodCallTranslator, nameof(methodCallTranslator));
            Check.NotNull(memberTranslator, nameof(memberTranslator));
            Check.NotNull(relationalTypeMapper, nameof(relationalTypeMapper));

            CompositeExpressionFragmentTranslator = compositeExpressionFragmentTranslator;
            MethodCallTranslator = methodCallTranslator;
            MemberTranslator = memberTranslator;
            RelationalTypeMapper = relationalTypeMapper;
        }

        /// <summary>
        ///     The composite expression fragment translator.
        /// </summary>
        public IExpressionFragmentTranslator CompositeExpressionFragmentTranslator { get; }

        /// <summary>
        ///     The method call translator.
        /// </summary>
        public ICompositeMethodCallTranslator MethodCallTranslator { get; }

        /// <summary>
        ///     The member translator.
        /// </summary>
        public IMemberTranslator MemberTranslator { get; }

        /// <summary>
        ///     The relational type mapper.
        /// </summary>
        public IRelationalTypeMapper RelationalTypeMapper { get; }

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="compositeExpressionFragmentTranslator"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public SqlTranslatingExpressionVisitorDependencies With([NotNull] IExpressionFragmentTranslator compositeExpressionFragmentTranslator)
            => new SqlTranslatingExpressionVisitorDependencies(
                compositeExpressionFragmentTranslator,
                MethodCallTranslator,
                MemberTranslator,
                RelationalTypeMapper);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="methodCallTranslator"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public SqlTranslatingExpressionVisitorDependencies With([NotNull] ICompositeMethodCallTranslator methodCallTranslator)
            => new SqlTranslatingExpressionVisitorDependencies(
                CompositeExpressionFragmentTranslator,
                methodCallTranslator,
                MemberTranslator,
                RelationalTypeMapper);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="memberTranslator"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public SqlTranslatingExpressionVisitorDependencies With([NotNull] IMemberTranslator memberTranslator)
            => new SqlTranslatingExpressionVisitorDependencies(
                CompositeExpressionFragmentTranslator,
                MethodCallTranslator,
                memberTranslator,
                RelationalTypeMapper);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="relationalTypeMapper"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public SqlTranslatingExpressionVisitorDependencies With([NotNull] IRelationalTypeMapper relationalTypeMapper)
            => new SqlTranslatingExpressionVisitorDependencies(
                CompositeExpressionFragmentTranslator,
                MethodCallTranslator,
                MemberTranslator,
                relationalTypeMapper);
    }
}
