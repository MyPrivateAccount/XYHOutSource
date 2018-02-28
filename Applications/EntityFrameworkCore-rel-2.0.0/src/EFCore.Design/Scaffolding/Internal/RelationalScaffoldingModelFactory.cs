// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using ScaffoldingAnnotationNames = Microsoft.EntityFrameworkCore.Scaffolding.Metadata.Internal.ScaffoldingAnnotationNames;

namespace Microsoft.EntityFrameworkCore.Scaffolding.Internal
{
    public class RelationalScaffoldingModelFactory : IScaffoldingModelFactory
    {
        internal const string NavigationNameUniquifyingPattern = "{0}Navigation";
        internal const string SelfReferencingPrincipalEndNavigationNamePattern = "Inverse{0}";

        protected virtual IOperationReporter Reporter { get; }
        protected virtual IRelationalTypeMapper TypeMapper { get; }
        protected virtual ICandidateNamingService CandidateNamingService { get; }

        private Dictionary<DatabaseTable, CSharpUniqueNamer<DatabaseColumn>> _columnNamers;
        private readonly DatabaseTable _nullTable = new DatabaseTable();
        private CSharpUniqueNamer<DatabaseTable> _tableNamer;
        private CSharpUniqueNamer<DatabaseTable> _dbSetNamer;
        private readonly IDatabaseModelFactory _databaseModelFactory;
        private readonly HashSet<DatabaseColumn> _unmappedColumns = new HashSet<DatabaseColumn>();
        private readonly IPluralizer _pluralizer;
        private readonly ICSharpUtilities _cSharpUtilities;
        private readonly IScaffoldingTypeMapper _scaffoldingTypeMapper;

        public RelationalScaffoldingModelFactory(
            [NotNull] IOperationReporter reporter,
            [NotNull] IRelationalTypeMapper typeMapper,
            [NotNull] IDatabaseModelFactory databaseModelFactory,
            [NotNull] ICandidateNamingService candidateNamingService,
            [NotNull] IPluralizer pluralizer,
            [NotNull] ICSharpUtilities cSharpUtilities,
            [NotNull] IScaffoldingTypeMapper scaffoldingTypeMapper)
        {
            Check.NotNull(reporter, nameof(reporter));
            Check.NotNull(typeMapper, nameof(typeMapper));
            Check.NotNull(databaseModelFactory, nameof(databaseModelFactory));
            Check.NotNull(candidateNamingService, nameof(candidateNamingService));
            Check.NotNull(pluralizer, nameof(pluralizer));
            Check.NotNull(cSharpUtilities, nameof(cSharpUtilities));
            Check.NotNull(scaffoldingTypeMapper, nameof(scaffoldingTypeMapper));

            Reporter = reporter;
            TypeMapper = typeMapper;
            CandidateNamingService = candidateNamingService;
            _databaseModelFactory = databaseModelFactory;
            _pluralizer = pluralizer;
            _cSharpUtilities = cSharpUtilities;
            _scaffoldingTypeMapper = scaffoldingTypeMapper;
        }

        public virtual IModel Create(
            string connectionString,
            IEnumerable<string> tables,
            IEnumerable<string> schemas,
            bool useDatabaseNames)
        {
            Check.NotEmpty(connectionString, nameof(connectionString));
            Check.NotNull(tables, nameof(tables));
            Check.NotNull(schemas, nameof(schemas));

            var databaseModel = _databaseModelFactory.Create(connectionString, tables, schemas);

            return CreateFromDatabaseModel(databaseModel, useDatabaseNames);
        }

        protected virtual IModel CreateFromDatabaseModel([NotNull] DatabaseModel databaseModel, bool useDatabaseNames)
        {
            Check.NotNull(databaseModel, nameof(databaseModel));

            var modelBuilder = new ModelBuilder(new ConventionSet());

            _tableNamer = new CSharpUniqueNamer<DatabaseTable>(
                useDatabaseNames
                    ? (Func<DatabaseTable, string>)(t => t.Name)
                    : t => CandidateNamingService.GenerateCandidateIdentifier(t.Name),
                _cSharpUtilities,
                useDatabaseNames
                    ? (Func<string, string>)null
                    : _pluralizer.Singularize);
            _dbSetNamer = new CSharpUniqueNamer<DatabaseTable>(
                useDatabaseNames
                    ? (Func<DatabaseTable, string>)(t => t.Name)
                    : t => CandidateNamingService.GenerateCandidateIdentifier(t.Name),
                _cSharpUtilities,
                useDatabaseNames
                    ? (Func<string, string>)null
                    : _pluralizer.Pluralize);
            _columnNamers = new Dictionary<DatabaseTable, CSharpUniqueNamer<DatabaseColumn>>();

            VisitDatabaseModel(modelBuilder, databaseModel);

            return modelBuilder.Model;
        }

        protected virtual string GetEntityTypeName([NotNull] DatabaseTable table)
            => _tableNamer.GetName(Check.NotNull(table, nameof(table)));

        protected virtual string GetDbSetName([NotNull] DatabaseTable table)
            => _dbSetNamer.GetName(Check.NotNull(table, nameof(table)));

        protected virtual string GetPropertyName([NotNull] DatabaseColumn column)
        {
            Check.NotNull(column, nameof(column));

            var table = column.Table ?? _nullTable;
            var usedNames = new List<string>();
            // TODO - need to clean up the way CSharpNamer & CSharpUniqueNamer work (see issue #1671)
            if (column.Table != null)
            {
                usedNames.Add(GetEntityTypeName(table));
            }

            if (!_columnNamers.ContainsKey(table))
            {
                _columnNamers.Add(
                    table,
                    new CSharpUniqueNamer<DatabaseColumn>(
                        c => CandidateNamingService.GenerateCandidateIdentifier(c.Name),
                        usedNames,
                        _cSharpUtilities,
                        singularizePluralizer: null));
            }

            return _columnNamers[table].GetName(column);
        }

        protected virtual ModelBuilder VisitDatabaseModel([NotNull] ModelBuilder modelBuilder, [NotNull] DatabaseModel databaseModel)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(databaseModel, nameof(databaseModel));

            if (!string.IsNullOrEmpty(databaseModel.DefaultSchema))
            {
                modelBuilder.HasDefaultSchema(databaseModel.DefaultSchema);
            }

            if (!string.IsNullOrEmpty(databaseModel.DatabaseName))
            {
                modelBuilder.Model.Scaffolding().DatabaseName = databaseModel.DatabaseName;
            }

            VisitSequences(modelBuilder, databaseModel.Sequences);
            VisitTables(modelBuilder, databaseModel.Tables);
            VisitForeignKeys(modelBuilder, databaseModel.Tables.SelectMany(table => table.ForeignKeys).ToList());

            modelBuilder.Model.AddAnnotations(databaseModel.GetAnnotations());

            return modelBuilder;
        }

        protected virtual ModelBuilder VisitSequences([NotNull] ModelBuilder modelBuilder, [NotNull] ICollection<DatabaseSequence> sequences)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(sequences, nameof(sequences));

            foreach (var sequence in sequences)
            {
                VisitSequence(modelBuilder, sequence);
            }

            return modelBuilder;
        }

        protected virtual SequenceBuilder VisitSequence([NotNull] ModelBuilder modelBuilder, [NotNull] DatabaseSequence sequence)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(sequence, nameof(sequence));

            if (string.IsNullOrEmpty(sequence.Name))
            {
                Reporter.WriteWarning(DesignStrings.SequencesRequireName);
                return null;
            }

            Type sequenceType = null;
            if (sequence.StoreType != null)
            {
                sequenceType = TypeMapper.FindMapping(sequence.StoreType)?.ClrType;
            }

            if (sequenceType != null
                && !Sequence.SupportedTypes.Contains(sequenceType))
            {
                Reporter.WriteWarning(DesignStrings.BadSequenceType(sequence.Name, sequence.StoreType));
                return null;
            }

            var builder = sequenceType != null
                ? modelBuilder.HasSequence(sequenceType, sequence.Name, sequence.Schema)
                : modelBuilder.HasSequence(sequence.Name, sequence.Schema);

            if (sequence.IncrementBy.HasValue)
            {
                builder.IncrementsBy(sequence.IncrementBy.Value);
            }

            if (sequence.MaxValue.HasValue)
            {
                builder.HasMax(sequence.MaxValue.Value);
            }

            if (sequence.MinValue.HasValue)
            {
                builder.HasMin(sequence.MinValue.Value);
            }

            if (sequence.StartValue.HasValue)
            {
                builder.StartsAt(sequence.StartValue.Value);
            }

            if (sequence.IsCyclic.HasValue)
            {
                builder.IsCyclic(sequence.IsCyclic.Value);
            }

            return builder;
        }

        protected virtual ModelBuilder VisitTables([NotNull] ModelBuilder modelBuilder, [NotNull] ICollection<DatabaseTable> tables)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(tables, nameof(tables));

            foreach (var table in tables)
            {
                VisitTable(modelBuilder, table);
            }

            return modelBuilder;
        }

        protected virtual EntityTypeBuilder VisitTable([NotNull] ModelBuilder modelBuilder, [NotNull] DatabaseTable table)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(table, nameof(table));

            var entityTypeName = GetEntityTypeName(table);

            var builder = modelBuilder.Entity(entityTypeName);

            var dbSetName = GetDbSetName(table);
            builder.Metadata.Scaffolding().DbSetName = dbSetName;

            builder.ToTable(table.Name, table.Schema);

            VisitColumns(builder, table.Columns);

            var keyBuilder = VisitPrimaryKey(builder, table);

            if (keyBuilder == null)
            {
                var errorMessage = DesignStrings.UnableToGenerateEntityType(table.DisplayName());
                Reporter.WriteWarning(errorMessage);

                var model = modelBuilder.Model;
                model.RemoveEntityType(entityTypeName);
                model.Scaffolding().EntityTypeErrors.Add(entityTypeName, errorMessage);
                return null;
            }

            VisitUniqueConstraints(builder, table.UniqueConstraints);
            VisitIndexes(builder, table.Indexes);

            builder.Metadata.AddAnnotations(table.GetAnnotations());

            return builder;
        }

        protected virtual EntityTypeBuilder VisitColumns([NotNull] EntityTypeBuilder builder, [NotNull] ICollection<DatabaseColumn> columns)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(columns, nameof(columns));

            foreach (var column in columns)
            {
                VisitColumn(builder, column);
            }

            return builder;
        }

        protected virtual PropertyBuilder VisitColumn([NotNull] EntityTypeBuilder builder, [NotNull] DatabaseColumn column)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(column, nameof(column));

            var typeScaffoldingInfo = GetTypeScaffoldingInfo(column);

            if (typeScaffoldingInfo == null)
            {
                _unmappedColumns.Add(column);
                Reporter.WriteWarning(
                    DesignStrings.CannotFindTypeMappingForColumn(column.DisplayName(), column.StoreType));
                return null;
            }

            var clrType = typeScaffoldingInfo.ClrType;
            var forceNullable = typeof(bool) == clrType && column.DefaultValueSql != null;
            if (forceNullable)
            {
                Reporter.WriteWarning(
                    DesignStrings.NonNullableBoooleanColumnHasDefaultConstraint(column.DisplayName()));
            }
            if (column.IsNullable || forceNullable)
            {
                clrType = clrType.MakeNullable();
            }

            var property = builder.Property(clrType, GetPropertyName(column));

            property.HasColumnName(column.Name);

            if (!typeScaffoldingInfo.IsInferred
                && !string.IsNullOrWhiteSpace(column.StoreType))
            {
                property.HasColumnType(column.StoreType);
            }

            if (typeScaffoldingInfo.ScaffoldUnicode.HasValue)
            {
                property.IsUnicode(typeScaffoldingInfo.ScaffoldUnicode.Value);
            }

            if (typeScaffoldingInfo.ScaffoldMaxLength.HasValue)
            {
                property.HasMaxLength(typeScaffoldingInfo.ScaffoldMaxLength.Value);
            }

            if (column.ValueGenerated == ValueGenerated.OnAdd)
            {
                property.ValueGeneratedOnAdd();
            }

            if (column.ValueGenerated == ValueGenerated.OnUpdate)
            {
                property.ValueGeneratedOnUpdate();
            }

            if (column.ValueGenerated == ValueGenerated.OnAddOrUpdate)
            {
                property.ValueGeneratedOnAddOrUpdate();
            }

            if (column.DefaultValueSql != null)
            {
                property.HasDefaultValueSql(column.DefaultValueSql);
            }

            if (column.ComputedColumnSql != null)
            {
                property.HasComputedColumnSql(column.ComputedColumnSql);
            }

            if (!(column.Table.PrimaryKey?.Columns.Contains(column) ?? false))
            {
                property.IsRequired(!column.IsNullable && !forceNullable);
            }

            if ((bool?)column[ScaffoldingAnnotationNames.ConcurrencyToken] == true)
            {
                property.IsConcurrencyToken();
            }

            property.Metadata.Scaffolding().ColumnOrdinal = column.Table.Columns.IndexOf(column);

            property.Metadata.AddAnnotations(
                column.GetAnnotations().Where(
                    a => a.Name != ScaffoldingAnnotationNames.UnderlyingStoreType
                         && a.Name != ScaffoldingAnnotationNames.ConcurrencyToken));

            return property;
        }

        protected virtual KeyBuilder VisitPrimaryKey([NotNull] EntityTypeBuilder builder, [NotNull] DatabaseTable table)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(table, nameof(table));

            var primaryKey = table.PrimaryKey;
            if (primaryKey == null)
            {
                Reporter.WriteWarning(DesignStrings.MissingPrimaryKey(table.DisplayName()));
                return null;
            }

            var unmappedColumns = primaryKey.Columns
                .Where(c => _unmappedColumns.Contains(c))
                .Select(c => c.Name)
                .ToList();
            if (unmappedColumns.Any())
            {
                Reporter.WriteWarning(
                    DesignStrings.PrimaryKeyErrorPropertyNotFound(
                        table.DisplayName(),
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedColumns)));
                return null;
            }

            var keyBuilder = builder.HasKey(primaryKey.Columns.Select(GetPropertyName).ToArray());

            if (primaryKey.Columns.Count == 1
                && primaryKey.Columns[0].ValueGenerated == null
                && primaryKey.Columns[0].DefaultValueSql == null)
            {
                var property = builder.Metadata.FindProperty(GetPropertyName(primaryKey.Columns[0]))?.AsProperty();
                if (property != null)
                {
                    var conventionalValueGenerated = new RelationalValueGeneratorConvention().GetValueGenerated(property);
                    if (conventionalValueGenerated == ValueGenerated.OnAdd)
                    {
                        property.ValueGenerated = ValueGenerated.Never;
                    }
                }
            }

            keyBuilder.Metadata.AddAnnotations(primaryKey.GetAnnotations());

            return keyBuilder;
        }

        protected virtual EntityTypeBuilder VisitUniqueConstraints([NotNull] EntityTypeBuilder builder, [NotNull] ICollection<DatabaseUniqueConstraint> uniqueConstraints)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(uniqueConstraints, nameof(uniqueConstraints));

            foreach (var uniqueConstraint in uniqueConstraints)
            {
                VisitUniqueConstraint(builder, uniqueConstraint);
            }

            return builder;
        }

        protected virtual IndexBuilder VisitUniqueConstraint([NotNull] EntityTypeBuilder builder, [NotNull] DatabaseUniqueConstraint uniqueConstraint)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(uniqueConstraint, nameof(uniqueConstraint));

            var unmappedColumns = uniqueConstraint.Columns
                .Where(c => _unmappedColumns.Contains(c))
                .Select(c => c.Name)
                .ToList();
            if (unmappedColumns.Any())
            {
                Reporter.WriteWarning(
                    DesignStrings.UnableToScaffoldIndexMissingProperty(
                        uniqueConstraint.Name,
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedColumns)));
                return null;
            }

            var propertyNames = uniqueConstraint.Columns.Select(GetPropertyName).ToArray();
            var indexBuilder = builder.HasIndex(propertyNames).IsUnique();

            if (!string.IsNullOrEmpty(uniqueConstraint.Name))
            {
                indexBuilder.HasName(uniqueConstraint.Name);
            }

            indexBuilder.Metadata.AddAnnotations(uniqueConstraint.GetAnnotations());

            return indexBuilder;
        }

        protected virtual EntityTypeBuilder VisitIndexes([NotNull] EntityTypeBuilder builder, [NotNull] ICollection<DatabaseIndex> indexes)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(indexes, nameof(indexes));

            foreach (var index in indexes)
            {
                VisitIndex(builder, index);
            }

            return builder;
        }

        protected virtual IndexBuilder VisitIndex([NotNull] EntityTypeBuilder builder, [NotNull] DatabaseIndex index)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(index, nameof(index));

            var unmappedColumns = index.Columns
                .Where(c => _unmappedColumns.Contains(c))
                .Select(c => c.Name)
                .ToList();
            if (unmappedColumns.Any())
            {
                Reporter.WriteWarning(
                    DesignStrings.UnableToScaffoldIndexMissingProperty(
                        index.Name,
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedColumns)));
                return null;
            }

            var propertyNames = index.Columns.Select(GetPropertyName).ToArray();
            var indexBuilder = builder.HasIndex(propertyNames)
                .IsUnique(index.IsUnique);

            if (index.Filter != null)
            {
                indexBuilder.HasFilter(index.Filter);
            }

            if (!string.IsNullOrEmpty(index.Name)
                && index.Name != ConstraintNamer.GetDefaultName(indexBuilder.Metadata))
            {
                indexBuilder.HasName(index.Name);
            }

            indexBuilder.Metadata.AddAnnotations(index.GetAnnotations());

            return indexBuilder;
        }

        protected virtual ModelBuilder VisitForeignKeys([NotNull] ModelBuilder modelBuilder, [NotNull] IList<DatabaseForeignKey> foreignKeys)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(foreignKeys, nameof(foreignKeys));

            foreach (var fk in foreignKeys)
            {
                VisitForeignKey(modelBuilder, fk);
            }

            // Note: must completely assign all foreign keys before assigning
            // navigation properties otherwise naming of navigation properties
            // when there are multiple foreign keys does not work.
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(et => et.GetForeignKeys()))
            {
                AddNavigationProperties(foreignKey);
            }

            return modelBuilder;
        }

        protected virtual IMutableForeignKey VisitForeignKey([NotNull] ModelBuilder modelBuilder, [NotNull] DatabaseForeignKey foreignKey)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(foreignKey, nameof(foreignKey));

            if (foreignKey.PrincipalTable == null)
            {
                Reporter.WriteWarning(
                    DesignStrings.ForeignKeyScaffoldErrorPrincipalTableNotFound(foreignKey.DisplayName()));
                return null;
            }

            if (foreignKey.Table == null)
            {
                return null;
            }

            var dependentEntityType = modelBuilder.Model.FindEntityType(GetEntityTypeName(foreignKey.Table));

            if (dependentEntityType == null)
            {
                return null;
            }

            var unmappedDependentColumns = foreignKey.Columns
                .Where(c => _unmappedColumns.Contains(c))
                .Select(c => c.Name)
                .ToList();
            if (unmappedDependentColumns.Any())
            {
                Reporter.WriteWarning(
                    DesignStrings.ForeignKeyScaffoldErrorPropertyNotFound(
                        foreignKey.DisplayName(),
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedDependentColumns)));
                return null;
            }

            var dependentProperties = foreignKey.Columns
                .Select(GetPropertyName)
                .Select(name => dependentEntityType.FindProperty(name))
                .ToList()
                .AsReadOnly();

            var principalEntityType = modelBuilder.Model.FindEntityType(GetEntityTypeName(foreignKey.PrincipalTable));
            if (principalEntityType == null)
            {
                Reporter.WriteWarning(
                    DesignStrings.ForeignKeyScaffoldErrorPrincipalTableScaffoldingError(
                        foreignKey.DisplayName(),
                        foreignKey.PrincipalTable.DisplayName()));
                return null;
            }

            var unmappedPrincipalColumns = foreignKey.PrincipalColumns
                .Where(pc => principalEntityType.FindProperty(GetPropertyName(pc)) == null)
                .Select(pc => pc.Name)
                .ToList();
            if (unmappedPrincipalColumns.Any())
            {
                Reporter.WriteWarning(
                    DesignStrings.ForeignKeyScaffoldErrorPropertyNotFound(
                        foreignKey.DisplayName(),
                        string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, unmappedPrincipalColumns)));
                return null;
            }

            var principalPropertiesMap = foreignKey.PrincipalColumns
                .Select(
                    fc => (property: principalEntityType.FindProperty(GetPropertyName(fc)), column: fc)).ToList();
            var principalProperties = principalPropertiesMap
                .Select(tuple => tuple.property)
                .ToList();

            var principalKey = principalEntityType.FindKey(principalProperties);
            if (principalKey == null)
            {
                var index = principalEntityType.FindIndex(principalProperties.AsReadOnly());
                if (index != null
                    && index.IsUnique)
                {
                    // ensure all principal properties are non-nullable even if the columns
                    // are nullable on the database. EF's concept of a key requires this.
                    var nullablePrincipalProperties =
                        principalPropertiesMap.Where(tuple => tuple.property.IsNullable).ToList();
                    if (nullablePrincipalProperties.Any())
                    {
                        Reporter.WriteWarning(
                            DesignStrings.ForeignKeyPrincipalEndContainsNullableColumns(
                                foreignKey.DisplayName(),
                                index.Relational().Name,
                                nullablePrincipalProperties.Select(tuple => tuple.column.DisplayName()).ToList()
                                    .Aggregate((a, b) => a + "," + b)));

                        nullablePrincipalProperties
                            .ToList()
                            .ForEach(tuple => tuple.property.IsNullable = false);
                    }
                    principalKey = principalEntityType.AddKey(principalProperties);
                }
                else
                {
                    var principalColumns = foreignKey.PrincipalColumns.Select(c => c.Name).ToList();

                    Reporter.WriteWarning(
                        DesignStrings.ForeignKeyScaffoldErrorPrincipalKeyNotFound(
                            foreignKey.DisplayName(),
                            string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, principalColumns),
                            principalEntityType.DisplayName()));

                    return null;
                }
            }

            var key = dependentEntityType.GetOrAddForeignKey(
                dependentProperties, principalKey, principalEntityType);

            var dependentKey = dependentEntityType.FindKey(dependentProperties);
            var dependentIndex = dependentEntityType.FindIndex(dependentProperties);
            key.IsUnique = dependentKey != null
                           || dependentIndex != null && dependentIndex.IsUnique;

            if (!string.IsNullOrEmpty(foreignKey.Name)
                && foreignKey.Name != ConstraintNamer.GetDefaultName(key))
            {
                key.Relational().Name = foreignKey.Name;
            }

            AssignOnDeleteAction(foreignKey, key);

            key.AddAnnotations(foreignKey.GetAnnotations());

            return key;
        }

        protected virtual void AddNavigationProperties([NotNull] IMutableForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            var dependentEndExistingIdentifiers = ExistingIdentifiers(foreignKey.DeclaringEntityType);
            var dependentEndNavigationPropertyCandidateName =
                CandidateNamingService.GetDependentEndCandidateNavigationPropertyName(foreignKey);
            var dependentEndNavigationPropertyName =
                _cSharpUtilities.GenerateCSharpIdentifier(
                    dependentEndNavigationPropertyCandidateName,
                    dependentEndExistingIdentifiers,
                    singularizePluralizer: null,
                    uniquifier: NavigationUniquifier);

            foreignKey.HasDependentToPrincipal(dependentEndNavigationPropertyName);

            var principalEndExistingIdentifiers = ExistingIdentifiers(foreignKey.PrincipalEntityType);
            var principalEndNavigationPropertyCandidateName = foreignKey.IsSelfReferencing()
                ? string.Format(
                    CultureInfo.CurrentCulture,
                    SelfReferencingPrincipalEndNavigationNamePattern,
                    dependentEndNavigationPropertyName)
                : CandidateNamingService.GetPrincipalEndCandidateNavigationPropertyName(
                    foreignKey, dependentEndNavigationPropertyName);

            if (!foreignKey.IsUnique
                && !foreignKey.IsSelfReferencing())
            {
                principalEndNavigationPropertyCandidateName = _pluralizer.Pluralize(principalEndNavigationPropertyCandidateName);
            }

            var principalEndNavigationPropertyName =
                _cSharpUtilities.GenerateCSharpIdentifier(
                    principalEndNavigationPropertyCandidateName,
                    principalEndExistingIdentifiers,
                    singularizePluralizer: null,
                    uniquifier: NavigationUniquifier);

            foreignKey.HasPrincipalToDependent(principalEndNavigationPropertyName);
        }

        // Stores the names of the EntityType itself and its Properties, but does not include any Navigation Properties
        private readonly Dictionary<IEntityType, List<string>> _entityTypeAndPropertyIdentifiers = new Dictionary<IEntityType, List<string>>();

        protected virtual List<string> ExistingIdentifiers([NotNull] IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            List<string> existingIdentifiers;
            if (!_entityTypeAndPropertyIdentifiers.TryGetValue(entityType, out existingIdentifiers))
            {
                existingIdentifiers = new List<string>();
                existingIdentifiers.Add(entityType.Name);
                existingIdentifiers.AddRange(entityType.GetProperties().Select(p => p.Name));
                _entityTypeAndPropertyIdentifiers[entityType] = existingIdentifiers;
            }

            existingIdentifiers.AddRange(entityType.GetNavigations().Select(p => p.Name));
            return existingIdentifiers;
        }

        protected virtual TypeScaffoldingInfo GetTypeScaffoldingInfo([NotNull] DatabaseColumn column)
        {
            if (column.StoreType == null)
            {
                return null;
            }

            var typeScaffoldingInfo = _scaffoldingTypeMapper.FindMapping(
                column.GetUnderlyingStoreType() ?? column.StoreType,
                column.IsKeyOrIndex(),
                column.IsRowVersion());

            if (column.GetUnderlyingStoreType() != null)
            {
                return new TypeScaffoldingInfo(
                    typeScaffoldingInfo.ClrType,
                    inferred: false,
                    scaffoldUnicode: typeScaffoldingInfo.ScaffoldUnicode,
                    scaffoldMaxLength: typeScaffoldingInfo.ScaffoldMaxLength);
            }

            return typeScaffoldingInfo;
        }

        private static void AssignOnDeleteAction(
            [NotNull] DatabaseForeignKey databaseForeignKey, [NotNull] IMutableForeignKey foreignKey)
        {
            Check.NotNull(databaseForeignKey, nameof(databaseForeignKey));
            Check.NotNull(foreignKey, nameof(foreignKey));

            switch (databaseForeignKey.OnDelete)
            {
                case ReferentialAction.Cascade:
                    foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
                    break;

                case ReferentialAction.SetNull:
                    foreignKey.DeleteBehavior = DeleteBehavior.SetNull;
                    break;

                default:
                    foreignKey.DeleteBehavior = DeleteBehavior.ClientSetNull;
                    break;
            }
        }

        // TODO use CSharpUniqueNamer
        private string NavigationUniquifier([NotNull] string proposedIdentifier, [CanBeNull] ICollection<string> existingIdentifiers)
        {
            if (existingIdentifiers == null
                || !existingIdentifiers.Contains(proposedIdentifier))
            {
                return proposedIdentifier;
            }

            var finalIdentifier =
                string.Format(CultureInfo.CurrentCulture, NavigationNameUniquifyingPattern, proposedIdentifier);
            var suffix = 1;
            while (existingIdentifiers.Contains(finalIdentifier))
            {
                finalIdentifier = proposedIdentifier + suffix;
                suffix++;
            }

            return finalIdentifier;
        }
    }
}
