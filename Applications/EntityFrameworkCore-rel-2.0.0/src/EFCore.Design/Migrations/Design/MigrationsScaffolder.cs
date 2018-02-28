// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Migrations.Design
{
    public class MigrationsScaffolder
    {
        private readonly Type _contextType;
        private readonly string _activeProvider;

        public MigrationsScaffolder([NotNull] MigrationsScaffolderDependencies dependencies)
        {
            Check.NotNull(dependencies, nameof(dependencies));

            _contextType = dependencies.CurrentDbContext.Context.GetType();
            _activeProvider = dependencies.DatabaseProvider.Name;
            Dependencies = dependencies;
        }

        protected virtual MigrationsScaffolderDependencies Dependencies { get; }

        public virtual ScaffoldedMigration ScaffoldMigration(
            [NotNull] string migrationName,
            [NotNull] string rootNamespace,
            [CanBeNull] string subNamespace = null)
        {
            Check.NotEmpty(migrationName, nameof(migrationName));
            Check.NotEmpty(rootNamespace, nameof(rootNamespace));

            if (Dependencies.MigrationsAssembly.FindMigrationId(migrationName) != null)
            {
                throw new OperationException(DesignStrings.DuplicateMigrationName(migrationName));
            }

            var subNamespaceDefaulted = false;
            if (string.IsNullOrEmpty(subNamespace))
            {
                subNamespaceDefaulted = true;
                subNamespace = "Migrations";
            }

            var lastMigration = Dependencies.MigrationsAssembly.Migrations.LastOrDefault();

            var migrationNamespace = rootNamespace + "." + subNamespace;
            if (subNamespaceDefaulted)
            {
                migrationNamespace = GetNamespace(lastMigration.Value?.AsType(), migrationNamespace);
            }

            var sanitizedContextName = _contextType.Name;
            var genericMarkIndex = sanitizedContextName.IndexOf('`');
            if (genericMarkIndex != -1)
            {
                sanitizedContextName = sanitizedContextName.Substring(0, genericMarkIndex);
            }

            if (ContainsForeignMigrations(migrationNamespace))
            {
                if (subNamespaceDefaulted)
                {
                    var builder = new StringBuilder()
                        .Append(rootNamespace)
                        .Append(".Migrations.");

                    if (sanitizedContextName.EndsWith("Context", StringComparison.Ordinal))
                    {
                        builder.Append(sanitizedContextName.Substring(0, sanitizedContextName.Length - 7));
                    }
                    else
                    {
                        builder
                            .Append(sanitizedContextName)
                            .Append("Migrations");
                    }

                    migrationNamespace = builder.ToString();
                }
                else
                {
                    Dependencies.OperationReporter.WriteWarning(DesignStrings.ForeignMigrations(migrationNamespace));
                }
            }

            var modelSnapshot = Dependencies.MigrationsAssembly.ModelSnapshot;
            var lastModel = Dependencies.SnapshotModelProcessor.Process(modelSnapshot?.Model);
            var upOperations = Dependencies.MigrationsModelDiffer.GetDifferences(lastModel, Dependencies.Model);
            var downOperations = upOperations.Any()
                ? Dependencies.MigrationsModelDiffer.GetDifferences(Dependencies.Model, lastModel)
                : new List<MigrationOperation>();
            var migrationId = Dependencies.MigrationsIdGenerator.GenerateId(migrationName);
            var modelSnapshotNamespace = GetNamespace(modelSnapshot?.GetType(), migrationNamespace);

            var modelSnapshotName = sanitizedContextName + "ModelSnapshot";
            if (modelSnapshot != null)
            {
                var lastModelSnapshotName = modelSnapshot.GetType().Name;
                if (lastModelSnapshotName != modelSnapshotName)
                {
                    Dependencies.OperationReporter.WriteVerbose(DesignStrings.ReusingSnapshotName(lastModelSnapshotName));

                    modelSnapshotName = lastModelSnapshotName;
                }
            }

            if (upOperations.Any(o => o.IsDestructiveChange))
            {
                Dependencies.OperationReporter.WriteWarning(DesignStrings.DestructiveOperation);
            }

            var migrationCode = Dependencies.MigrationCodeGenerator.GenerateMigration(
                migrationNamespace,
                migrationName,
                upOperations,
                downOperations);
            var migrationMetadataCode = Dependencies.MigrationCodeGenerator.GenerateMetadata(
                migrationNamespace,
                _contextType,
                migrationName,
                migrationId,
                Dependencies.Model);
            var modelSnapshotCode = Dependencies.MigrationCodeGenerator.GenerateSnapshot(
                modelSnapshotNamespace,
                _contextType,
                modelSnapshotName,
                Dependencies.Model);

            return new ScaffoldedMigration(
                Dependencies.MigrationCodeGenerator.FileExtension,
                lastMigration.Key,
                migrationCode,
                migrationId,
                migrationMetadataCode,
                GetSubNamespace(rootNamespace, migrationNamespace),
                modelSnapshotCode,
                modelSnapshotName,
                GetSubNamespace(rootNamespace, modelSnapshotNamespace));
        }

        protected virtual string GetSubNamespace([NotNull] string rootNamespace, [NotNull] string @namespace) =>
            @namespace == rootNamespace
                ? string.Empty
                : @namespace.StartsWith(rootNamespace + '.', StringComparison.Ordinal)
                    ? @namespace.Substring(rootNamespace.Length + 1)
                    : @namespace;

        // TODO: DRY (file names)
        public virtual MigrationFiles RemoveMigration([NotNull] string projectDir, [NotNull] string rootNamespace, bool force)
        {
            Check.NotEmpty(projectDir, nameof(projectDir));
            Check.NotEmpty(rootNamespace, nameof(rootNamespace));

            var files = new MigrationFiles();

            var modelSnapshot = Dependencies.MigrationsAssembly.ModelSnapshot;
            if (modelSnapshot == null)
            {
                throw new OperationException(DesignStrings.NoSnapshot);
            }

            var language = Dependencies.MigrationCodeGenerator.FileExtension;

            IModel model = null;
            var migrations = Dependencies.MigrationsAssembly.Migrations
                .Select(m => Dependencies.MigrationsAssembly.CreateMigration(m.Value, _activeProvider))
                .ToList();
            if (migrations.Count != 0)
            {
                var migration = migrations[migrations.Count - 1];
                model = migration.TargetModel;

                if (!Dependencies.MigrationsModelDiffer.HasDifferences(model, Dependencies.SnapshotModelProcessor.Process(modelSnapshot.Model)))
                {
                    if (force)
                    {
                        Dependencies.OperationReporter.WriteWarning(DesignStrings.ForceRemoveMigration(migration.GetId()));
                    }
                    else if (Dependencies.HistoryRepository.GetAppliedMigrations().Any(
                        e => e.MigrationId.Equals(migration.GetId(), StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new OperationException(DesignStrings.RevertMigration(migration.GetId()));
                    }

                    var migrationFileName = migration.GetId() + language;
                    var migrationFile = TryGetProjectFile(projectDir, migrationFileName);
                    if (migrationFile != null)
                    {
                        Dependencies.OperationReporter.WriteInformation(DesignStrings.RemovingMigration(migration.GetId()));
                        File.Delete(migrationFile);
                        files.MigrationFile = migrationFile;
                    }
                    else
                    {
                        Dependencies.OperationReporter.WriteWarning(
                            DesignStrings.NoMigrationFile(migrationFileName, migration.GetType().ShortDisplayName()));
                    }

                    var migrationMetadataFileName = migration.GetId() + ".Designer" + language;
                    var migrationMetadataFile = TryGetProjectFile(projectDir, migrationMetadataFileName);
                    if (migrationMetadataFile != null)
                    {
                        File.Delete(migrationMetadataFile);
                        files.MetadataFile = migrationMetadataFile;
                    }
                    else
                    {
                        Dependencies.OperationReporter.WriteVerbose(
                            DesignStrings.NoMigrationMetadataFile(migrationMetadataFile));
                    }

                    model = migrations.Count > 1
                        ? migrations[migrations.Count - 2].TargetModel
                        : null;
                }
                else
                {
                    Dependencies.OperationReporter.WriteVerbose(DesignStrings.ManuallyDeleted);
                }
            }

            var modelSnapshotName = modelSnapshot.GetType().Name;
            var modelSnapshotFileName = modelSnapshotName + language;
            var modelSnapshotFile = TryGetProjectFile(projectDir, modelSnapshotFileName);
            if (model == null)
            {
                if (modelSnapshotFile != null)
                {
                    Dependencies.OperationReporter.WriteInformation(DesignStrings.RemovingSnapshot);
                    File.Delete(modelSnapshotFile);
                    files.SnapshotFile = modelSnapshotFile;
                }
                else
                {
                    Dependencies.OperationReporter.WriteWarning(
                        DesignStrings.NoSnapshotFile(
                            modelSnapshotFileName,
                            modelSnapshot.GetType().ShortDisplayName()));
                }
            }
            else
            {
                var modelSnapshotNamespace = modelSnapshot.GetType().Namespace;
                Debug.Assert(!string.IsNullOrEmpty(modelSnapshotNamespace));
                var modelSnapshotCode = Dependencies.MigrationCodeGenerator.GenerateSnapshot(
                    modelSnapshotNamespace,
                    _contextType,
                    modelSnapshotName,
                    model);

                if (modelSnapshotFile == null)
                {
                    modelSnapshotFile = Path.Combine(
                        GetDirectory(projectDir, null, GetSubNamespace(rootNamespace, modelSnapshotNamespace)),
                        modelSnapshotFileName);
                }

                Dependencies.OperationReporter.WriteInformation(DesignStrings.RevertingSnapshot);
                File.WriteAllText(modelSnapshotFile, modelSnapshotCode, Encoding.UTF8);
            }

            return files;
        }

        public virtual MigrationFiles Save(
            [NotNull] string projectDir,
            [NotNull] ScaffoldedMigration migration,
            [CanBeNull] string outputDir)
        {
            Check.NotEmpty(projectDir, nameof(projectDir));
            Check.NotNull(migration, nameof(migration));

            var lastMigrationFileName = migration.PreviousMigrationId + migration.FileExtension;
            var migrationDirectory = outputDir ?? GetDirectory(projectDir, lastMigrationFileName, migration.MigrationSubNamespace);
            var migrationFile = Path.Combine(migrationDirectory, migration.MigrationId + migration.FileExtension);
            var migrationMetadataFile = Path.Combine(migrationDirectory, migration.MigrationId + ".Designer" + migration.FileExtension);
            var modelSnapshotFileName = migration.SnapshotName + migration.FileExtension;
            var modelSnapshotDirectory = outputDir ?? GetDirectory(projectDir, modelSnapshotFileName, migration.SnapshotSubnamespace);
            var modelSnapshotFile = Path.Combine(modelSnapshotDirectory, modelSnapshotFileName);

            Dependencies.OperationReporter.WriteVerbose(DesignStrings.WritingMigration(migrationFile));
            Directory.CreateDirectory(migrationDirectory);
            File.WriteAllText(migrationFile, migration.MigrationCode, Encoding.UTF8);
            File.WriteAllText(migrationMetadataFile, migration.MetadataCode, Encoding.UTF8);

            Dependencies.OperationReporter.WriteVerbose(DesignStrings.WritingSnapshot(modelSnapshotFile));
            Directory.CreateDirectory(modelSnapshotDirectory);
            File.WriteAllText(modelSnapshotFile, migration.SnapshotCode, Encoding.UTF8);

            return new MigrationFiles
            {
                MigrationFile = migrationFile,
                MetadataFile = migrationMetadataFile,
                SnapshotFile = modelSnapshotFile
            };
        }

        protected virtual string GetNamespace([CanBeNull] Type siblingType, [NotNull] string defaultNamespace)
        {
            if (siblingType != null)
            {
                var lastNamespace = siblingType.Namespace;
                if (lastNamespace != defaultNamespace)
                {
                    Dependencies.OperationReporter.WriteVerbose(DesignStrings.ReusingNamespace(siblingType.ShortDisplayName()));

                    return lastNamespace;
                }
            }

            return defaultNamespace;
        }

        protected virtual string GetDirectory(
            [NotNull] string projectDir,
            [CanBeNull] string siblingFileName,
            [NotNull] string subnamespace)
        {
            Check.NotEmpty(projectDir, nameof(projectDir));
            Check.NotNull(subnamespace, nameof(subnamespace));

            var defaultDirectory = Path.Combine(projectDir, Path.Combine(subnamespace.Split('.')));

            if (siblingFileName != null)
            {
                var siblingPath = TryGetProjectFile(projectDir, siblingFileName);
                if (siblingPath != null)
                {
                    var lastDirectory = Path.GetDirectoryName(siblingPath);
                    if (!defaultDirectory.Equals(lastDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        Dependencies.OperationReporter.WriteVerbose(DesignStrings.ReusingNamespace(siblingFileName));

                        return lastDirectory;
                    }
                }
            }

            return defaultDirectory;
        }

        protected virtual string TryGetProjectFile([NotNull] string projectDir, [NotNull] string fileName) =>
            Directory.EnumerateFiles(projectDir, fileName, SearchOption.AllDirectories).FirstOrDefault();

        private bool ContainsForeignMigrations(string migrationsNamespace)
            => (from t in Dependencies.MigrationsAssembly.Assembly.GetConstructableTypes()
                where t.Namespace == migrationsNamespace
                      && t.IsSubclassOf(typeof(Migration))
                let contextTypeAttribute = t.GetCustomAttribute<DbContextAttribute>()
                where contextTypeAttribute != null
                      && contextTypeAttribute.ContextType != _contextType
                select t).Any();
    }
}
