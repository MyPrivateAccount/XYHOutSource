// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Migrations.Design
{
    public class MigrationScaffolderTest
    {
        [Fact]
        public void ScaffoldMigration_reuses_model_snapshot()
        {
            var scaffolder = CreateMigrationScaffolder<ContextWithSnapshot>();

            var migration = scaffolder.ScaffoldMigration("EmptyMigration", "WebApplication1");

            Assert.Equal(nameof(ContextWithSnapshotModelSnapshot), migration.SnapshotName);
            Assert.Equal(typeof(ContextWithSnapshotModelSnapshot).Namespace, migration.SnapshotSubnamespace);
        }

        [Fact]
        public void ScaffoldMigration_handles_generic_contexts()
        {
            var scaffolder = CreateMigrationScaffolder<GenericContext<int>>();

            var migration = scaffolder.ScaffoldMigration("EmptyMigration", "WebApplication1");

            Assert.Equal("GenericContextModelSnapshot", migration.SnapshotName);
        }

        private MigrationsScaffolder CreateMigrationScaffolder<TContext>()
            where TContext : DbContext, new()
        {
            var currentContext = new CurrentDbContext(new TContext());
            var idGenerator = new MigrationsIdGenerator();
            var code = new CSharpHelper();
            var reporter = new TestOperationReporter();

            return new MigrationsScaffolder(
                new MigrationsScaffolderDependencies(
                    currentContext,
                    new Model(),
                    new MigrationsAssembly(
                        currentContext,
                        new DbContextOptions<TContext>().WithExtension(new FakeRelationalOptionsExtension()),
                        idGenerator),
                    new MigrationsModelDiffer(
                        new TestRelationalTypeMapper(new RelationalTypeMapperDependencies()),
                        new MigrationsAnnotationProvider(new MigrationsAnnotationProviderDependencies())),
                    idGenerator,
                    new CSharpMigrationsGenerator(
                        new MigrationsCodeGeneratorDependencies(),
                        new CSharpMigrationsGeneratorDependencies(
                            code,
                            new CSharpMigrationOperationGenerator(
                                new CSharpMigrationOperationGeneratorDependencies(code)),
                            new CSharpSnapshotGenerator(new CSharpSnapshotGeneratorDependencies(code)))),
                    new MockHistoryRepository(),
                    reporter,
                    new MockProvider(),
                    new SnapshotModelProcessor(reporter)));
        }

        private class GenericContext<T> : DbContext
        {
        }

        private class ContextWithSnapshot : DbContext
        {
        }

        [DbContext(typeof(ContextWithSnapshot))]
        private class ContextWithSnapshotModelSnapshot : ModelSnapshot
        {
            protected override void BuildModel(ModelBuilder modelBuilder)
            {
            }
        }

        private class MockHistoryRepository : IHistoryRepository
        {
            public string GetBeginIfExistsScript(string migrationId) => null;
            public string GetBeginIfNotExistsScript(string migrationId) => null;
            public string GetCreateScript() => null;
            public string GetCreateIfNotExistsScript() => null;
            public string GetEndIfScript() => null;
            public bool Exists() => false;
            public Task<bool> ExistsAsync(CancellationToken cancellationToken) => Task.FromResult(false);
            public IReadOnlyList<HistoryRow> GetAppliedMigrations() => null;

            public Task<IReadOnlyList<HistoryRow>> GetAppliedMigrationsAsync(CancellationToken cancellationToken)
                => Task.FromResult<IReadOnlyList<HistoryRow>>(null);

            public string GetDeleteScript(string migrationId) => null;
            public string GetInsertScript(HistoryRow row) => null;
        }

        private class MockProvider : IDatabaseProvider
        {
            public string Name => "Mock.Provider";
            public bool IsConfigured(IDbContextOptions options) => true;
        }
    }
}
