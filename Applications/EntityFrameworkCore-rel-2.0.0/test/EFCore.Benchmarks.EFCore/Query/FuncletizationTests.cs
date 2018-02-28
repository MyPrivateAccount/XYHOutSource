// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Benchmarks.EFCore.Models.Orders;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Benchmarks.EFCore.Query
{
    [SqlServerRequired]
    public class FuncletizationTests : IClassFixture<FuncletizationTests.FuncletizationFixture>
    {
        private const int FuncletizationIterationCount = 100;

        private readonly FuncletizationFixture _fixture;

        public FuncletizationTests(FuncletizationFixture fixture)
        {
            _fixture = fixture;
        }

        [Benchmark]
        public void NewQueryInstance(IMetricCollector collector)
        {
            using (var context = _fixture.CreateContext())
            {
                using (collector.StartCollection())
                {
                    var val = 11;
                    for (var i = 0; i < FuncletizationIterationCount; i++)
                    {
                        var result = context.Products.Where(p => p.ProductId < val).ToList();

                        Assert.Equal(10, result.Count);
                    }
                }
            }
        }

        [Benchmark]
        public void SameQueryInstance(IMetricCollector collector)
        {
            using (var context = _fixture.CreateContext())
            {
                using (collector.StartCollection())
                {
                    var val = 11;
                    var query = context.Products.Where(p => p.ProductId < val);

                    for (var i = 0; i < FuncletizationIterationCount; i++)
                    {
                        var result = query.ToList();

                        Assert.Equal(10, result.Count);
                    }
                }
            }
        }

        [Benchmark]
        public void ValueFromObject(IMetricCollector collector)
        {
            using (var context = _fixture.CreateContext())
            {
                using (collector.StartCollection())
                {
                    var valueHolder = new ValueHolder();
                    for (var i = 0; i < FuncletizationIterationCount; i++)
                    {
                        var result = context.Products.Where(p => p.ProductId < valueHolder.SecondLevelProperty).ToList();

                        Assert.Equal(10, result.Count);
                    }
                }
            }
        }

        public class ValueHolder
        {
            public int FirstLevelProperty { get; } = 11;

            public int SecondLevelProperty => FirstLevelProperty;
        }

        public class FuncletizationFixture : OrdersFixture
        {
            public FuncletizationFixture()
                : base("Perf_Query_Funcletization", 100, 0, 0, 0)
            {
            }
        }
    }
}
