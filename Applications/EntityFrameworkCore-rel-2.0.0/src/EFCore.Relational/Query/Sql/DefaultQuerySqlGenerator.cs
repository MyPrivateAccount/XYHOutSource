// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing;

// ReSharper disable SwitchStatementMissingSomeCases
// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.Sql
{
    /// <summary>
    ///     The default query SQL generator.
    /// </summary>
    public class DefaultQuerySqlGenerator : ThrowingExpressionVisitor, ISqlExpressionVisitor, IQuerySqlGenerator
    {
        private IRelationalCommandBuilder _relationalCommandBuilder;
        private IReadOnlyDictionary<string, object> _parametersValues;
        private ParameterNameGenerator _parameterNameGenerator;
        private RelationalTypeMapping _typeMapping;
        private NullComparisonTransformingVisitor _nullComparisonTransformingVisitor;
        private RelationalNullsExpandingVisitor _relationalNullsExpandingVisitor;
        private PredicateReductionExpressionOptimizer _predicateReductionExpressionOptimizer;
        private PredicateNegationExpressionOptimizer _predicateNegationExpressionOptimizer;
        private ReducingExpressionVisitor _reducingExpressionVisitor;
        private BooleanExpressionTranslatingVisitor _booleanExpressionTranslatingVisitor;

        private static readonly Dictionary<ExpressionType, string> _operatorMap = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Equal, " = " },
            { ExpressionType.NotEqual, " <> " },
            { ExpressionType.GreaterThan, " > " },
            { ExpressionType.GreaterThanOrEqual, " >= " },
            { ExpressionType.LessThan, " < " },
            { ExpressionType.LessThanOrEqual, " <= " },
            { ExpressionType.AndAlso, " AND " },
            { ExpressionType.OrElse, " OR " },
            { ExpressionType.Add, " + " },
            { ExpressionType.Subtract, " - " },
            { ExpressionType.Multiply, " * " },
            { ExpressionType.Divide, " / " },
            { ExpressionType.Modulo, " % " },
            { ExpressionType.And, " & " },
            { ExpressionType.Or, " | " }
        };

        /// <summary>
        ///     Creates a new instance of <see cref="DefaultQuerySqlGenerator" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
        /// <param name="selectExpression"> The select expression. </param>
        protected DefaultQuerySqlGenerator(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            [NotNull] SelectExpression selectExpression)

        {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(selectExpression, nameof(selectExpression));

            Dependencies = dependencies;
            SelectExpression = selectExpression;
        }

        /// <summary>
        ///     Parameter object containing service dependencies.
        /// </summary>
        protected virtual QuerySqlGeneratorDependencies Dependencies { get; }

        /// <summary>
        ///     Gets a value indicating whether this SQL query is cacheable.
        /// </summary>
        /// <value>
        ///     true if this SQL query is cacheable, false if not.
        /// </value>
        public virtual bool IsCacheable { get; private set; }

        /// <summary>
        ///     Gets the select expression.
        /// </summary>
        /// <value>
        ///     The select expression.
        /// </value>
        protected virtual SelectExpression SelectExpression { get; }

        /// <summary>
        ///     Gets the SQL generation helper.
        /// </summary>
        /// <value>
        ///     The SQL generation helper.
        /// </value>
        protected virtual ISqlGenerationHelper SqlGenerator => Dependencies.SqlGenerationHelper;

        /// <summary>
        ///     Gets the parameter values.
        /// </summary>
        /// <value>
        ///     The parameter values.
        /// </value>
        protected virtual IReadOnlyDictionary<string, object> ParameterValues => _parametersValues;

        /// <summary>
        ///     Generates SQL for the given parameter values.
        /// </summary>
        /// <param name="parameterValues"> The parameter values. </param>
        /// <returns>
        ///     A relational command.
        /// </returns>
        public virtual IRelationalCommand GenerateSql(IReadOnlyDictionary<string, object> parameterValues)
        {
            Check.NotNull(parameterValues, nameof(parameterValues));

            _relationalCommandBuilder = Dependencies.CommandBuilderFactory.Create();
            _parameterNameGenerator = Dependencies.ParameterNameGeneratorFactory.Create();

            _parametersValues = parameterValues;
            _nullComparisonTransformingVisitor = new NullComparisonTransformingVisitor(parameterValues);
            IsCacheable = true;

            Visit(SelectExpression);

            return _relationalCommandBuilder.Build();
        }

        /// <summary>
        ///     Creates a relational value buffer factory.
        /// </summary>
        /// <param name="relationalValueBufferFactoryFactory"> The relational value buffer factory. </param>
        /// <param name="dataReader"> The data reader. </param>
        /// <returns>
        ///     The new value buffer factory.
        /// </returns>
        public virtual IRelationalValueBufferFactory CreateValueBufferFactory(
            IRelationalValueBufferFactoryFactory relationalValueBufferFactoryFactory, DbDataReader dataReader)
        {
            Check.NotNull(relationalValueBufferFactoryFactory, nameof(relationalValueBufferFactoryFactory));

            return relationalValueBufferFactoryFactory
                .Create(SelectExpression.GetProjectionTypes().ToArray(), indexMap: null);
        }

        /// <summary>
        ///     The generated SQL.
        /// </summary>
        protected virtual IRelationalCommandBuilder Sql => _relationalCommandBuilder;

        /// <summary>
        ///     The default true literal SQL.
        /// </summary>
        protected virtual string TypedTrueLiteral => "CAST(1 AS BIT)";

        /// <summary>
        ///     The default false literal SQL.
        /// </summary>
        protected virtual string TypedFalseLiteral => "CAST(0 AS BIT)";

        /// <summary>
        ///     Visit a top-level SelectExpression.
        /// </summary>
        /// <param name="selectExpression"> The select expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitSelect(SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            IDisposable subQueryIndent = null;

            if (selectExpression.Alias != null)
            {
                _relationalCommandBuilder.AppendLine("(");

                subQueryIndent = _relationalCommandBuilder.Indent();
            }

            _relationalCommandBuilder.Append("SELECT ");

            if (selectExpression.IsDistinct)
            {
                _relationalCommandBuilder.Append("DISTINCT ");
            }

            GenerateTop(selectExpression);

            var projectionAdded = false;

            if (selectExpression.IsProjectStar)
            {
                var tableAlias = selectExpression.ProjectStarTable.Alias;

                _relationalCommandBuilder
                    .Append(SqlGenerator.DelimitIdentifier(tableAlias))
                    .Append(".*");

                projectionAdded = true;
            }

            if (selectExpression.Projection.Any())
            {
                if (selectExpression.IsProjectStar)
                {
                    _relationalCommandBuilder.Append(", ");
                }

                ProcessExpressionList(selectExpression.Projection, GenerateProjection);

                projectionAdded = true;
            }

            if (!projectionAdded)
            {
                _relationalCommandBuilder.Append("1");
            }

            if (selectExpression.Tables.Any())
            {
                _relationalCommandBuilder.AppendLine()
                    .Append("FROM ");

                ProcessExpressionList(selectExpression.Tables, sql => sql.AppendLine());
            }

            if (selectExpression.Predicate != null)
            {
                GeneratePredicate(selectExpression.Predicate);
            }

            if (selectExpression.OrderBy.Any())
            {
                _relationalCommandBuilder.AppendLine();

                GenerateOrderBy(selectExpression.OrderBy);
            }

            GenerateLimitOffset(selectExpression);

            if (subQueryIndent != null)
            {
                subQueryIndent.Dispose();

                _relationalCommandBuilder.AppendLine()
                    .Append(")");

                if (selectExpression.Alias.Length > 0)
                {
                    _relationalCommandBuilder.Append(" AS ")
                        .Append(SqlGenerator.DelimitIdentifier(selectExpression.Alias));
                }
            }

            return selectExpression;
        }

        private Expression ApplyOptimizations(Expression expression, bool searchCondition, bool joinCondition = false)
        {
            var newExpression = _nullComparisonTransformingVisitor.Visit(expression);

            if (_relationalNullsExpandingVisitor == null)
            {
                _relationalNullsExpandingVisitor = new RelationalNullsExpandingVisitor();
            }

            if (_predicateReductionExpressionOptimizer == null)
            {
                _predicateReductionExpressionOptimizer = new PredicateReductionExpressionOptimizer();
            }

            if (_predicateNegationExpressionOptimizer == null)
            {
                _predicateNegationExpressionOptimizer = new PredicateNegationExpressionOptimizer();
            }

            if (_reducingExpressionVisitor == null)
            {
                _reducingExpressionVisitor = new ReducingExpressionVisitor();
            }

            if (_booleanExpressionTranslatingVisitor == null)
            {
                _booleanExpressionTranslatingVisitor = new BooleanExpressionTranslatingVisitor();
            }

            if (joinCondition
                && newExpression is BinaryExpression binaryExpression
                && binaryExpression.NodeType == ExpressionType.Equal)
            {
                newExpression = Expression.MakeBinary(
                    binaryExpression.NodeType,
                    ApplyNullSemantics(binaryExpression.Left),
                    ApplyNullSemantics(binaryExpression.Right));
            }
            else
            {
                newExpression = ApplyNullSemantics(newExpression);
            }

            newExpression = _predicateReductionExpressionOptimizer.Visit(newExpression);
            newExpression = _predicateNegationExpressionOptimizer.Visit(newExpression);
            newExpression = _reducingExpressionVisitor.Visit(newExpression);
            newExpression = _booleanExpressionTranslatingVisitor.Translate(newExpression, searchCondition);

            return newExpression;
        }

        private Expression ApplyNullSemantics(Expression expression)
        {
            var relationalNullsOptimizedExpandingVisitor = new RelationalNullsOptimizedExpandingVisitor();
            var optimizedRightExpression = relationalNullsOptimizedExpandingVisitor.Visit(expression);

            return relationalNullsOptimizedExpandingVisitor.IsOptimalExpansion
                ? optimizedRightExpression
                : _relationalNullsExpandingVisitor.Visit(expression);
        }

        /// <summary>
        ///     Visit a single projection in SQL SELECT clause
        /// </summary>
        /// <param name="projection"> The projection expression. </param>
        protected virtual void GenerateProjection([NotNull] Expression projection) => Visit(ApplyOptimizations(projection, searchCondition: false));

        /// <summary>
        ///     Visit the predicate in SQL WHERE clause
        /// </summary>
        /// <param name="predicate"> The predicate expression. </param>
        protected virtual void GeneratePredicate([NotNull] Expression predicate)
        {
            var optimizedPredicate = ApplyOptimizations(predicate, searchCondition: true);

            if (optimizedPredicate is BinaryExpression binaryExpression)
            {
                var leftBooleanConstant = GetBooleanConstantValue(binaryExpression.Left);
                var rightBooleanConstant = GetBooleanConstantValue(binaryExpression.Right);

                if (binaryExpression.NodeType == ExpressionType.Equal
                    && leftBooleanConstant == true
                    && rightBooleanConstant == true
                    || binaryExpression.NodeType == ExpressionType.NotEqual
                    && leftBooleanConstant == false
                    && rightBooleanConstant == false)
                {
                    return;
                }
            }

            _relationalCommandBuilder.AppendLine()
                .Append("WHERE ");

            Visit(optimizedPredicate);
        }

        private static bool? GetBooleanConstantValue(Expression expression)
            => expression is ConstantExpression constantExpression
               && constantExpression.Type.UnwrapNullableType() == typeof(bool)
                ? (bool?)constantExpression.Value
                : null;

        /// <summary>
        ///     Generates the ORDER BY SQL.
        /// </summary>
        /// <param name="orderings"> The orderings. </param>
        protected virtual void GenerateOrderBy([NotNull] IReadOnlyList<Ordering> orderings)
        {
            _relationalCommandBuilder.Append("ORDER BY ");

            ProcessExpressionList(orderings, GenerateOrdering);
        }

        /// <summary>
        ///     Generates a single ordering in an SQL ORDER BY clause.
        /// </summary>
        /// <param name="ordering"> The ordering. </param>
        protected virtual void GenerateOrdering([NotNull] Ordering ordering)
        {
            Check.NotNull(ordering, nameof(ordering));

            var orderingExpression = ordering.Expression;

            if (orderingExpression is AliasExpression aliasExpression)
            {
                _relationalCommandBuilder.Append(SqlGenerator.DelimitIdentifier(aliasExpression.Alias));
            }
            else if (orderingExpression is ConstantExpression
                     || orderingExpression is ParameterExpression)
            {
                _relationalCommandBuilder.Append("(SELECT 1)");
            }
            else
            {
                Visit(ApplyOptimizations(orderingExpression, searchCondition: false));
            }

            if (ordering.OrderingDirection == OrderingDirection.Desc)
            {
                _relationalCommandBuilder.Append(" DESC");
            }
        }

        private void ProcessExpressionList(
            IReadOnlyList<Expression> expressions, Action<IRelationalCommandBuilder> joinAction = null)
            => ProcessExpressionList(expressions, e => Visit(e), joinAction);

        private void ProcessExpressionList<T>(
            IReadOnlyList<T> items, Action<T> itemAction, Action<IRelationalCommandBuilder> joinAction = null)
        {
            joinAction = joinAction ?? (isb => isb.Append(", "));

            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    joinAction(_relationalCommandBuilder);
                }

                itemAction(items[i]);
            }
        }

        /// <summary>
        ///     Visit a FromSqlExpression.
        /// </summary>
        /// <param name="fromSqlExpression"> The FromSql expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitFromSql(FromSqlExpression fromSqlExpression)
        {
            Check.NotNull(fromSqlExpression, nameof(fromSqlExpression));

            _relationalCommandBuilder.AppendLine("(");

            using (_relationalCommandBuilder.Indent())
            {
                GenerateFromSql(fromSqlExpression.Sql, fromSqlExpression.Arguments, _parametersValues);
            }

            _relationalCommandBuilder.Append(") AS ")
                .Append(SqlGenerator.DelimitIdentifier(fromSqlExpression.Alias));

            return fromSqlExpression;
        }

        /// <summary>
        ///     Generate SQL corresponding to a FromSql query.
        /// </summary>
        /// <param name="sql"> The FromSql SQL query. </param>
        /// <param name="arguments"> The arguments. </param>
        /// <param name="parameters"> The parameters for this query. </param>
        protected virtual void GenerateFromSql(
            [NotNull] string sql,
            [NotNull] Expression arguments,
            [NotNull] IReadOnlyDictionary<string, object> parameters)
        {
            Check.NotEmpty(sql, nameof(sql));
            Check.NotNull(arguments, nameof(arguments));
            Check.NotNull(parameters, nameof(parameters));

            string[] substitutions = null;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (arguments.NodeType)
            {
                case ExpressionType.Parameter:
                {
                    var parameterExpression = (ParameterExpression)arguments;

                    if (parameters.TryGetValue(parameterExpression.Name, out var parameterValue))
                    {
                        var argumentValues = (object[])parameterValue;

                        substitutions = new string[argumentValues.Length];

                        _relationalCommandBuilder.AddCompositeParameter(
                            parameterExpression.Name,
                            builder =>
                                {
                                    for (var i = 0; i < argumentValues.Length; i++)
                                    {
                                        var parameterName = _parameterNameGenerator.GenerateNext();

                                        substitutions[i] = SqlGenerator.GenerateParameterName(parameterName);

                                        builder.AddParameter(
                                            parameterName,
                                            substitutions[i]);
                                    }
                                });
                    }

                    break;
                }
                case ExpressionType.Constant:
                {
                    var constantExpression = (ConstantExpression)arguments;
                    var argumentValues = (object[])constantExpression.Value;

                    substitutions = new string[argumentValues.Length];

                    for (var i = 0; i < argumentValues.Length; i++)
                    {
                        var value = argumentValues[i];

                        if (value is DbParameter dbParameter)
                        {
                            if (string.IsNullOrEmpty(dbParameter.ParameterName))
                            {
                                dbParameter.ParameterName = SqlGenerator.GenerateParameterName(_parameterNameGenerator.GenerateNext());
                            }

                            substitutions[i] = dbParameter.ParameterName;

                            _relationalCommandBuilder.AddRawParameter(
                                dbParameter.ParameterName,
                                dbParameter);
                        }
                        else
                        {
                            substitutions[i] = GetTypeMapping(value).GenerateSqlLiteral(value);
                        }
                    }

                    break;
                }
                case ExpressionType.NewArrayInit:
                {
                    var newArrayExpression = (NewArrayExpression)arguments;

                    substitutions = new string[newArrayExpression.Expressions.Count];

                    for (var i = 0; i < newArrayExpression.Expressions.Count; i++)
                    {
                        var expression = newArrayExpression.Expressions[i].RemoveConvert();

                        // ReSharper disable once SwitchStatementMissingSomeCases
                        switch (expression.NodeType)
                        {
                            case ExpressionType.Constant:
                            {
                                var value = ((ConstantExpression)expression).Value;
                                substitutions[i]
                                    = GetTypeMapping(value).GenerateSqlLiteral(value);

                                break;
                            }
                            case ExpressionType.Parameter:
                            {
                                var parameter = (ParameterExpression)expression;

                                if (_parametersValues.ContainsKey(parameter.Name))
                                {
                                    substitutions[i] = SqlGenerator.GenerateParameterName(parameter.Name);

                                    _relationalCommandBuilder.AddParameter(
                                        parameter.Name,
                                        substitutions[i]);
                                }

                                break;
                            }
                        }
                    }

                    break;
                }
            }

            if (substitutions != null)
            {
                // ReSharper disable once CoVariantArrayConversion
                // InvariantCulture not needed since substitutions are all strings
                sql = string.Format(sql, substitutions);
            }

            _relationalCommandBuilder.AppendLines(sql);
        }

        private RelationalTypeMapping GetTypeMapping(object value)
        {
            return _typeMapping != null
                   && (value == null
                       || _typeMapping.ClrType.IsInstanceOfType(value))
                ? _typeMapping
                : Dependencies.RelationalTypeMapper.GetMappingForValue(value);
        }

        /// <summary>
        ///     Visit a TableExpression.
        /// </summary>
        /// <param name="tableExpression"> The table expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitTable(TableExpression tableExpression)
        {
            Check.NotNull(tableExpression, nameof(tableExpression));

            if (tableExpression.Schema != null)
            {
                _relationalCommandBuilder.Append(SqlGenerator.DelimitIdentifier(tableExpression.Schema))
                    .Append(".");
            }

            _relationalCommandBuilder.Append(SqlGenerator.DelimitIdentifier(tableExpression.Table))
                .Append(" AS ")
                .Append(SqlGenerator.DelimitIdentifier(tableExpression.Alias));

            return tableExpression;
        }

        /// <summary>
        ///     Visit a CrossJoin expression.
        /// </summary>
        /// <param name="crossJoinExpression"> The cross join expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitCrossJoin(CrossJoinExpression crossJoinExpression)
        {
            Check.NotNull(crossJoinExpression, nameof(crossJoinExpression));

            _relationalCommandBuilder.Append("CROSS JOIN ");

            Visit(crossJoinExpression.TableExpression);

            return crossJoinExpression;
        }

        /// <summary>
        ///     Visit a CrossJoinLateralExpression expression.
        /// </summary>
        /// <param name="crossJoinLateralExpression"> The cross join lateral expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitCrossJoinLateral(CrossJoinLateralExpression crossJoinLateralExpression)
        {
            Check.NotNull(crossJoinLateralExpression, nameof(crossJoinLateralExpression));

            _relationalCommandBuilder.Append("CROSS JOIN LATERAL ");

            Visit(crossJoinLateralExpression.TableExpression);

            return crossJoinLateralExpression;
        }

        /// <summary>
        ///     Visit a SqlFragmentExpression.
        /// </summary>
        /// <param name="sqlFragmentExpression"> The SqlFragmentExpression expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitSqlFragment(SqlFragmentExpression sqlFragmentExpression)
        {
            Check.NotNull(sqlFragmentExpression, nameof(sqlFragmentExpression));

            _relationalCommandBuilder.Append(sqlFragmentExpression.Sql);

            return sqlFragmentExpression;
        }

        /// <summary>
        ///     Visit a StringCompareExpression.
        /// </summary>
        /// <param name="stringCompareExpression"> The string compare expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitStringCompare(StringCompareExpression stringCompareExpression)
        {
            Visit(stringCompareExpression.Left);

            _relationalCommandBuilder.Append(GenerateOperator(stringCompareExpression));

            Visit(stringCompareExpression.Right);

            return stringCompareExpression;
        }

        /// <summary>
        ///     Visit an InExpression.
        /// </summary>
        /// <param name="inExpression"> The in expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitIn(InExpression inExpression)
        {
            if (inExpression.Values != null)
            {
                var inValues = ProcessInExpressionValues(inExpression.Values);
                var inValuesNotNull = ExtractNonNullExpressionValues(inValues);

                if (inValues.Count != inValuesNotNull.Count)
                {
                    var relationalNullsInExpression
                        = Expression.OrElse(
                            new InExpression(inExpression.Operand, inValuesNotNull),
                            new IsNullExpression(inExpression.Operand));

                    _relationalCommandBuilder.Append("(");

                    Visit(relationalNullsInExpression);

                    _relationalCommandBuilder.Append(")");

                    return inExpression;
                }

                if (inValuesNotNull.Count > 0)
                {
                    var parentTypeMapping = _typeMapping;
                    _typeMapping = InferTypeMappingFromColumn(inExpression.Operand) ?? parentTypeMapping;

                    Visit(inExpression.Operand);

                    _relationalCommandBuilder.Append(" IN (");

                    ProcessExpressionList(inValuesNotNull);

                    _relationalCommandBuilder.Append(")");

                    _typeMapping = parentTypeMapping;
                }
                else
                {
                    _relationalCommandBuilder.Append("0 = 1");
                }
            }
            else
            {
                var parentTypeMapping = _typeMapping;
                _typeMapping = InferTypeMappingFromColumn(inExpression.Operand) ?? parentTypeMapping;

                Visit(inExpression.Operand);

                _relationalCommandBuilder.Append(" IN ");

                Visit(inExpression.SubQuery);

                _typeMapping = parentTypeMapping;
            }

            return inExpression;
        }

        /// <summary>
        ///     Visit a negated InExpression.
        /// </summary>
        /// <param name="inExpression"> The in expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        protected virtual Expression GenerateNotIn([NotNull] InExpression inExpression)
        {
            if (inExpression.Values != null)
            {
                var inValues = ProcessInExpressionValues(inExpression.Values);
                var inValuesNotNull = ExtractNonNullExpressionValues(inValues);

                if (inValues.Count != inValuesNotNull.Count)
                {
                    var relationalNullsNotInExpression
                        = Expression.AndAlso(
                            Expression.Not(new InExpression(inExpression.Operand, inValuesNotNull)),
                            Expression.Not(new IsNullExpression(inExpression.Operand)));

                    return Visit(relationalNullsNotInExpression);
                }

                if (inValues.Count > 0)
                {
                    Visit(inExpression.Operand);

                    _relationalCommandBuilder.Append(" NOT IN (");

                    ProcessExpressionList(inValues);

                    _relationalCommandBuilder.Append(")");
                }
                else
                {
                    _relationalCommandBuilder.Append("1 = 1");
                }
            }
            else
            {
                Visit(inExpression.Operand);

                _relationalCommandBuilder.Append(" NOT IN ");

                Visit(inExpression.SubQuery);
            }

            return inExpression;
        }

        /// <summary>
        ///     Process the InExpression values.
        /// </summary>
        /// <param name="inExpressionValues"> The in expression values. </param>
        /// <returns>
        ///     A list of expressions.
        /// </returns>
        protected virtual IReadOnlyList<Expression> ProcessInExpressionValues(
            [NotNull] IEnumerable<Expression> inExpressionValues)
        {
            Check.NotNull(inExpressionValues, nameof(inExpressionValues));

            var inConstants = new List<Expression>();

            foreach (var inValue in inExpressionValues)
            {
                if (inValue is ConstantExpression inConstant)
                {
                    AddInExpressionValues(inConstant.Value, inConstants, inConstant);
                }
                else
                {
                    if (inValue is ParameterExpression inParameter)
                    {
                        if (_parametersValues.TryGetValue(inParameter.Name, out var parameterValue))
                        {
                            AddInExpressionValues(parameterValue, inConstants, inParameter);

                            IsCacheable = false;
                        }
                    }
                    else
                    {
                        if (inValue is ListInitExpression inListInit)
                        {
                            inConstants.AddRange(
                                ProcessInExpressionValues(
                                    inListInit.Initializers.SelectMany(i => i.Arguments)));
                        }
                        else
                        {
                            if (inValue is NewArrayExpression newArray)
                            {
                                inConstants.AddRange(ProcessInExpressionValues(newArray.Expressions));
                            }
                        }
                    }
                }
            }

            return inConstants;
        }

        private static void AddInExpressionValues(
            object value, List<Expression> inConstants, Expression expression)
        {
            if (value is IEnumerable valuesEnumerable
                && value.GetType() != typeof(string)
                && value.GetType() != typeof(byte[]))
            {
                inConstants.AddRange(valuesEnumerable.Cast<object>().Select(Expression.Constant));
            }
            else
            {
                inConstants.Add(expression);
            }
        }

        /// <summary>
        ///     Extracts the non null expression values from a list of expressions.
        /// </summary>
        /// <param name="inExpressionValues"> The list of expressions. </param>
        /// <returns>
        ///     The extracted non null expression values.
        /// </returns>
        protected virtual IReadOnlyList<Expression> ExtractNonNullExpressionValues(
            [NotNull] IReadOnlyList<Expression> inExpressionValues)
        {
            var inValuesNotNull = new List<Expression>();

            foreach (var inValue in inExpressionValues)
            {
                var inConstant = inValue as ConstantExpression;

                if (inConstant?.Value != null)
                {
                    inValuesNotNull.Add(inValue);

                    continue;
                }

                if (inValue is ParameterExpression inParameter)
                {
                    if (_parametersValues.TryGetValue(inParameter.Name, out var parameterValue))
                    {
                        if (parameterValue != null)
                        {
                            inValuesNotNull.Add(inValue);
                        }
                    }
                }
            }

            return inValuesNotNull;
        }

        /// <summary>
        ///     Visit an InnerJoinExpression.
        /// </summary>
        /// <param name="innerJoinExpression"> The inner join expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitInnerJoin(InnerJoinExpression innerJoinExpression)
        {
            Check.NotNull(innerJoinExpression, nameof(innerJoinExpression));

            _relationalCommandBuilder.Append("INNER JOIN ");

            Visit(innerJoinExpression.TableExpression);

            _relationalCommandBuilder.Append(" ON ");

            Visit(ApplyOptimizations(innerJoinExpression.Predicate, searchCondition: true, joinCondition: true));

            return innerJoinExpression;
        }

        /// <summary>
        ///     Visit an LeftOuterJoinExpression.
        /// </summary>
        /// <param name="leftOuterJoinExpression"> The left outer join expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitLeftOuterJoin(LeftOuterJoinExpression leftOuterJoinExpression)
        {
            Check.NotNull(leftOuterJoinExpression, nameof(leftOuterJoinExpression));

            _relationalCommandBuilder.Append("LEFT JOIN ");

            Visit(leftOuterJoinExpression.TableExpression);

            _relationalCommandBuilder.Append(" ON ");

            Visit(ApplyOptimizations(leftOuterJoinExpression.Predicate, searchCondition: true, joinCondition: true));

            return leftOuterJoinExpression;
        }

        /// <summary>
        ///     Generates the TOP part of the SELECT statement,
        /// </summary>
        /// <param name="selectExpression"> The select expression. </param>
        protected virtual void GenerateTop([NotNull] SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Limit != null
                && selectExpression.Offset == null)
            {
                _relationalCommandBuilder.Append("TOP(");

                Visit(selectExpression.Limit);

                _relationalCommandBuilder.Append(") ");
            }
        }

        /// <summary>
        ///     Generates the LIMIT OFFSET part of the SELECT statement,
        /// </summary>
        /// <param name="selectExpression"> The select expression. </param>
        protected virtual void GenerateLimitOffset([NotNull] SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Offset != null)
            {
                _relationalCommandBuilder.AppendLine()
                    .Append("OFFSET ");

                Visit(selectExpression.Offset);

                _relationalCommandBuilder.Append(" ROWS");

                if (selectExpression.Limit != null)
                {
                    _relationalCommandBuilder.Append(" FETCH NEXT ");

                    Visit(selectExpression.Limit);

                    _relationalCommandBuilder.Append(" ROWS ONLY");
                }
            }
        }

        /// <summary>
        ///     Visit a ConditionalExpression.
        /// </summary>
        /// <param name="conditionalExpression"> The conditional expression to visit. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        protected override Expression VisitConditional(ConditionalExpression conditionalExpression)
        {
            Check.NotNull(conditionalExpression, nameof(conditionalExpression));

            _relationalCommandBuilder.AppendLine("CASE");

            using (_relationalCommandBuilder.Indent())
            {
                _relationalCommandBuilder.Append("WHEN ");

                Visit(conditionalExpression.Test);

                _relationalCommandBuilder.AppendLine();
                _relationalCommandBuilder.Append("THEN ");

                if (conditionalExpression.IfTrue is ConstantExpression constantIfTrue
                    && constantIfTrue.Type == typeof(bool))
                {
                    _relationalCommandBuilder.Append((bool)constantIfTrue.Value ? TypedTrueLiteral : TypedFalseLiteral);
                }
                else
                {
                    Visit(conditionalExpression.IfTrue);
                }

                _relationalCommandBuilder.Append(" ELSE ");

                if (conditionalExpression.IfFalse is ConstantExpression constantIfFalse
                    && constantIfFalse.Type == typeof(bool))
                {
                    _relationalCommandBuilder.Append((bool)constantIfFalse.Value ? TypedTrueLiteral : TypedFalseLiteral);
                }
                else
                {
                    Visit(conditionalExpression.IfFalse);
                }

                _relationalCommandBuilder.AppendLine();
            }

            _relationalCommandBuilder.Append("END");

            return conditionalExpression;
        }

        /// <summary>
        ///     Visit an ExistsExpression.
        /// </summary>
        /// <param name="existsExpression"> The exists expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitExists(ExistsExpression existsExpression)
        {
            Check.NotNull(existsExpression, nameof(existsExpression));

            _relationalCommandBuilder.AppendLine("EXISTS (");

            using (_relationalCommandBuilder.Indent())
            {
                Visit(existsExpression.Subquery);
            }

            _relationalCommandBuilder.Append(")");

            return existsExpression;
        }

        /// <summary>
        ///     Visit a BinaryExpression.
        /// </summary>
        /// <param name="binaryExpression"> The binary expression to visit. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            Check.NotNull(binaryExpression, nameof(binaryExpression));

            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Coalesce:
                {
                    _relationalCommandBuilder.Append("COALESCE(");
                    Visit(binaryExpression.Left);
                    _relationalCommandBuilder.Append(", ");
                    Visit(binaryExpression.Right);
                    _relationalCommandBuilder.Append(")");

                    break;
                }
                default:
                {
                    var parentTypeMapping = _typeMapping;

                    if (binaryExpression.IsComparisonOperation()
                        || binaryExpression.NodeType == ExpressionType.Add)
                    {
                        _typeMapping
                            = InferTypeMappingFromColumn(binaryExpression.Left)
                              ?? InferTypeMappingFromColumn(binaryExpression.Right)
                              ?? parentTypeMapping;
                    }

                    var needParens = binaryExpression.Left.RemoveConvert() is BinaryExpression leftBinaryExpression
                                     && leftBinaryExpression.NodeType != ExpressionType.Coalesce;

                    if (needParens)
                    {
                        _relationalCommandBuilder.Append("(");
                    }

                    Visit(binaryExpression.Left);

                    if (needParens)
                    {
                        _relationalCommandBuilder.Append(")");
                    }

                    _relationalCommandBuilder.Append(GenerateOperator(binaryExpression));

                    needParens = binaryExpression.Right.RemoveConvert() is BinaryExpression rightBinaryExpression
                                 && rightBinaryExpression.NodeType != ExpressionType.Coalesce;

                    if (needParens)
                    {
                        _relationalCommandBuilder.Append("(");
                    }

                    Visit(binaryExpression.Right);

                    if (needParens)
                    {
                        _relationalCommandBuilder.Append(")");
                    }

                    _typeMapping = parentTypeMapping;

                    break;
                }
            }

            return binaryExpression;
        }

        /// <summary>
        ///     Visits a ColumnExpression.
        /// </summary>
        /// <param name="columnExpression"> The column expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitColumn(ColumnExpression columnExpression)
        {
            Check.NotNull(columnExpression, nameof(columnExpression));

            _relationalCommandBuilder.Append(SqlGenerator.DelimitIdentifier(columnExpression.Table.Alias))
                .Append(".")
                .Append(SqlGenerator.DelimitIdentifier(columnExpression.Name));

            return columnExpression;
        }

        /// <summary>
        ///     Visits a ColumnReferenceExpression.
        /// </summary>
        /// <param name="columnReferenceExpression"> The column reference expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitColumnReference(ColumnReferenceExpression columnReferenceExpression)
        {
            Check.NotNull(columnReferenceExpression, nameof(columnReferenceExpression));

            _relationalCommandBuilder.Append(SqlGenerator.DelimitIdentifier(columnReferenceExpression.Table.Alias))
                .Append(".")
                .Append(SqlGenerator.DelimitIdentifier(columnReferenceExpression.Name));

            return columnReferenceExpression;
        }

        /// <summary>
        ///     Visits an AliasExpression.
        /// </summary>
        /// <param name="aliasExpression"> The alias expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitAlias(AliasExpression aliasExpression)
        {
            Check.NotNull(aliasExpression, nameof(aliasExpression));

            Visit(aliasExpression.Expression);

            if (aliasExpression.Alias != null)
            {
                _relationalCommandBuilder.Append(" AS ");
            }

            if (aliasExpression.Alias != null)
            {
                _relationalCommandBuilder.Append(SqlGenerator.DelimitIdentifier(aliasExpression.Alias));
            }

            return aliasExpression;
        }

        /// <summary>
        ///     Visits an IsNullExpression.
        /// </summary>
        /// <param name="isNullExpression"> The is null expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitIsNull(IsNullExpression isNullExpression)
        {
            Check.NotNull(isNullExpression, nameof(isNullExpression));

            Visit(isNullExpression.Operand);

            _relationalCommandBuilder.Append(" IS NULL");

            return isNullExpression;
        }

        /// <summary>
        ///     Visits an IsNotNullExpression.
        /// </summary>
        /// <param name="isNotNullExpression"> The is not null expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        protected virtual Expression GenerateIsNotNull([NotNull] IsNullExpression isNotNullExpression)
        {
            Check.NotNull(isNotNullExpression, nameof(isNotNullExpression));

            Visit(isNotNullExpression.Operand);

            _relationalCommandBuilder.Append(" IS NOT NULL");

            return isNotNullExpression;
        }

        /// <summary>
        ///     Visit a LikeExpression.
        /// </summary>
        /// <param name="likeExpression"> The like expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitLike(LikeExpression likeExpression)
        {
            Check.NotNull(likeExpression, nameof(likeExpression));

            var parentTypeMapping = _typeMapping;
            _typeMapping = InferTypeMappingFromColumn(likeExpression.Match) ?? parentTypeMapping;

            Visit(likeExpression.Match);

            _relationalCommandBuilder.Append(" LIKE ");

            Visit(likeExpression.Pattern);

            if (likeExpression.EscapeChar != null)
            {
                _relationalCommandBuilder.Append(" ESCAPE ");
                Visit(likeExpression.EscapeChar);
            }

            _typeMapping = parentTypeMapping;

            return likeExpression;
        }

        /// <summary>
        ///     Visits a SqlFunctionExpression.
        /// </summary>
        /// <param name="sqlFunctionExpression"> The SQL function expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            GenerateFunctionCall(sqlFunctionExpression.FunctionName, sqlFunctionExpression.Arguments, sqlFunctionExpression.Schema);

            return sqlFunctionExpression;
        }

        /// <summary>
        ///     Generates a SQL function call.
        /// </summary>
        /// <param name="functionName">The function name</param>
        /// <param name="arguments">The function arguments</param>
        /// <param name="schema">The function schema</param>
        protected virtual void GenerateFunctionCall(
            [NotNull] string functionName, [NotNull] IReadOnlyList<Expression> arguments,
            [CanBeNull] string schema = null)
        {
            Check.NotEmpty(functionName, nameof(functionName));
            Check.NotNull(arguments, nameof(arguments));

            if (!string.IsNullOrWhiteSpace(schema))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                _relationalCommandBuilder.Append(SqlGenerator.DelimitIdentifier(schema))
                    .Append(".");
            }

            var parentTypeMapping = _typeMapping;
            _typeMapping = null;

            _relationalCommandBuilder.Append(functionName);
            _relationalCommandBuilder.Append("(");

            ProcessExpressionList(arguments);

            _relationalCommandBuilder.Append(")");

            _typeMapping = parentTypeMapping;
        }

        /// <summary>
        ///     Visit a SQL ExplicitCastExpression.
        /// </summary>
        /// <param name="explicitCastExpression"> The explicit cast expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitExplicitCast(ExplicitCastExpression explicitCastExpression)
        {
            _relationalCommandBuilder.Append("CAST(");

            var parentTypeMapping = _typeMapping;

            _typeMapping = InferTypeMappingFromColumn(explicitCastExpression.Operand);

            Visit(explicitCastExpression.Operand);

            _relationalCommandBuilder.Append(" AS ");

            var typeMapping = Dependencies.RelationalTypeMapper.FindMapping(explicitCastExpression.Type);

            if (typeMapping == null)
            {
                throw new InvalidOperationException(RelationalStrings.UnsupportedType(explicitCastExpression.Type.ShortDisplayName()));
            }

            _relationalCommandBuilder.Append(typeMapping.StoreType);

            _relationalCommandBuilder.Append(")");

            _typeMapping = parentTypeMapping;

            return explicitCastExpression;
        }

        /// <summary>
        ///     Visits a UnaryExpression.
        /// </summary>
        /// <param name="expression"> The unary expression to visit. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        protected override Expression VisitUnary(UnaryExpression expression)
        {
            Check.NotNull(expression, nameof(expression));

            switch (expression.NodeType)
            {
                case ExpressionType.Not:
                {
                    if (expression.Operand is InExpression inExpression)
                    {
                        return GenerateNotIn(inExpression);
                    }

                    if (expression.Operand is IsNullExpression isNullExpression)
                    {
                        return GenerateIsNotNull(isNullExpression);
                    }

                    if (expression.Operand is ExistsExpression)
                    {
                        _relationalCommandBuilder.Append("NOT ");

                        Visit(expression.Operand);

                        return expression;
                    }

                    _relationalCommandBuilder.Append("NOT (");

                    Visit(expression.Operand);

                    _relationalCommandBuilder.Append(")");

                    return expression;
                }
                case ExpressionType.Convert:
                {
                    Visit(expression.Operand);

                    return expression;
                }
                case ExpressionType.Negate:
                {
                    _relationalCommandBuilder.Append("-");

                    Visit(expression.Operand);

                    return expression;
                }
            }

            return base.VisitUnary(expression);
        }

        /// <summary>
        ///     Visits a ConstantExpression.
        /// </summary>
        /// <param name="constantExpression"> The constant expression to visit. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {
            Check.NotNull(constantExpression, nameof(constantExpression));

            var value = constantExpression.Value;

            if (constantExpression.Type.UnwrapNullableType().IsEnum)
            {
                var underlyingType = constantExpression.Type.UnwrapEnumType();
                value = Convert.ChangeType(value, underlyingType);
            }

            _relationalCommandBuilder.Append(
                value == null
                    ? "NULL"
                    : GetTypeMapping(value).GenerateSqlLiteral(value));

            return constantExpression;
        }

        /// <summary>
        ///     Visits a ParameterExpression.
        /// </summary>
        /// <param name="parameterExpression"> The parameter expression to visit. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            Check.NotNull(parameterExpression, nameof(parameterExpression));

            var parameterName = SqlGenerator.GenerateParameterName(parameterExpression.Name);

            if (_relationalCommandBuilder.ParameterBuilder.Parameters
                .All(p => p.InvariantName != parameterExpression.Name))
            {
                _relationalCommandBuilder.AddParameter(
                    parameterExpression.Name,
                    parameterName,
                    _typeMapping ?? Dependencies.RelationalTypeMapper.GetMapping(parameterExpression.Type),
                    parameterExpression.Type.IsNullableType());
            }

            _relationalCommandBuilder.Append(parameterName);

            return parameterExpression;
        }

        /// <summary>
        ///     Visits a PropertyParameterExpression.
        /// </summary>
        /// <param name="propertyParameterExpression"> The property parameter expression. </param>
        /// <returns>
        ///     An Expression.
        /// </returns>
        public virtual Expression VisitPropertyParameter(PropertyParameterExpression propertyParameterExpression)
        {
            var parameterName
                = SqlGenerator.GenerateParameterName(
                    propertyParameterExpression.PropertyParameterName);

            if (_relationalCommandBuilder.ParameterBuilder.Parameters
                .All(p => p.InvariantName != propertyParameterExpression.PropertyParameterName))
            {
                _relationalCommandBuilder.AddPropertyParameter(
                    propertyParameterExpression.Name,
                    parameterName,
                    propertyParameterExpression.Property);
            }

            _relationalCommandBuilder.Append(parameterName);

            return propertyParameterExpression;
        }

        /// <summary>
        ///     Infers a type mapping from a column expression.
        /// </summary>
        /// <param name="expression"> The expression to infer a type mapping for. </param>
        /// <returns>
        ///     A RelationalTypeMapping.
        /// </returns>
        protected virtual RelationalTypeMapping InferTypeMappingFromColumn([NotNull] Expression expression)
        {
            switch (expression)
            {
                case ColumnExpression columnExpression:
                    return Dependencies.RelationalTypeMapper.FindMapping(columnExpression.Property);
                case ColumnReferenceExpression columnReferenceExpression:
                    return InferTypeMappingFromColumn(columnReferenceExpression.Expression);
                case AliasExpression aliasExpression:
                    return InferTypeMappingFromColumn(aliasExpression.Expression);
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Attempts to generate binary operator for a given expression type.
        /// </summary>
        /// <param name="op"> The operation. </param>
        /// <param name="result"> [out] The SQL binary operator. </param>
        /// <returns>
        ///     true if it succeeds, false if it fails.
        /// </returns>
        protected virtual bool TryGenerateBinaryOperator(ExpressionType op, [NotNull] out string result)
            => _operatorMap.TryGetValue(op, out result);

        /// <summary>
        ///     Generates SQL for a given binary operation type.
        /// </summary>
        /// <param name="op"> The operation. </param>
        /// <returns>
        ///     The binary operator.
        /// </returns>
        protected virtual string GenerateBinaryOperator(ExpressionType op) => _operatorMap[op];

        /// <summary>
        ///     Generates an SQL operator for a given expression.
        /// </summary>
        /// <param name="expression"> The expression. </param>
        /// <returns>
        ///     The operator.
        /// </returns>
        protected virtual string GenerateOperator([NotNull] Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Extension:
                {
                    if (expression is StringCompareExpression asStringCompareExpression)
                    {
                        return GenerateBinaryOperator(asStringCompareExpression.Operator);
                    }
                    goto default;
                }
                default:
                {
                    string op;
                    if (expression is BinaryExpression)
                    {
                        if (!TryGenerateBinaryOperator(expression.NodeType, out op))
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        return op;
                    }
                    if (!_operatorMap.TryGetValue(expression.NodeType, out op))
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    return op;
                }
            }
        }

        /// <summary>
        ///     Creates unhandled item exception.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="unhandledItem"> The unhandled item. </param>
        /// <param name="visitMethod"> The visit method. </param>
        /// <returns>
        ///     The new unhandled item exception.
        /// </returns>
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
            => new NotImplementedException(visitMethod);

        private class NullComparisonTransformingVisitor : RelinqExpressionVisitor
        {
            private readonly IReadOnlyDictionary<string, object> _parameterValues;

            public NullComparisonTransformingVisitor(IReadOnlyDictionary<string, object> parameterValues)
            {
                _parameterValues = parameterValues;
            }

            protected override Expression VisitBinary(BinaryExpression expression)
            {
                if (expression.NodeType == ExpressionType.Equal
                    || expression.NodeType == ExpressionType.NotEqual)
                {
                    var leftExpression = expression.Left.RemoveConvert();
                    var rightExpression = expression.Right.RemoveConvert();

                    var parameterExpression = leftExpression as ParameterExpression
                                              ?? rightExpression as ParameterExpression;

                    if (parameterExpression != null
                        && _parameterValues.TryGetValue(parameterExpression.Name, out var parameterValue))
                    {
                        var nonParameterExpression = leftExpression is ParameterExpression ? rightExpression : leftExpression;

                        if (nonParameterExpression is ConstantExpression constantExpression)
                        {
                            if (parameterValue == null
                                && constantExpression.Value == null)
                            {
                                return
                                    expression.NodeType == ExpressionType.Equal
                                        ? Expression.Constant(true)
                                        : Expression.Constant(false);
                            }

                            if (parameterValue == null && constantExpression.Value != null
                                || parameterValue != null && constantExpression.Value == null)
                            {
                                return
                                    expression.NodeType == ExpressionType.Equal
                                        ? Expression.Constant(false)
                                        : Expression.Constant(true);
                            }
                        }

                        if (parameterValue == null)
                        {
                            return
                                expression.NodeType == ExpressionType.Equal
                                    ? (Expression)new IsNullExpression(nonParameterExpression)
                                    : Expression.Not(new IsNullExpression(nonParameterExpression));
                        }
                    }
                }

                return base.VisitBinary(expression);
            }

            protected override Expression VisitExtension(Expression node)
                => node is NullCompensatedExpression
                    ? node
                    : base.VisitExtension(node);
        }

        private class BooleanExpressionTranslatingVisitor : RelinqExpressionVisitor
        {
            private bool _isSearchCondition;

            /// <summary>
            ///     Translates given expression to either boolean condition or value
            /// </summary>
            /// <param name="expression">The expression to translate</param>
            /// <param name="searchCondition">Specifies if the returned value should be boolean condition or value</param>
            /// <returns>The translated expression</returns>
            /// General flow of overriden methods
            /// 1. Inspect expression type and set _isSearchCondition flag
            /// 2. Visit the children
            /// 3. Restore _isSearchCondition
            /// 4. Update the expression
            /// 5. Convert to value/search condition as per _isSearchConditionFlag
            public Expression Translate(Expression expression, bool searchCondition)
            {
                _isSearchCondition = searchCondition;

                return Visit(expression);
            }

            protected override Expression VisitBinary(BinaryExpression binaryExpression)
            {
                var parentIsSearchCondition = _isSearchCondition;

                switch (binaryExpression.NodeType)
                {
                    // Only logical operations need conditions on both sides
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        _isSearchCondition = true;
                        break;
                    default:
                        _isSearchCondition = false;
                        break;
                }

                var newLeft = Visit(binaryExpression.Left);
                var newRight = Visit(binaryExpression.Right);

                _isSearchCondition = parentIsSearchCondition;

                binaryExpression = binaryExpression.Update(newLeft, binaryExpression.Conversion, newRight);

                return ApplyConversion(binaryExpression);
            }

            protected override Expression VisitConditional(ConditionalExpression conditionalExpression)
            {
                var parentIsSearchCondition = _isSearchCondition;

                // Test is always a condition
                _isSearchCondition = true;
                var test = Visit(conditionalExpression.Test);
                // Results are always values
                _isSearchCondition = false;
                var ifTrue = Visit(conditionalExpression.IfTrue);
                var ifFalse = Visit(conditionalExpression.IfFalse);

                _isSearchCondition = parentIsSearchCondition;

                conditionalExpression = conditionalExpression.Update(test, ifTrue, ifFalse);

                return ApplyConversion(conditionalExpression);
            }

            protected override Expression VisitConstant(ConstantExpression constantExpression)
                => ApplyConversion(constantExpression);

            protected override Expression VisitUnary(UnaryExpression unaryExpression)
            {
                // Special optimization
                // NOT(A) => A == false
                if (unaryExpression.NodeType == ExpressionType.Not
                    && unaryExpression.Operand.IsSimpleExpression())
                {
                    return Visit(BuildCompareToExpression(unaryExpression.Operand, compareTo: false));
                }

                var parentIsSearchCondition = _isSearchCondition;

                switch (unaryExpression.NodeType)
                {
                    // For convert preserve the flag since they are transparent to SQL
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        break;

                    // For Not operand must be search condition
                    case ExpressionType.Not:
                        _isSearchCondition = true;
                        break;

                    // For rest, operand must be value
                    default:
                        _isSearchCondition = false;
                        break;
                }

                var operand = Visit(unaryExpression.Operand);

                _isSearchCondition = parentIsSearchCondition;

                unaryExpression = unaryExpression.Update(operand);

                // Convert nodes are transparent to SQL hence no conversion needed
                if (unaryExpression.NodeType == ExpressionType.Convert
                    || unaryExpression.NodeType == ExpressionType.ConvertChecked)
                {
                    return unaryExpression;
                }

                return ApplyConversion(unaryExpression);
            }

            protected override Expression VisitExtension(Expression extensionExpression)
            {
                var parentIsSearchCondition = _isSearchCondition;

                // All current Extension expressions have value type children
                _isSearchCondition = false;
                var newExpression = base.VisitExtension(extensionExpression);

                _isSearchCondition = parentIsSearchCondition;

                return ApplyConversion(newExpression);
            }

            protected override Expression VisitParameter(ParameterExpression parameterExpression)
                => ApplyConversion(parameterExpression);

            private Expression ApplyConversion(Expression expression)
                => _isSearchCondition
                    ? ConvertToSearchCondition(expression)
                    : ConvertToValue(expression);

            private static bool IsSearchCondition(Expression expression)
            {
                expression = expression.RemoveConvert();

                if (!(expression is BinaryExpression)
                    && expression.NodeType != ExpressionType.Not
                    && expression.NodeType != ExpressionType.Extension)
                {
                    return false;
                }

                return expression.IsComparisonOperation()
                       || expression.IsLogicalOperation()
                       || expression is ExistsExpression
                       || expression is InExpression
                       || expression is IsNullExpression
                       || expression is LikeExpression
                       || expression is StringCompareExpression;
            }

            private static Expression BuildCompareToExpression(Expression expression, bool compareTo)
            {
                var equalExpression = Expression.Equal(
                    expression,
                    Expression.Constant(compareTo, expression.Type));

                // Compensate for type change since Expression.Equal always returns expression of boolean type
                return expression.Type == typeof(bool)
                    ? (Expression)equalExpression
                    : Expression.Convert(equalExpression, expression.Type);
            }

            private static Expression ConvertToSearchCondition(Expression expression)
                => IsSearchCondition(expression)
                    ? expression
                    : BuildCompareToExpression(expression, compareTo: true);

            private static Expression ConvertToValue(Expression expression)
                => IsSearchCondition(expression)
                    ? Expression.Condition(
                        expression,
                        Expression.Constant(true, expression.Type),
                        Expression.Constant(false, expression.Type))
                    : expression;
        }
    }
}
