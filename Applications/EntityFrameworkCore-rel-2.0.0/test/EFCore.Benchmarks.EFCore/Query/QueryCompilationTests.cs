// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Benchmarks.EFCore.Models.Orders;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Benchmarks.EFCore.Query
{
    [SqlServerRequired]
    public class QueryCompilationTests : IClassFixture<QueryCompilationTests.QueryCompilationFixture>
    {
        private readonly QueryCompilationFixture _fixture;

        public QueryCompilationTests(QueryCompilationFixture fixture)
        {
            _fixture = fixture;
        }

        [Benchmark]
        [BenchmarkVariation("Default (10 queries)")]
        public void ToList(IMetricCollector collector)
        {
            using (var context = _fixture.CreateContext())
            {
                var query = context.Products
                    .AsNoTracking();

                using (collector.StartCollection())
                {
                    for (var i = 0; i < 10; i++)
                    {
                        query.ToList();
                    }
                }

                Assert.Equal(0, query.Count());
            }
        }

        [Benchmark]
        [BenchmarkVariation("Default (10 queries)")]
        public void FilterOrderProject(IMetricCollector collector)
        {
            using (var context = _fixture.CreateContext())
            {
                var query = context.Products
                    .AsNoTracking()
                    .Where(p => p.Retail < 1000)
                    .OrderBy(p => p.Name).ThenBy(p => p.Retail)
                    .Select(
                        p => new
                        {
                            p.ProductId,
                            p.Name,
                            p.Description,
                            p.ActualStockLevel,
                            p.SKU,
                            Savings = p.Retail - p.CurrentPrice,
                            Surplus = p.ActualStockLevel - p.TargetStockLevel
                        });

                using (collector.StartCollection())
                {
                    for (var i = 0; i < 10; i++)
                    {
                        query.ToList();
                    }
                }

                Assert.Equal(0, query.Count());
            }
        }

        public class QueryCompilationFixture : OrdersFixture
        {
            private readonly IServiceProvider _noQueryCacheServiceProvider;

            public QueryCompilationFixture()
                : base("Perf_Query_Compilation", 0, 0, 0, 0)
            {
                _noQueryCacheServiceProvider = new ServiceCollection()
                    .AddEntityFrameworkSqlServer()
                    .AddSingleton<IMemoryCache, NonCachingMemoryCache>()
                    .BuildServiceProvider();
            }

            public override OrdersContext CreateContext()
                => new OrdersContext(_noQueryCacheServiceProvider, ConnectionString);

            // ReSharper disable once ClassNeverInstantiated.Local
            private class NonCachingMemoryCache : IMemoryCache
            {
                public bool TryGetValue(object key, out object value)
                {
                    value = null;
                    return false;
                }

                public ICacheEntry CreateEntry(object key) => new FakeEntry();

                private class FakeEntry : ICacheEntry
                {
                    public void Dispose()
                    {
                    }

                    public object Key { get; }
                    public object Value { get; set; }
                    public DateTimeOffset? AbsoluteExpiration { get; set; }
                    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
                    public TimeSpan? SlidingExpiration { get; set; }
                    public IList<IChangeToken> ExpirationTokens { get; }
                    public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; }
                    public CacheItemPriority Priority { get; set; }
                    public long? Size { get; set; }
                }

                public void Remove(object key)
                {
                }

                public void Dispose()
                {
                }
            }
        }
    }
}
