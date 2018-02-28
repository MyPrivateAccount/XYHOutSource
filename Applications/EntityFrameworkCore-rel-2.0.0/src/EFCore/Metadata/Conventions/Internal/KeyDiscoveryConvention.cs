// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class KeyDiscoveryConvention :
        IEntityTypeAddedConvention,
        IPropertyAddedConvention,
        IKeyRemovedConvention,
        IBaseTypeChangedConvention,
        IPropertyFieldChangedConvention,
        IForeignKeyAddedConvention,
        IForeignKeyRemovedConvention
    {
        private const string KeySuffix = "Id";

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalEntityTypeBuilder Apply(InternalEntityTypeBuilder entityTypeBuilder)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            var entityType = entityTypeBuilder.Metadata;

            if (entityType.BaseType == null
                && ConfigurationSource.Convention.Overrides(entityType.GetPrimaryKeyConfigurationSource()))
            {
                IReadOnlyList<string> keyPropertyNames = null;
                if (entityType.HasDefiningNavigation())
                {
                    var definingFk = entityType.FindDefiningNavigation()?.ForeignKey;
                    if (definingFk != null)
                    {
                        // Make sure that the properties won't be reuniquified
                        definingFk.UpdateForeignKeyPropertiesConfigurationSource(ConfigurationSource.Convention);
                        keyPropertyNames = definingFk.Properties.Select(p => p.Name).ToList();
                    }
                }

                if (keyPropertyNames == null)
                {
                    var candidateProperties = entityType.GetProperties().Where(p =>
                        !p.IsShadowProperty
                        || !ConfigurationSource.Convention.Overrides(p.GetConfigurationSource())).ToList();
                    keyPropertyNames = DiscoverKeyProperties(entityType, candidateProperties).Select(p => p.Name).ToList();
                    if (keyPropertyNames.Count > 1)
                    {
                        //TODO - log using Strings.MultiplePropertiesMatchedAsKeys()
                        return entityTypeBuilder;
                    }
                }

                if (keyPropertyNames.Any())
                {
                    entityTypeBuilder.PrimaryKey(keyPropertyNames, ConfigurationSource.Convention);
                }
            }

            return entityTypeBuilder;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IEnumerable<Property> DiscoverKeyProperties(
            [NotNull] EntityType entityType, [NotNull] IReadOnlyList<Property> candidateProperties)
        {
            Check.NotNull(entityType, nameof(entityType));

            var keyProperties = candidateProperties.Where(p => string.Equals(p.Name, KeySuffix, StringComparison.OrdinalIgnoreCase));
            if (!keyProperties.Any())
            {
                var entityTypeName = entityType.ShortName();
                keyProperties = candidateProperties.Where(
                    p => p.Name.Length == entityTypeName.Length + KeySuffix.Length
                         && p.Name.StartsWith(entityTypeName, StringComparison.OrdinalIgnoreCase)
                         && p.Name.EndsWith(KeySuffix, StringComparison.OrdinalIgnoreCase));
            }

            return keyProperties;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool Apply(InternalEntityTypeBuilder entityTypeBuilder, EntityType oldBaseType)
            => Apply(entityTypeBuilder) != null;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            Apply(propertyBuilder.Metadata.DeclaringEntityType.Builder);

            return propertyBuilder;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool Apply(InternalPropertyBuilder propertyBuilder, FieldInfo oldFieldInfo)
        {
            Apply(propertyBuilder);
            return true;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Apply(InternalEntityTypeBuilder entityTypeBuilder, Key key)
        {
            if (entityTypeBuilder.Metadata.FindPrimaryKey() == null)
            {
                Apply(entityTypeBuilder);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Apply(InternalRelationshipBuilder relationshipBuilder)
        {
            var entityType = relationshipBuilder.Metadata.DeclaringEntityType;
            if (entityType.HasDefiningNavigation())
            {
                Apply(entityType.Builder);
            }

            return relationshipBuilder;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void Apply(InternalEntityTypeBuilder entityTypeBuilder, ForeignKey foreignKey)
        {
            if (entityTypeBuilder.Metadata.HasDefiningNavigation())
            {
                Apply(entityTypeBuilder);
            }
        }
    }
}
