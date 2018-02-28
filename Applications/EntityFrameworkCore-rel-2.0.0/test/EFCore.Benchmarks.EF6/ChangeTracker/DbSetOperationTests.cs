// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Benchmarks.EF6.Models.Orders;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Benchmarks.EF6.ChangeTracker
{
    [SqlServerRequired]
    [SkipForNonBenchmarkTestRuns("Test takes a long time to execute, only run during benchmark collection runs.")]
    public class DbSetOperationTests : IClassFixture<DbSetOperationTests.DbSetOperationFixture>
    {
        private readonly DbSetOperationFixture _fixture;

        public DbSetOperationTests(DbSetOperationFixture fixture)
        {
            _fixture = fixture;
        }

        [Benchmark]
        [BenchmarkVariation("AutoDetectChanges On", iterations: 1, data: new object[] { true })]
        [BenchmarkVariation("AutoDetectChanges Off", false)]
        public void Add(IMetricCollector collector, bool autoDetectChanges)
        {
            using (var context = _fixture.CreateContext())
            {
                context.Configuration.AutoDetectChangesEnabled = autoDetectChanges;

                var customers = _fixture.CreateCustomers(20000, setPrimaryKeys: false);

                using (collector.StartCollection())
                {
                    foreach (var customer in customers)
                    {
                        context.Customers.Add(customer);
                    }
                }
            }
        }

        [Benchmark]
        [BenchmarkVariation("AutoDetectChanges On", iterations: 1, data: new object[] { true })]
        [BenchmarkVariation("AutoDetectChanges Off", false)]
        public void AddRange(IMetricCollector collector, bool autoDetectChanges)
        {
            using (var context = _fixture.CreateContext())
            {
                context.Configuration.AutoDetectChangesEnabled = autoDetectChanges;

                var customers = _fixture.CreateCustomers(20000, setPrimaryKeys: false);

                using (collector.StartCollection())
                {
                    context.Customers.AddRange(customers);
                }
            }
        }

        [Benchmark]
        [BenchmarkVariation("AutoDetectChanges On", iterations: 1, data: new object[] { true })]
        [BenchmarkVariation("AutoDetectChanges Off", false)]
        public void Attach(IMetricCollector collector, bool autoDetectChanges)
        {
            using (var context = _fixture.CreateContext())
            {
                context.Configuration.AutoDetectChangesEnabled = autoDetectChanges;

                var customers = _fixture.CreateCustomers(20000, setPrimaryKeys: true);

                using (collector.StartCollection())
                {
                    foreach (var customer in customers)
                    {
                        context.Customers.Attach(customer);
                    }
                }
            }
        }

        // Note: AttachRange() not implemented because there is no
        //       API for bulk attach in EF6.x

        [Benchmark]
        [BenchmarkVariation("AutoDetectChanges On", iterations: 1, data: new object[] { true })]
        [BenchmarkVariation("AutoDetectChanges Off", false)]
        public void Remove(IMetricCollector collector, bool autoDetectChanges)
        {
            using (var context = _fixture.CreateContext())
            {
                context.Configuration.AutoDetectChangesEnabled = autoDetectChanges;

                var customers = _fixture.CreateCustomers(20000, setPrimaryKeys: true);
                customers.ForEach(c => context.Customers.Attach(c));

                using (collector.StartCollection())
                {
                    foreach (var customer in customers)
                    {
                        context.Customers.Remove(customer);
                    }
                }
            }
        }

        [Benchmark]
        [BenchmarkVariation("AutoDetectChanges On", iterations: 1, data: new object[] { true })]
        [BenchmarkVariation("AutoDetectChanges Off", false)]
        public void RemoveRange(IMetricCollector collector, bool autoDetectChanges)
        {
            using (var context = _fixture.CreateContext())
            {
                context.Configuration.AutoDetectChangesEnabled = autoDetectChanges;

                var customers = _fixture.CreateCustomers(20000, setPrimaryKeys: true);
                customers.ForEach(c => context.Customers.Attach(c));

                using (collector.StartCollection())
                {
                    context.Customers.RemoveRange(customers);
                }
            }
        }

        [Benchmark]
        [BenchmarkVariation("AutoDetectChanges On", iterations: 1, data: new object[] { true })]
        [BenchmarkVariation("AutoDetectChanges Off", false)]
        public void Update(IMetricCollector collector, bool autoDetectChanges)
        {
            using (var context = _fixture.CreateContext())
            {
                context.Configuration.AutoDetectChangesEnabled = autoDetectChanges;

                var customers = _fixture.CreateCustomers(20000, setPrimaryKeys: true);
                customers.ForEach(c => context.Customers.Attach(c));

                using (collector.StartCollection())
                {
                    foreach (var customer in customers)
                    {
                        context.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }
        }

        // Note: UpdateRange() not implemented because there is no
        //       API for bulk update in EF6.x

        public class DbSetOperationFixture : OrdersFixture
        {
            public DbSetOperationFixture()
                : base("Perf_ChangeTracker_DbSetOperation_EF6", 0, 0, 0, 0)
            {
            }
        }
    }
}
