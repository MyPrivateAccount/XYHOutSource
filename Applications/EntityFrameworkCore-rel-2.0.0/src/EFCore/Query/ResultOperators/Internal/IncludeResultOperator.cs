// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;

namespace Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class IncludeResultOperator : SequenceTypePreservingResultOperatorBase, IQueryAnnotation
    {
        private List<string> _navigationPropertyPaths;
        private INavigation[] _navigationPath;
        private IQuerySource _querySource;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IncludeResultOperator(
            [NotNull] INavigation[] navigationPath, [NotNull] Expression pathFromQuerySource)
            : this(navigationPath.Select(n => n.Name), pathFromQuerySource)
        {
            _navigationPath = navigationPath;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IncludeResultOperator(
            [NotNull] IEnumerable<string> navigationPropertyPaths, [NotNull] Expression pathFromQuerySource)
        {
            _navigationPropertyPaths = new List<string>(navigationPropertyPaths);
            PathFromQuerySource = pathFromQuerySource;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IQuerySource QuerySource
        {
            get => _querySource ?? (_querySource = GetQuerySource(PathFromQuerySource));
            set => _querySource = value;
        }

        private static IQuerySource GetQuerySource(Expression expression)
            => expression.TryGetReferencedQuerySource()
               ?? (expression is MemberExpression memberExpression
                   ? GetQuerySource(memberExpression.Expression.RemoveConvert())
                   : null);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual QueryModel QueryModel { get; set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<string> NavigationPropertyPaths => _navigationPropertyPaths;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression PathFromQuerySource { get; [param: NotNull] set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void AppendToNavigationPath([NotNull] IReadOnlyList<PropertyInfo> propertyInfos)
        {
            if (_navigationPropertyPaths == null)
            {
                _navigationPropertyPaths = new List<string>();
            }

            _navigationPropertyPaths.AddRange(propertyInfos.Select(pi => pi.Name));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual INavigation[] GetNavigationPath([NotNull] QueryCompilationContext queryCompilationContext)
        {
            if (_navigationPath == null)
            {
                IEntityType entityType = null;
                if (PathFromQuerySource is QuerySourceReferenceExpression qsre)
                {
                    entityType = queryCompilationContext.FindEntityType(qsre.ReferencedQuerySource);
                }
                if (entityType == null)
                {
                    entityType = queryCompilationContext.Model.FindEntityType(PathFromQuerySource.Type);

                    if (entityType == null)
                    {
                        var pathFromSource = MemberAccessBindingExpressionVisitor.GetPropertyPath(
                            PathFromQuerySource, queryCompilationContext, out qsre);
                        if (pathFromSource.Count > 0
                            && pathFromSource[pathFromSource.Count - 1] is INavigation navigation)
                        {
                            entityType = navigation.GetTargetType();
                        }
                    }
                }

                if (entityType == null)
                {
                    throw new NotSupportedException(
                        CoreStrings.IncludeNotSpecifiedDirectlyOnEntityType(
                            ToString(),
                            NavigationPropertyPaths.FirstOrDefault()));
                }

                _navigationPath = new INavigation[NavigationPropertyPaths.Count];

                for (var i = 0; i < NavigationPropertyPaths.Count; i++)
                {
                    _navigationPath[i] = entityType.FindNavigation(NavigationPropertyPaths[i]);

                    if (_navigationPath[i] == null)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.IncludeBadNavigation(NavigationPropertyPaths[i], entityType.DisplayName()));
                    }

                    entityType = _navigationPath[i].GetTargetType();
                }
            }

            return _navigationPath;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override string ToString()
            => $@"Include(""{NavigationPropertyPaths.Join(".")}"")";

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string DisplayString()
            => $"{PathFromQuerySource}.{_navigationPropertyPaths.Join(".")}";

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ResultOperatorBase Clone(CloneContext cloneContext)
            => new IncludeResultOperator(NavigationPropertyPaths, PathFromQuerySource);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void TransformExpressions([NotNull] Func<Expression, Expression> transformation)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override StreamedSequence ExecuteInMemory<T>([NotNull] StreamedSequence input) => input;
    }
}
