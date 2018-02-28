// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.ChangeTracking.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class CompositeValueFactory : IDependentKeyValueFactory<object[]>
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public CompositeValueFactory([NotNull] IReadOnlyList<IProperty> properties)
        {
            Properties = properties;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual IReadOnlyList<IProperty> Properties { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool TryCreateFromBuffer(ValueBuffer valueBuffer, out object[] key)
        {
            key = new object[Properties.Count];
            var index = 0;

            foreach (var property in Properties)
            {
                if ((key[index++] = valueBuffer[property.GetIndex()]) == null)
                {
                    key = null;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool TryCreateFromCurrentValues(InternalEntityEntry entry, out object[] key)
            => TryCreateFromEntry(entry, (e, p) => e.GetCurrentValue(p), out key);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool TryCreateFromPreStoreGeneratedCurrentValues(InternalEntityEntry entry, out object[] key)
            => TryCreateFromEntry(entry, (e, p) => e.GetPreStoreGeneratedCurrentValue(p), out key);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool TryCreateFromOriginalValues(InternalEntityEntry entry, out object[] key)
            => TryCreateFromEntry(entry, (e, p) => e.GetOriginalValue(p), out key);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool TryCreateFromRelationshipSnapshot(InternalEntityEntry entry, out object[] key)
            => TryCreateFromEntry(entry, (e, p) => e.GetRelationshipSnapshotValue(p), out key);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual bool TryCreateFromEntry(
            [NotNull] InternalEntityEntry entry,
            [NotNull] Func<InternalEntityEntry, IProperty, object> getValue,
            [CanBeNull] out object[] key)
        {
            key = new object[Properties.Count];
            var index = 0;

            foreach (var property in Properties)
            {
                if ((key[index++] = getValue(entry, property)) == null)
                {
                    key = null;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected static IEqualityComparer<object[]> CreateEqualityComparer([NotNull] IReadOnlyList<IProperty> properties)
            => properties.Any(p => typeof(IStructuralEquatable).GetTypeInfo().IsAssignableFrom(p.ClrType.GetTypeInfo()))
                ? (IEqualityComparer<object[]>)new StructuralCompositeComparer()
                : new CompositeComparer();

        private sealed class CompositeComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[] x, object[] y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x.Length != y.Length)
                {
                    return false;
                }

                for (var i = 0; i < x.Length; i++)
                {
                    if (!Equals(x[i], y[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(object[] obj)
            {
                var hashCode = 0;

                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < obj.Length; i++)
                {
                    hashCode = (hashCode * 397) ^ (obj[i] != null ? obj[i].GetHashCode() : 0);
                }

                return hashCode;
            }
        }

        private sealed class StructuralCompositeComparer : IEqualityComparer<object[]>
        {
            private readonly IEqualityComparer _structuralEqualityComparer
                = StructuralComparisons.StructuralEqualityComparer;

            public bool Equals(object[] x, object[] y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x.Length != y.Length)
                {
                    return false;
                }

                for (var i = 0; i < x.Length; i++)
                {
                    if (!_structuralEqualityComparer.Equals(x[i], y[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(object[] obj)
            {
                var hashCode = 0;

                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < obj.Length; i++)
                {
                    hashCode = (hashCode * 397) ^ _structuralEqualityComparer.GetHashCode(obj[i]);
                }

                return hashCode;
            }
        }
    }
}
