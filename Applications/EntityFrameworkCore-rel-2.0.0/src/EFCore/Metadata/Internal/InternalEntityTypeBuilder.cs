// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [DebuggerDisplay("{Metadata,nq}")]
    public class InternalEntityTypeBuilder : InternalMetadataItemBuilder<EntityType>
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public InternalEntityTypeBuilder([NotNull] EntityType metadata, [NotNull] InternalModelBuilder modelBuilder)
            : base(metadata, modelBuilder)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalKeyBuilder PrimaryKey([CanBeNull] IReadOnlyList<string> propertyNames, ConfigurationSource configurationSource)
            => PrimaryKey(GetOrCreateProperties(propertyNames, configurationSource), configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalKeyBuilder PrimaryKey([CanBeNull] IReadOnlyList<PropertyInfo> clrProperties, ConfigurationSource configurationSource)
            => PrimaryKey(GetOrCreateProperties(clrProperties, configurationSource), configurationSource);

        private InternalKeyBuilder PrimaryKey(IReadOnlyList<Property> properties, ConfigurationSource configurationSource)
        {
            var previousPrimaryKey = Metadata.FindPrimaryKey();
            if (properties == null)
            {
                if (previousPrimaryKey == null)
                {
                    return null;
                }
            }
            else if (previousPrimaryKey != null
                     && PropertyListComparer.Instance.Compare(previousPrimaryKey.Properties, properties) == 0)
            {
                return Metadata.SetPrimaryKey(properties, configurationSource).Builder;
            }

            var primaryKeyConfigurationSource = Metadata.GetPrimaryKeyConfigurationSource();
            if (primaryKeyConfigurationSource.HasValue
                && !configurationSource.Overrides(primaryKeyConfigurationSource.Value))
            {
                return null;
            }

            InternalKeyBuilder keyBuilder = null;
            if (properties == null)
            {
                Metadata.SetPrimaryKey(properties, configurationSource);
            }
            else
            {
                using (ModelBuilder.Metadata.ConventionDispatcher.StartBatch())
                {
                    keyBuilder = HasKeyInternal(properties, configurationSource);
                    if (keyBuilder == null)
                    {
                        return null;
                    }

                    Metadata.SetPrimaryKey(keyBuilder.Metadata.Properties, configurationSource);
                    foreach (var key in Metadata.GetDeclaredKeys().ToList())
                    {
                        if (key == keyBuilder.Metadata)
                        {
                            continue;
                        }

                        var referencingForeignKeys = key
                            .GetReferencingForeignKeys()
                            .Where(fk => fk.GetPrincipalKeyConfigurationSource() == null)
                            .ToList();
                        foreach (var referencingForeignKey in referencingForeignKeys)
                        {
                            DetachRelationship(referencingForeignKey).Attach();
                        }
                    }
                }
            }

            if (previousPrimaryKey?.Builder != null)
            {
                RemoveKeyIfUnused(previousPrimaryKey);
            }

            return keyBuilder?.Metadata.Builder == null ? null : keyBuilder;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalKeyBuilder HasKey([NotNull] IReadOnlyList<string> propertyNames, ConfigurationSource configurationSource)
            => HasKeyInternal(GetOrCreateProperties(propertyNames, configurationSource), configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalKeyBuilder HasKey([NotNull] IReadOnlyList<PropertyInfo> clrProperties, ConfigurationSource configurationSource)
            => HasKeyInternal(GetOrCreateProperties(clrProperties, configurationSource), configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalKeyBuilder HasKey([NotNull] IReadOnlyList<Property> properties, ConfigurationSource? configurationSource)
            => HasKeyInternal(properties, configurationSource);

        private InternalKeyBuilder HasKeyInternal(IReadOnlyList<Property> properties, ConfigurationSource? configurationSource)
        {
            if (properties == null)
            {
                return null;
            }

            var actualProperties = GetActualProperties(properties, configurationSource);
            var key = Metadata.FindDeclaredKey(actualProperties);
            if (key == null)
            {
                if (configurationSource == null)
                {
                    return null;
                }

                var containingForeignKeys = actualProperties
                    .SelectMany(p => p.GetContainingForeignKeys().Where(k => k.DeclaringEntityType != Metadata))
                    .ToList();

                if (containingForeignKeys.Any(fk => !configurationSource.Overrides(fk.GetForeignKeyPropertiesConfigurationSource())))
                {
                    return null;
                }

                if (configurationSource != ConfigurationSource.Explicit // let it throw for explicit
                    && actualProperties.Any(p => !p.Builder.CanSetRequired(true, configurationSource)))
                {
                    return null;
                }

                using (Metadata.Model.ConventionDispatcher.StartBatch())
                {
                    foreach (var foreignKey in containingForeignKeys
                        // let it throw for explicit
                        .Where(fk => fk.GetForeignKeyPropertiesConfigurationSource() != ConfigurationSource.Explicit)
                        .ToList())
                    {
                        foreignKey.Builder.HasForeignKey(null, configurationSource);
                    }

                    foreach (var actualProperty in actualProperties)
                    {
                        actualProperty.Builder.IsRequired(true, configurationSource.Value);
                    }

                    key = Metadata.AddKey(actualProperties, configurationSource.Value);
                }
                if (key.Builder == null)
                {
                    key = Metadata.FindDeclaredKey(actualProperties);
                }
            }
            else if (configurationSource.HasValue)
            {
                key.UpdateConfigurationSource(configurationSource.Value);
            }

            return key?.Builder;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ConfigurationSource? RemoveKey([NotNull] Key key, ConfigurationSource configurationSource)
        {
            var currentConfigurationSource = key.GetConfigurationSource();
            if (!configurationSource.Overrides(currentConfigurationSource))
            {
                return null;
            }

            using (Metadata.Model.ConventionDispatcher.StartBatch())
            {
                foreach (var foreignKey in key.GetReferencingForeignKeys().ToList())
                {
                    var removed = foreignKey.DeclaringEntityType.Builder.RemoveForeignKey(foreignKey, configurationSource);
                    Debug.Assert(removed.HasValue);
                }

                var removedKey = Metadata.RemoveKey(key.Properties);
                Debug.Assert(removedKey == key);

                RemoveShadowPropertiesIfUnused(key.Properties);
                foreach (var property in key.Properties)
                {
                    if (property.ClrType.IsNullableType())
                    {
                        // TODO: This should be handled by reference tracking
                        property.Builder?.IsRequired(false, configurationSource);
                    }
                }
            }

            return currentConfigurationSource;
        }

        private class KeyBuildersSnapshot
        {
            public KeyBuildersSnapshot(
                IReadOnlyList<Tuple<InternalKeyBuilder, ConfigurationSource>> keys,
                Tuple<InternalKeyBuilder, ConfigurationSource> primaryKey)
            {
                Keys = keys;
                PrimaryKey = primaryKey;
            }

            private IReadOnlyList<Tuple<InternalKeyBuilder, ConfigurationSource>> Keys { get; }
            private Tuple<InternalKeyBuilder, ConfigurationSource> PrimaryKey { get; }

            public void Attach()
            {
                foreach (var keyTuple in Keys)
                {
                    var detachedKeyBuilder = keyTuple.Item1;
                    var detachedConfigurationSource = keyTuple.Item2;
                    var attachedKey = detachedKeyBuilder.Attach(detachedConfigurationSource);
                    if (attachedKey != null
                        && PrimaryKey != null
                        && PrimaryKey.Item1 == detachedKeyBuilder)
                    {
                        var rootType = detachedKeyBuilder.Metadata.DeclaringEntityType.RootType();
                        var primaryKeyConfigurationSource = rootType.GetPrimaryKeyConfigurationSource();
                        if (primaryKeyConfigurationSource == null
                            || !primaryKeyConfigurationSource.Value.Overrides(PrimaryKey.Item2))
                        {
                            rootType.Builder.PrimaryKey(attachedKey.Metadata.Properties, PrimaryKey.Item2);
                        }
                    }
                }
            }
        }

        private static KeyBuildersSnapshot DetachKeys(IEnumerable<Key> keysToDetach)
        {
            var keysToDetachList = keysToDetach.ToList();
            if (keysToDetachList.Count == 0)
            {
                return null;
            }

            var detachedKeys = new List<Tuple<InternalKeyBuilder, ConfigurationSource>>();
            Tuple<InternalKeyBuilder, ConfigurationSource> primaryKey = null;
            foreach (var keyToDetach in keysToDetachList)
            {
                var entityTypeBuilder = keyToDetach.DeclaringEntityType.Builder;
                var keyBuilder = keyToDetach.Builder;
                if (keyToDetach.IsPrimaryKey())
                {
                    var primaryKeyConfigurationSource = entityTypeBuilder.Metadata.GetPrimaryKeyConfigurationSource();
                    Debug.Assert(primaryKeyConfigurationSource.HasValue);
                    primaryKey = Tuple.Create(keyBuilder, primaryKeyConfigurationSource.Value);
                }
                var removedConfigurationSource = entityTypeBuilder.RemoveKey(keyToDetach, keyToDetach.GetConfigurationSource());
                Debug.Assert(removedConfigurationSource != null);

                detachedKeys.Add(Tuple.Create(keyBuilder, removedConfigurationSource.Value));
            }

            return new KeyBuildersSnapshot(detachedKeys, primaryKey);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalPropertyBuilder Property(
            [NotNull] string propertyName,
            [NotNull] Type propertyType,
            ConfigurationSource configurationSource)
            => Property(propertyName, propertyType, configurationSource, typeConfigurationSource: configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalPropertyBuilder Property(
            [NotNull] string propertyName,
            [NotNull] Type propertyType,
            ConfigurationSource configurationSource,
            [CanBeNull] ConfigurationSource? typeConfigurationSource)
            => Property(propertyName, propertyType, memberInfo: null,
                configurationSource: configurationSource, typeConfigurationSource: typeConfigurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalPropertyBuilder Property([NotNull] string propertyName, ConfigurationSource configurationSource)
            => Property(propertyName, propertyType: null, memberInfo: null, configurationSource: configurationSource, typeConfigurationSource: configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalPropertyBuilder Property([NotNull] MemberInfo clrProperty, ConfigurationSource configurationSource)
            => Property(clrProperty.Name, clrProperty.GetMemberType(), clrProperty, configurationSource, configurationSource);

        private InternalPropertyBuilder Property(
            [NotNull] string propertyName,
            [CanBeNull] Type propertyType,
            [CanBeNull] MemberInfo memberInfo,
            [CanBeNull] ConfigurationSource? configurationSource,
            [CanBeNull] ConfigurationSource? typeConfigurationSource)
        {
            if (IsIgnored(propertyName, configurationSource))
            {
                return null;
            }

            Metadata.Unignore(propertyName);

            IEnumerable<Property> propertiesToDetach = null;
            var existingProperty = Metadata.FindProperty(propertyName);
            if (existingProperty != null)
            {
                if (existingProperty.DeclaringEntityType != Metadata)
                {
                    if (memberInfo != null
                        && existingProperty.MemberInfo == null)
                    {
                        propertiesToDetach = new[] { existingProperty };
                    }
                    else
                    {
                        return existingProperty.DeclaringEntityType.Builder
                            .Property(existingProperty, propertyName, propertyType, memberInfo, configurationSource, typeConfigurationSource);
                    }
                }
            }
            else
            {
                propertiesToDetach = Metadata.FindDerivedProperties(propertyName);
            }

            InternalPropertyBuilder builder;
            using (Metadata.Model.ConventionDispatcher.StartBatch())
            {
                var detachedProperties = propertiesToDetach == null ? null : DetachProperties(propertiesToDetach);

                builder = Property(existingProperty, propertyName, propertyType, memberInfo, configurationSource, typeConfigurationSource);

                detachedProperties?.Attach(this);
            }

            if (builder != null
                && builder.Metadata.Builder == null)
            {
                return Metadata.FindProperty(propertyName)?.Builder;
            }

            return builder;
        }

        private InternalPropertyBuilder Property(
            [CanBeNull] Property existingProperty,
            [NotNull] string propertyName,
            [CanBeNull] Type propertyType,
            [CanBeNull] MemberInfo clrProperty,
            [CanBeNull] ConfigurationSource? configurationSource,
            [CanBeNull] ConfigurationSource? typeConfigurationSource)
        {
            var property = existingProperty;
            if (existingProperty == null)
            {
                if (!configurationSource.HasValue)
                {
                    return null;
                }

                var duplicateNavigation = Metadata.FindNavigationsInHierarchy(propertyName).FirstOrDefault();
                if (duplicateNavigation != null)
                {
                    throw new InvalidOperationException(CoreStrings.PropertyCalledOnNavigation(propertyName, Metadata.DisplayName()));
                }

                property = clrProperty != null
                    ? Metadata.AddProperty(clrProperty, configurationSource.Value)
                    : Metadata.AddProperty(propertyName, propertyType, configurationSource.Value, typeConfigurationSource);
            }
            else
            {
                if ((propertyType != null
                     && propertyType != existingProperty.ClrType)
                    || (clrProperty != null
                        && existingProperty.PropertyInfo == null))
                {
                    if (!configurationSource.HasValue
                        || !configurationSource.Value.Overrides(existingProperty.GetConfigurationSource()))
                    {
                        return null;
                    }

                    using (Metadata.Model.ConventionDispatcher.StartBatch())
                    {
                        var detachedProperties = DetachProperties(new[] { existingProperty });

                        property = clrProperty != null
                            ? Metadata.AddProperty(clrProperty, configurationSource.Value)
                            : Metadata.AddProperty(propertyName, propertyType, configurationSource.Value, typeConfigurationSource);

                        detachedProperties.Attach(this);
                    }
                }
                else
                {
                    if (configurationSource.HasValue)
                    {
                        property.UpdateConfigurationSource(configurationSource.Value);
                    }
                    if (typeConfigurationSource.HasValue)
                    {
                        property.UpdateConfigurationSource(typeConfigurationSource.Value);
                    }
                }
            }

            return property?.Builder;
        }

        private bool CanRemoveProperty(
            [NotNull] Property property, ConfigurationSource configurationSource, bool canOverrideSameSource = true)
        {
            Check.NotNull(property, nameof(property));
            Debug.Assert(property.DeclaringEntityType == Metadata);

            var currentConfigurationSource = property.GetConfigurationSource();
            return configurationSource.Overrides(currentConfigurationSource)
                   && (canOverrideSameSource || (configurationSource != currentConfigurationSource));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool CanAddNavigation([NotNull] string navigationName, ConfigurationSource configurationSource)
            => !IsIgnored(navigationName, configurationSource: configurationSource)
               && !Metadata.FindNavigationsInHierarchy(navigationName).Any();

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool CanAddOrReplaceNavigation([NotNull] string navigationName, ConfigurationSource configurationSource)
            => !IsIgnored(navigationName, configurationSource: configurationSource)
               && Metadata.FindNavigationsInHierarchy(navigationName).All(n =>
                   n.ForeignKey.Builder.CanSetNavigation((string)null, n.IsDependentToPrincipal(), configurationSource));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool IsIgnored([NotNull] string name, ConfigurationSource? configurationSource)
        {
            Check.NotEmpty(name, nameof(name));

            var ignoredConfigurationSource = Metadata.FindIgnoredMemberConfigurationSource(name);
            if (!configurationSource.HasValue
                || !configurationSource.Value.Overrides(ignoredConfigurationSource))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool CanRemoveForeignKey([NotNull] ForeignKey foreignKey, ConfigurationSource configurationSource)
        {
            Debug.Assert(foreignKey.DeclaringEntityType == Metadata);

            var currentConfigurationSource = foreignKey.GetConfigurationSource();
            return configurationSource.Overrides(currentConfigurationSource);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool Ignore([NotNull] string name, ConfigurationSource configurationSource)
        {
            var ignoredConfigurationSource = Metadata.FindIgnoredMemberConfigurationSource(name);
            if (ignoredConfigurationSource.HasValue
                && ignoredConfigurationSource.Value.Overrides(configurationSource))
            {
                return true;
            }

            using (Metadata.Model.ConventionDispatcher.StartBatch())
            {
                Metadata.Ignore(name, configurationSource);

                var navigation = Metadata.FindNavigation(name);
                if (navigation != null)
                {
                    var foreignKey = navigation.ForeignKey;
                    if (navigation.DeclaringEntityType != Metadata)
                    {
                        if (configurationSource == ConfigurationSource.Explicit)
                        {
                            throw new InvalidOperationException(CoreStrings.InheritedPropertyCannotBeIgnored(
                                name, Metadata.DisplayName(), navigation.DeclaringEntityType.DisplayName()));
                        }
                        return false;
                    }

                    if (foreignKey.DeclaringEntityType.Builder.RemoveForeignKey(
                        foreignKey, configurationSource, canOverrideSameSource: configurationSource == ConfigurationSource.Explicit) == null)
                    {
                        Metadata.Unignore(name);
                        return false;
                    }
                }
                else
                {
                    var property = Metadata.FindProperty(name);
                    if (property != null)
                    {
                        if (property.DeclaringEntityType != Metadata)
                        {
                            if (configurationSource == ConfigurationSource.Explicit)
                            {
                                throw new InvalidOperationException(CoreStrings.InheritedPropertyCannotBeIgnored(
                                    name, Metadata.DisplayName(), property.DeclaringEntityType.DisplayName()));
                            }
                            return false;
                        }

                        if (property.DeclaringEntityType.Builder.RemoveProperty(
                            property, configurationSource, canOverrideSameSource: configurationSource == ConfigurationSource.Explicit) == null)
                        {
                            Metadata.Unignore(name);
                            return false;
                        }
                    }
                }

                foreach (var derivedType in Metadata.GetDerivedTypes())
                {
                    var derivedNavigation = derivedType.FindDeclaredNavigation(name);
                    if (derivedNavigation != null)
                    {
                        var foreignKey = derivedNavigation.ForeignKey;
                        foreignKey.DeclaringEntityType.Builder.RemoveForeignKey(foreignKey, configurationSource, canOverrideSameSource: false);
                    }
                    else
                    {
                        var derivedProperty = derivedType.FindDeclaredProperty(name);
                        if (derivedProperty != null)
                        {
                            derivedType.Builder.RemoveProperty(derivedProperty, configurationSource, canOverrideSameSource: false);
                        }
                    }

                    var derivedIgnoredSource = derivedType.FindDeclaredIgnoredMemberConfigurationSource(name);
                    if (derivedIgnoredSource.HasValue
                        && configurationSource.Overrides(derivedIgnoredSource))
                    {
                        derivedType.Unignore(name);
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void HasQueryFilter([CanBeNull] LambdaExpression filter)
        {
            Metadata.QueryFilter = filter;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalEntityTypeBuilder HasBaseType([CanBeNull] Type baseEntityType, ConfigurationSource configurationSource)
        {
            if (baseEntityType == null)
            {
                return HasBaseType((EntityType)null, configurationSource);
            }

            var baseType = ModelBuilder.Entity(baseEntityType, configurationSource);
            return baseType == null
                ? null
                : HasBaseType(baseType.Metadata, configurationSource);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalEntityTypeBuilder HasBaseType([CanBeNull] string baseEntityTypeName, ConfigurationSource configurationSource)
        {
            if (baseEntityTypeName == null)
            {
                return HasBaseType((EntityType)null, configurationSource);
            }

            var baseType = ModelBuilder.Entity(baseEntityTypeName, configurationSource);
            return baseType == null
                ? null
                : HasBaseType(baseType.Metadata, configurationSource);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalEntityTypeBuilder HasBaseType([CanBeNull] EntityType baseEntityType, ConfigurationSource configurationSource)
        {
            if (Metadata.BaseType == baseEntityType)
            {
                Metadata.HasBaseType(baseEntityType, configurationSource);
                return this;
            }

            if (!configurationSource.Overrides(Metadata.GetBaseTypeConfigurationSource()))
            {
                return null;
            }

            using (Metadata.Model.ConventionDispatcher.StartBatch())
            {
                List<RelationshipBuilderSnapshot> detachedRelationships = null;
                PropertyBuildersSnapshot detachedProperties = null;
                KeyBuildersSnapshot detachedKeys = null;
                // We use at least DataAnnotation as ConfigurationSource while removing to allow us
                // to remove metadata object which were defined in derived type
                // while corresponding annotations were present on properties in base type.
                var configurationSourceForRemoval = ConfigurationSource.DataAnnotation.Max(configurationSource);
                if (baseEntityType != null)
                {
                    if (Metadata.GetDeclaredKeys().Any(k => !configurationSourceForRemoval.Overrides(k.GetConfigurationSource())))
                    {
                        return null;
                    }

                    var relationshipsToBeDetached = FindConflictingRelationships(baseEntityType, configurationSourceForRemoval);
                    if (relationshipsToBeDetached == null)
                    {
                        return null;
                    }

                    var foreignKeysUsingKeyProperties = Metadata.GetDerivedForeignKeysInclusive()
                        .Where(fk => fk.Properties.Any(p => baseEntityType.FindProperty(p.Name)?.IsKey() == true)).ToList();

                    if (foreignKeysUsingKeyProperties.Any(fk =>
                        !configurationSourceForRemoval.Overrides(fk.GetForeignKeyPropertiesConfigurationSource())))
                    {
                        return null;
                    }

                    foreach (var foreignKeyUsingKeyProperties in foreignKeysUsingKeyProperties)
                    {
                        foreignKeyUsingKeyProperties.Builder.HasForeignKey((IReadOnlyList<Property>)null, configurationSourceForRemoval);
                    }

                    foreach (var relationshipToBeRemoved in relationshipsToBeDetached)
                    {
                        if (detachedRelationships == null)
                        {
                            detachedRelationships = new List<RelationshipBuilderSnapshot>();
                        }
                        detachedRelationships.Add(DetachRelationship(relationshipToBeRemoved));
                    }

                    foreach (var key in Metadata.GetDeclaredKeys().ToList())
                    {
                        foreach (var referencingForeignKey in key.GetReferencingForeignKeys().ToList())
                        {
                            if (detachedRelationships == null)
                            {
                                detachedRelationships = new List<RelationshipBuilderSnapshot>();
                            }
                            detachedRelationships.Add(DetachRelationship(referencingForeignKey));
                        }
                    }

                    detachedKeys = DetachKeys(Metadata.GetDeclaredKeys());

                    var duplicatedProperties = baseEntityType.GetProperties()
                        .SelectMany(p => Metadata.FindDerivedPropertiesInclusive(p.Name))
                        .Where(p => p != null);

                    detachedProperties = DetachProperties(duplicatedProperties);

                    var propertiesToRemove = Metadata.GetDerivedTypesInclusive().SelectMany(et => et.GetDeclaredProperties())
                        .Where(p => !p.GetConfigurationSource().Overrides(baseEntityType.FindIgnoredMemberConfigurationSource(p.Name))).ToList();
                    foreach (var property in propertiesToRemove)
                    {
                        property.DeclaringEntityType.Builder.RemoveProperty(property, ConfigurationSource.Explicit);
                    }

                    foreach (var ignoredMember in Metadata.GetIgnoredMembers().ToList())
                    {
                        var ignoredSource = Metadata.FindDeclaredIgnoredMemberConfigurationSource(ignoredMember);
                        var baseIgnoredSource = baseEntityType.FindIgnoredMemberConfigurationSource(ignoredMember);

                        if (baseIgnoredSource.HasValue
                            && baseIgnoredSource.Value.Overrides(ignoredSource))
                        {
                            Metadata.Unignore(ignoredMember);
                        }
                    }

                    baseEntityType.UpdateConfigurationSource(configurationSource);
                }

                List<IndexBuilderSnapshot> detachedIndexes = null;
                HashSet<Property> removedInheritedPropertiesToDuplicate = null;
                if (Metadata.BaseType != null)
                {
                    var removedInheritedProperties = new HashSet<Property>(Metadata.BaseType.GetProperties()
                        .Where(p => baseEntityType == null || baseEntityType.FindProperty(p.Name) != p));
                    if (removedInheritedProperties.Count != 0)
                    {
                        removedInheritedPropertiesToDuplicate = new HashSet<Property>();
                        foreach (var foreignKey in Metadata.GetDerivedForeignKeysInclusive()
                            .Where(fk => fk.Properties.Any(p => removedInheritedProperties.Contains(p))).ToList())
                        {
                            foreach (var property in foreignKey.Properties)
                            {
                                if (removedInheritedProperties.Contains(property))
                                {
                                    removedInheritedPropertiesToDuplicate.Add(property);
                                }
                            }

                            if (detachedRelationships == null)
                            {
                                detachedRelationships = new List<RelationshipBuilderSnapshot>();
                            }
                            detachedRelationships.Add(DetachRelationship(foreignKey));
                        }

                        foreach (var index in Metadata.GetDerivedIndexesInclusive()
                            .Where(i => i.Properties.Any(p => removedInheritedProperties.Contains(p))).ToList())
                        {
                            foreach (var property in index.Properties)
                            {
                                if (removedInheritedProperties.Contains(property))
                                {
                                    removedInheritedPropertiesToDuplicate.Add(property);
                                }
                            }

                            if (detachedIndexes == null)
                            {
                                detachedIndexes = new List<IndexBuilderSnapshot>();
                            }
                            detachedIndexes.Add(DetachIndex(index));
                        }
                    }
                }

                Metadata.HasBaseType(baseEntityType, configurationSource);

                if (removedInheritedPropertiesToDuplicate != null)
                {
                    foreach (var property in removedInheritedPropertiesToDuplicate)
                    {
                        property.Builder?.Attach(this, property.GetConfigurationSource());
                    }
                }

                detachedProperties?.Attach(this);

                detachedKeys?.Attach();

                if (detachedIndexes != null)
                {
                    foreach (var indexBuilderSnapshot in detachedIndexes)
                    {
                        indexBuilderSnapshot.Attach();
                    }
                }

                if (detachedRelationships != null)
                {
                    foreach (var detachedRelationship in detachedRelationships)
                    {
                        detachedRelationship.Attach();
                    }
                }
            }

            return this;
        }

        private static PropertyBuildersSnapshot DetachProperties(IEnumerable<Property> propertiesToDetach)
        {
            var propertiesToDetachList = propertiesToDetach.ToList();
            if (propertiesToDetachList.Count == 0)
            {
                return null;
            }

            List<RelationshipBuilderSnapshot> detachedRelationships = null;
            foreach (var propertyToDetach in propertiesToDetachList)
            {
                foreach (var relationship in propertyToDetach.GetContainingForeignKeys().ToList())
                {
                    if (detachedRelationships == null)
                    {
                        detachedRelationships = new List<RelationshipBuilderSnapshot>();
                    }
                    detachedRelationships.Add(DetachRelationship(relationship));
                }
            }

            var detachedIndexes = propertiesToDetachList.SelectMany(p => p.GetContainingIndexes()).Distinct().ToList()
                .Select(DetachIndex).ToList();

            var keysToDetach = propertiesToDetachList.SelectMany(p => p.GetContainingKeys()).Distinct().ToList();
            foreach (var key in keysToDetach)
            {
                foreach (var referencingForeignKey in key.GetReferencingForeignKeys().ToList())
                {
                    if (detachedRelationships == null)
                    {
                        detachedRelationships = new List<RelationshipBuilderSnapshot>();
                    }
                    detachedRelationships.Add(DetachRelationship(referencingForeignKey));
                }
            }

            var detachedKeys = DetachKeys(keysToDetach);

            var detachedProperties = new List<Tuple<InternalPropertyBuilder, ConfigurationSource>>();
            foreach (var propertyToDetach in propertiesToDetachList)
            {
                var property = propertyToDetach.DeclaringEntityType.FindDeclaredProperty(propertyToDetach.Name);
                if (property != null)
                {
                    var entityTypeBuilder = propertyToDetach.DeclaringEntityType.Builder;
                    var propertyBuilder = propertyToDetach.Builder;
                    var removedConfigurationSource = entityTypeBuilder
                        .RemoveProperty(propertyToDetach, propertyToDetach.GetConfigurationSource());
                    Debug.Assert(removedConfigurationSource.HasValue);
                    detachedProperties.Add(Tuple.Create(propertyBuilder, removedConfigurationSource.Value));
                }
            }

            return new PropertyBuildersSnapshot(detachedProperties, detachedIndexes, detachedKeys, detachedRelationships);
        }

        private class PropertyBuildersSnapshot
        {
            public PropertyBuildersSnapshot(
                IReadOnlyList<Tuple<InternalPropertyBuilder, ConfigurationSource>> properties,
                IReadOnlyList<IndexBuilderSnapshot> indexes,
                KeyBuildersSnapshot keys,
                IReadOnlyList<RelationshipBuilderSnapshot> relationships)
            {
                Properties = properties;
                Indexes = indexes;
                Keys = keys;
                Relationships = relationships;
            }

            private IReadOnlyList<Tuple<InternalPropertyBuilder, ConfigurationSource>> Properties { get; }
            private IReadOnlyList<RelationshipBuilderSnapshot> Relationships { get; }
            private IReadOnlyList<IndexBuilderSnapshot> Indexes { get; }
            private KeyBuildersSnapshot Keys { get; }

            public void Attach(InternalEntityTypeBuilder entityTypeBuilder)
            {
                foreach (var propertyTuple in Properties)
                {
                    propertyTuple.Item1.Attach(entityTypeBuilder, propertyTuple.Item2);
                }

                Keys?.Attach();

                foreach (var indexBuilderSnapshot in Indexes)
                {
                    indexBuilderSnapshot.Attach();
                }

                if (Relationships != null)
                {
                    foreach (var detachedRelationship in Relationships)
                    {
                        detachedRelationship.Attach();
                    }
                }
            }
        }

        private List<ForeignKey> FindConflictingRelationships(
            EntityType baseEntityType,
            ConfigurationSource configurationSource)
        {
            var relationshipsToBeDetached = new List<ForeignKey>();
            foreach (var navigation in Metadata.GetDerivedNavigationsInclusive())
            {
                if (!navigation.ForeignKey.GetConfigurationSource().Overrides(
                    baseEntityType.FindIgnoredMemberConfigurationSource(navigation.Name)))
                {
                    relationshipsToBeDetached.Add(navigation.ForeignKey);
                    continue;
                }

                var baseNavigation = baseEntityType.FindNavigation(navigation.Name);
                if (baseNavigation == null)
                {
                    continue;
                }

                // When reattached the FK will override the other one if not compatible
                if (navigation.ForeignKey.DeclaringEntityType.Builder
                    .CanRemoveForeignKey(navigation.ForeignKey, configurationSource))
                {
                    relationshipsToBeDetached.Add(baseNavigation.ForeignKey);
                }
                else if (baseNavigation.ForeignKey.DeclaringEntityType.Builder
                    .CanRemoveForeignKey(baseNavigation.ForeignKey, configurationSource))
                {
                    relationshipsToBeDetached.Add(navigation.ForeignKey);
                }
                else
                {
                    return null;
                }
            }

            return relationshipsToBeDetached;
        }

        private ConfigurationSource? RemoveProperty(
            Property property, ConfigurationSource configurationSource, bool canOverrideSameSource = true)
        {
            var currentConfigurationSource = property.GetConfigurationSource();
            if (!configurationSource.Overrides(currentConfigurationSource)
                || !(canOverrideSameSource || (configurationSource != currentConfigurationSource)))
            {
                return null;
            }

            using (Metadata.Model.ConventionDispatcher.StartBatch())
            {
                var detachedRelationships = property.GetContainingForeignKeys().ToList()
                    .Select(DetachRelationship).ToList();

                foreach (var key in property.GetContainingKeys().ToList())
                {
                    detachedRelationships.AddRange(key.GetReferencingForeignKeys().ToList()
                        .Select(DetachRelationship));
                    var removed = RemoveKey(key, configurationSource);
                    Debug.Assert(removed.HasValue);
                }

                foreach (var index in property.GetContainingIndexes().ToList())
                {
                    var removed = RemoveIndex(index, configurationSource);
                    Debug.Assert(removed.HasValue);
                }

                if (property.Builder != null)
                {
                    var removedProperty = Metadata.RemoveProperty(property.Name);
                    Debug.Assert(removedProperty == property);
                }

                foreach (var detachedRelationship in detachedRelationships)
                {
                    detachedRelationship.Attach();
                }
            }

            return currentConfigurationSource;
        }

        private static RelationshipBuilderSnapshot DetachRelationship([NotNull] ForeignKey foreignKey)
        {
            var relationshipBuilder = foreignKey.Builder;
            var relationshipConfigurationSource = foreignKey.DeclaringEntityType.Builder
                .RemoveForeignKey(foreignKey, foreignKey.GetConfigurationSource());
            Debug.Assert(relationshipConfigurationSource != null);

            return new RelationshipBuilderSnapshot(relationshipBuilder, relationshipConfigurationSource.Value);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ConfigurationSource? RemoveForeignKey(
            [NotNull] ForeignKey foreignKey,
            ConfigurationSource configurationSource,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            bool canOverrideSameSource = true)
        {
            Debug.Assert(foreignKey.DeclaringEntityType == Metadata);

            var currentConfigurationSource = foreignKey.GetConfigurationSource();
            if (!configurationSource.Overrides(currentConfigurationSource)
                || !(canOverrideSameSource || (configurationSource != currentConfigurationSource)))
            {
                return null;
            }

            var removedForeignKey = Metadata.RemoveForeignKey(
                foreignKey.Properties, foreignKey.PrincipalKey, foreignKey.PrincipalEntityType);

            if (removedForeignKey == null)
            {
                return null;
            }
            Debug.Assert(removedForeignKey == foreignKey);

            RemoveShadowPropertiesIfUnused(foreignKey.Properties.Where(p => p.DeclaringEntityType.FindDeclaredProperty(p.Name) != null));
            foreignKey.PrincipalKey.DeclaringEntityType.Builder?.RemoveKeyIfUnused(foreignKey.PrincipalKey);

            return currentConfigurationSource;
        }

        private void RemoveKeyIfUnused(Key key)
        {
            if (Metadata.FindPrimaryKey() == key)
            {
                return;
            }

            if (key.GetReferencingForeignKeys().Any())
            {
                return;
            }

            RemoveKey(key, ConfigurationSource.Convention);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void RemoveShadowPropertiesIfUnused([NotNull] IEnumerable<Property> properties)
        {
            foreach (var property in properties.ToList())
            {
                if (property != null
                    && property.IsShadowProperty)
                {
                    RemovePropertyIfUnused(property);
                }
            }
        }

        private static void RemovePropertyIfUnused(Property property)
        {
            if (!property.DeclaringEntityType.Builder.CanRemoveProperty(property, ConfigurationSource.Convention))
            {
                return;
            }

            if (property.GetContainingIndexes().Any())
            {
                return;
            }

            if (property.GetContainingForeignKeys().Any())
            {
                return;
            }

            if (property.GetContainingKeys().Any())
            {
                return;
            }

            var removedProperty = property.DeclaringEntityType.RemoveProperty(property.Name);
            Debug.Assert(removedProperty == property);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalIndexBuilder HasIndex([NotNull] IReadOnlyList<string> propertyNames, ConfigurationSource configurationSource)
            => HasIndex(GetOrCreateProperties(propertyNames, configurationSource), configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalIndexBuilder HasIndex([NotNull] IReadOnlyList<PropertyInfo> clrProperties, ConfigurationSource configurationSource)
            => HasIndex(GetOrCreateProperties(clrProperties, configurationSource), configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalIndexBuilder HasIndex([CanBeNull] IReadOnlyList<Property> properties, ConfigurationSource configurationSource)
        {
            if (properties == null)
            {
                return null;
            }

            List<IndexBuilderSnapshot> detachedIndexes = null;
            var existingIndex = Metadata.FindIndex(properties);
            if (existingIndex == null)
            {
                detachedIndexes = Metadata.FindDerivedIndexes(properties).ToList().Select(DetachIndex).ToList();
            }
            else if (existingIndex.DeclaringEntityType != Metadata)
            {
                return existingIndex.DeclaringEntityType.Builder.HasIndex(existingIndex, properties, configurationSource);
            }

            var indexBuilder = HasIndex(existingIndex, properties, configurationSource);

            if (detachedIndexes != null)
            {
                foreach (var indexBuilderSnapshot in detachedIndexes)
                {
                    indexBuilderSnapshot.Attach();
                }
            }

            return indexBuilder;
        }

        private InternalIndexBuilder HasIndex(
            Index index, IReadOnlyList<Property> properties, ConfigurationSource configurationSource)
        {
            if (index == null)
            {
                index = Metadata.AddIndex(properties, configurationSource);
            }
            else
            {
                index.UpdateConfigurationSource(configurationSource);
            }

            return index?.Builder;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual ConfigurationSource? RemoveIndex([NotNull] Index index, ConfigurationSource configurationSource)
        {
            var currentConfigurationSource = index.GetConfigurationSource();
            if (!configurationSource.Overrides(currentConfigurationSource))
            {
                return null;
            }

            var removedIndex = Metadata.RemoveIndex(index.Properties);
            Debug.Assert(removedIndex == index);

            RemoveShadowPropertiesIfUnused(index.Properties);

            return currentConfigurationSource;
        }

        private class IndexBuilderSnapshot
        {
            public IndexBuilderSnapshot(InternalIndexBuilder index, ConfigurationSource configurationSource)
            {
                Index = index;
                IndexConfigurationSource = configurationSource;
            }

            private InternalIndexBuilder Index { get; }
            private ConfigurationSource IndexConfigurationSource { get; }

            public void Attach() => Index.Attach(IndexConfigurationSource);
        }

        private static IndexBuilderSnapshot DetachIndex(Index indexToDetach)
        {
            var entityTypeBuilder = indexToDetach.DeclaringEntityType.Builder;
            var indexBuilder = indexToDetach.Builder;
            var removedConfigurationSource = entityTypeBuilder.RemoveIndex(indexToDetach, indexToDetach.GetConfigurationSource());
            Debug.Assert(removedConfigurationSource != null);
            return new IndexBuilderSnapshot(indexBuilder, removedConfigurationSource.Value);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder HasForeignKey(
            [NotNull] string principalEntityTypeName,
            [NotNull] IReadOnlyList<string> propertyNames,
            ConfigurationSource configurationSource)
        {
            Check.NotEmpty(principalEntityTypeName, nameof(principalEntityTypeName));
            Check.NotEmpty(propertyNames, nameof(propertyNames));

            var principalType = ModelBuilder.Entity(principalEntityTypeName, configurationSource);
            return principalType == null
                ? null
                : HasForeignKeyInternal(
                    principalType,
                    GetOrCreateProperties(
                        propertyNames, configurationSource, principalType.Metadata.FindPrimaryKey()?.Properties, useDefaultType: true),
                    null,
                    configurationSource);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder HasForeignKey(
            [NotNull] string principalEntityTypeName,
            [NotNull] IReadOnlyList<string> propertyNames,
            [NotNull] Key principalKey,
            ConfigurationSource configurationSource)
        {
            Check.NotEmpty(principalEntityTypeName, nameof(principalEntityTypeName));
            Check.NotEmpty(propertyNames, nameof(propertyNames));

            var principalType = ModelBuilder.Entity(principalEntityTypeName, configurationSource);
            return principalType == null
                ? null
                : HasForeignKeyInternal(
                    principalType,
                    GetOrCreateProperties(propertyNames, configurationSource, principalKey.Properties, useDefaultType: true),
                    principalKey,
                    configurationSource);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder HasForeignKey(
            [NotNull] Type principalClrType,
            [NotNull] IReadOnlyList<PropertyInfo> clrProperties,
            ConfigurationSource configurationSource)
        {
            Check.NotNull(principalClrType, nameof(principalClrType));
            Check.NotEmpty(clrProperties, nameof(clrProperties));

            var principalType = ModelBuilder.Entity(principalClrType, configurationSource);
            return principalType == null
                ? null
                : HasForeignKeyInternal(
                    principalType,
                    GetOrCreateProperties(clrProperties, configurationSource),
                    null,
                    configurationSource);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder HasForeignKey(
            [NotNull] Type principalClrType,
            [NotNull] IReadOnlyList<PropertyInfo> clrProperties,
            [NotNull] Key principalKey,
            ConfigurationSource configurationSource)
        {
            Check.NotNull(principalClrType, nameof(principalClrType));
            Check.NotEmpty(clrProperties, nameof(clrProperties));

            var principalType = ModelBuilder.Entity(principalClrType, configurationSource);
            return principalType == null
                ? null
                : HasForeignKeyInternal(
                    principalType,
                    GetOrCreateProperties(clrProperties, configurationSource),
                    principalKey,
                    configurationSource);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder HasForeignKey(
            [NotNull] InternalEntityTypeBuilder principalEntityTypeBuilder,
            [NotNull] IReadOnlyList<Property> dependentProperties,
            ConfigurationSource configurationSource)
            => HasForeignKeyInternal(principalEntityTypeBuilder,
                GetActualProperties(dependentProperties, configurationSource),
                null,
                configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder HasForeignKey(
            [NotNull] InternalEntityTypeBuilder principalEntityTypeBuilder,
            [NotNull] IReadOnlyList<Property> dependentProperties,
            [NotNull] Key principalKey,
            ConfigurationSource configurationSource)
            => HasForeignKeyInternal(principalEntityTypeBuilder,
                GetActualProperties(dependentProperties, configurationSource),
                principalKey,
                configurationSource);

        private InternalRelationshipBuilder HasForeignKeyInternal(
            [NotNull] InternalEntityTypeBuilder principalEntityTypeBuilder,
            [CanBeNull] IReadOnlyList<Property> dependentProperties,
            [CanBeNull] Key principalKey,
            ConfigurationSource configurationSource)
        {
            if (dependentProperties == null)
            {
                return null;
            }

            var newRelationship = RelationshipInternal(principalEntityTypeBuilder, principalKey, configurationSource);
            var relationship = newRelationship.HasForeignKey(dependentProperties, configurationSource);
            if (relationship == null
                && newRelationship.Metadata.Builder != null)
            {
                RemoveForeignKey(newRelationship.Metadata, configurationSource);
            }

            return relationship;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Relationship(
            [NotNull] InternalEntityTypeBuilder targetEntityTypeBuilder,
            [CanBeNull] string navigationToTargetName,
            [CanBeNull] string inverseNavigationName,
            ConfigurationSource configurationSource,
            bool setTargetAsPrincipal = false)
            => Relationship(
                Check.NotNull(targetEntityTypeBuilder, nameof(targetEntityTypeBuilder)),
                PropertyIdentity.Create(navigationToTargetName),
                PropertyIdentity.Create(inverseNavigationName),
                setTargetAsPrincipal,
                configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Relationship(
            [NotNull] InternalEntityTypeBuilder targetEntityTypeBuilder,
            [CanBeNull] PropertyInfo navigationToTarget,
            [CanBeNull] PropertyInfo inverseNavigation,
            ConfigurationSource configurationSource,
            bool setTargetAsPrincipal = false)
            => Relationship(
                Check.NotNull(targetEntityTypeBuilder, nameof(targetEntityTypeBuilder)),
                PropertyIdentity.Create(navigationToTarget),
                PropertyIdentity.Create(inverseNavigation),
                setTargetAsPrincipal,
                configurationSource);

        private InternalRelationshipBuilder Relationship(
            InternalEntityTypeBuilder targetEntityTypeBuilder,
            PropertyIdentity? navigationToTarget,
            PropertyIdentity? inverseNavigation,
            bool setTargetAsPrincipal,
            ConfigurationSource configurationSource,
            bool? required = null)
        {
            Check.NotNull(targetEntityTypeBuilder, nameof(targetEntityTypeBuilder));

            Debug.Assert(navigationToTarget != null
                         || inverseNavigation != null);

            var navigationProperty = navigationToTarget?.Property;
            if (inverseNavigation == null
                && navigationProperty != null
                && !navigationProperty.PropertyType.GetTypeInfo().IsAssignableFrom(
                    targetEntityTypeBuilder.Metadata.ClrType.GetTypeInfo()))
            {
                // Only one nav specified and it can't be the nav to principal
                return targetEntityTypeBuilder.Relationship(
                    this, null, navigationToTarget, setTargetAsPrincipal, configurationSource, required);
            }

            var existingRelationship = InternalRelationshipBuilder.FindCurrentRelationshipBuilder(
                targetEntityTypeBuilder.Metadata,
                Metadata,
                navigationToTarget,
                inverseNavigation,
                dependentProperties: null,
                principalProperties: null);
            if (existingRelationship != null)
            {
                if (navigationToTarget != null)
                {
                    existingRelationship.Metadata.UpdateDependentToPrincipalConfigurationSource(configurationSource);
                    if (navigationToTarget.Value.Name != null)
                    {
                        Metadata.Unignore(navigationToTarget.Value.Name);
                    }
                }
                if (inverseNavigation != null)
                {
                    existingRelationship.Metadata.UpdatePrincipalToDependentConfigurationSource(configurationSource);
                    if (inverseNavigation.Value.Name != null)
                    {
                        targetEntityTypeBuilder.Metadata.Unignore(inverseNavigation.Value.Name);
                    }
                }
                existingRelationship.Metadata.UpdateConfigurationSource(configurationSource);
                if (!setTargetAsPrincipal)
                {
                    if (required.HasValue)
                    {
                        existingRelationship.IsRequired(required.Value, configurationSource);
                    }
                    return existingRelationship;
                }
            }

            existingRelationship = InternalRelationshipBuilder.FindCurrentRelationshipBuilder(
                Metadata,
                targetEntityTypeBuilder.Metadata,
                inverseNavigation,
                navigationToTarget,
                dependentProperties: null,
                principalProperties: null);
            if (existingRelationship != null)
            {
                if (navigationToTarget != null)
                {
                    existingRelationship.Metadata.UpdatePrincipalToDependentConfigurationSource(configurationSource);
                    if (navigationToTarget.Value.Name != null)
                    {
                        Metadata.Unignore(navigationToTarget.Value.Name);
                    }
                }
                if (inverseNavigation != null)
                {
                    existingRelationship.Metadata.UpdateDependentToPrincipalConfigurationSource(configurationSource);
                    if (inverseNavigation.Value.Name != null)
                    {
                        targetEntityTypeBuilder.Metadata.Unignore(inverseNavigation.Value.Name);
                    }
                }
                existingRelationship.Metadata.UpdateConfigurationSource(configurationSource);
                if (!setTargetAsPrincipal)
                {
                    if (required.HasValue)
                    {
                        existingRelationship.IsRequired(required.Value, configurationSource);
                    }
                    return existingRelationship;
                }
            }

            InternalRelationshipBuilder relationship;
            InternalRelationshipBuilder newRelationship = null;
            using (var batcher = Metadata.Model.ConventionDispatcher.StartBatch())
            {
                if (existingRelationship != null)
                {
                    relationship = existingRelationship;
                }
                else
                {
                    if (setTargetAsPrincipal
                        || targetEntityTypeBuilder.Metadata.DefiningEntityType != Metadata)
                    {
                        newRelationship = CreateForeignKey(
                            targetEntityTypeBuilder,
                            dependentProperties: null,
                            principalKey: null,
                            navigationToPrincipalName: navigationProperty?.Name,
                            isRequired: required,
                            configurationSource: configurationSource);
                    }
                    else
                    {
                        var navigation = navigationToTarget;
                        navigationToTarget = inverseNavigation;
                        inverseNavigation = navigation;

                        navigationProperty = navigationToTarget?.Property;

                        newRelationship = targetEntityTypeBuilder.CreateForeignKey(
                            this,
                            dependentProperties: null,
                            principalKey: null,
                            navigationToPrincipalName: navigationProperty?.Name,
                            isRequired: null,
                            configurationSource: configurationSource);
                    }

                    relationship = newRelationship;
                }

                if (setTargetAsPrincipal)
                {
                    relationship = relationship
                        .RelatedEntityTypes(targetEntityTypeBuilder.Metadata, Metadata, configurationSource);
                }

                var inverseProperty = inverseNavigation?.Property;
                if (inverseNavigation == null)
                {
                    relationship = navigationProperty != null
                        ? relationship.DependentToPrincipal(navigationProperty, configurationSource)
                        : relationship.DependentToPrincipal(navigationToTarget.Value.Name, configurationSource);
                }
                else if (navigationToTarget == null)
                {
                    relationship = inverseProperty != null
                        ? relationship.PrincipalToDependent(inverseProperty, configurationSource)
                        : relationship.PrincipalToDependent(inverseNavigation.Value.Name, configurationSource);
                }
                else
                {
                    relationship = navigationProperty != null || inverseProperty != null
                        ? relationship.Navigations(navigationProperty, inverseProperty, configurationSource)
                        : relationship.Navigations(navigationToTarget.Value.Name, inverseNavigation.Value.Name, configurationSource);
                }

                if (relationship != null)
                {
                    relationship = batcher.Run(relationship);
                }
            }

            if (relationship != null
                && ((navigationToTarget != null
                     && relationship.Metadata.DependentToPrincipal?.Name != navigationToTarget.Value.Name)
                    || (inverseNavigation != null
                        && relationship.Metadata.PrincipalToDependent?.Name != inverseNavigation.Value.Name))
                && ((inverseNavigation != null
                     && relationship.Metadata.DependentToPrincipal?.Name != inverseNavigation.Value.Name)
                    || (navigationToTarget != null
                        && relationship.Metadata.PrincipalToDependent?.Name != navigationToTarget.Value.Name)))
            {
                relationship = null;
            }

            if (relationship == null)
            {
                if (newRelationship?.Metadata.Builder != null)
                {
                    newRelationship.Metadata.DeclaringEntityType.Builder.RemoveForeignKey(newRelationship.Metadata, configurationSource);
                }
                return null;
            }

            return relationship;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Relationship(
            [NotNull] EntityType principalEntityType,
            ConfigurationSource configurationSource)
            => RelationshipInternal(principalEntityType.Builder, principalKey: null, configurationSource: configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Relationship(
            [NotNull] EntityType principalEntityType,
            [NotNull] Key principalKey,
            ConfigurationSource configurationSource)
            => RelationshipInternal(principalEntityType.Builder, principalKey, configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Relationship(
            [NotNull] InternalEntityTypeBuilder principalEntityTypeBuilder,
            ConfigurationSource configurationSource)
            => RelationshipInternal(principalEntityTypeBuilder, principalKey: null, configurationSource: configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Relationship(
            [NotNull] InternalEntityTypeBuilder principalEntityTypeBuilder,
            [NotNull] Key principalKey,
            ConfigurationSource configurationSource)
            => RelationshipInternal(principalEntityTypeBuilder, principalKey, configurationSource);

        private InternalRelationshipBuilder RelationshipInternal(
            InternalEntityTypeBuilder targetEntityTypeBuilder,
            Key principalKey,
            ConfigurationSource configurationSource)
        {
            InternalRelationshipBuilder relationship;
            InternalRelationshipBuilder newRelationship;
            using (var batch = Metadata.Model.ConventionDispatcher.StartBatch())
            {
                relationship = CreateForeignKey(
                    targetEntityTypeBuilder,
                    null,
                    principalKey,
                    null,
                    null,
                    configurationSource);

                newRelationship = relationship;
                if (principalKey != null)
                {
                    newRelationship = newRelationship.RelatedEntityTypes(targetEntityTypeBuilder.Metadata, Metadata, configurationSource)
                        ?.HasPrincipalKey(principalKey.Properties, configurationSource);
                }

                newRelationship = newRelationship == null ? null : batch.Run(newRelationship);
            }

            if (newRelationship == null)
            {
                if (relationship?.Metadata.Builder != null)
                {
                    relationship.Metadata.DeclaringEntityType.Builder.RemoveForeignKey(relationship.Metadata, configurationSource);
                }
                return null;
            }

            return newRelationship;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Owns(
            [NotNull] string targetEntityTypeName,
            [NotNull] string navigationName,
            ConfigurationSource configurationSource)
            => Owns(new TypeIdentity(targetEntityTypeName), PropertyIdentity.Create(navigationName),
                inverse: null, configurationSource: configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Owns(
            [NotNull] string targetEntityTypeName,
            [NotNull] PropertyInfo navigationProperty,
            ConfigurationSource configurationSource)
            => Owns(new TypeIdentity(targetEntityTypeName), PropertyIdentity.Create(navigationProperty),
                inverse: null, configurationSource: configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Owns(
            [NotNull] Type targetEntityType,
            [NotNull] string navigationName,
            ConfigurationSource configurationSource)
            => Owns(new TypeIdentity(targetEntityType), PropertyIdentity.Create(navigationName),
                inverse: null, configurationSource: configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Owns(
            [NotNull] Type targetEntityType,
            [NotNull] PropertyInfo navigationProperty,
            ConfigurationSource configurationSource)
            => Owns(new TypeIdentity(targetEntityType), PropertyIdentity.Create(navigationProperty),
                inverse: null, configurationSource: configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Owns(
            [NotNull] Type targetEntityType,
            [NotNull] PropertyInfo navigationProperty,
            [CanBeNull] PropertyInfo inverseProperty,
            ConfigurationSource configurationSource)
            => Owns(
                new TypeIdentity(targetEntityType),
                PropertyIdentity.Create(navigationProperty),
                PropertyIdentity.Create(inverseProperty),
                configurationSource);

        private InternalRelationshipBuilder Owns(
            TypeIdentity targetEntityType,
            PropertyIdentity navigation,
            PropertyIdentity? inverse,
            ConfigurationSource configurationSource)
        {
            InternalEntityTypeBuilder ownedEntityType;
            InternalRelationshipBuilder relationship;
            using (var batch = Metadata.Model.ConventionDispatcher.StartBatch())
            {
                var existingNavigation = Metadata
                    .FindNavigationsInHierarchy(navigation.Name)
                    .SingleOrDefault(n => n.GetTargetType().Name == targetEntityType.Name && n.GetTargetType().HasDefiningNavigation());
                
                var builder = existingNavigation?.ForeignKey.Builder;
                
                if (builder != null)
                {
                    builder = builder.RelatedEntityTypes(Metadata, existingNavigation.GetTargetType(), configurationSource);
                    builder = builder?.IsRequired(true, configurationSource);
                    builder = builder?.IsOwnership(true, configurationSource);
                    builder = builder?.Navigations(inverse, navigation, configurationSource);
                    
                    return builder == null ? null : batch.Run(builder);
                }

                ownedEntityType = targetEntityType.Type == null
                    ? ModelBuilder.Entity(targetEntityType.Name, navigation.Name, Metadata, configurationSource)
                    : ModelBuilder.Entity(targetEntityType.Type, navigation.Name, Metadata, configurationSource);

                if (ownedEntityType == null)
                {
                    return null;
                }

                relationship = ownedEntityType.Relationship(
                    targetEntityTypeBuilder: this,
                    navigationToTarget: inverse,
                    inverseNavigation: navigation,
                    setTargetAsPrincipal: true,
                    configurationSource: configurationSource,
                    required: true);
                relationship = batch.Run(relationship.IsOwnership(true, configurationSource));
            }

            if (relationship?.Metadata.Builder == null)
            {
                if (ownedEntityType.Metadata.Builder != null)
                {
                    ModelBuilder.RemoveEntityType(ownedEntityType.Metadata, configurationSource);
                }
                return null;
            }

            return relationship;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Navigation(
            [NotNull] InternalEntityTypeBuilder targetEntityTypeBuilder,
            [CanBeNull] string navigationName,
            ConfigurationSource configurationSource,
            bool setTargetAsPrincipal = false)
            => Relationship(
                Check.NotNull(targetEntityTypeBuilder, nameof(targetEntityTypeBuilder)),
                PropertyIdentity.Create(navigationName),
                null,
                setTargetAsPrincipal,
                configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder Navigation(
            [NotNull] InternalEntityTypeBuilder targetEntityTypeBuilder,
            [CanBeNull] PropertyInfo navigationProperty,
            ConfigurationSource configurationSource,
            bool setTargetAsPrincipal = false)
            => Relationship(
                Check.NotNull(targetEntityTypeBuilder, nameof(targetEntityTypeBuilder)),
                PropertyIdentity.Create(navigationProperty),
                null,
                setTargetAsPrincipal,
                configurationSource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual InternalRelationshipBuilder CreateForeignKey(
            [NotNull] InternalEntityTypeBuilder principalEntityTypeBuilder,
            [CanBeNull] IReadOnlyList<Property> dependentProperties,
            [CanBeNull] Key principalKey,
            [CanBeNull] string navigationToPrincipalName,
            bool? isRequired,
            ConfigurationSource configurationSource)
        {
            using (var batch = ModelBuilder.Metadata.ConventionDispatcher.StartBatch())
            {
                var principalType = principalEntityTypeBuilder.Metadata;
                var principalBaseEntityTypeBuilder = principalType.RootType().Builder;
                if (principalKey == null)
                {
                    principalKey = principalType.FindPrimaryKey();
                    if (principalKey != null
                        && dependentProperties != null)
                    {
                        if (!ForeignKey.AreCompatible(
                            principalKey.Properties,
                            dependentProperties,
                            principalType,
                            Metadata,
                            shouldThrow: false))
                        {
                            if (dependentProperties.All(p => p.GetTypeConfigurationSource() == null))
                            {
                                var detachedProperties = DetachProperties(dependentProperties);
                                GetOrCreateProperties(dependentProperties.Select(p => p.Name).ToList(), configurationSource, principalKey.Properties, isRequired ?? false);
                                detachedProperties.Attach(this);
                            }
                            else
                            {
                                principalKey = null;
                            }
                        }
                        else if (Metadata.FindForeignKeysInHierarchy(dependentProperties, principalKey, principalType).Any())
                        {
                            principalKey = null;
                        }
                    }
                }

                if (dependentProperties != null)
                {
                    dependentProperties = GetActualProperties(dependentProperties, ConfigurationSource.Convention);
                    if (principalKey == null)
                    {
                        var principalKeyProperties = principalBaseEntityTypeBuilder.CreateUniqueProperties(
                            dependentProperties.Count, null, Enumerable.Repeat("", dependentProperties.Count), dependentProperties.Select(p => p.ClrType), isRequired: true, baseName: "TempId");
                        var keyBuilder = principalBaseEntityTypeBuilder.HasKeyInternal(principalKeyProperties, ConfigurationSource.Convention);

                        principalKey = keyBuilder.Metadata;
                    }
                    else
                    {
                        Debug.Assert(Metadata.FindForeignKey(dependentProperties, principalKey, principalType) == null);
                    }
                }
                else
                {
                    if (principalKey == null)
                    {
                        var principalKeyProperties = principalBaseEntityTypeBuilder.CreateUniqueProperties(
                            1, null, new[] { "TempId" }, new[] { typeof(int) }, isRequired: true, baseName: "");

                        principalKey = principalBaseEntityTypeBuilder.HasKeyInternal(principalKeyProperties, ConfigurationSource.Convention).Metadata;
                    }

                    var baseName = string.IsNullOrEmpty(navigationToPrincipalName) ? principalType.ShortName() : navigationToPrincipalName;
                    dependentProperties = CreateUniqueProperties(null, principalKey.Properties, isRequired ?? false, baseName);
                }

                var foreignKey = Metadata.AddForeignKey(dependentProperties, principalKey, principalType, configurationSource: null);
                foreignKey.UpdateConfigurationSource(configurationSource);
                principalType.UpdateConfigurationSource(configurationSource);

                return batch.Run(foreignKey)?.Builder;
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<Property> ReUniquifyTemporaryProperties(
            [NotNull] IReadOnlyList<Property> currentProperties,
            [NotNull] IReadOnlyList<Property> principalProperties,
            bool isRequired,
            [NotNull] string baseName) => CreateUniqueProperties(currentProperties, principalProperties, isRequired, baseName);

        private IReadOnlyList<Property> CreateUniqueProperties(
            IReadOnlyList<Property> currentProperties,
            IReadOnlyList<Property> principalProperties,
            bool isRequired,
            string baseName)
            => CreateUniqueProperties(
                principalProperties.Count,
                currentProperties,
                principalProperties.Select(p => p.Name),
                principalProperties.Select(p => p.ClrType),
                isRequired,
                baseName);

        private IReadOnlyList<Property> CreateUniqueProperties(
            int propertyCount,
            IReadOnlyList<Property> currentProperties,
            IEnumerable<string> principalPropertyNames,
            IEnumerable<Type> principalPropertyTypes,
            bool isRequired,
            string baseName)
        {
            var newProperties = new Property[propertyCount];
            var clrMembers = Metadata.ClrType == null
                ? null
                : new HashSet<string>(Metadata.ClrType.GetRuntimeProperties().Select(p => p.Name)
                    .Concat(Metadata.ClrType.GetRuntimeFields().Select(p => p.Name)));
            var noNewProperties = true;
            using (var principalPropertyNamesEnumerator = principalPropertyNames.GetEnumerator())
            {
                using (var principalPropertyTypesEnumerator = principalPropertyTypes.GetEnumerator())
                {
                    for (var i = 0; i < newProperties.Length
                                    && principalPropertyNamesEnumerator.MoveNext()
                                    && principalPropertyTypesEnumerator.MoveNext(); i++)
                    {
                        var keyPropertyName = principalPropertyNamesEnumerator.Current;
                        var keyPropertyType = principalPropertyTypesEnumerator.Current;
                        var keyModifiedBaseName = (keyPropertyName.StartsWith(baseName, StringComparison.OrdinalIgnoreCase) ? "" : baseName)
                                                  + keyPropertyName;
                        string propertyName;
                        var clrType = isRequired ? keyPropertyType : keyPropertyType.MakeNullable();
                        var index = -1;
                        while (true)
                        {
                            propertyName = keyModifiedBaseName + (++index > 0 ? index.ToString(CultureInfo.InvariantCulture) : "");
                            if (!Metadata.FindPropertiesInHierarchy(propertyName).Any()
                                && clrMembers?.Contains(propertyName) != true)
                            {
                                var propertyBuilder = Property(propertyName, clrType, ConfigurationSource.Convention, typeConfigurationSource: null);
                                if (propertyBuilder == null)
                                {
                                    RemoveShadowPropertiesIfUnused(newProperties);
                                    return null;
                                }

                                if (clrType.IsNullableType())
                                {
                                    propertyBuilder.IsRequired(isRequired, ConfigurationSource.Convention);
                                }
                                newProperties[i] = propertyBuilder.Metadata;
                                noNewProperties = false;
                                break;
                            }
                            if (currentProperties != null
                                && newProperties.All(p => p == null || p.Name != propertyName))
                            {
                                var currentProperty = currentProperties.SingleOrDefault(p => p.Name == propertyName);
                                if (currentProperty != null
                                    && currentProperty.ClrType == clrType
                                    && currentProperty.IsNullable == !isRequired)
                                {
                                    newProperties[i] = currentProperty;
                                    noNewProperties = noNewProperties && newProperties[i] == currentProperties[i];
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return noNewProperties ? null : newProperties;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<Property> GetOrCreateProperties(
            [CanBeNull] IReadOnlyList<string> propertyNames,
            ConfigurationSource configurationSource,
            [CanBeNull] IReadOnlyList<Property> referencedProperties = null,
            bool required = false,
            bool useDefaultType = false)
        {
            if (propertyNames == null)
            {
                return null;
            }

            if (referencedProperties != null
                && referencedProperties.Count != propertyNames.Count)
            {
                referencedProperties = null;
            }

            var propertyList = new List<Property>();
            for (var i = 0; i < propertyNames.Count; i++)
            {
                var propertyName = propertyNames[i];
                var property = Metadata.FindProperty(propertyName);
                if (property == null)
                {
                    var clrProperty = Metadata.ClrType?.GetMembersInHierarchy(propertyName).FirstOrDefault();
                    var type = referencedProperties == null
                        ? useDefaultType ? typeof(int) : null
                        : referencedProperties[i].ClrType;

                    InternalPropertyBuilder propertyBuilder;
                    if (clrProperty != null)
                    {
                        propertyBuilder = Property(clrProperty, configurationSource);
                    }
                    else if (type != null)
                    {
                        // TODO: Log that a shadow property is created
                        propertyBuilder = Property(
                            propertyName, required ? type : type.MakeNullable(), configurationSource, typeConfigurationSource: null);
                    }
                    else
                    {
                        throw new InvalidOperationException(CoreStrings.NoPropertyType(propertyName, Metadata.DisplayName()));
                    }

                    if (propertyBuilder == null)
                    {
                        return null;
                    }
                    property = propertyBuilder.Metadata;
                }
                else
                {
                    property.DeclaringEntityType.UpdateConfigurationSource(configurationSource);
                    property = property.DeclaringEntityType.Builder.Property(property.Name, configurationSource).Metadata;
                }
                propertyList.Add(property);
            }
            return propertyList;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<Property> GetOrCreateProperties([CanBeNull] IEnumerable<MemberInfo> clrProperties, ConfigurationSource configurationSource)
        {
            if (clrProperties == null)
            {
                return null;
            }

            var list = new List<Property>();
            foreach (var propertyInfo in clrProperties)
            {
                var propertyBuilder = Property(propertyInfo, configurationSource);
                if (propertyBuilder == null)
                {
                    return null;
                }

                list.Add(propertyBuilder.Metadata);
            }
            return list;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<Property> GetActualProperties([CanBeNull] IReadOnlyList<Property> properties, ConfigurationSource? configurationSource)
        {
            if (properties == null)
            {
                return null;
            }

            var actualProperties = new Property[properties.Count];
            for (var i = 0; i < actualProperties.Length; i++)
            {
                var property = properties[i];
                var builder = property.Builder != null && property.DeclaringEntityType.IsAssignableFrom(Metadata)
                    ? property.Builder
                    : Metadata.FindProperty(property.Name)?.Builder
                      ?? (property.IsShadowProperty
                          ? null
                          : Property(property.Name, property.ClrType, property.PropertyInfo, configurationSource, property.GetTypeConfigurationSource()));
                if (builder == null)
                {
                    return null;
                }

                actualProperties[i] = builder.Metadata;
            }

            return actualProperties;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual bool UsePropertyAccessMode(PropertyAccessMode propertyAccessMode, ConfigurationSource configurationSource)
            => HasAnnotation(CoreAnnotationNames.PropertyAccessModeAnnotation, propertyAccessMode, configurationSource);

        private struct RelationshipSnapshot
        {
            public readonly ForeignKey ForeignKey;
            public readonly Navigation NavigationFrom;
            public readonly Navigation NavigationTo;
            public readonly bool IsDependent;

            public RelationshipSnapshot(ForeignKey foreignKey, Navigation navigationFrom, Navigation navigationTo, bool isDependent)
            {
                ForeignKey = foreignKey;
                NavigationFrom = navigationFrom;
                NavigationTo = navigationTo;
                IsDependent = isDependent;
            }
        }

        private class RelationshipBuilderSnapshot
        {
            public RelationshipBuilderSnapshot(
                InternalRelationshipBuilder relationship,
                ConfigurationSource relationshipConfigurationSource)
            {
                Relationship = relationship;
                RelationshipConfigurationSource = relationshipConfigurationSource;
            }

            private InternalRelationshipBuilder Relationship { [DebuggerStepThrough] get; }
            private ConfigurationSource RelationshipConfigurationSource { [DebuggerStepThrough] get; }

            [DebuggerStepThrough]
            public void Attach()
                => Relationship.Attach(RelationshipConfigurationSource);
        }
    }
}
