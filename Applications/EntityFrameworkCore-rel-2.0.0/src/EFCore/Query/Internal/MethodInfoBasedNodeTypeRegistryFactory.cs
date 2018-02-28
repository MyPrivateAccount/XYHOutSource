// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    /// <summary>
    ///     Creates <see cref="INodeTypeProvider" /> instances for use by the query compiler
    ///     based on a <see cref="MethodInfoBasedNodeTypeRegistry" />.
    /// </summary>
    public class MethodInfoBasedNodeTypeRegistryFactory : INodeTypeProviderFactory
    {
        private readonly MethodInfoBasedNodeTypeRegistry _methodInfoBasedNodeTypeRegistry;

        /// <summary>
        ///     Creates a new <see cref="MethodInfoBasedNodeTypeRegistryFactory" /> that will use the given
        ///     <see cref="MethodInfoBasedNodeTypeRegistry" />
        /// </summary>
        /// <param name="methodInfoBasedNodeTypeRegistry">The registry to use./></param>
        public MethodInfoBasedNodeTypeRegistryFactory(
            [NotNull] MethodInfoBasedNodeTypeRegistry methodInfoBasedNodeTypeRegistry)
        {
            Check.NotNull(methodInfoBasedNodeTypeRegistry, nameof(methodInfoBasedNodeTypeRegistry));

            _methodInfoBasedNodeTypeRegistry = methodInfoBasedNodeTypeRegistry;
        }

        /// <summary>
        ///     Registers methods to be used with the <see cref="INodeTypeProvider" />.
        /// </summary>
        /// <param name="methods">The methods to register.</param>
        /// <param name="nodeType">The node type for these methods.</param>
        public virtual void RegisterMethods(IEnumerable<MethodInfo> methods, Type nodeType)
        {
            Check.NotNull(methods, nameof(methods));
            Check.NotNull(nodeType, nameof(nodeType));

            _methodInfoBasedNodeTypeRegistry.Register(methods, nodeType);
        }

        /// <summary>
        ///     Creates a <see cref="INodeTypeProvider" />.
        /// </summary>
        /// <returns>The <see cref="INodeTypeProvider" />.</returns>
        public virtual INodeTypeProvider Create()
        {
            _methodInfoBasedNodeTypeRegistry
                .Register(TrackingExpressionNode.SupportedMethods, typeof(TrackingExpressionNode));

            _methodInfoBasedNodeTypeRegistry
                .Register(IgnoreQueryFiltersExpressionNode.SupportedMethods, typeof(IgnoreQueryFiltersExpressionNode));

            _methodInfoBasedNodeTypeRegistry
                .Register(IncludeExpressionNode.SupportedMethods, typeof(IncludeExpressionNode));

            _methodInfoBasedNodeTypeRegistry
                .Register(StringIncludeExpressionNode.SupportedMethods, typeof(StringIncludeExpressionNode));

            _methodInfoBasedNodeTypeRegistry
                .Register(ThenIncludeExpressionNode.SupportedMethods, typeof(ThenIncludeExpressionNode));

            var innerProviders
                = new INodeTypeProvider[]
                {
                    _methodInfoBasedNodeTypeRegistry,
                    MethodNameBasedNodeTypeRegistry.CreateFromRelinqAssembly()
                };

            return new CompoundNodeTypeProvider(innerProviders);
        }
    }
}
