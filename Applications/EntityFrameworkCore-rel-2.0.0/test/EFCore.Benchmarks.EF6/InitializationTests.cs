// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Benchmarks.EF6.Models.AdventureWorks;
using Microsoft.EntityFrameworkCore.Benchmarks.Models.AdventureWorks;
using Microsoft.EntityFrameworkCore.Benchmarks.Models.AdventureWorks.TestHelpers;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Benchmarks.EF6
{
    public class InitializationTests : IClassFixture<AdventureWorksFixture>
    {
        [Benchmark]
        [BenchmarkVariation("Warm (10000 instances)", false, 10000)]
        [BenchmarkVariation("Cold (1 instance)", true, 1)]
        public void CreateAndDisposeUnusedContext(IMetricCollector collector, bool cold, int count)
        {
            RunColdStartEnabledTest(cold, c => c.CreateAndDisposeUnusedContext(collector, count));
        }

        [Benchmark]
        [AdventureWorksDatabaseRequired]
        [BenchmarkVariation("Warm (1000 instances)", false, 1000)]
        [BenchmarkVariation("Cold (1 instance)", true, 1)]
        public void InitializeAndQuery_AdventureWorks(IMetricCollector collector, bool cold, int count)
        {
            RunColdStartEnabledTest(cold, c => c.InitializeAndQuery_AdventureWorks(collector, count));
        }

        [Benchmark]
        [AdventureWorksDatabaseRequired]
        [BenchmarkVariation("Warm (100 instances)", false, 100)]
        [BenchmarkVariation("Cold (1 instance)", true, 1)]
        public void InitializeAndSaveChanges_AdventureWorks(IMetricCollector collector, bool cold, int count)
        {
            RunColdStartEnabledTest(cold, t => t.InitializeAndSaveChanges_AdventureWorks(collector, count));
        }

        [Benchmark]
        public void BuildModel_AdventureWorks(IMetricCollector collector)
        {
            collector.StartCollection();

            var builder = new DbModelBuilder();
            AdventureWorksContext.ConfigureModel(builder);
            var model = builder.Build(new SqlConnection(AdventureWorksFixtureBase.ConnectionString));

            collector.StopCollection();

            Assert.Equal(67, model.ConceptualModel.EntityTypes.Count());
        }

        private void RunColdStartEnabledTest(bool cold, Action<ColdStartEnabledTests> test)
        {
            if (cold)
            {
                using (var sandbox = new ColdStartSandbox())
                {
                    var testClass = sandbox.CreateInstance<ColdStartEnabledTests>();
                    test(testClass);
                }
            }
            else
            {
                test(new ColdStartEnabledTests());
            }
        }

        private class ColdStartEnabledTests : MarshalByRefObject
        {
            public void CreateAndDisposeUnusedContext(IMetricCollector collector, int count)
            {
                using (collector.StartCollection())
                {
                    for (var i = 0; i < count; i++)
                    {
                        using (var context = AdventureWorksFixture.CreateContext())
                        {
                        }
                    }
                }
            }

            public void InitializeAndQuery_AdventureWorks(IMetricCollector collector, int count)
            {
                using (collector.StartCollection())
                {
                    for (var i = 0; i < count; i++)
                    {
                        using (var context = AdventureWorksFixture.CreateContext())
                        {
                            context.Department.First();
                        }
                    }
                }
            }

            public void InitializeAndSaveChanges_AdventureWorks(IMetricCollector collector, int count)
            {
                using (collector.StartCollection())
                {
                    for (var i = 0; i < count; i++)
                    {
                        using (var context = AdventureWorksFixture.CreateContext())
                        {
                            context.Currency.Add(new Currency
                            {
                                CurrencyCode = "TMP",
                                Name = "Temporary"
                            });

                            using (context.Database.BeginTransaction())
                            {
                                context.SaveChanges();

                                // Don't mesure transaction rollback
                                collector.StopCollection();
                            }
                            collector.StartCollection();
                        }
                    }
                }
            }
        }
    }
}
