// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    /// <summary>
    ///     <para>
    ///         Provides a simple API for configuring a one-to-one ownership.
    ///     </para>
    /// </summary>
    public class ReferenceOwnershipBuilder : ReferenceReferenceBuilderBase, IInfrastructure<InternalEntityTypeBuilder>
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public ReferenceOwnershipBuilder(
            [NotNull] EntityType declaringEntityType,
            [NotNull] EntityType relatedEntityType,
            [NotNull] InternalRelationshipBuilder builder)
            : base(builder,
                new ReferenceReferenceBuilderBase(declaringEntityType, relatedEntityType, builder))
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected ReferenceOwnershipBuilder(
            InternalRelationshipBuilder builder,
            ReferenceOwnershipBuilder oldBuilder,
            bool inverted = false,
            bool foreignKeySet = false,
            bool principalKeySet = false,
            bool requiredSet = false)
            : base(builder, oldBuilder, inverted, foreignKeySet, principalKeySet, requiredSet)
        {
        }

        /// <summary>
        ///     Gets the internal builder being used to configure the entity type.
        /// </summary>
        InternalEntityTypeBuilder IInfrastructure<InternalEntityTypeBuilder>.Instance => RelatedEntityType.Builder;

        /// <summary>
        ///     The entity type being configured.
        /// </summary>
        public virtual IMutableEntityType OwnedEntityType => RelatedEntityType;

        /// <summary>
        ///     Adds or updates an annotation on the foreign key. If an annotation with the key specified in
        ///     <paramref name="annotation" /> already exists its value will be updated.
        /// </summary>
        /// <param name="annotation"> The key of the annotation to be added or updated. </param>
        /// <param name="value"> The value to be stored in the annotation. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceOwnershipBuilder HasForeignKeyAnnotation([NotNull] string annotation, [NotNull] object value)
        {
            Check.NotEmpty(annotation, nameof(annotation));
            Check.NotNull(value, nameof(value));

            Builder.HasAnnotation(annotation, value, ConfigurationSource.Explicit);

            return this;
        }

        /// <summary>
        ///     <para>
        ///         Configures the property(s) to use as the foreign key for this relationship.
        ///     </para>
        ///     <para>
        ///         If the specified property name(s) do not exist on the entity type then a new shadow state
        ///         property(s) will be added to serve as the foreign key. A shadow state property is one
        ///         that does not have a corresponding property in the entity class. The current value for the
        ///         property is stored in the <see cref="ChangeTracker" /> rather than being stored in instances
        ///         of the entity class.
        ///     </para>
        ///     <para>
        ///         If <see cref="HasPrincipalKey(string[])" /> is not specified, then an attempt will be made to
        ///         match the data type and order of foreign key properties against the primary key of the principal
        ///         entity type. If they do not match, new shadow state properties that form a unique index will be
        ///         added to the principal entity type to serve as the reference key.
        ///     </para>
        /// </summary>
        /// <param name="foreignKeyPropertyNames">
        ///     The name(s) of the foreign key property(s).
        /// </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceOwnershipBuilder HasForeignKey(
            [NotNull] params string[] foreignKeyPropertyNames)
            => new ReferenceOwnershipBuilder(
                Builder.HasForeignKey(
                    Check.NotNull(foreignKeyPropertyNames, nameof(foreignKeyPropertyNames)),
                    RelatedEntityType,
                    ConfigurationSource.Explicit),
                this,
                foreignKeySet: true);

        /// <summary>
        ///     Configures the unique property(s) that this relationship targets. Typically you would only call this
        ///     method if you want to use a property(s) other than the primary key as the principal property(s). If
        ///     the specified property(s) is not already a unique constraint (or the primary key) then a new unique
        ///     constraint will be introduced.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If multiple principal key properties are specified, the order of principal key properties should
        ///         match the order that the primary key or unique constraint properties were configured on the principal
        ///         entity type.
        ///     </para>
        /// </remarks>
        /// <param name="keyPropertyNames"> The name(s) of the reference key property(s). </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceOwnershipBuilder HasPrincipalKey(
            [NotNull] params string[] keyPropertyNames)
            => new ReferenceOwnershipBuilder(
                Builder.HasPrincipalKey(
                    Check.NotNull(keyPropertyNames, nameof(keyPropertyNames)),
                    ConfigurationSource.Explicit),
                this,
                principalKeySet: true);

        /// <summary>
        ///     Configures how a delete operation is applied to dependent entities in the relationship when the
        ///     principal is deleted or the relationship is severed.
        /// </summary>
        /// <param name="deleteBehavior"> The action to perform. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceOwnershipBuilder OnDelete(DeleteBehavior deleteBehavior)
            => new ReferenceOwnershipBuilder(Builder.DeleteBehavior(deleteBehavior, ConfigurationSource.Explicit), this);

        /// <summary>
        ///     Adds or updates an annotation on the entity type. If an annotation with the key specified in
        ///     <paramref name="annotation" /> already exists its value will be updated.
        /// </summary>
        /// <param name="annotation"> The key of the annotation to be added or updated. </param>
        /// <param name="value"> The value to be stored in the annotation. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceOwnershipBuilder HasEntityTypeAnnotation([NotNull] string annotation, [NotNull] object value)
        {
            Check.NotEmpty(annotation, nameof(annotation));
            Check.NotNull(value, nameof(value));

            RelatedEntityType.Builder.HasAnnotation(annotation, value, ConfigurationSource.Explicit);

            return this;
        }

        /// <summary>
        ///     <para>
        ///         Returns an object that can be used to configure a property of the entity type.
        ///         If no property with the given name exists, then a new property will be added.
        ///     </para>
        ///     <para>
        ///         When adding a new property, if a property with the same name exists in the entity class
        ///         then it will be added to the model. If no property exists in the entity class, then
        ///         a new shadow state property will be added. A shadow state property is one that does not have a
        ///         corresponding property in the entity class. The current value for the property is stored in
        ///         the <see cref="ChangeTracker" /> rather than being stored in instances of the entity class.
        ///     </para>
        /// </summary>
        /// <typeparam name="TProperty"> The type of the property to be configured. </typeparam>
        /// <param name="propertyName"> The name of the property to be configured. </param>
        /// <returns> An object that can be used to configure the property. </returns>
        public virtual PropertyBuilder<TProperty> Property<TProperty>([NotNull] string propertyName)
            => new PropertyBuilder<TProperty>(RelatedEntityType.Builder.Property(
                Check.NotEmpty(propertyName, nameof(propertyName)),
                typeof(TProperty),
                ConfigurationSource.Explicit));

        /// <summary>
        ///     <para>
        ///         Returns an object that can be used to configure a property of the entity type.
        ///         If no property with the given name exists, then a new property will be added.
        ///     </para>
        ///     <para>
        ///         When adding a new property, if a property with the same name exists in the entity class
        ///         then it will be added to the model. If no property exists in the entity class, then
        ///         a new shadow state property will be added. A shadow state property is one that does not have a
        ///         corresponding property in the entity class. The current value for the property is stored in
        ///         the <see cref="ChangeTracker" /> rather than being stored in instances of the entity class.
        ///     </para>
        /// </summary>
        /// <param name="propertyType"> The type of the property to be configured. </param>
        /// <param name="propertyName"> The name of the property to be configured. </param>
        /// <returns> An object that can be used to configure the property. </returns>
        public virtual PropertyBuilder Property([NotNull] Type propertyType, [NotNull] string propertyName)
            => new PropertyBuilder(RelatedEntityType.Builder.Property(
                Check.NotEmpty(propertyName, nameof(propertyName)),
                Check.NotNull(propertyType, nameof(propertyType)),
                ConfigurationSource.Explicit));

        /// <summary>
        ///     <para>
        ///         Returns an object that can be used to configure a property of the entity type.
        ///         If no property with the given name exists, then a new property will be added.
        ///     </para>
        ///     <para>
        ///         When adding a new property with this overload the property name must match the
        ///         name of a CLR property or field on the entity type. This overload cannot be used to
        ///         add a new shadow state property.
        ///     </para>
        /// </summary>
        /// <param name="propertyName"> The name of the property to be configured. </param>
        /// <returns> An object that can be used to configure the property. </returns>
        public virtual PropertyBuilder Property([NotNull] string propertyName)
            => new PropertyBuilder(RelatedEntityType.Builder.Property(
                Check.NotEmpty(propertyName, nameof(propertyName)),
                ConfigurationSource.Explicit));

        /// <summary>
        ///     Excludes the given property from the entity type. This method is typically used to remove properties
        ///     from the entity type that were added by convention.
        /// </summary>
        /// <param name="propertyName"> The name of then property to be removed from the entity type. </param>
        public virtual ReferenceOwnershipBuilder Ignore([NotNull] string propertyName)
        {
            Check.NotEmpty(propertyName, nameof(propertyName));

            RelatedEntityType.Builder.Ignore(propertyName, ConfigurationSource.Explicit);

            return this;
        }

        /// <summary>
        ///     Configures an index on the specified properties. If there is an existing index on the given
        ///     set of properties, then the existing index will be returned for configuration.
        /// </summary>
        /// <param name="propertyNames"> The names of the properties that make up the index. </param>
        /// <returns> An object that can be used to configure the index. </returns>
        public virtual IndexBuilder HasIndex([NotNull] params string[] propertyNames)
            => new IndexBuilder(RelatedEntityType.Builder.HasIndex(
                Check.NotEmpty(propertyNames, nameof(propertyNames)), ConfigurationSource.Explicit));

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///         The target entity key value is always propagated from the entity it belongs to.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        /// </summary>
        /// <param name="ownedType"> The entity type that this relationship targets. </param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <returns> An object that can be used to configure the relationship. </returns>
        public virtual ReferenceOwnershipBuilder OwnsOne(
            [NotNull] Type ownedType,
            [NotNull] string navigationName)
            => OwnsOneBuilder(
                new TypeIdentity(Check.NotNull(ownedType, nameof(ownedType))),
                Check.NotEmpty(navigationName, nameof(navigationName)));

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///         The target entity key value is always propagated from the entity it belongs to.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        /// </summary>
        /// <param name="ownedTypeName"> The name of the entity type that this relationship targets. </param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <returns> An object that can be used to configure the relationship. </returns>
        public virtual ReferenceOwnershipBuilder OwnsOne(
            [NotNull] string ownedTypeName,
            [NotNull] string navigationName)
            => OwnsOneBuilder(
                new TypeIdentity(Check.NotEmpty(ownedTypeName, nameof(ownedTypeName))),
                Check.NotEmpty(navigationName, nameof(navigationName)));

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///         The target entity key value is always propagated from the entity it belongs to.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        /// </summary>
        /// <param name="ownedType"> The entity type that this relationship targets. </param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <param name="buildAction"> An action that performs configuration of the relationship. </param>
        /// <returns> An object that can be used to configure the entity type. </returns>
        public virtual ReferenceOwnershipBuilder OwnsOne(
            [NotNull] Type ownedType,
            [NotNull] string navigationName,
            [NotNull] Action<ReferenceOwnershipBuilder> buildAction)
        {
            Check.NotNull(ownedType, nameof(ownedType));
            Check.NotEmpty(navigationName, nameof(navigationName));
            Check.NotNull(buildAction, nameof(buildAction));

            using (DeclaringEntityType.Model.ConventionDispatcher.StartBatch())
            {
                buildAction.Invoke(OwnsOneBuilder(new TypeIdentity(ownedType), navigationName));
                return this;
            }
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where the target entity is owned by (or part of) this entity.
        ///         The target entity key value is always propagated from the entity it belongs to.
        ///     </para>
        ///     <para>
        ///         The target entity type for each ownership relationship is treated as a different entity type
        ///         even if the navigation is of the same type. Configuration of the target entity type
        ///         isn't applied to the target entity type of other ownership relationships.
        ///     </para>
        ///     <para>
        ///         Most operations on an owned entity require accessing it through the owner entity using the corresponding navigation.
        ///     </para>
        /// </summary>
        /// <param name="ownedTypeName"> The name of the entity type that this relationship targets. </param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship.
        /// </param>
        /// <param name="buildAction"> An action that performs configuration of the relationship. </param>
        /// <returns> An object that can be used to configure the entity type. </returns>
        public virtual ReferenceOwnershipBuilder OwnsOne(
            [NotNull] string ownedTypeName,
            [NotNull] string navigationName,
            [NotNull] Action<ReferenceOwnershipBuilder> buildAction)
        {
            Check.NotEmpty(ownedTypeName, nameof(ownedTypeName));
            Check.NotEmpty(navigationName, nameof(navigationName));
            Check.NotNull(buildAction, nameof(buildAction));

            using (DeclaringEntityType.Model.ConventionDispatcher.StartBatch())
            {
                buildAction.Invoke(OwnsOneBuilder(new TypeIdentity(ownedTypeName), navigationName));
                return this;
            }
        }

        private ReferenceOwnershipBuilder OwnsOneBuilder(TypeIdentity ownedType, string navigationName)
        {
            InternalRelationshipBuilder relationship;
            using (RelatedEntityType.Model.ConventionDispatcher.StartBatch())
            {
                relationship = ownedType.Type == null
                    ? RelatedEntityType.Builder.Owns(ownedType.Name, navigationName, ConfigurationSource.Explicit)
                    : RelatedEntityType.Builder.Owns(ownedType.Type, navigationName, ConfigurationSource.Explicit);
                relationship.IsUnique(true, ConfigurationSource.Explicit);
            }

            return new ReferenceOwnershipBuilder(
                RelatedEntityType,
                relationship.Metadata.DeclaringEntityType,
                relationship);
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where this entity type has a reference that points
        ///         to a single instance of the other type in the relationship.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="ReferenceNavigationBuilder.WithMany" />
        ///         or <see cref="ReferenceNavigationBuilder.WithOne" /> to fully configure
        ///         the relationship. Calling just this method without the chained call will not
        ///         produce a valid relationship.
        ///     </para>
        /// </summary>
        /// <param name="relatedType"> The entity type that this relationship targets. </param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship. If
        ///     no property is specified, the relationship will be configured without a navigation property on this
        ///     end.
        /// </param>
        /// <returns> An object that can be used to configure the relationship. </returns>
        public virtual ReferenceNavigationBuilder HasOne(
            [NotNull] Type relatedType,
            [CanBeNull] string navigationName = null)
        {
            Check.NotNull(relatedType, nameof(relatedType));
            Check.NullButNotEmpty(navigationName, nameof(navigationName));

            var relatedEntityType = RelatedEntityType.FindInDefinitionPath(relatedType) ??
                                    RelatedEntityType.Builder.ModelBuilder.Entity(relatedType, ConfigurationSource.Explicit).Metadata;

            return new ReferenceNavigationBuilder(
                RelatedEntityType,
                relatedEntityType,
                navigationName,
                RelatedEntityType.Builder.Navigation(relatedEntityType.Builder, navigationName, ConfigurationSource.Explicit,
                    setTargetAsPrincipal: RelatedEntityType == relatedEntityType));
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where this entity type has a reference that points
        ///         to a single instance of the other type in the relationship.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="ReferenceNavigationBuilder.WithMany" />
        ///         or <see cref="ReferenceNavigationBuilder.WithOne" /> to fully configure
        ///         the relationship. Calling just this method without the chained call will not
        ///         produce a valid relationship.
        ///     </para>
        /// </summary>
        /// <param name="relatedTypeName"> The name of the entity type that this relationship targets. </param>
        /// <param name="navigationName">
        ///     The name of the reference navigation property on this entity type that represents the relationship. If
        ///     no property is specified, the relationship will be configured without a navigation property on this
        ///     end.
        /// </param>
        /// <returns> An object that can be used to configure the relationship. </returns>
        public virtual ReferenceNavigationBuilder HasOne(
            [NotNull] string relatedTypeName,
            [CanBeNull] string navigationName = null)
        {
            Check.NotEmpty(relatedTypeName, nameof(relatedTypeName));
            Check.NullButNotEmpty(navigationName, nameof(navigationName));

            var relatedEntityType = RelatedEntityType.FindInDefinitionPath(relatedTypeName) ??
                                    RelatedEntityType.Builder.ModelBuilder.Entity(relatedTypeName, ConfigurationSource.Explicit).Metadata;

            return new ReferenceNavigationBuilder(
                RelatedEntityType,
                relatedEntityType,
                navigationName,
                RelatedEntityType.Builder.Navigation(relatedEntityType.Builder, navigationName, ConfigurationSource.Explicit,
                    setTargetAsPrincipal: RelatedEntityType == relatedEntityType));
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where this entity type has a collection that contains
        ///         instances of the other type in the relationship.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="CollectionNavigationBuilder.WithOne" />
        ///         to fully configure the relationship. Calling just this method without the chained call will not
        ///         produce a valid relationship.
        ///     </para>
        /// </summary>
        /// <param name="relatedType"> The entity type that this relationship targets. </param>
        /// <param name="navigationName">
        ///     The name of the collection navigation property on this entity type that represents the relationship. If
        ///     no property is specified, the relationship will be configured without a navigation property on this
        ///     end.
        /// </param>
        /// <returns> An object that can be used to configure the relationship. </returns>
        public virtual CollectionNavigationBuilder HasMany(
            [NotNull] Type relatedType,
            [CanBeNull] string navigationName = null)
        {
            Check.NotNull(relatedType, nameof(relatedType));
            Check.NullButNotEmpty(navigationName, nameof(navigationName));

            var relatedEntityType = RelatedEntityType.Builder.ModelBuilder.Entity(relatedType, ConfigurationSource.Explicit).Metadata;

            InternalRelationshipBuilder relationship;
            using (var batch = DeclaringEntityType.Model.ConventionDispatcher.StartBatch())
            {
                relationship = relatedEntityType.Builder
                    .Relationship(RelatedEntityType.Builder, ConfigurationSource.Explicit)
                    .IsUnique(false, ConfigurationSource.Explicit)
                    .RelatedEntityTypes(RelatedEntityType, relatedEntityType, ConfigurationSource.Explicit)
                    .PrincipalToDependent(navigationName, ConfigurationSource.Explicit);
                relationship = batch.Run(relationship);
            }

            return new CollectionNavigationBuilder(
                RelatedEntityType,
                relatedEntityType,
                navigationName,
                relationship);
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where this entity type has a collection that contains
        ///         instances of the other type in the relationship.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="CollectionNavigationBuilder.WithOne" />
        ///         to fully configure the relationship. Calling just this method without the chained call will not
        ///         produce a valid relationship.
        ///     </para>
        /// </summary>
        /// <param name="relatedTypeName"> The name of the entity type that this relationship targets. </param>
        /// <param name="navigationName">
        ///     The name of the collection navigation property on this entity type that represents the relationship. If
        ///     no property is specified, the relationship will be configured without a navigation property on this
        ///     end.
        /// </param>
        /// <returns> An object that can be used to configure the relationship. </returns>
        public virtual CollectionNavigationBuilder HasMany(
            [NotNull] string relatedTypeName,
            [CanBeNull] string navigationName = null)
        {
            Check.NotEmpty(relatedTypeName, nameof(relatedTypeName));
            Check.NullButNotEmpty(navigationName, nameof(navigationName));

            var relatedEntityType = RelatedEntityType.Builder.ModelBuilder.Entity(relatedTypeName, ConfigurationSource.Explicit).Metadata;

            InternalRelationshipBuilder relationship;
            using (var batch = DeclaringEntityType.Model.ConventionDispatcher.StartBatch())
            {
                relationship = relatedEntityType.Builder
                    .Relationship(RelatedEntityType.Builder, ConfigurationSource.Explicit)
                    .IsUnique(false, ConfigurationSource.Explicit)
                    .RelatedEntityTypes(RelatedEntityType, relatedEntityType, ConfigurationSource.Explicit)
                    .PrincipalToDependent(navigationName, ConfigurationSource.Explicit);
                relationship = batch.Run(relationship);
            }

            return new CollectionNavigationBuilder(
                RelatedEntityType,
                relatedEntityType,
                navigationName,
                relationship);
        }

        /// <summary>
        ///     Configures the <see cref="ChangeTrackingStrategy" /> to be used for this entity type.
        ///     This strategy indicates how the context detects changes to properties for an instance of the entity type.
        /// </summary>
        /// <param name="changeTrackingStrategy"> The change tracking strategy to be used. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceOwnershipBuilder HasChangeTrackingStrategy(ChangeTrackingStrategy changeTrackingStrategy)
        {
            RelatedEntityType.Builder.Metadata.ChangeTrackingStrategy = changeTrackingStrategy;

            return this;
        }

        /// <summary>
        ///     <para>
        ///         Sets the <see cref="PropertyAccessMode" /> to use for all properties of this entity type.
        ///     </para>
        ///     <para>
        ///         By default, the backing field, if one is found by convention or has been specified, is used when
        ///         new objects are constructed, typically when entities are queried from the database.
        ///         Properties are used for all other accesses.  Calling this method witll change that behavior
        ///         for all properties of this entity type as described in the <see cref="PropertyAccessMode" /> enum.
        ///     </para>
        ///     <para>
        ///         Calling this method overrrides for all properties of this entity type any access mode that was
        ///         set on the model.
        ///     </para>
        /// </summary>
        /// <param name="propertyAccessMode"> The <see cref="PropertyAccessMode" /> to use for properties of this entity type. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceOwnershipBuilder UsePropertyAccessMode(PropertyAccessMode propertyAccessMode)
        {
            RelatedEntityType.Builder.UsePropertyAccessMode(propertyAccessMode, ConfigurationSource.Explicit);

            return this;
        }
    }
}
