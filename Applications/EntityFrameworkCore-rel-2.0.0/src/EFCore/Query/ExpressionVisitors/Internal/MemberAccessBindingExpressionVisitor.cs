// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class MemberAccessBindingExpressionVisitor : RelinqExpressionVisitor
    {
        private readonly QuerySourceMapping _querySourceMapping;
        private readonly EntityQueryModelVisitor _queryModelVisitor;
        private readonly bool _inProjection;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public MemberAccessBindingExpressionVisitor(
            [NotNull] QuerySourceMapping querySourceMapping,
            [NotNull] EntityQueryModelVisitor queryModelVisitor,
            bool inProjection)
        {
            _querySourceMapping = querySourceMapping;
            _queryModelVisitor = queryModelVisitor;
            _inProjection = inProjection;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitNew(NewExpression expression)
        {
            var newArguments = Visit(expression.Arguments).ToList();

            for (var i = 0; i < newArguments.Count; i++)
            {
                if (newArguments[i].Type == typeof(ValueBuffer))
                {
                    newArguments[i]
                        = _queryModelVisitor
                            .BindReadValueMethod(expression.Arguments[i].Type, newArguments[i], 0);
                }
            }

            return expression.Update(newArguments);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var isLeftNullConstant = node.Left.IsNullConstantExpression();
            var isRightNullConstant = node.Right.IsNullConstantExpression();

            if (isLeftNullConstant || isRightNullConstant)
            {
                var nonNullExpression = isLeftNullConstant ? node.Right : node.Left;

                var methodCallExpression = nonNullExpression as MethodCallExpression;
                if (methodCallExpression != null)
                {
                    if (methodCallExpression.Method.IsEFPropertyMethod())
                    {
                        var firstArgument = methodCallExpression.Arguments[0];
                        var visitedArgument = Visit(firstArgument);
                        if (visitedArgument.Type == typeof(ValueBuffer))
                        {
                            var nullCheck = ValueBufferNullComparisonCheck(visitedArgument);
                            var propertyAccessExpression = Visit(nonNullExpression);

                            return Expression.MakeBinary(
                                node.NodeType,
                                Expression.Condition(
                                    nullCheck,
                                    propertyAccessExpression,
                                    Expression.Constant(null, propertyAccessExpression.Type)),
                                Expression.Constant(null));
                        }
                    }
                }
            }

            var newLeft = Visit(node.Left);
            var newRight = Visit(node.Right);

            if (newLeft.Type == typeof(ValueBuffer))
            {
                newLeft = _queryModelVisitor.BindReadValueMethod(node.Left.Type, newLeft, 0);
            }

            if (newRight.Type == typeof(ValueBuffer))
            {
                newRight = _queryModelVisitor.BindReadValueMethod(node.Right.Type, newRight, 0);
            }

            var newConversion = VisitAndConvert(node.Conversion, "VisitBinary");

            return node.Update(newLeft, newConversion, newRight);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            var newOperand = Visit(node.Operand);
            if (node.NodeType == ExpressionType.Convert
                && newOperand?.Type == typeof(ValueBuffer))
            {
                return newOperand;
            }

            return node.Update(newOperand);
        }

        private Expression ValueBufferNullComparisonCheck(Expression valueBufferExpression) => Expression.Not(
            Expression.MakeMemberAccess(
                valueBufferExpression,
                typeof(ValueBuffer).GetRuntimeProperty(nameof(ValueBuffer.IsEmpty))));

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            var newExpression
                = _querySourceMapping.ContainsMapping(expression.ReferencedQuerySource)
                    ? _querySourceMapping.GetExpression(expression.ReferencedQuerySource)
                    : expression;

            if (_inProjection
                && newExpression.Type.IsConstructedGenericType)
            {
                var genericTypeDefinition = newExpression.Type.GetGenericTypeDefinition();

                if (genericTypeDefinition == typeof(IOrderedAsyncEnumerable<>))
                {
                    newExpression
                        = Expression.Call(
                            _queryModelVisitor.LinqOperatorProvider.ToOrdered
                                .MakeGenericMethod(newExpression.Type.GenericTypeArguments[0]),
                            newExpression);
                }
                else if (genericTypeDefinition == typeof(IAsyncEnumerable<>))
                {
                    newExpression
                        = Expression.Call(
                            _queryModelVisitor.LinqOperatorProvider.ToEnumerable
                                .MakeGenericMethod(newExpression.Type.GenericTypeArguments[0]),
                            newExpression);
                }
            }

            return newExpression;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            expression.QueryModel.TransformExpressions(Visit);

            return expression;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitExtension(Expression node)
        {
            if (node is NullConditionalExpression nullConditionalExpression)
            {
                var newCaller = Visit(nullConditionalExpression.Caller);

                if (newCaller != nullConditionalExpression.Caller
                    && newCaller.Type == typeof(ValueBuffer))
                {
                    var newAccessOperation = Visit(nullConditionalExpression.AccessOperation);
                    if (newAccessOperation != nullConditionalExpression.AccessOperation)
                    {
                        var test = ValueBufferNullComparisonCheck(newCaller);
                        var newAccessOperationWithoutConvert = newAccessOperation.RemoveConvert();
                        if (!newAccessOperationWithoutConvert.Type.IsNullableType())
                        {
                            // since we are in the NullConditionalExpression, member we are trying to bind to could be coming from Left Join
                            // and therefore its value could be null, even though the property type itself is not nullable
                            // we need to compensate for this with additional check
                            var nullableAccessOperation = TryCreateNullableAccessOperation(newAccessOperationWithoutConvert);
                            if (nullableAccessOperation != null)
                            {
                                test = Expression.AndAlso(
                                    test,
                                    Expression.NotEqual(
                                        nullableAccessOperation,
                                        Expression.Constant(null, nullableAccessOperation.Type)));
                            }
                        }

                        return Expression.Condition(
                            test: test,
                            ifTrue: newAccessOperation.Type != nullConditionalExpression.Type
                                ? Expression.Convert(newAccessOperation, nullConditionalExpression.Type)
                                : newAccessOperation,
                            ifFalse: Expression.Default(nullConditionalExpression.Type));
                    }
                }
            }

            return base.VisitExtension(node);
        }

        private static Expression TryCreateNullableAccessOperation(Expression accessOperation)
        {
            if (accessOperation is MethodCallExpression methodCallExpression
                && methodCallExpression.Method.MethodIsClosedFormOf(EntityMaterializerSource.TryReadValueMethod))
            {
                var tryReadValueMethodInfo = EntityMaterializerSource.TryReadValueMethod.MakeGenericMethod(accessOperation.Type.MakeNullable());

                return Expression.Call(
                    tryReadValueMethodInfo,
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1],
                    methodCallExpression.Arguments[2]);
            }

            return null;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitMember(MemberExpression node)
        {
            var newExpression = Visit(node.Expression);

            if (newExpression != node.Expression)
            {
                if (newExpression.Type == typeof(ValueBuffer))
                {
                    return _queryModelVisitor
                               .BindMemberToValueBuffer(node, newExpression)
                           ?? node;
                }

                var member = node.Member;
                var typeInfo = newExpression.Type.GetTypeInfo();

                if (typeInfo.IsGenericType
                    && (typeInfo.GetGenericTypeDefinition() == typeof(IGrouping<,>)
                        || typeInfo.GetGenericTypeDefinition() == typeof(IAsyncGrouping<,>)))
                {
                    member = typeInfo.GetDeclaredProperty("Key");
                }

                return Expression.MakeMemberAccess(newExpression, member);
            }

            return node;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            MethodCallExpression newExpression = null;
            Expression firstArgument = null;

            if (methodCallExpression.Method.IsEFPropertyMethod())
            {
                var newArguments
                    = VisitAndConvert(
                        new List<Expression>
                        {
                            methodCallExpression.Arguments[0],
                            methodCallExpression.Arguments[1]
                        }.AsReadOnly(),
                        "VisitMethodCall");

                if (newArguments[0].Type == typeof(ValueBuffer))
                {
                    firstArgument = newArguments[0];

                    // Compensate for ValueBuffer being a struct, and hence not compatible with Object method
                    newExpression
                        = Expression.Call(
                            methodCallExpression.Method,
                            Expression.Convert(newArguments[0], typeof(object)),
                            newArguments[1]);
                }
            }

            if (newExpression == null)
            {
                newExpression
                    = (MethodCallExpression)base.VisitMethodCall(methodCallExpression);
            }

            firstArgument = firstArgument ?? newExpression.Arguments.FirstOrDefault();

            if (newExpression != methodCallExpression
                && firstArgument?.Type == typeof(ValueBuffer))
            {
                return
                    _queryModelVisitor
                        .BindMethodCallToValueBuffer(methodCallExpression, firstArgument)
                    ?? newExpression;
            }

            return _queryModelVisitor
                       .BindMethodCallExpression<Expression>(
                           methodCallExpression,
                           (property, _) =>
                               {
                                   var propertyType = newExpression.Method.GetGenericArguments()[0];

                                   if (newExpression.Arguments[0] is ConstantExpression maybeConstantExpression)
                                   {
                                       return Expression.Constant(
                                           property.GetGetter().GetClrValue(maybeConstantExpression.Value),
                                           propertyType);
                                   }

                                   if (newExpression.Arguments[0] is MethodCallExpression maybeMethodCallExpression
                                       && maybeMethodCallExpression.Method.IsGenericMethod
                                       && maybeMethodCallExpression.Method.GetGenericMethodDefinition()
                                           .Equals(DefaultQueryExpressionVisitor.GetParameterValueMethodInfo)
                                       || newExpression.Arguments[0].NodeType == ExpressionType.Parameter
                                       && !property.IsShadowProperty)
                                   {
                                       // The target is a parameter, try and get the value from it directly.
                                       return Expression.Call(
                                           _getValueFromEntityMethodInfo
                                               .MakeGenericMethod(propertyType),
                                           Expression.Constant(property.GetGetter()),
                                           newExpression.Arguments[0]);
                                   }

                                   return Expression.Call(
                                       _getValueMethodInfo.MakeGenericMethod(propertyType),
                                       EntityQueryModelVisitor.QueryContextParameter,
                                       newExpression.Arguments[0],
                                       Expression.Constant(property));
                               })
                   ?? newExpression;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static List<IPropertyBase> GetPropertyPath(
            [NotNull] Expression expression,
            [NotNull] QueryCompilationContext queryCompilationContext,
            out QuerySourceReferenceExpression querySourceReferenceExpression)
        {
            var memberExpression = expression as MemberExpression;
            var methodCallExpression = expression as MethodCallExpression;

            var innerExpression
                = memberExpression?.Expression
                    ?? ((methodCallExpression?.Method).IsEFPropertyMethod()
                        ? methodCallExpression?.Arguments[0]
                        : null);

            if (innerExpression == null)
            {
                querySourceReferenceExpression = expression as QuerySourceReferenceExpression;
                return new List<IPropertyBase>();
            }

            Debug.Assert(memberExpression?.Member.Name != null || methodCallExpression != null);
            var propertyName = memberExpression?.Member.Name
                               ?? (string)(methodCallExpression.Arguments[1] as ConstantExpression)?.Value;

            // in case of inheritance there might be convert to derived type here, so we want to check it first
            var entityType = queryCompilationContext.Model.FindEntityType(innerExpression.Type);

            innerExpression = innerExpression.RemoveConvert();

            if (entityType == null)
            {
                entityType = queryCompilationContext.Model.FindEntityType(innerExpression.Type);
            }

            var innerProperties = GetPropertyPath(innerExpression, queryCompilationContext, out var innerQsre);

            if (entityType == null)
            {
                if (innerProperties.Count > 0)
                {
                    entityType = (innerProperties[innerProperties.Count - 1] as INavigation)?.GetTargetType();
                }
                else if (innerQsre != null)
                {
                    entityType = queryCompilationContext.FindEntityType(innerQsre.ReferencedQuerySource);
                }

                if (entityType == null)
                {
                    querySourceReferenceExpression = null;
                    innerProperties.Clear();
                    return innerProperties;
                }
            }

            var property = propertyName == null
                ? null
                : (IPropertyBase)entityType.FindProperty(propertyName)
                  ?? entityType.FindNavigation(propertyName);

            if (property == null)
            {
                if ((methodCallExpression?.Method).IsEFPropertyMethod())
                {
                    throw new InvalidOperationException(
                        CoreStrings.PropertyNotFound(propertyName, entityType.DisplayName()));
                }

                querySourceReferenceExpression = null;
                innerProperties.Clear();
                return innerProperties;
            }

            innerProperties.Add(property);
            querySourceReferenceExpression = innerQsre;

            return innerProperties;
        }

        private static readonly MethodInfo _getValueMethodInfo
            = typeof(MemberAccessBindingExpressionVisitor)
                .GetTypeInfo().GetDeclaredMethod(nameof(GetValue));

        [UsedImplicitly]
        private static T GetValue<T>(QueryContext queryContext, object entity, IProperty property)
        {
            if (entity == null)
            {
                return default(T);
            }

            return (T)queryContext.QueryBuffer.GetPropertyValue(entity, property);
        }

        private static readonly MethodInfo _getValueFromEntityMethodInfo
            = typeof(MemberAccessBindingExpressionVisitor)
                .GetTypeInfo().GetDeclaredMethod(nameof(GetValueFromEntity));

        [UsedImplicitly]
        private static T GetValueFromEntity<T>(IClrPropertyGetter clrPropertyGetter, object entity)
        {
            if (entity == null)
            {
                return default(T);
            }

            return (T)clrPropertyGetter.GetClrValue(entity);
        }
    }
}
