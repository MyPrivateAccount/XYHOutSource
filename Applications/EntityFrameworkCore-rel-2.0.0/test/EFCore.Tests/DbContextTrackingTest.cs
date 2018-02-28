// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public partial class DbContextTest
    {
        [Fact]
        public async Task Can_add_existing_entities_to_context_to_be_deleted()
        {
            await TrackEntitiesTest((c, e) => c.Remove(e), (c, e) => c.Remove(e), EntityState.Deleted);
        }

        [Fact]
        public async Task Can_add_new_entities_to_context_with_graph_method()
        {
            await TrackEntitiesTest((c, e) => c.Add(e), (c, e) => c.Add(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_new_entities_to_context_with_graph_method_async()
        {
            await TrackEntitiesTest((c, e) => c.AddAsync(e), (c, e) => c.AddAsync(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_existing_entities_to_context_to_be_attached_with_graph_method()
        {
            await TrackEntitiesTest((c, e) => c.Attach(e), (c, e) => c.Attach(e), EntityState.Unchanged);
        }

        [Fact]
        public async Task Can_add_existing_entities_to_context_to_be_updated_with_graph_method()
        {
            await TrackEntitiesTest((c, e) => c.Update(e), (c, e) => c.Update(e), EntityState.Modified);
        }

        private static Task TrackEntitiesTest(
            Func<DbContext, DbContextTest.Category, EntityEntry<DbContextTest.Category>> categoryAdder,
            Func<DbContext, DbContextTest.Product, EntityEntry<DbContextTest.Product>> productAdder, EntityState expectedState)
            => TrackEntitiesTest(
                (c, e) => Task.FromResult(categoryAdder(c, e)),
                (c, e) => Task.FromResult(productAdder(c, e)),
                expectedState);

        private static async Task TrackEntitiesTest(
            Func<DbContext, DbContextTest.Category, Task<EntityEntry<DbContextTest.Category>>> categoryAdder,
            Func<DbContext, DbContextTest.Product, Task<EntityEntry<DbContextTest.Product>>> productAdder, EntityState expectedState)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var relatedDependent = new DbContextTest.Product { Id = 1, Name = "Marmite", Price = 7.99m };
                var principal = new DbContextTest.Category { Id = 1, Name = "Beverages", Products = new List<DbContextTest.Product> { relatedDependent } };

                var relatedPrincipal = new DbContextTest.Category { Id = 2, Name = "Foods" };
                var dependent = new DbContextTest.Product { Id = 2, Name = "Bovril", Price = 4.99m, Category = relatedPrincipal };

                var principalEntry = await categoryAdder(context, principal);
                var dependentEntry = await productAdder(context, dependent);

                var relatedPrincipalEntry = context.Entry(relatedPrincipal);
                var relatedDependentEntry = context.Entry(relatedDependent);

                Assert.Same(principal, principalEntry.Entity);
                Assert.Same(relatedPrincipal, relatedPrincipalEntry.Entity);
                Assert.Same(relatedDependent, relatedDependentEntry.Entity);
                Assert.Same(dependent, dependentEntry.Entity);

                var expectedRelatedState = expectedState == EntityState.Deleted ? EntityState.Unchanged : expectedState;

                Assert.Same(principal, principalEntry.Entity);
                Assert.Equal(expectedState, principalEntry.State);
                Assert.Same(relatedPrincipal, relatedPrincipalEntry.Entity);
                Assert.Equal(expectedRelatedState, relatedPrincipalEntry.State);

                Assert.Same(relatedDependent, relatedDependentEntry.Entity);
                Assert.Equal(expectedRelatedState, relatedDependentEntry.State);
                Assert.Same(dependent, dependentEntry.Entity);
                Assert.Equal(expectedState, dependentEntry.State);

                Assert.Same(principalEntry.GetInfrastructure(), context.Entry(principal).GetInfrastructure());
                Assert.Same(relatedPrincipalEntry.GetInfrastructure(), context.Entry(relatedPrincipal).GetInfrastructure());
                Assert.Same(relatedDependentEntry.GetInfrastructure(), context.Entry(relatedDependent).GetInfrastructure());
                Assert.Same(dependentEntry.GetInfrastructure(), context.Entry(dependent).GetInfrastructure());
            }
        }

        [Fact]
        public async Task Can_add_multiple_new_entities_to_context()
        {
            await TrackMultipleEntitiesTest((c, e) => c.AddRange(e[0], e[1]), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_new_entities_to_context_async()
        {
            await TrackMultipleEntitiesTest((c, e) => c.AddRangeAsync(e[0], e[1]), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_to_context_to_be_attached()
        {
            await TrackMultipleEntitiesTest((c, e) => c.AttachRange(e[0], e[1]), EntityState.Unchanged);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_to_context_to_be_updated()
        {
            await TrackMultipleEntitiesTest((c, e) => c.UpdateRange(e[0], e[1]), EntityState.Modified);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_to_context_to_be_deleted()
        {
            await TrackMultipleEntitiesTest((c, e) => c.RemoveRange(e[0], e[1]), EntityState.Deleted);
        }

        private static Task TrackMultipleEntitiesTest(
            Action<DbContext, object[]> adder,
            EntityState expectedState)
            => TrackMultipleEntitiesTest(
                (c, e) =>
                    {
                        adder(c, e);
                        return Task.FromResult(0);
                    },
                expectedState);

        private static async Task TrackMultipleEntitiesTest(
            Func<DbContext, object[], Task> adder,
            EntityState expectedState)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var relatedDependent = new DbContextTest.Product { Id = 1, Name = "Marmite", Price = 7.99m };
                var principal = new DbContextTest.Category { Id = 1, Name = "Beverages", Products = new List<DbContextTest.Product> { relatedDependent } };

                var relatedPrincipal = new DbContextTest.Category { Id = 2, Name = "Foods" };
                var dependent = new DbContextTest.Product { Id = 2, Name = "Bovril", Price = 4.99m, Category = relatedPrincipal };

                await adder(context, new object[] { principal, dependent });

                Assert.Same(principal, context.Entry(principal).Entity);
                Assert.Same(relatedPrincipal, context.Entry(relatedPrincipal).Entity);
                Assert.Same(relatedDependent, context.Entry(relatedDependent).Entity);
                Assert.Same(dependent, context.Entry(dependent).Entity);

                var expectedRelatedState = expectedState == EntityState.Deleted ? EntityState.Unchanged : expectedState;

                Assert.Same(principal, context.Entry(principal).Entity);
                Assert.Equal(expectedState, context.Entry(principal).State);
                Assert.Same(relatedPrincipal, context.Entry(relatedPrincipal).Entity);
                Assert.Equal(expectedRelatedState, context.Entry(relatedPrincipal).State);

                Assert.Same(relatedDependent, context.Entry(relatedDependent).Entity);
                Assert.Equal(expectedRelatedState, context.Entry(relatedDependent).State);
                Assert.Same(dependent, context.Entry(dependent).Entity);
                Assert.Equal(expectedState, context.Entry(dependent).State);
            }
        }

        [Fact]
        public async Task Can_add_existing_entities_with_default_value_to_context_to_be_deleted()
        {
            await TrackEntitiesDefaultValueTest((c, e) => c.Remove(e), (c, e) => c.Remove(e), EntityState.Deleted);
        }

        [Fact]
        public async Task Can_add_new_entities_with_default_value_to_context_with_graph_method()
        {
            await TrackEntitiesDefaultValueTest((c, e) => c.Add(e), (c, e) => c.Add(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_new_entities_with_default_value_to_context_with_graph_method_async()
        {
            await TrackEntitiesDefaultValueTest((c, e) => c.AddAsync(e), (c, e) => c.AddAsync(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_existing_entities_with_default_value_to_context_to_be_attached_with_graph_method()
        {
            await TrackEntitiesDefaultValueTest((c, e) => c.Attach(e), (c, e) => c.Attach(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_existing_entities_with_default_value_to_context_to_be_updated_with_graph_method()
        {
            await TrackEntitiesDefaultValueTest((c, e) => c.Update(e), (c, e) => c.Update(e), EntityState.Added);
        }

        private static Task TrackEntitiesDefaultValueTest(
            Func<DbContext, DbContextTest.Category, EntityEntry<DbContextTest.Category>> categoryAdder,
            Func<DbContext, DbContextTest.Product, EntityEntry<DbContextTest.Product>> productAdder, EntityState expectedState)
            => TrackEntitiesDefaultValueTest(
                (c, e) => Task.FromResult(categoryAdder(c, e)),
                (c, e) => Task.FromResult(productAdder(c, e)),
                expectedState);

        // Issue #3890
        private static async Task TrackEntitiesDefaultValueTest(
            Func<DbContext, DbContextTest.Category, Task<EntityEntry<DbContextTest.Category>>> categoryAdder,
            Func<DbContext, DbContextTest.Product, Task<EntityEntry<DbContextTest.Product>>> productAdder, EntityState expectedState)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category1 = new DbContextTest.Category { Id = 0, Name = "Beverages" };
                var product1 = new DbContextTest.Product { Id = 0, Name = "Marmite", Price = 7.99m };

                var categoryEntry1 = await categoryAdder(context, category1);
                var productEntry1 = await productAdder(context, product1);

                Assert.Same(category1, categoryEntry1.Entity);
                Assert.Same(product1, productEntry1.Entity);

                Assert.Same(category1, categoryEntry1.Entity);
                Assert.Equal(expectedState, categoryEntry1.State);

                Assert.Same(product1, productEntry1.Entity);
                Assert.Equal(expectedState, productEntry1.State);

                Assert.Same(categoryEntry1.GetInfrastructure(), context.Entry(category1).GetInfrastructure());
                Assert.Same(productEntry1.GetInfrastructure(), context.Entry(product1).GetInfrastructure());
            }
        }

        [Fact]
        public async Task Can_add_multiple_new_entities_with_default_values_to_context()
        {
            await TrackMultipleEntitiesDefaultValuesTest((c, e) => c.AddRange(e[0]), (c, e) => c.AddRange(e[0]), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_new_entities_with_default_values_to_context_async()
        {
            await TrackMultipleEntitiesDefaultValuesTest((c, e) => c.AddRangeAsync(e[0]), (c, e) => c.AddRangeAsync(e[0]), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_with_default_values_to_context_to_be_attached()
        {
            await TrackMultipleEntitiesDefaultValuesTest((c, e) => c.AttachRange(e[0]), (c, e) => c.AttachRange(e[0]), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_with_default_values_to_context_to_be_updated()
        {
            await TrackMultipleEntitiesDefaultValuesTest((c, e) => c.UpdateRange(e[0]), (c, e) => c.UpdateRange(e[0]), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_with_default_values_to_context_to_be_deleted()
        {
            await TrackMultipleEntitiesDefaultValuesTest((c, e) => c.RemoveRange(e[0]), (c, e) => c.RemoveRange(e[0]), EntityState.Deleted);
        }

        private static Task TrackMultipleEntitiesDefaultValuesTest(
            Action<DbContext, object[]> categoryAdder,
            Action<DbContext, object[]> productAdder, EntityState expectedState)
            => TrackMultipleEntitiesDefaultValuesTest(
                (c, e) =>
                    {
                        categoryAdder(c, e);
                        return Task.FromResult(0);
                    },
                (c, e) =>
                    {
                        productAdder(c, e);
                        return Task.FromResult(0);
                    },
                expectedState);

        // Issue #3890
        private static async Task TrackMultipleEntitiesDefaultValuesTest(
            Func<DbContext, object[], Task> categoryAdder,
            Func<DbContext, object[], Task> productAdder, EntityState expectedState)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category1 = new DbContextTest.Category { Id = 0, Name = "Beverages" };
                var product1 = new DbContextTest.Product { Id = 0, Name = "Marmite", Price = 7.99m };

                await categoryAdder(context, new[] { category1 });
                await productAdder(context, new[] { product1 });

                Assert.Same(category1, context.Entry(category1).Entity);
                Assert.Same(product1, context.Entry(product1).Entity);

                Assert.Same(category1, context.Entry(category1).Entity);
                Assert.Equal(expectedState, context.Entry(category1).State);

                Assert.Same(product1, context.Entry(product1).Entity);
                Assert.Equal(expectedState, context.Entry(product1).State);
            }
        }

        [Fact]
        public void Can_add_no_new_entities_to_context()
        {
            TrackNoEntitiesTest(c => c.AddRange(), c => c.AddRange());
        }

        [Fact]
        public async Task Can_add_no_new_entities_to_context_async()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                await context.AddRangeAsync();
                await context.AddRangeAsync();
                Assert.Empty(context.ChangeTracker.Entries());
            }
        }

        [Fact]
        public void Can_add_no_existing_entities_to_context_to_be_attached()
        {
            TrackNoEntitiesTest(c => c.AttachRange(), c => c.AttachRange());
        }

        [Fact]
        public void Can_add_no_existing_entities_to_context_to_be_updated()
        {
            TrackNoEntitiesTest(c => c.UpdateRange(), c => c.UpdateRange());
        }

        [Fact]
        public void Can_add_no_existing_entities_to_context_to_be_deleted()
        {
            TrackNoEntitiesTest(c => c.RemoveRange(), c => c.RemoveRange());
        }

        private static void TrackNoEntitiesTest(Action<DbContext> categoryAdder, Action<DbContext> productAdder)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                categoryAdder(context);
                productAdder(context);
                Assert.Empty(context.ChangeTracker.Entries());
            }
        }

        [Fact]
        public async Task Can_add_existing_entities_to_context_to_be_deleted_non_generic()
        {
            await TrackEntitiesTestNonGeneric((c, e) => c.Remove(e), (c, e) => c.Remove(e), EntityState.Deleted);
        }

        [Fact]
        public async Task Can_add_new_entities_to_context_non_generic_graph()
        {
            await TrackEntitiesTestNonGeneric((c, e) => c.AddAsync(e), (c, e) => c.AddAsync(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_new_entities_to_context_non_generic_graph_async()
        {
            await TrackEntitiesTestNonGeneric((c, e) => c.Add(e), (c, e) => c.Add(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_existing_entities_to_context_to_be_attached_non_generic_graph()
        {
            await TrackEntitiesTestNonGeneric((c, e) => c.Attach(e), (c, e) => c.Attach(e), EntityState.Unchanged);
        }

        [Fact]
        public async Task Can_add_existing_entities_to_context_to_be_updated_non_generic_graph()
        {
            await TrackEntitiesTestNonGeneric((c, e) => c.Update(e), (c, e) => c.Update(e), EntityState.Modified);
        }

        private static Task TrackEntitiesTestNonGeneric(
            Func<DbContext, object, EntityEntry> categoryAdder,
            Func<DbContext, object, EntityEntry> productAdder, EntityState expectedState)
            => TrackEntitiesTestNonGeneric(
                (c, e) => Task.FromResult(categoryAdder(c, e)),
                (c, e) => Task.FromResult(productAdder(c, e)),
                expectedState);

        private static async Task TrackEntitiesTestNonGeneric(
            Func<DbContext, object, Task<EntityEntry>> categoryAdder,
            Func<DbContext, object, Task<EntityEntry>> productAdder, EntityState expectedState)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var relatedDependent = new DbContextTest.Product { Id = 1, Name = "Marmite", Price = 7.99m };
                var principal = new DbContextTest.Category { Id = 1, Name = "Beverages", Products = new List<DbContextTest.Product> { relatedDependent } };

                var relatedPrincipal = new DbContextTest.Category { Id = 2, Name = "Foods" };
                var dependent = new DbContextTest.Product { Id = 2, Name = "Bovril", Price = 4.99m, Category = relatedPrincipal };

                var principalEntry = await categoryAdder(context, principal);
                var dependentEntry = await productAdder(context, dependent);

                var relatedPrincipalEntry = context.Entry(relatedPrincipal);
                var relatedDependentEntry = context.Entry(relatedDependent);

                Assert.Same(principal, principalEntry.Entity);
                Assert.Same(relatedPrincipal, relatedPrincipalEntry.Entity);
                Assert.Same(relatedDependent, relatedDependentEntry.Entity);
                Assert.Same(dependent, dependentEntry.Entity);

                var expectedRelatedState = expectedState == EntityState.Deleted ? EntityState.Unchanged : expectedState;

                Assert.Same(principal, principalEntry.Entity);
                Assert.Equal(expectedState, principalEntry.State);
                Assert.Same(relatedPrincipal, relatedPrincipalEntry.Entity);
                Assert.Equal(expectedRelatedState, relatedPrincipalEntry.State);

                Assert.Same(relatedDependent, relatedDependentEntry.Entity);
                Assert.Equal(expectedRelatedState, relatedDependentEntry.State);
                Assert.Same(dependent, dependentEntry.Entity);
                Assert.Equal(expectedState, dependentEntry.State);

                Assert.Same(principalEntry.GetInfrastructure(), context.Entry(principal).GetInfrastructure());
                Assert.Same(relatedPrincipalEntry.GetInfrastructure(), context.Entry(relatedPrincipal).GetInfrastructure());
                Assert.Same(relatedDependentEntry.GetInfrastructure(), context.Entry(relatedDependent).GetInfrastructure());
                Assert.Same(dependentEntry.GetInfrastructure(), context.Entry(dependent).GetInfrastructure());
            }
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_to_context_to_be_deleted_Enumerable()
        {
            await TrackMultipleEntitiesTestEnumerable((c, e) => c.RemoveRange(e), EntityState.Deleted);
        }

        [Fact]
        public async Task Can_add_multiple_new_entities_to_context_Enumerable_graph()
        {
            await TrackMultipleEntitiesTestEnumerable((c, e) => c.AddRange(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_new_entities_to_context_Enumerable_graph_async()
        {
            await TrackMultipleEntitiesTestEnumerable((c, e) => c.AddRangeAsync(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_to_context_to_be_attached_Enumerable_graph()
        {
            await TrackMultipleEntitiesTestEnumerable((c, e) => c.AttachRange(e), EntityState.Unchanged);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_to_context_to_be_updated_Enumerable_graph()
        {
            await TrackMultipleEntitiesTestEnumerable((c, e) => c.UpdateRange(e), EntityState.Modified);
        }

        private static Task TrackMultipleEntitiesTestEnumerable(
            Action<DbContext, IEnumerable<object>> adder,
            EntityState expectedState)
            => TrackMultipleEntitiesTestEnumerable(
                (c, e) =>
                    {
                        adder(c, e);
                        return Task.FromResult(0);
                    },
                expectedState);

        private static async Task TrackMultipleEntitiesTestEnumerable(
            Func<DbContext, IEnumerable<object>, Task> adder,
            EntityState expectedState)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var relatedDependent = new DbContextTest.Product { Id = 1, Name = "Marmite", Price = 7.99m };
                var principal = new DbContextTest.Category { Id = 1, Name = "Beverages", Products = new List<DbContextTest.Product> { relatedDependent } };

                var relatedPrincipal = new DbContextTest.Category { Id = 2, Name = "Foods" };
                var dependent = new DbContextTest.Product { Id = 2, Name = "Bovril", Price = 4.99m, Category = relatedPrincipal };

                await adder(context, new object[] { principal, dependent });

                Assert.Same(principal, context.Entry(principal).Entity);
                Assert.Same(relatedPrincipal, context.Entry(relatedPrincipal).Entity);
                Assert.Same(relatedDependent, context.Entry(relatedDependent).Entity);
                Assert.Same(dependent, context.Entry(dependent).Entity);

                var expectedRelatedState = expectedState == EntityState.Deleted ? EntityState.Unchanged : expectedState;

                Assert.Same(principal, context.Entry(principal).Entity);
                Assert.Equal(expectedState, context.Entry(principal).State);
                Assert.Same(relatedPrincipal, context.Entry(relatedPrincipal).Entity);
                Assert.Equal(expectedRelatedState, context.Entry(relatedPrincipal).State);

                Assert.Same(relatedDependent, context.Entry(relatedDependent).Entity);
                Assert.Equal(expectedRelatedState, context.Entry(relatedDependent).State);
                Assert.Same(dependent, context.Entry(dependent).Entity);
                Assert.Equal(expectedState, context.Entry(dependent).State);
            }
        }

        [Fact]
        public async Task Can_add_existing_entities_with_default_value_to_context_to_be_deleted_non_generic()
        {
            await TrackEntitiesDefaultValuesTestNonGeneric((c, e) => c.Remove(e), (c, e) => c.Remove(e), EntityState.Deleted);
        }

        [Fact]
        public async Task Can_add_new_entities_with_default_value_to_context_non_generic_graph()
        {
            await TrackEntitiesDefaultValuesTestNonGeneric((c, e) => c.Add(e), (c, e) => c.Add(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_new_entities_with_default_value_to_context_non_generic_graph_async()
        {
            await TrackEntitiesDefaultValuesTestNonGeneric((c, e) => c.AddAsync(e), (c, e) => c.AddAsync(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_existing_entities_with_default_value_to_context_to_be_attached_non_generic_graph()
        {
            await TrackEntitiesDefaultValuesTestNonGeneric((c, e) => c.Attach(e), (c, e) => c.Attach(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_existing_entities_with_default_value_to_context_to_be_updated_non_generic_graph()
        {
            await TrackEntitiesDefaultValuesTestNonGeneric((c, e) => c.Update(e), (c, e) => c.Update(e), EntityState.Added);
        }

        private static Task TrackEntitiesDefaultValuesTestNonGeneric(
            Func<DbContext, object, EntityEntry> categoryAdder,
            Func<DbContext, object, EntityEntry> productAdder, EntityState expectedState)
            => TrackEntitiesDefaultValuesTestNonGeneric(
                (c, e) => Task.FromResult(categoryAdder(c, e)),
                (c, e) => Task.FromResult(productAdder(c, e)),
                expectedState);

        // Issue #3890
        private static async Task TrackEntitiesDefaultValuesTestNonGeneric(
            Func<DbContext, object, Task<EntityEntry>> categoryAdder,
            Func<DbContext, object, Task<EntityEntry>> productAdder, EntityState expectedState)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category1 = new DbContextTest.Category { Id = 0, Name = "Beverages" };
                var product1 = new DbContextTest.Product { Id = 0, Name = "Marmite", Price = 7.99m };

                var categoryEntry1 = await categoryAdder(context, category1);
                var productEntry1 = await productAdder(context, product1);

                Assert.Same(category1, categoryEntry1.Entity);
                Assert.Same(product1, productEntry1.Entity);

                Assert.Same(category1, categoryEntry1.Entity);
                Assert.Equal(expectedState, categoryEntry1.State);

                Assert.Same(product1, productEntry1.Entity);
                Assert.Equal(expectedState, productEntry1.State);

                Assert.Same(categoryEntry1.GetInfrastructure(), context.Entry(category1).GetInfrastructure());
                Assert.Same(productEntry1.GetInfrastructure(), context.Entry(product1).GetInfrastructure());
            }
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_with_default_values_to_context_to_be_deleted_Enumerable()
        {
            await TrackMultipleEntitiesDefaultValueTestEnumerable((c, e) => c.RemoveRange(e), (c, e) => c.RemoveRange(e), EntityState.Deleted);
        }

        [Fact]
        public async Task Can_add_multiple_new_entities_with_default_values_to_context_Enumerable_graph()
        {
            await TrackMultipleEntitiesDefaultValueTestEnumerable((c, e) => c.AddRange(e), (c, e) => c.AddRange(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_new_entities_with_default_values_to_context_Enumerable_graph_async()
        {
            await TrackMultipleEntitiesDefaultValueTestEnumerable((c, e) => c.AddRangeAsync(e), (c, e) => c.AddRangeAsync(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_with_default_values_to_context_to_be_attached_Enumerable_graph()
        {
            await TrackMultipleEntitiesDefaultValueTestEnumerable((c, e) => c.AttachRange(e), (c, e) => c.AttachRange(e), EntityState.Added);
        }

        [Fact]
        public async Task Can_add_multiple_existing_entities_with_default_values_to_context_to_be_updated_Enumerable_graph()
        {
            await TrackMultipleEntitiesDefaultValueTestEnumerable((c, e) => c.UpdateRange(e), (c, e) => c.UpdateRange(e), EntityState.Added);
        }

        private static Task TrackMultipleEntitiesDefaultValueTestEnumerable(
            Action<DbContext, IEnumerable<object>> categoryAdder,
            Action<DbContext, IEnumerable<object>> productAdder, EntityState expectedState)
            => TrackMultipleEntitiesDefaultValueTestEnumerable(
                (c, e) =>
                    {
                        categoryAdder(c, e);
                        return Task.FromResult(0);
                    },
                (c, e) =>
                    {
                        productAdder(c, e);
                        return Task.FromResult(0);
                    },
                expectedState);

        // Issue #3890
        private static async Task TrackMultipleEntitiesDefaultValueTestEnumerable(
            Func<DbContext, IEnumerable<object>, Task> categoryAdder,
            Func<DbContext, IEnumerable<object>, Task> productAdder, EntityState expectedState)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category1 = new DbContextTest.Category { Id = 0, Name = "Beverages" };
                var product1 = new DbContextTest.Product { Id = 0, Name = "Marmite", Price = 7.99m };

                await categoryAdder(context, new List<DbContextTest.Category> { category1 });
                await productAdder(context, new List<DbContextTest.Product> { product1 });

                Assert.Same(category1, context.Entry(category1).Entity);
                Assert.Same(product1, context.Entry(product1).Entity);

                Assert.Same(category1, context.Entry(category1).Entity);
                Assert.Equal(expectedState, context.Entry(category1).State);

                Assert.Same(product1, context.Entry(product1).Entity);
                Assert.Equal(expectedState, context.Entry(product1).State);
            }
        }

        [Fact]
        public void Can_add_no_existing_entities_to_context_to_be_deleted_Enumerable()
        {
            TrackNoEntitiesTestEnumerable((c, e) => c.RemoveRange(e), (c, e) => c.RemoveRange(e));
        }

        [Fact]
        public void Can_add_no_new_entities_to_context_Enumerable_graph()
        {
            TrackNoEntitiesTestEnumerable((c, e) => c.AddRange(e), (c, e) => c.AddRange(e));
        }

        [Fact]
        public async Task Can_add_no_new_entities_to_context_Enumerable_graph_async()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                await context.AddRangeAsync(new HashSet<DbContextTest.Category>());
                await context.AddRangeAsync(new HashSet<DbContextTest.Product>());
                Assert.Empty(context.ChangeTracker.Entries());
            }
        }

        [Fact]
        public void Can_add_no_existing_entities_to_context_to_be_attached_Enumerable_graph()
        {
            TrackNoEntitiesTestEnumerable((c, e) => c.AttachRange(e), (c, e) => c.AttachRange(e));
        }

        [Fact]
        public void Can_add_no_existing_entities_to_context_to_be_updated_Enumerable_graph()
        {
            TrackNoEntitiesTestEnumerable((c, e) => c.UpdateRange(e), (c, e) => c.UpdateRange(e));
        }

        private static void TrackNoEntitiesTestEnumerable(
            Action<DbContext, IEnumerable<object>> categoryAdder,
            Action<DbContext, IEnumerable<object>> productAdder)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                categoryAdder(context, new HashSet<DbContextTest.Category>());
                productAdder(context, new HashSet<DbContextTest.Product>());
                Assert.Empty(context.ChangeTracker.Entries());
            }
        }

        [Theory]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, false, true)]
        [InlineData(true, false, false)]
        [InlineData(true, true, false)]
        public async Task Can_add_new_entities_to_context_with_key_generation_graph(bool attachFirst, bool useEntry, bool async)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var gu1 = new DbContextTest.TheGu { ShirtColor = "Red" };
                var gu2 = new DbContextTest.TheGu { ShirtColor = "Still Red" };

                if (attachFirst)
                {
                    context.Entry(gu1).State = EntityState.Unchanged;
                    Assert.Equal(default(Guid), gu1.Id);
                    Assert.Equal(EntityState.Unchanged, context.Entry(gu1).State);
                }

                if (async)
                {
                    Assert.Same(gu1, (await context.AddAsync(gu1)).Entity);
                    Assert.Same(gu2, (await context.AddAsync(gu2)).Entity);
                }
                else
                {
                    if (useEntry)
                    {
                        context.Entry(gu1).State = EntityState.Added;
                        context.Entry(gu2).State = EntityState.Added;
                    }
                    else
                    {
                        Assert.Same(gu1, context.Add(gu1).Entity);
                        Assert.Same(gu2, context.Add(gu2).Entity);
                    }
                }

                Assert.NotEqual(default(Guid), gu1.Id);
                Assert.NotEqual(default(Guid), gu2.Id);
                Assert.NotEqual(gu1.Id, gu2.Id);

                var categoryEntry = context.Entry(gu1);
                Assert.Same(gu1, categoryEntry.Entity);
                Assert.Equal(EntityState.Added, categoryEntry.State);

                categoryEntry = context.Entry(gu2);
                Assert.Same(gu2, categoryEntry.Entity);
                Assert.Equal(EntityState.Added, categoryEntry.State);
            }
        }

        [Fact]
        public async Task Can_use_Remove_to_change_entity_state()
        {
            await ChangeStateWithMethod((c, e) => c.Remove(e), EntityState.Detached, EntityState.Deleted);
            await ChangeStateWithMethod((c, e) => c.Remove(e), EntityState.Unchanged, EntityState.Deleted);
            await ChangeStateWithMethod((c, e) => c.Remove(e), EntityState.Deleted, EntityState.Deleted);
            await ChangeStateWithMethod((c, e) => c.Remove(e), EntityState.Modified, EntityState.Deleted);
            await ChangeStateWithMethod((c, e) => c.Remove(e), EntityState.Added, EntityState.Detached);
        }

        [Fact]
        public async Task Can_use_graph_Add_to_change_entity_state()
        {
            await ChangeStateWithMethod((c, e) => c.Add(e), EntityState.Detached, EntityState.Added);
            await ChangeStateWithMethod((c, e) => c.Add(e), EntityState.Unchanged, EntityState.Added);
            await ChangeStateWithMethod((c, e) => c.Add(e), EntityState.Deleted, EntityState.Added);
            await ChangeStateWithMethod((c, e) => c.Add(e), EntityState.Modified, EntityState.Added);
            await ChangeStateWithMethod((c, e) => c.Add(e), EntityState.Added, EntityState.Added);
        }

        [Fact]
        public async Task Can_use_graph_Add_to_change_entity_state_async()
        {
            await ChangeStateWithMethod((c, e) => c.AddAsync(e), EntityState.Detached, EntityState.Added);
            await ChangeStateWithMethod((c, e) => c.AddAsync(e), EntityState.Unchanged, EntityState.Added);
            await ChangeStateWithMethod((c, e) => c.AddAsync(e), EntityState.Deleted, EntityState.Added);
            await ChangeStateWithMethod((c, e) => c.AddAsync(e), EntityState.Modified, EntityState.Added);
            await ChangeStateWithMethod((c, e) => c.AddAsync(e), EntityState.Added, EntityState.Added);
        }

        [Fact]
        public async Task Can_use_graph_Attach_to_change_entity_state()
        {
            await ChangeStateWithMethod((c, e) => c.Attach(e), EntityState.Detached, EntityState.Unchanged);
            await ChangeStateWithMethod((c, e) => c.Attach(e), EntityState.Unchanged, EntityState.Unchanged);
            await ChangeStateWithMethod((c, e) => c.Attach(e), EntityState.Deleted, EntityState.Unchanged);
            await ChangeStateWithMethod((c, e) => c.Attach(e), EntityState.Modified, EntityState.Unchanged);
            await ChangeStateWithMethod((c, e) => c.Attach(e), EntityState.Added, EntityState.Unchanged);
        }

        [Fact]
        public async Task Can_use_graph_Update_to_change_entity_state()
        {
            await ChangeStateWithMethod((c, e) => c.Update(e), EntityState.Detached, EntityState.Modified);
            await ChangeStateWithMethod((c, e) => c.Update(e), EntityState.Unchanged, EntityState.Modified);
            await ChangeStateWithMethod((c, e) => c.Update(e), EntityState.Deleted, EntityState.Modified);
            await ChangeStateWithMethod((c, e) => c.Update(e), EntityState.Modified, EntityState.Modified);
            await ChangeStateWithMethod((c, e) => c.Update(e), EntityState.Added, EntityState.Modified);
        }

        private Task ChangeStateWithMethod(
            Action<DbContext, object> action,
            EntityState initialState,
            EntityState expectedState)
            => ChangeStateWithMethod(
                (c, e) =>
                    {
                        action(c, e);
                        return Task.FromResult(0);
                    },
                initialState,
                expectedState);

        private async Task ChangeStateWithMethod(
            Func<DbContext, object, Task> action,
            EntityState initialState,
            EntityState expectedState)
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var entity = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var entry = context.Entry(entity);

                entry.State = initialState;

                await action(context, entity);

                Assert.Equal(expectedState, entry.State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_principal_first_fully_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Attach(product);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);

                // Dependent is Unchanged here because the FK change happened before it was attached
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_dependent_first_fully_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product> { product };

                context.Attach(product);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Attach(category);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_principal_first_collection_not_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product>();

                context.Attach(category);

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Attach(product);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_dependent_first_collection_not_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product>();

                context.Attach(product);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Attach(category);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_principal_first_reference_not_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite" };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Null(product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Attach(product);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_dependent_first_reference_not_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite" };
                category.Products = new List<DbContextTest.Product> { product };

                context.Attach(product);

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Null(product.Category);
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Attach(category);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_principal_first_fully_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);

                // Dependent is Unchanged here because the FK change happened before it was attached
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_dependent_first_fully_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_principal_first_collection_not_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product>();

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_dependent_first_collection_not_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product>();

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_principal_first_reference_not_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite" };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Null(product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_dependent_first_reference_not_fixed_up()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite" };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Null(product.Category);
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_principal_first_fully_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Empty(category7.Products);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Attach(product);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Same(product, category.Products.Single());
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);

                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_dependent_first_fully_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product> { product };

                context.Attach(product);

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Attach(category);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_principal_first_collection_not_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product>();

                context.Attach(category);

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category, product.Category);
                Assert.Empty(category7.Products);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Attach(product);

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_dependent_first_collection_not_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product>();

                context.Attach(product);

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Attach(category);

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_principal_first_reference_not_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite" };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Null(product.Category);
                Assert.Empty(category7.Products);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Attach(product);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Same(product, category.Products.Single());
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_attach_with_inconsistent_FK_dependent_first_reference_not_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite" };
                category.Products = new List<DbContextTest.Product> { product };

                context.Attach(product);

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Attach(category);

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_principal_first_fully_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Empty(category7.Products);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Same(product, category.Products.Single());
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_dependent_first_fully_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_principal_first_collection_not_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product>();

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category, product.Category);
                Assert.Empty(category7.Products);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_dependent_first_collection_not_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite", Category = category };
                category.Products = new List<DbContextTest.Product>();

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Empty(category.Products);
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_principal_first_reference_not_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite" };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Null(product.Category);
                Assert.Empty(category7.Products);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Detached, context.Entry(product).State);

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Same(product, category.Products.Single());
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }

        [Fact] // Issue #1246
        public void Can_set_set_to_Unchanged_with_inconsistent_FK_dependent_first_reference_not_fixed_up_with_tracked_FK_match()
        {
            using (var context = new DbContextTest.EarlyLearningCenter(InMemoryTestHelpers.Instance.CreateServiceProvider()))
            {
                var category7 = context.Attach(new DbContextTest.Category { Id = 7, Products = new List<DbContextTest.Product>() }).Entity;

                var category = new DbContextTest.Category { Id = 1, Name = "Beverages" };
                var product = new DbContextTest.Product { Id = 1, CategoryId = 7, Name = "Marmite" };
                category.Products = new List<DbContextTest.Product> { product };

                context.Entry(product).State = EntityState.Unchanged;

                Assert.Equal(7, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category7, product.Category);
                Assert.Same(product, category7.Products.Single());
                Assert.Equal(EntityState.Detached, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);

                context.Entry(category).State = EntityState.Unchanged;

                Assert.Equal(1, product.CategoryId);
                Assert.Same(product, category.Products.Single());
                Assert.Same(category, product.Category);
                Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
                Assert.Equal(EntityState.Unchanged, context.Entry(product).State);
            }
        }
    }
}
