// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    [SqlServerConfiguredCondition]
    public class ConfigurationPatternsTest
    {
        [ConditionalFact]
        public void Can_register_multiple_context_types()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<MultipleContext1>()
                .AddDbContext<MultipleContext2>()
                .BuildServiceProvider();

            using (SqlServerTestStore.GetNorthwindStore())
            {
                using (var context = serviceProvider.GetRequiredService<MultipleContext1>())
                {
                    Assert.True(context.Customers.Any());
                }

                using (var context = serviceProvider.GetRequiredService<MultipleContext2>())
                {
                    Assert.False(context.Customers.Any());
                }
            }
        }

        [ConditionalFact]
        public void Can_register_multiple_context_types_with_default_service_provider()
        {
            using (SqlServerTestStore.GetNorthwindStore())
            {
                using (var context = new MultipleContext1(new DbContextOptions<MultipleContext1>()))
                {
                    Assert.True(context.Customers.Any());
                }

                using (var context = new MultipleContext2(new DbContextOptions<MultipleContext2>()))
                {
                    Assert.False(context.Customers.Any());
                }
            }
        }

        private class MultipleContext1 : NorthwindContextBase
        {
            private readonly DbContextOptions<MultipleContext1> _options;

            public MultipleContext1(DbContextOptions<MultipleContext1> options)
                : base(options)
            {
                _options = options;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                Assert.Same(_options, optionsBuilder.Options);

                optionsBuilder.UseSqlServer(SqlServerTestStore.NorthwindConnectionString, b => b.ApplyConfiguration());

                Assert.NotSame(_options, optionsBuilder.Options);
            }
        }

        private class MultipleContext2 : NorthwindContextBase
        {
            private readonly DbContextOptions<MultipleContext2> _options;

            public MultipleContext2(DbContextOptions<MultipleContext2> options)
                : base(options)
            {
                _options = options;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                Assert.Same(_options, optionsBuilder.Options);

                optionsBuilder.UseInMemoryDatabase(nameof(NorthwindContextBase));

                Assert.NotSame(_options, optionsBuilder.Options);
            }
        }

        [ConditionalFact]
        public void Can_select_appropriate_provider_when_multiple_registered()
        {
            var serviceProvider
                = new ServiceCollection()
                    .AddScoped<SomeService>()
                    .AddDbContext<MultipleProvidersContext>()
                    .BuildServiceProvider();

            using (SqlServerTestStore.GetNorthwindStore())
            {
                MultipleProvidersContext context1;
                MultipleProvidersContext context2;

                using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (context1 = serviceScope.ServiceProvider.GetRequiredService<MultipleProvidersContext>())
                    {
                        context1.UseSqlServer = true;

                        Assert.True(context1.Customers.Any());
                    }

                    using (var context1B = serviceScope.ServiceProvider.GetRequiredService<MultipleProvidersContext>())
                    {
                        Assert.Same(context1, context1B);
                    }

                    var someService = serviceScope.ServiceProvider.GetRequiredService<SomeService>();
                    Assert.Same(context1, someService.Context);
                }
                using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (context2 = serviceScope.ServiceProvider.GetRequiredService<MultipleProvidersContext>())
                    {
                        context2.UseSqlServer = false;

                        Assert.False(context2.Customers.Any());
                    }

                    using (var context2B = serviceScope.ServiceProvider.GetRequiredService<MultipleProvidersContext>())
                    {
                        Assert.Same(context2, context2B);
                    }

                    var someService = serviceScope.ServiceProvider.GetRequiredService<SomeService>();
                    Assert.Same(context2, someService.Context);
                }

                Assert.NotSame(context1, context2);
            }
        }

        [ConditionalFact]
        public void Can_select_appropriate_provider_when_multiple_registered_with_default_service_provider()
        {
            using (SqlServerTestStore.GetNorthwindStore())
            {
                using (var context = new MultipleProvidersContext())
                {
                    context.UseSqlServer = true;

                    Assert.True(context.Customers.Any());
                }

                using (var context = new MultipleProvidersContext())
                {
                    context.UseSqlServer = false;

                    Assert.False(context.Customers.Any());
                }
            }
        }

        private class NorthwindContextBase : DbContext
        {
            protected NorthwindContextBase()
            {
            }

            protected NorthwindContextBase(DbContextOptions options)
                : base(options)
            {
            }

            public DbSet<Customer> Customers { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Customer>(b =>
                    {
                        b.HasKey(c => c.CustomerID);
                        b.ToTable("Customers");
                    });
            }
        }

        private class Customer
        {
            public string CustomerID { get; set; }
            public string CompanyName { get; set; }
            public string Fax { get; set; }
        }

        private class MultipleProvidersContext : NorthwindContextBase
        {
            public bool UseSqlServer { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (UseSqlServer)
                {
                    optionsBuilder.UseSqlServer(SqlServerTestStore.NorthwindConnectionString, b => b.ApplyConfiguration());
                }
                else
                {
                    optionsBuilder.UseInMemoryDatabase(nameof(NorthwindContextBase));
                }
            }
        }

        private class SomeService
        {
            public SomeService(MultipleProvidersContext context)
            {
                Context = context;
            }

            public MultipleProvidersContext Context { get; }
        }

        [SqlServerConfiguredCondition]
        public class NestedContextDifferentStores
        {
            [ConditionalFact]
            public async Task Can_use_one_context_nested_inside_another_of_a_different_type()
            {
                using (SqlServerTestStore.GetNorthwindStore())
                {
                    var inMemoryServiceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                    var sqlServerServiceProvider = new ServiceCollection()
                        .AddEntityFrameworkSqlServer()
                        .BuildServiceProvider();

                    await NestedContextTest(
                        () => new BlogContext(inMemoryServiceProvider), 
                        () => new NorthwindContext(sqlServerServiceProvider));
                }
            }

            [ConditionalFact]
            public async Task Can_use_one_context_nested_inside_another_of_a_different_type_with_implicit_services()
            {
                using (SqlServerTestStore.GetNorthwindStore())
                {
                    await NestedContextTest(() => new BlogContext(), () => new NorthwindContext());
                }
            }

            private async Task NestedContextTest(Func<BlogContext> createBlogContext, Func<NorthwindContext> createNorthwindContext)
            {
                using (var context0 = createBlogContext())
                {
                    Assert.Equal(0, context0.ChangeTracker.Entries().Count());
                    var blog0 = context0.Add(new Blog { Id = 1, Name = "Giddyup" }).Entity;
                    Assert.Same(blog0, context0.ChangeTracker.Entries().Select(e => e.Entity).Single());
                    await context0.SaveChangesAsync();

                    using (var context1 = createNorthwindContext())
                    {
                        var customers1 = await context1.Customers.ToListAsync();
                        Assert.Equal(91, customers1.Count);
                        Assert.Equal(91, context1.ChangeTracker.Entries().Count());
                        Assert.Same(blog0, context0.ChangeTracker.Entries().Select(e => e.Entity).Single());

                        using (var context2 = createBlogContext())
                        {
                            Assert.Equal(0, context2.ChangeTracker.Entries().Count());
                            Assert.Same(blog0, context0.ChangeTracker.Entries().Select(e => e.Entity).Single());

                            var blog0Prime = (await context2.Blogs.ToArrayAsync()).Single();
                            Assert.Same(blog0Prime, context2.ChangeTracker.Entries().Select(e => e.Entity).Single());

                            Assert.Equal(blog0.Id, blog0Prime.Id);
                            Assert.NotSame(blog0, blog0Prime);
                        }
                    }
                }
            }

            private class BlogContext : DbContext
            {
                private readonly IServiceProvider _serviceProvider;

                public BlogContext()
                {
                }

                public BlogContext(IServiceProvider serviceProvider)
                {
                    _serviceProvider = serviceProvider;
                }

                public DbSet<Blog> Blogs { get; set; }

                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                    => optionsBuilder
                        .UseInMemoryDatabase(nameof(BlogContext))
                        .UseInternalServiceProvider(_serviceProvider);
            }

            private class Blog
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }

            private class NorthwindContext : NorthwindContextBase
            {
                private readonly IServiceProvider _serviceProvider;

                public NorthwindContext()
                {
                }

                public NorthwindContext(IServiceProvider serviceProvider)
                {
                    _serviceProvider = serviceProvider;
                }

                protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                    => optionsBuilder
                        .UseSqlServer(SqlServerTestStore.NorthwindConnectionString, b => b.ApplyConfiguration())
                        .UseInternalServiceProvider(_serviceProvider);
            }
        }
    }
}
