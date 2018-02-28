﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    /// <summary>
    ///     The validator that enforces core rules common for all providers.
    /// </summary>
    public class ModelValidator : IModelValidator
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ModelValidator" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
        public ModelValidator([NotNull] ModelValidatorDependencies dependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        /// <summary>
        ///     Dependencies used to create a <see cref="ModelValidator" />
        /// </summary>
        protected virtual ModelValidatorDependencies Dependencies { get; }

        /// <summary>
        ///     Validates a model, throwing an exception if any errors are found.
        /// </summary>
        /// <param name="model"> The model to validate. </param>
        public virtual void Validate(IModel model)
        {
            ValidateNoShadowEntities(model);
            ValidateNonNullPrimaryKeys(model);
            ValidateNoShadowKeys(model);
            ValidateNoMutableKeys(model);
            ValidateClrInheritance(model);
            ValidateChangeTrackingStrategy(model);
            ValidateOwnership(model);
            ValidateDefiningNavigations(model);
            ValidateFieldMapping(model);
            ValidateQueryFilters(model);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateQueryFilters([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            var filterValidatingExppressionVisitor = new FilterValidatingExppressionVisitor();

            foreach (var entityType in model.GetEntityTypes())
            {
                if (entityType.QueryFilter != null)
                {
                    if (entityType.BaseType != null)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.BadFilterDerivedType(entityType.QueryFilter, entityType.DisplayName()));
                    }

                    if (!filterValidatingExppressionVisitor.IsValid(entityType))
                    {
                        throw new InvalidOperationException(
                            CoreStrings.BadFilterExpression(entityType.QueryFilter, entityType.DisplayName(), entityType.ClrType));
                    }
                }
            }
        }

        private sealed class FilterValidatingExppressionVisitor : ExpressionVisitor
        {
            private IEntityType _entityType;

            private bool _valid = true;

            public bool IsValid(IEntityType entityType)
            {
                _entityType = entityType;

                Visit(entityType.QueryFilter.Body);

                return _valid;
            }

            protected override Expression VisitMember(MemberExpression memberExpression)
            {
                if (memberExpression.Expression == _entityType.QueryFilter.Parameters[0]
                    && memberExpression.Member is PropertyInfo propertyInfo
                    && _entityType.FindNavigation(propertyInfo) != null)
                {
                    _valid = false;

                    return memberExpression;
                }

                return base.VisitMember(memberExpression);
            }

            protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.IsEFProperty()
                    && methodCallExpression.Arguments[0] == _entityType.QueryFilter.Parameters[0]
                    && (!(methodCallExpression.Arguments[1] is ConstantExpression constantExpression)
                        || !(constantExpression.Value is string propertyName)
                        || _entityType.FindNavigation(propertyName) != null))
                {
                    _valid = false;

                    return methodCallExpression;
                }

                return base.VisitMethodCall(methodCallExpression);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateNoShadowEntities([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            var firstShadowEntity = model.GetEntityTypes().FirstOrDefault(entityType => !entityType.HasClrType());
            if (firstShadowEntity != null)
            {
                throw new InvalidOperationException(
                    CoreStrings.ShadowEntity(firstShadowEntity.DisplayName()));
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateNoShadowKeys([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes().Where(t => t.ClrType != null))
            {
                foreach (var key in entityType.GetDeclaredKeys())
                {
                    if (key.Properties.Any(p => p.IsShadowProperty)
                        && key is Key concreteKey
                        && ConfigurationSource.Convention.Overrides(concreteKey.GetConfigurationSource())
                        && !key.IsPrimaryKey())
                    {
                        var referencingFk = key.GetReferencingForeignKeys().FirstOrDefault();

                        if (referencingFk != null)
                        {
                            throw new InvalidOperationException(
                                CoreStrings.ReferencedShadowKey(
                                    referencingFk.DeclaringEntityType.DisplayName() +
                                    (referencingFk.DependentToPrincipal == null
                                        ? ""
                                        : "." + referencingFk.DependentToPrincipal.Name),
                                    entityType.DisplayName() +
                                    (referencingFk.PrincipalToDependent == null
                                        ? ""
                                        : "." + referencingFk.PrincipalToDependent.Name),
                                    Property.Format(referencingFk.Properties, includeTypes: true),
                                    Property.Format(entityType.FindPrimaryKey().Properties, includeTypes: true)));
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateNoMutableKeys([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                foreach (var key in entityType.GetDeclaredKeys())
                {
                    var mutableProperty = key.Properties.FirstOrDefault(p => p.ValueGenerated.HasFlag(ValueGenerated.OnUpdate));
                    if (mutableProperty != null)
                    {
                        throw new InvalidOperationException(CoreStrings.MutableKeyProperty(mutableProperty.Name));
                    }
                }
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateNonNullPrimaryKeys([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            var entityTypeWithNullPk = model.GetEntityTypes().FirstOrDefault(et => et.FindPrimaryKey() == null);
            if (entityTypeWithNullPk != null)
            {
                throw new InvalidOperationException(
                    CoreStrings.EntityRequiresKey(entityTypeWithNullPk.DisplayName()));
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateClrInheritance([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            var validEntityTypes = new HashSet<IEntityType>();
            foreach (var entityType in model.GetEntityTypes())
            {
                ValidateClrInheritance(model, entityType, validEntityTypes);
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateClrInheritance(
            [NotNull] IModel model, [NotNull] IEntityType entityType, [NotNull] HashSet<IEntityType> validEntityTypes)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(entityType, nameof(entityType));
            Check.NotNull(validEntityTypes, nameof(validEntityTypes));

            if (validEntityTypes.Contains(entityType))
            {
                return;
            }

            var baseClrType = entityType.ClrType?.GetTypeInfo().BaseType;
            while (baseClrType != null)
            {
                var baseEntityType = model.FindEntityType(baseClrType);
                if (baseEntityType != null)
                {
                    if (!baseEntityType.IsAssignableFrom(entityType))
                    {
                        throw new InvalidOperationException(
                            CoreStrings.InconsistentInheritance(entityType.DisplayName(), baseEntityType.DisplayName()));
                    }
                    ValidateClrInheritance(model, baseEntityType, validEntityTypes);
                    break;
                }
                baseClrType = baseClrType.GetTypeInfo().BaseType;
            }

            if (entityType.ClrType?.IsInstantiable() == false
                && !entityType.GetDerivedTypes().Any())
            {
                throw new InvalidOperationException(
                    CoreStrings.AbstractLeafEntityType(entityType.DisplayName()));
            }

            validEntityTypes.Add(entityType);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateChangeTrackingStrategy([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            var detectChangesNeeded = false;
            foreach (var entityType in model.GetEntityTypes())
            {
                var changeTrackingStrategy = entityType.GetChangeTrackingStrategy();
                if (changeTrackingStrategy == ChangeTrackingStrategy.Snapshot)
                {
                    detectChangesNeeded = true;
                }

                var errorMessage = entityType.CheckChangeTrackingStrategy(changeTrackingStrategy);
                if (errorMessage != null)
                {
                    throw new InvalidOperationException(errorMessage);
                }
            }

            if (!detectChangesNeeded)
            {
                (model as IMutableModel)?.GetOrAddAnnotation(ChangeDetector.SkipDetectChangesAnnotation, "true");
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateOwnership([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                var ownerships = entityType.GetForeignKeys().Where(fk => fk.IsOwnership).ToList();
                if (ownerships.Count > 1)
                {
                    throw new InvalidOperationException(CoreStrings.MultipleOwnerships(entityType.DisplayName()));
                }
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateDefiningNavigations([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                if (entityType.DefiningEntityType != null)
                {
                    if (entityType.FindDefiningNavigation() == null
                        || (entityType.DefiningEntityType as EntityType)?.Builder == null)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.NoDefiningNavigation(
                                entityType.DefiningNavigationName, entityType.DisplayName(), entityType.DefiningEntityType.DisplayName()));
                    }

                    var ownership = entityType.GetForeignKeys().SingleOrDefault(fk => fk.IsOwnership);
                    if (ownership != null)
                    {
                        if (ownership.PrincipalToDependent?.Name != entityType.DefiningNavigationName)
                        {
                            var ownershipNavigation = ownership.PrincipalToDependent == null
                                ? ""
                                : "." + ownership.PrincipalToDependent.Name;
                            throw new InvalidOperationException(
                                CoreStrings.NonDefiningOwnership(
                                    ownership.PrincipalEntityType.DisplayName() + ownershipNavigation,
                                    entityType.DefiningNavigationName,
                                    entityType.DisplayName()));
                        }

                        foreach (var otherEntityType in model.GetEntityTypes().Where(et => et.ClrType == entityType.ClrType && et != entityType))
                        {
                            if (!otherEntityType.GetForeignKeys().Any(fk => fk.IsOwnership))
                            {
                                throw new InvalidOperationException(
                                    CoreStrings.InconsistentOwnership(entityType.DisplayName(), otherEntityType.DisplayName()));
                            }
                        }

                        foreach (var referencingFk in entityType.GetReferencingForeignKeys().Where(fk => !fk.IsOwnership))
                        {
                            throw new InvalidOperationException(
                                CoreStrings.PrincipalOwnedType(
                                    referencingFk.DeclaringEntityType.DisplayName() +
                                    (referencingFk.DependentToPrincipal == null
                                        ? ""
                                        : "." + referencingFk.DependentToPrincipal.Name),
                                    referencingFk.PrincipalEntityType.DisplayName() +
                                    (referencingFk.PrincipalToDependent == null
                                        ? ""
                                        : "." + referencingFk.PrincipalToDependent.Name),
                                    entityType.DisplayName()));
                        }
                        
                        foreach (var fk in entityType.GetDeclaredForeignKeys().Where(fk => !fk.IsOwnership && fk.PrincipalToDependent != null))
                        {
                            throw new InvalidOperationException(
                                CoreStrings.InverseToOwnedType(
                                    fk.PrincipalEntityType.DisplayName(),
                                    fk.PrincipalToDependent.Name,
                                    entityType.DisplayName()));
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected virtual void ValidateFieldMapping([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            foreach (var entityType in model.GetEntityTypes())
            {
                foreach (var propertyBase in entityType
                    .GetDeclaredProperties()
                    .Where(e => !e.IsShadowProperty)
                    .Cast<IPropertyBase>()
                    .Concat(entityType.GetDeclaredNavigations()))
                {
                    if (!propertyBase.TryGetMemberInfo(
                        forConstruction: true,
                        forSet: true,
                        memberInfo: out _,
                        errorMessage: out var errorMessage))
                    {
                        throw new InvalidOperationException(errorMessage);
                    }

                    if (!propertyBase.TryGetMemberInfo(
                        forConstruction: false,
                        forSet: true,
                        memberInfo: out _,
                        errorMessage: out errorMessage))
                    {
                        throw new InvalidOperationException(errorMessage);
                    }

                    if (!propertyBase.TryGetMemberInfo(
                        forConstruction: false,
                        forSet: false,
                        memberInfo: out _,
                        errorMessage: out errorMessage))
                    {
                        throw new InvalidOperationException(errorMessage);
                    }
                }
            }
        }
    }
}
