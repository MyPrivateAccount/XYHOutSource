// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public interface IQueryBuffer : IDisposable
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        object GetEntity(
            [NotNull] IKey key,
            EntityLoadInfo entityLoadInfo,
            bool queryStateManager,
            bool throwOnNullKey);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        object GetPropertyValue(
            [NotNull] object entity,
            [NotNull] IProperty property);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void StartTracking(
            [NotNull] object entity,
            [NotNull] EntityTrackingInfo entityTrackingInfo);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void StartTracking(
            [NotNull] object entity,
            [NotNull] IEntityType entityType);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        void IncludeCollection(
            int includeId,
            [NotNull] INavigation navigation,
            [CanBeNull] INavigation inverseNavigation,
            [NotNull] IEntityType targetEntityType,
            [NotNull] IClrCollectionAccessor clrCollectionAccessor,
            [CanBeNull] IClrPropertySetter inverseClrPropertySetter,
            bool tracking,
            [NotNull] object instance,
            [NotNull] Func<IEnumerable<object>> valuesFactory);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        Task IncludeCollectionAsync(
            int includeId,
            [NotNull] INavigation navigation,
            [CanBeNull] INavigation inverseNavigation,
            [NotNull] IEntityType targetEntityType,
            [NotNull] IClrCollectionAccessor clrCollectionAccessor,
            [CanBeNull] IClrPropertySetter inverseClrPropertySetter,
            bool tracking,
            [NotNull] object instance,
            [NotNull] Func<IAsyncEnumerable<object>> valuesFactory,
            CancellationToken cancellationToken);
    }
}
