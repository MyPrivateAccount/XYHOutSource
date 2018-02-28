// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Benchmarks.EFCore.Models.Orders;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Benchmarks.EFCore.UpdatePipeline
{
    [SqlServerRequired]
    public class SimpleUpdatePipelineTests : IClassFixture<SimpleUpdatePipelineTests.SimpleUpdatePipelineFixture>
    {
        private readonly SimpleUpdatePipelineFixture _fixture;

        public SimpleUpdatePipelineTests(SimpleUpdatePipelineFixture fixture)
        {
            _fixture = fixture;
        }

        [Benchmark]
        [BenchmarkVariation("Sync - Batching Off", true, false)]
        [BenchmarkVariation("Sync", false, false)]
        [BenchmarkVariation("Async - Batching Off", true, true)]
        [BenchmarkVariation("Async", false, true)]
        public async Task Insert(IMetricCollector collector, bool disableBatching, bool async)
        {
            using (var context = _fixture.CreateContext(disableBatching))
            {
                using (context.Database.BeginTransaction())
                {
                    var customers = _fixture.CreateCustomers(1000, setPrimaryKeys: false);
                    context.Customers.AddRange(customers);

                    collector.StartCollection();
                    var records = async 
                        ? await context.SaveChangesAsync() 
                        : context.SaveChanges();
                    collector.StopCollection();

                    Assert.Equal(1000, records);
                }
            }
        }

        [Benchmark]
        [BenchmarkVariation("Sync - Batching Off", true, false)]
        [BenchmarkVariation("Sync", false, false)]
        [BenchmarkVariation("Async - Batching Off", true, true)]
        [BenchmarkVariation("Async", false, true)]
        public async Task Update(IMetricCollector collector, bool disableBatching, bool async)
        {
            using (var context = _fixture.CreateContext(disableBatching))
            {
                using (context.Database.BeginTransaction())
                {
                    foreach (var customer in context.Customers)
                    {
                        customer.FirstName += " Modified";
                    }

                    collector.StartCollection();
                    var records = async
                        ? await context.SaveChangesAsync()
                        : context.SaveChanges();
                    collector.StopCollection();

                    Assert.Equal(1000, records);
                }
            }
        }

        [Benchmark]
        [BenchmarkVariation("Sync - Batching Off", true, false)]
        [BenchmarkVariation("Sync", false, false)]
        [BenchmarkVariation("Async - Batching Off", true, true)]
        [BenchmarkVariation("Async", false, true)]
        public async Task Delete(IMetricCollector collector, bool disableBatching, bool async)
        {
            using (var context = _fixture.CreateContext(disableBatching))
            {
                using (context.Database.BeginTransaction())
                {
                    context.Customers.RemoveRange(context.Customers.ToList());

                    collector.StartCollection();
                    var records = async
                        ? await context.SaveChangesAsync()
                        : context.SaveChanges();
                    collector.StopCollection();

                    Assert.Equal(1000, records);
                }
            }
        }

        [Benchmark]
        [BenchmarkVariation("Sync - Batching Off", true, false)]
        [BenchmarkVariation("Sync", false, false)]
        [BenchmarkVariation("Async - Batching Off", true, true)]
        [BenchmarkVariation("Async", false, true)]
        public async Task Mixed(IMetricCollector collector, bool disableBatching, bool async)
        {
            using (var context = _fixture.CreateContext(disableBatching))
            {
                using (context.Database.BeginTransaction())
                {
                    var existingCustomers = context.Customers.ToArray();

                    var newCustomers = _fixture.CreateCustomers(333, setPrimaryKeys: false);
                    context.Customers.AddRange(newCustomers);

                    for (var i = 0; i < 1000; i += 3)
                    {
                        context.Customers.Remove(existingCustomers[i]);
                    }

                    for (var i = 1; i < 1000; i += 3)
                    {
                        existingCustomers[i].FirstName += " Modified";
                    }

                    collector.StartCollection();
                    var records = async
                        ? await context.SaveChangesAsync()
                        : context.SaveChanges();
                    collector.StopCollection();

                    Assert.Equal(1000, records);
                }
            }
        }

        public class SimpleUpdatePipelineFixture : OrdersFixture
        {
            public SimpleUpdatePipelineFixture()
                : base("Perf_UpdatePipeline_Simple", 0, 1000, 0, 0)
            {
            }

            public OrdersContext CreateContext(bool disableBatching) => new OrdersContext(ConnectionString, disableBatching);
        }
    }
}
