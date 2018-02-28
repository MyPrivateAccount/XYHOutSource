// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.TestModels;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    public class SharedCrossStoreFixture : CrossStoreFixture
    {
        private const string StoreName = "SharedCrossStore";

        private readonly IServiceProvider _serviceProvider;
        private Guid id = Guid.NewGuid();

        public SharedCrossStoreFixture()
            : this(new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider())
        {
        }

        public SharedCrossStoreFixture(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override TestStore CreateTestStore(Type testStoreType)
        {
            if (testStoreType == typeof(InMemoryTestStore))
            {
                return new InMemoryTestStore();
            }

            if (testStoreType == typeof(SqlServerTestStore))
            {
                return SqlServerTestStore.Create(StoreName);
            }

            if (testStoreType == typeof(SqliteTestStore))
            {
                return SqliteTestStore.CreateScratch();
            }

            throw new NotImplementedException();
        }

        public override CrossStoreContext CreateContext(TestStore testStore)
        {
            var inMemoryTestStore = testStore as InMemoryTestStore;
            if (inMemoryTestStore != null)
            {
                var optionsBuilder = new DbContextOptionsBuilder()
                    .UseInMemoryDatabase(StoreName)
                    .UseInternalServiceProvider(_serviceProvider);

                return new CrossStoreContext(optionsBuilder.Options);
            }

            var sqliteTestStore = testStore as SqliteTestStore;
            if (sqliteTestStore != null)
            {
                var optionsBuilder = new DbContextOptionsBuilder();
                optionsBuilder.UseSqlite(sqliteTestStore.Connection);

                var context = new CrossStoreContext(optionsBuilder.Options);
                context.Database.EnsureCreated();
                context.Database.UseTransaction(sqliteTestStore.Transaction);

                return context;
            }

            var sqlServerTestStore = testStore as SqlServerTestStore;
            if (sqlServerTestStore != null)
            {
                var optionsBuilder = new DbContextOptionsBuilder();
                optionsBuilder.UseSqlServer(sqlServerTestStore.Connection, b => b.ApplyConfiguration());

                var context = new CrossStoreContext(optionsBuilder.Options);
                context.Database.EnsureCreated();
                context.Database.UseTransaction(sqlServerTestStore.Transaction);

                return context;
            }

            throw new NotImplementedException();
        }
    }
}
