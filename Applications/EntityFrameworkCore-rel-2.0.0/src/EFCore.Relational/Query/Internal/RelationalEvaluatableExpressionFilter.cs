﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class RelationalEvaluatableExpressionFilter : EvaluatableExpressionFilter
    {
        private readonly IModel _model;

        public RelationalEvaluatableExpressionFilter([NotNull] IModel model)
        {
            Check.NotNull(model, nameof(model));

            _model = model;
        }

        public override bool IsEvaluatableMethodCall(MethodCallExpression methodCallExpression) 
            => _model.Relational().FindDbFunction(methodCallExpression.Method) == null 
                    && base.IsEvaluatableMethodCall(methodCallExpression);
    }
}
