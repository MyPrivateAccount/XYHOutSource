// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.TestModels.ComplexNavigationsModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class ComplexNavigationsQueryInMemoryFixture : ComplexNavigationsQueryFixtureBase<InMemoryTestStore>
    {
        public const string DatabaseName = "InMemoryQueryTest";

        private readonly DbContextOptions _options;

        public ComplexNavigationsQueryInMemoryFixture()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddSingleton(TestModelSource.GetFactory(OnModelCreating))
                .AddSingleton<ILoggerFactory>(new TestLoggerFactory())
                .BuildServiceProvider(validateScopes: true);

            _options = new DbContextOptionsBuilder()
                .UseInternalServiceProvider(serviceProvider)
                .UseInMemoryDatabase(nameof(ComplexNavigationsQueryInMemoryFixture)).Options;
        }

        public override InMemoryTestStore CreateTestStore()
        {
            return InMemoryTestStore.GetOrCreateShared(DatabaseName, () =>
                {
                    using (var context = new ComplexNavigationsContext(_options))
                    {
                        ComplexNavigationsModelInitializer.Seed(context);
                    }
                });
        }

        public override ComplexNavigationsContext CreateContext(InMemoryTestStore _)
        {
            var context = new ComplexNavigationsContext(_options);

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return context;
        }
    }
}
