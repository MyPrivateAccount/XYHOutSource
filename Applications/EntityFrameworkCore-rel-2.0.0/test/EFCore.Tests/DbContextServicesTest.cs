// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public partial class DbContextTest
    {
        [Fact]
        public void Can_log_debug_events_with_OnConfiguring()
        {
            DebugLogTest(useLoggerFactory: false, configureForDebug: false, shouldLog: true);
        }

        [Fact]
        public void Cannot_log_debug_events_with_default_UseLoggerFactory()
        {
            DebugLogTest(useLoggerFactory: true, configureForDebug: false, shouldLog: false);
        }

        [Fact]
        public void Can_log_debug_events_with_UseLoggerFactory_when_configured()
        {
            DebugLogTest(useLoggerFactory: true, configureForDebug: true, shouldLog: true);
        }

        private void DebugLogTest(bool useLoggerFactory, bool configureForDebug, bool shouldLog)
        {
            MyLoggerProvider.LogList = new List<(EventId EventId, LogLevel Level)>();

            using (var context = new InfoLogContext(useLoggerFactory, configureForDebug))
            {
                context.GetService<ILoggerFactory>().AddProvider(new MyLoggerProvider());

                var logger = context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>();

                logger.ServiceProviderCreated(new ServiceCollection().BuildServiceProvider());

                var resultQuery = MyLoggerProvider.LogList.Where(e => e.EventId.Id == CoreEventId.ServiceProviderCreated.Id);

                if (shouldLog)
                {
                    Assert.Equal(LogLevel.Debug, resultQuery.Single().Level);
                }
                else
                {
                    Assert.Empty(resultQuery);
                }
            }
        }

        private class InfoLogContext : DbContext
        {
            private readonly bool _useLoggerFactory;
            private readonly bool _configureForDebug;

            public InfoLogContext(bool useLoggerFactory, bool configureForDebug)
            {
                _useLoggerFactory = useLoggerFactory;
                _configureForDebug = configureForDebug;
            }

            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase(typeof(InfoLogContext).FullName);

                if (_useLoggerFactory)
                {
                    var externalProvider =
                        _configureForDebug
                            ? new ServiceCollection()
                                .AddLogging(b => b.SetMinimumLevel(LogLevel.Debug))
                                .BuildServiceProvider()
                            : new ServiceCollection()
                                .AddLogging()
                                .BuildServiceProvider();

                    optionsBuilder.UseLoggerFactory(externalProvider.GetService<ILoggerFactory>());
                }
                else
                {
                    var internalServiceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                    optionsBuilder.UseInternalServiceProvider(internalServiceProvider);
                }
            }
        }

        private class MyLoggerProvider : ILoggerProvider
        {
            public static IList<(EventId EventId, LogLevel Level)> LogList;

            public ILogger CreateLogger(string categoryName) => new MyLogger();

            public void Dispose()
            {
            }

            private class MyLogger : ILogger
            {
                public bool IsEnabled(LogLevel logLevel) => true;

                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                    Func<TState, Exception, string> formatter)
                    => LogList.Add((eventId, logLevel));

                public IDisposable BeginScope<TState>(TState state) => null;
            }
        }

    [Fact]
        public void Can_use_GetInfrastructure_with_inferred_generic_to_get_service_provider()
        {
            using (var context = new DbContextTest.EarlyLearningCenter())
            {
                Assert.Same(
                    context.GetService<IChangeDetector>(),
                    context.GetInfrastructure().GetService<IChangeDetector>());
            }
        }

        [Fact]
        public void Each_context_gets_new_scoped_services()
        {
            var serviceProvider = InMemoryTestHelpers.Instance.CreateServiceProvider();

            IServiceProvider contextServices;
            using (var context = new DbContextTest.EarlyLearningCenter(serviceProvider))
            {
                contextServices = ((IInfrastructure<IServiceProvider>)context).Instance;
                Assert.Same(contextServices, ((IInfrastructure<IServiceProvider>)context).Instance);
            }

            using (var context = new DbContextTest.EarlyLearningCenter(serviceProvider))
            {
                Assert.NotSame(contextServices, ((IInfrastructure<IServiceProvider>)context).Instance);
            }
        }

        [Fact]
        public void Each_context_gets_new_scoped_services_with_explicit_config()
        {
            var serviceProvider = InMemoryTestHelpers.Instance.CreateServiceProvider();

            var options = new DbContextOptionsBuilder().UseInternalServiceProvider(serviceProvider).UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            IServiceProvider contextServices;
            using (var context = new DbContext(options))
            {
                contextServices = ((IInfrastructure<IServiceProvider>)context).Instance;
                Assert.Same(contextServices, ((IInfrastructure<IServiceProvider>)context).Instance);
            }

            using (var context = new DbContext(options))
            {
                Assert.NotSame(contextServices, ((IInfrastructure<IServiceProvider>)context).Instance);
            }
        }

        [Fact]
        public void Each_context_gets_new_scoped_services_with_implicit_services_and_explicit_config()
        {
            var options = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            IServiceProvider contextServices;
            using (var context = new DbContext(options))
            {
                contextServices = ((IInfrastructure<IServiceProvider>)context).Instance;
                Assert.Same(contextServices, ((IInfrastructure<IServiceProvider>)context).Instance);
            }

            using (var context = new DbContext(options))
            {
                Assert.NotSame(contextServices, ((IInfrastructure<IServiceProvider>)context).Instance);
            }
        }

        [Fact]
        public void Default_services_are_registered_when_parameterless_constructor_used()
        {
            using (var context = new EarlyLearningCenter())
            {
                Assert.IsType<DbSetFinder>(context.GetService<IDbSetFinder>());
            }
        }

        [Fact]
        public void Default_context_scoped_services_are_registered_when_parameterless_constructor_used()
        {
            using (var context = new EarlyLearningCenter())
            {
                Assert.IsType<InternalEntityEntryFactory>(context.GetService<IInternalEntityEntryFactory>());
            }
        }

        [Fact]
        public void Can_get_singleton_service_from_scoped_configuration()
        {
            using (var context = new EarlyLearningCenter())
            {
                Assert.IsType<StateManager>(context.GetService<IStateManager>());
            }
        }

        [Fact]
        public void Can_start_with_custom_services_by_passing_in_base_service_provider()
        {
            var factory = Mock.Of<INavigationFixer>();

            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddSingleton(factory)
                .BuildServiceProvider();

            using (var context = new EarlyLearningCenter(provider))
            {
                Assert.Same(factory, context.GetService<INavigationFixer>());
            }
        }

        [Fact]
        public void Required_low_level_services_are_added_if_needed()
        {
            var serviceCollection = new ServiceCollection();
            new EntityFrameworkServicesBuilder(serviceCollection).TryAddCoreServices();
            var provider = serviceCollection.BuildServiceProvider();

            Assert.IsType<LoggerFactory>(provider.GetRequiredService<ILoggerFactory>());
        }

        [Fact]
        public void Required_low_level_services_are_not_added_if_already_present()
        {
            var serviceCollection = new ServiceCollection();
            var loggerFactory = new FakeLoggerFactory();

            serviceCollection.AddSingleton<ILoggerFactory>(loggerFactory);

            new EntityFrameworkServicesBuilder(serviceCollection).TryAddCoreServices();

            var provider = serviceCollection.BuildServiceProvider();

            Assert.Same(loggerFactory, provider.GetRequiredService<ILoggerFactory>());
        }

        [Fact]
        public void Low_level_services_can_be_replaced_after_being_added()
        {
            var serviceCollection = new ServiceCollection();
            var loggerFactory = new FakeLoggerFactory();

            new EntityFrameworkServicesBuilder(serviceCollection).TryAddCoreServices();

            serviceCollection.AddSingleton<ILoggerFactory>(loggerFactory);

            var provider = serviceCollection.BuildServiceProvider();

            Assert.Same(loggerFactory, provider.GetRequiredService<ILoggerFactory>());
        }

        [Fact]
        public void Can_replace_already_registered_service_with_new_service()
        {
            var factory = Mock.Of<INavigationFixer>();
            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddSingleton(factory)
                .BuildServiceProvider();

            using (var context = new EarlyLearningCenter(provider))
            {
                Assert.Same(factory, context.GetService<INavigationFixer>());
            }
        }

        [Fact]
        public void Can_set_known_singleton_services_using_instance_sugar()
        {
            var modelSource = (IModelSource)new FakeModelSource();

            var services = new ServiceCollection()
                .AddSingleton(modelSource);

            var provider = InMemoryTestHelpers.Instance.CreateServiceProvider(services);

            using (var context = new EarlyLearningCenter(provider))
            {
                Assert.Same(modelSource, context.GetService<IModelSource>());
            }
        }

        [Fact]
        public void Can_set_known_singleton_services_using_type_activation()
        {
            var services = new ServiceCollection()
                .AddSingleton<IModelSource, FakeModelSource>();

            var provider = InMemoryTestHelpers.Instance.CreateServiceProvider(services);

            using (var context = new EarlyLearningCenter(provider))
            {
                Assert.IsType<FakeModelSource>(context.GetService<IModelSource>());
            }
        }

        [Fact]
        public void Can_set_known_context_scoped_services_using_type_activation()
        {
            var services = new ServiceCollection()
                .AddScoped<IStateManager, FakeStateManager>();

            var provider = InMemoryTestHelpers.Instance.CreateServiceProvider(services);

            using (var context = new EarlyLearningCenter(provider))
            {
                Assert.IsType<FakeStateManager>(context.GetService<IStateManager>());
            }
        }

        [Fact]
        public void Replaced_services_are_scoped_appropriately()
        {
            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddSingleton<IModelSource, FakeModelSource>()
                .AddScoped<IStateManager, FakeStateManager>()
                .BuildServiceProvider();

            var context = new EarlyLearningCenter(provider);

            var modelSource = context.GetService<IModelSource>();

            context.Dispose();

            context = new EarlyLearningCenter(provider);

            var stateManager = context.GetService<IStateManager>();

            Assert.Same(stateManager, context.GetService<IStateManager>());

            Assert.Same(modelSource, context.GetService<IModelSource>());

            context.Dispose();

            context = new EarlyLearningCenter(provider);

            Assert.NotSame(stateManager, context.GetService<IStateManager>());

            Assert.Same(modelSource, context.GetService<IModelSource>());

            context.Dispose();
        }

        [Fact]
        public void Can_get_replaced_singleton_service_from_scoped_configuration()
        {
            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddSingleton<IEntityMaterializerSource, FakeEntityMaterializerSource>()
                .BuildServiceProvider();

            using (var context = new EarlyLearningCenter(provider))
            {
                Assert.IsType<FakeEntityMaterializerSource>(context.GetService<IEntityMaterializerSource>());
            }
        }

        private class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public List<Product> Products { get; set; }
        }

        private class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }

            public int CategoryId { get; set; }
            public Category Category { get; set; }
        }

        private class TheGu
        {
            public Guid Id { get; set; }
            public string ShirtColor { get; set; }
        }

        private class EarlyLearningCenter : DbContext
        {
            private readonly IServiceProvider _serviceProvider;

            public EarlyLearningCenter()
            {
            }

            public EarlyLearningCenter(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public EarlyLearningCenter(IServiceProvider serviceProvider, DbContextOptions options)
                : base(options)
            {
                _serviceProvider = serviceProvider;
            }

            public DbSet<Product> Products { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<TheGu> Gus { get; set; }

            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .UseInternalServiceProvider(_serviceProvider);

            protected internal override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder
                    .Entity<Category>().HasMany(e => e.Products).WithOne(e => e.Category);
            }
        }

        private class FakeEntityMaterializerSource : EntityMaterializerSource
        {
        }

        private class FakeLoggerFactory : ILoggerFactory
        {
            public ILogger CreateLogger(string name) => null;

            public void AddProvider(ILoggerProvider provider)
            {
            }

            public void Dispose()
            {
            }
        }

        private class FakeModelSource : IModelSource
        {
            public virtual IModel GetModel(DbContext context, IConventionSetBuilder conventionSetBuilder, IModelValidator validator = null)
                => new Model();
        }

        [Fact]
        public void Can_use_derived_context()
        {
            var singleton = new object[3];

            using (var context = new ConstructorTestContextWithOC1A())
            {
                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var context = new ConstructorTestContextWithOC1A())
            {
                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
            }
        }

        [Fact]
        public void Can_use_derived_context_with_external_services()
        {
            var appServiceProivder = new ServiceCollection()
                .AddLogging()
                .AddMemoryCache()
                .BuildServiceProvider();

            var loggerFactory = new WrappingLoggerFactory(appServiceProivder.GetService<ILoggerFactory>());
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            IInMemoryStoreCache singleton;

            using (var context = new ConstructorTestContextWithOC1B(loggerFactory, memoryCache))
            {
                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
                Assert.Contains(DbLoggerCategory.Infrastructure.Name, loggerFactory.CreatedLoggers);
            }

            using (var context = new ConstructorTestContextWithOC1B(loggerFactory, memoryCache))
            {
                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
            }
        }

        [Fact]
        public void Can_use_derived_context_with_options()
        {
            var options = new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var singleton = new object[3];

            using (var context = new ConstructorTestContextWithOC3A(options))
            {
                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var context = new ConstructorTestContextWithOC3A(options))
            {
                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_use_derived_context_with_options_and_external_services()
        {
            var appServiceProivder = new ServiceCollection()
                .AddLogging()
                .AddMemoryCache()
                .BuildServiceProvider();

            var loggerFactory = new WrappingLoggerFactory(appServiceProivder.GetService<ILoggerFactory>());
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            var options = new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerFactory(loggerFactory)
                .UseMemoryCache(memoryCache)
                .Options;

            IInMemoryStoreCache singleton;

            using (var context = new ConstructorTestContextWithOC3A(options))
            {
                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
                Assert.Contains(DbLoggerCategory.Infrastructure.Name, loggerFactory.CreatedLoggers);
            }

            using (var context = new ConstructorTestContextWithOC3A(options))
            {
                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_use_derived_context_controlling_internal_services()
        {
            var internalServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var singleton = new object[3];

            using (var context = new ConstructorTestContextWithOC2A(internalServiceProivder))
            {
                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                Assert.Same(singleton[0], internalServiceProivder.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], internalServiceProivder.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], internalServiceProivder.GetService<IMemoryCache>());
            }

            using (var context = new ConstructorTestContextWithOC2A(internalServiceProivder))
            {
                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
            }
        }

        [Fact]
        public void Can_use_derived_context_controlling_internal_services_with_options()
        {
            var internalServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(internalServiceProivder)
                .Options;

            var singleton = new object[3];

            using (var context = new ConstructorTestContextWithOC3A(options))
            {
                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                Assert.Same(singleton[0], internalServiceProivder.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], internalServiceProivder.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], internalServiceProivder.GetService<IMemoryCache>());
            }

            using (var context = new ConstructorTestContextWithOC3A(options))
            {
                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_use_derived_context_with_options_no_OnConfiguring()
        {
            var options = new DbContextOptionsBuilder<ConstructorTestContext1A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var singleton = new object[3];

            using (var context = new ConstructorTestContext1A(options))
            {
                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var context = new ConstructorTestContext1A(options))
            {
                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_use_derived_context_with_options_and_external_services_no_OnConfiguring()
        {
            var appServiceProivder = new ServiceCollection()
                .AddLogging()
                .AddMemoryCache()
                .BuildServiceProvider();

            var loggerFactory = new WrappingLoggerFactory(appServiceProivder.GetService<ILoggerFactory>());
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            var options = new DbContextOptionsBuilder<ConstructorTestContext1A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerFactory(loggerFactory)
                .UseMemoryCache(memoryCache)
                .Options;

            IInMemoryStoreCache singleton;

            using (var context = new ConstructorTestContext1A(options))
            {
                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
                Assert.Contains(DbLoggerCategory.Infrastructure.Name, loggerFactory.CreatedLoggers);
            }

            using (var context = new ConstructorTestContext1A(options))
            {
                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_use_derived_context_controlling_internal_services_with_options_no_OnConfiguring()
        {
            var internalServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ConstructorTestContext1A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(internalServiceProivder)
                .Options;

            var singleton = new object[3];

            using (var context = new ConstructorTestContext1A(options))
            {
                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                Assert.Same(singleton[0], internalServiceProivder.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], internalServiceProivder.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], internalServiceProivder.GetService<IMemoryCache>());
            }

            using (var context = new ConstructorTestContext1A(options))
            {
                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_use_non_derived_context_with_options()
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var singleton = new object[3];

            using (var context = new DbContext(options))
            {
                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var context = new DbContext(options))
            {
                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_use_non_derived_context_with_options_and_external_services()
        {
            var appServiceProivder = new ServiceCollection()
                .AddLogging()
                .AddMemoryCache()
                .BuildServiceProvider();

            var loggerFactory = new WrappingLoggerFactory(appServiceProivder.GetService<ILoggerFactory>());
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseLoggerFactory(loggerFactory)
                .UseMemoryCache(memoryCache)
                .Options;

            IInMemoryStoreCache singleton;

            using (var context = new DbContext(options))
            {
                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
                Assert.Contains(DbLoggerCategory.Infrastructure.Name, loggerFactory.CreatedLoggers);
            }

            using (var context = new DbContext(options))
            {
                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_use_non_derived_context_controlling_internal_services_with_options()
        {
            var internalServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(internalServiceProivder)
                .Options;

            var singleton = new object[3];

            using (var context = new DbContext(options))
            {
                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                Assert.Same(singleton[0], internalServiceProivder.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], internalServiceProivder.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], internalServiceProivder.GetService<IMemoryCache>());
            }

            using (var context = new DbContext(options))
            {
                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.Same(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_derived_context()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC1A>()
                .BuildServiceProvider();

            var singleton = new object[3];
            DbContext context1;
            DbContext context2;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context1 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC1A>();

                Assert.NotNull(singleton[0] = context1.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context1.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context1.GetService<IMemoryCache>());

                Assert.NotNull(context1.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            Assert.Throws<ObjectDisposedException>(() => context1.Model);

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context2 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC1A>();

                Assert.Same(singleton[0], context2.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context2.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context2.GetService<IMemoryCache>());
            }

            Assert.NotSame(context1, context2);
            Assert.Throws<ObjectDisposedException>(() => context2.Model);
        }

        [Fact]
        public void Can_add_derived_context_with_external_services()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC1B>()
                .BuildServiceProvider();

            var loggerFactory = appServiceProivder.GetService<ILoggerFactory>();
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            IInMemoryStoreCache singleton;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC1B>();

                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC1B>();

                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
            }
        }

        private class SomeAppService
        {
        }

        private class SomeScopedAppService
        {
        }

        [Fact]
        public void Can_add_derived_context_with_options()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC3A>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddSingleton<SomeAppService>()
                .AddScoped<SomeScopedAppService>()
                .BuildServiceProvider();

            var singleton = new object[4];
            SomeAppService appSingleton;
            SomeScopedAppService appScoped;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.NotNull(singleton[3] = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                appSingleton = context.GetService<SomeAppService>();
                appScoped = context.GetService<SomeScopedAppService>();
                Assert.NotNull(appSingleton);
                Assert.NotNull(appScoped);

                Assert.Same(appSingleton, serviceScope.ServiceProvider.GetService<SomeAppService>());
                Assert.Same(appScoped, serviceScope.ServiceProvider.GetService<SomeScopedAppService>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.NotSame(singleton[3], context.GetService<IDbContextOptions>());

                var scoped = context.GetService<SomeScopedAppService>();
                Assert.NotSame(appScoped, scoped);
                Assert.Same(scoped, serviceScope.ServiceProvider.GetService<SomeScopedAppService>());

                Assert.Same(appSingleton, context.GetService<SomeAppService>());
                Assert.Same(appSingleton, serviceScope.ServiceProvider.GetService<SomeAppService>());
            }
        }

        [Fact]
        public void Can_add_derived_context_with_options_and_external_services()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC3A>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            var loggerFactory = appServiceProivder.GetService<ILoggerFactory>();
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            IInMemoryStoreCache singleton;
            IDbContextOptions options;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.NotNull(options = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.NotSame(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_derived_context_controlling_internal_services()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ConstructorTestContextWithOC2A>()
                .BuildServiceProvider();

            var singleton = new object[3];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC2A>();

                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC2A>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
            }
        }

        [Fact]
        public void Can_add_derived_context_controlling_internal_services_with_options()
        {
            var internalServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    b => b.UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseInternalServiceProvider(internalServiceProivder))
                .BuildServiceProvider();

            var singleton = new object[4];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.NotNull(singleton[3] = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.NotSame(singleton[3], context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_derived_context_one_service_provider_with_options()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    (p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p))
                .BuildServiceProvider();

            var singleton = new object[4];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.NotNull(singleton[3] = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.NotSame(singleton[3], context.GetService<IDbContextOptions>());
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Can_add_derived_context_one_service_provider_with_options_and_external_services(bool singletonOptions)
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    (p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p),
                    ServiceLifetime.Scoped,
                    singletonOptions ? ServiceLifetime.Singleton : ServiceLifetime.Scoped)
                .BuildServiceProvider();

            var loggerFactory = appServiceProivder.GetService<ILoggerFactory>();
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            IInMemoryStoreCache singleton;
            IDbContextOptions options;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.NotNull(options = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                if (singletonOptions)
                {
                    Assert.Same(options, context.GetService<IDbContextOptions>());
                }
                else
                {
                    Assert.NotSame(options, context.GetService<IDbContextOptions>());
                }
            }
        }

        [Fact]
        public void Can_add_derived_context_with_options_no_OnConfiguring()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContext1A>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            var singleton = new object[4];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.NotNull(singleton[3] = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.NotSame(singleton[3], context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_derived_context_with_options_and_external_services_no_OnConfiguring()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContext1A>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            var loggerFactory = appServiceProivder.GetService<ILoggerFactory>();
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            IInMemoryStoreCache singleton;
            IDbContextOptions options;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.NotNull(options = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.NotSame(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_derived_context_controlling_internal_services_with_options_no_OnConfiguring()
        {
            var internalServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContext1A>(
                    b => b.UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseInternalServiceProvider(internalServiceProivder))
                .BuildServiceProvider();

            var singleton = new object[4];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.NotNull(singleton[3] = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.NotSame(singleton[3], context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_derived_context_one_provider_with_options_no_OnConfiguring()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ConstructorTestContext1A>(
                    (p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p))
                .BuildServiceProvider();

            var singleton = new object[4];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.NotNull(singleton[3] = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.NotSame(singleton[3], context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_derived_context_one_provider_with_options_and_external_services_no_OnConfiguring()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ConstructorTestContext1A>(
                    (p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p))
                .BuildServiceProvider();

            var loggerFactory = appServiceProivder.GetService<ILoggerFactory>();
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            IInMemoryStoreCache singleton;
            IDbContextOptions options;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.NotNull(options = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.NotSame(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_non_derived_context_with_options()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<DbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            var singleton = new object[4];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.NotNull(singleton[3] = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.NotSame(singleton[3], context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_non_derived_context_with_options_and_external_services()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<DbContext>(
                    (p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseMemoryCache(p.GetService<IMemoryCache>())
                        .UseLoggerFactory(p.GetService<ILoggerFactory>()))
                .BuildServiceProvider();

            var loggerFactory = appServiceProivder.GetService<ILoggerFactory>();
            var memoryCache = appServiceProivder.GetService<IMemoryCache>();

            IDbContextOptions options;
            IInMemoryStoreCache singleton;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.NotNull(singleton = context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.NotNull(options = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.Same(singleton, context.GetService<IInMemoryStoreCache>());
                Assert.Same(loggerFactory, context.GetService<ILoggerFactory>());
                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
                Assert.NotSame(options, context.GetService<IDbContextOptions>());
            }
        }

        [Fact]
        public void Can_add_non_derived_context_controlling_internal_services_with_options()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DbContext>((p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p))
                .BuildServiceProvider();

            var singleton = new object[4];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.NotNull(singleton[0] = context.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context.GetService<IMemoryCache>());
                Assert.NotNull(singleton[3] = context.GetService<IDbContextOptions>());

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());
                Assert.NotSame(singleton[3], context.GetService<IDbContextOptions>());
            }
        }


        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void Can_add_derived_context_as_singleton(bool addSingletonFirst, bool useDbContext)
        {
            var appServiceProivder = useDbContext
                ? new ServiceCollection()
                    .AddDbContext<ConstructorTestContextWithOC1A>(ServiceLifetime.Singleton)
                    .BuildServiceProvider()
                : (addSingletonFirst
                    ? new ServiceCollection()
                        .AddSingleton<ConstructorTestContextWithOC1A>()
                        .AddDbContext<ConstructorTestContextWithOC1A>()
                        .BuildServiceProvider()
                    : new ServiceCollection()
                        .AddDbContext<ConstructorTestContextWithOC1A>()
                        .AddSingleton<ConstructorTestContextWithOC1A>()
                        .BuildServiceProvider());

            var singleton = new object[3];
            DbContext context1;
            DbContext context2;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context1 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC1A>();

                Assert.NotNull(singleton[0] = context1.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context1.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context1.GetService<IMemoryCache>());

                Assert.NotNull(context1.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            Assert.NotNull(context1.Model);

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context2 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC1A>();

                Assert.Same(singleton[0], context2.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context2.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context2.GetService<IMemoryCache>());
            }

            Assert.Same(context1, context2);
            Assert.Same(context1.Model, context2.Model);
        }

        [Fact]
        public void Throws_when_used_with_parameterless_constructor_context()
        {
            var serviceCollection = new ServiceCollection();

            Assert.Equal(CoreStrings.DbContextMissingConstructor(nameof(ConstructorTestContextWithOC1A)),
                Assert.Throws<ArgumentException>(
                    () => serviceCollection.AddDbContext<ConstructorTestContextWithOC1A>(
                        _ => { })).Message);

            Assert.Equal(CoreStrings.DbContextMissingConstructor(nameof(ConstructorTestContextWithOC1A)),
                Assert.Throws<ArgumentException>(
                    () => serviceCollection.AddDbContext<ConstructorTestContextWithOC1A>(
                        (_, __) => { })).Message);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void Can_add_derived_context_as_singleton_controlling_internal_services(bool addSingletonFirst, bool useDbContext)
        {
            var appServiceProivder = useDbContext
                ? new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<ConstructorTestContextWithOC3A>(
                        (p, b) => b.UseInternalServiceProvider(p).UseInMemoryDatabase(Guid.NewGuid().ToString()),
                        ServiceLifetime.Singleton)
                    .BuildServiceProvider()
                : (addSingletonFirst
                    ? new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .AddSingleton<ConstructorTestContextWithOC3A>()
                        .AddDbContext<ConstructorTestContextWithOC3A>((p, b) => b.UseInternalServiceProvider(p).UseInMemoryDatabase(Guid.NewGuid().ToString()))
                        .BuildServiceProvider()
                    : new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .AddDbContext<ConstructorTestContextWithOC3A>((p, b) => b.UseInternalServiceProvider(p).UseInMemoryDatabase(Guid.NewGuid().ToString()))
                        .AddSingleton<ConstructorTestContextWithOC3A>()
                        .BuildServiceProvider());

            var singleton = new object[3];
            DbContext context1;
            DbContext context2;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context1 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.NotNull(singleton[0] = context1.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context1.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context1.GetService<IMemoryCache>());

                Assert.NotNull(context1.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            Assert.NotNull(context1.Model);

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context2 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Same(singleton[0], context2.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context2.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context2.GetService<IMemoryCache>());
            }

            Assert.Same(context1, context2);
            Assert.Same(context1.Model, context2.Model);
        }

        [Theory]
        [InlineData(true, false, ServiceLifetime.Scoped)]
        [InlineData(false, false, ServiceLifetime.Scoped)]
        [InlineData(true, true, ServiceLifetime.Transient)]
        [InlineData(true, true, ServiceLifetime.Scoped)]
        [InlineData(true, true, ServiceLifetime.Singleton)]
        public void Can_add_derived_context_as_transient(bool addTransientFirst, bool useDbContext, ServiceLifetime optionsLifetime)
        {
            var appServiceProivder = useDbContext
                ? new ServiceCollection()
                    .AddDbContext<ConstructorTestContextWithOC1A>(ServiceLifetime.Transient, optionsLifetime)
                    .BuildServiceProvider()
                : (addTransientFirst
                    ? new ServiceCollection()
                        .AddTransient<ConstructorTestContextWithOC1A>()
                        .AddDbContext<ConstructorTestContextWithOC1A>()
                        .BuildServiceProvider()
                    : new ServiceCollection()
                        .AddDbContext<ConstructorTestContextWithOC1A>()
                        .AddTransient<ConstructorTestContextWithOC1A>()
                        .BuildServiceProvider());

            var singleton = new object[3];
            DbContextOptions options = null;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context1 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC1A>();
                var context2 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC1A>();

                Assert.NotSame(context1, context2);

                Assert.NotNull(singleton[0] = context1.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context1.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context1.GetService<IMemoryCache>());

                Assert.NotNull(context1.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                if (useDbContext)
                {
                    options = serviceScope.ServiceProvider.GetService<DbContextOptions>();

                    if (optionsLifetime != ServiceLifetime.Transient)
                    {
                        Assert.Same(options, serviceScope.ServiceProvider.GetService<DbContextOptions>());
                    }
                    else
                    {
                        Assert.NotSame(options, serviceScope.ServiceProvider.GetService<DbContextOptions>());
                    }
                }

                context1.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context1.Model);
                Assert.NotNull(context2.Model);

                context2.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context2.Model);
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC1A>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());

                if (useDbContext)
                {
                    if (optionsLifetime == ServiceLifetime.Singleton)
                    {
                        Assert.Same(options, serviceScope.ServiceProvider.GetService<DbContextOptions>());
                    }
                    else
                    {
                        Assert.NotSame(options, serviceScope.ServiceProvider.GetService<DbContextOptions>());
                    }
                }

                context.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context.Model);
            }
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void Can_add_derived_context_as_transient_controlling_internal_services(bool addTransientFirst, bool useDbContext)
        {
            var appServiceProivder = useDbContext
                ? new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<ConstructorTestContextWithOC3A>(
                        (p, b) => b.UseInternalServiceProvider(p).UseInMemoryDatabase(Guid.NewGuid().ToString()),
                        ServiceLifetime.Transient)
                    .BuildServiceProvider()
                : (addTransientFirst
                    ? new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .AddTransient<ConstructorTestContextWithOC3A>()
                        .AddDbContext<ConstructorTestContextWithOC3A>((p, b) => b.UseInternalServiceProvider(p).UseInMemoryDatabase(Guid.NewGuid().ToString()))
                        .BuildServiceProvider()
                    : new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .AddDbContext<ConstructorTestContextWithOC3A>((p, b) => b.UseInternalServiceProvider(p).UseInMemoryDatabase(Guid.NewGuid().ToString()))
                        .AddTransient<ConstructorTestContextWithOC3A>()
                        .BuildServiceProvider());

            var singleton = new object[3];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context1 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();
                var context2 = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.NotSame(context1, context2);

                Assert.NotNull(singleton[0] = context1.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context1.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context1.GetService<IMemoryCache>());

                Assert.NotNull(context1.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                context1.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context1.Model);
                Assert.NotNull(context2.Model);

                context2.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context2.Model);
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());

                context.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context.Model);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Can_add_non_derived_context_as_singleton(bool addSingletonFirst)
        {
            var appServiceProivder = addSingletonFirst
                ? new ServiceCollection()
                    .AddSingleton<DbContext>()
                    .AddDbContext<DbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                    .BuildServiceProvider()
                : new ServiceCollection()
                    .AddDbContext<DbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                    .AddSingleton<DbContext>()
                    .BuildServiceProvider();

            var singleton = new object[3];
            DbContext context1;
            DbContext context2;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context1 = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.NotNull(singleton[0] = context1.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context1.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context1.GetService<IMemoryCache>());

                Assert.NotNull(context1.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            Assert.NotNull(context1.Model);

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context2 = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.Same(singleton[0], context2.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context2.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context2.GetService<IMemoryCache>());
            }

            Assert.Same(context1, context2);
            Assert.Same(context1.Model, context2.Model);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void Can_add_non_derived_context_as_singleton_controlling_internal_services(bool addSingletonFirst, bool addEfFirst)
        {
            var serviceCollection = new ServiceCollection();

            if (addEfFirst)
            {
                serviceCollection.AddEntityFrameworkInMemoryDatabase();
            }

            if (addSingletonFirst)
            {
                serviceCollection
                    .AddSingleton<DbContext>()
                    .AddDbContext<DbContext>((p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p));
            }
            else
            {
                serviceCollection
                    .AddDbContext<DbContext>((p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p))
                    .AddSingleton<DbContext>();
            }

            if (!addEfFirst)
            {
                serviceCollection.AddEntityFrameworkInMemoryDatabase();
            }

            var appServiceProivder = serviceCollection.BuildServiceProvider();

            var singleton = new object[3];
            DbContext context1;
            DbContext context2;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context1 = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.NotNull(singleton[0] = context1.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context1.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context1.GetService<IMemoryCache>());

                Assert.NotNull(context1.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            Assert.NotNull(context1.Model);

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                context2 = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.Same(singleton[0], context2.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context2.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context2.GetService<IMemoryCache>());
            }

            Assert.Same(context1, context2);
            Assert.Same(context1.Model, context2.Model);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Can_add_non_derived_context_as_transient(bool addTransientFirst)
        {
            var appServiceProivder = addTransientFirst
                ? new ServiceCollection()
                    .AddTransient<DbContext>()
                    .AddDbContext<DbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                    .BuildServiceProvider()
                : new ServiceCollection()
                    .AddDbContext<DbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                    .AddTransient<DbContext>()
                    .BuildServiceProvider();

            var singleton = new object[3];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context1 = serviceScope.ServiceProvider.GetService<DbContext>();
                var context2 = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.NotSame(context1, context2);

                Assert.NotNull(singleton[0] = context1.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context1.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context1.GetService<IMemoryCache>());

                Assert.NotNull(context1.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                context1.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context1.Model);
                Assert.NotNull(context2.Model);

                context2.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context2.Model);
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());

                context.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context.Model);
            }
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void Can_add_non_derived_context_as_transient_controlling_internal_services(bool addTransientFirst, bool addEfFirst)
        {
            var serviceCollection = new ServiceCollection();

            if (addEfFirst)
            {
                serviceCollection.AddEntityFrameworkInMemoryDatabase();
            }

            if (addTransientFirst)
            {
                serviceCollection
                    .AddTransient<DbContext>()
                    .AddDbContext<DbContext>((p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p));
            }
            else
            {
                serviceCollection
                    .AddDbContext<DbContext>((p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p))
                    .AddTransient<DbContext>();
            }

            if (!addEfFirst)
            {
                serviceCollection.AddEntityFrameworkInMemoryDatabase();
            }

            var appServiceProivder = serviceCollection.BuildServiceProvider();

            var singleton = new object[3];

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context1 = serviceScope.ServiceProvider.GetService<DbContext>();
                var context2 = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.NotSame(context1, context2);

                Assert.NotNull(singleton[0] = context1.GetService<IInMemoryStoreCache>());
                Assert.NotNull(singleton[1] = context1.GetService<ILoggerFactory>());
                Assert.NotNull(singleton[2] = context1.GetService<IMemoryCache>());

                Assert.NotNull(context1.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                context1.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context1.Model);
                Assert.NotNull(context2.Model);

                context2.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context2.Model);
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();

                Assert.Same(singleton[0], context.GetService<IInMemoryStoreCache>());
                Assert.Same(singleton[1], context.GetService<ILoggerFactory>());
                Assert.Same(singleton[2], context.GetService<IMemoryCache>());

                context.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context.Model);
            }
        }

        [Fact]
        public void Can_use_logger_before_context_exists_and_after_disposed()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DbContext>((p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p))
                .BuildServiceProvider();

            Assert.NotNull(appServiceProivder.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();
                _ = context.Model;

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            }

            Assert.NotNull(appServiceProivder.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
        }

        [Fact]
        public void Can_use_logger_before_context_exists_and_after_disposed_when_logger_factory_replaced()
        {
            WrappingLoggerFactory loggerFactory = null;

            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DbContext>((p, b) =>
                    b.UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseLoggerFactory(loggerFactory = new WrappingLoggerFactory(p.GetService<ILoggerFactory>())))
                .BuildServiceProvider();

            Assert.NotNull(appServiceProivder.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            Assert.Null(loggerFactory);

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();
                _ = context.Model;

                Assert.NotNull(context.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());

                Assert.Equal(1, loggerFactory.CreatedLoggers.Count(n => n == DbLoggerCategory.Infrastructure.Name));
            }

            Assert.NotNull(appServiceProivder.GetService<IDiagnosticsLogger<DbLoggerCategory.Infrastructure>>());
            Assert.Equal(1, loggerFactory.CreatedLoggers.Count(n => n == DbLoggerCategory.Infrastructure.Name));
        }

        [Fact]
        public void Can_use_memory_cache_before_context_exists_and_after_disposed()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DbContext>((p, b) => b.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(p))
                .BuildServiceProvider();

            var memoryCache = appServiceProivder.GetService<IMemoryCache>();
            Assert.NotNull(memoryCache);

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();
                _ = context.Model;

                Assert.Same(memoryCache, context.GetService<IMemoryCache>());
            }

            Assert.Same(memoryCache, appServiceProivder.GetService<IMemoryCache>());
        }

        [Fact]
        public void Can_use_memory_cache_before_context_exists_and_after_disposed_when_logger_factory_replaced()
        {
            var replacecMemoryCache = new MemoryCache(new MemoryCacheOptions());
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DbContext>((p, b) =>
                    b.UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseMemoryCache(replacecMemoryCache))
                .BuildServiceProvider();

            var memoryCache = appServiceProivder.GetService<IMemoryCache>();
            Assert.NotSame(replacecMemoryCache, memoryCache);

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DbContext>();
                _ = context.Model;

                Assert.Same(replacecMemoryCache, context.GetService<IMemoryCache>());
            }

            Assert.Same(memoryCache, appServiceProivder.GetService<IMemoryCache>());
        }

        [Fact]
        public void Throws_with_new_when_no_EF_services()
        {
            var options = new DbContextOptionsBuilder<ConstructorTestContextWithSets>()
                .UseInternalServiceProvider(new ServiceCollection().BuildServiceProvider())
                .Options;

            Assert.Equal(
                CoreStrings.NoEfServices,
                Assert.Throws<InvalidOperationException>(() => new ConstructorTestContextWithSets(options)).Message);
        }

        [Fact]
        public void Throws_with_add_when_no_EF_services()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithSets>(
                    (p, b) => b.UseInternalServiceProvider(p))
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                Assert.Equal(
                    CoreStrings.NoEfServices,
                    Assert.Throws<InvalidOperationException>(
                        () => serviceScope.ServiceProvider.GetService<ConstructorTestContextWithSets>()).Message);
            }
        }

        [Fact]
        public void Throws_with_new_when_no_EF_services_and_no_sets()
        {
            var options = new DbContextOptionsBuilder<ConstructorTestContext1A>()
                .UseInternalServiceProvider(new ServiceCollection().BuildServiceProvider())
                .Options;

            Assert.Equal(
                CoreStrings.NoEfServices,
                Assert.Throws<InvalidOperationException>(() => new ConstructorTestContext1A(options)).Message);
        }

        [Fact]
        public void Throws_with_add_when_no_EF_services_and_no_sets()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContext1A>(
                    (p, b) => b.UseInternalServiceProvider(p))
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                Assert.Equal(
                    CoreStrings.NoEfServices,
                    Assert.Throws<InvalidOperationException>(
                        () => serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>()).Message);
            }
        }

        [Fact]
        public void Throws_with_new_when_no_provider()
        {
            var serviceCollection = new ServiceCollection();
            new EntityFrameworkServicesBuilder(serviceCollection).TryAddCoreServices();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ConstructorTestContextWithSets>()
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var context = new ConstructorTestContextWithSets(options))
            {
                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_with_add_when_no_provider()
        {
            var serviceCollection = new ServiceCollection();
            new EntityFrameworkServicesBuilder(serviceCollection).TryAddCoreServices();

            var appServiceProivder = serviceCollection
                .AddDbContext<ConstructorTestContextWithSets>(
                    (p, b) => b.UseInternalServiceProvider(p))
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithSets>();

                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_with_new_when_no_provider_and_no_sets()
        {
            var serviceCollection = new ServiceCollection();
            new EntityFrameworkServicesBuilder(serviceCollection).TryAddCoreServices();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ConstructorTestContext1A>()
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            using (var context = new ConstructorTestContext1A(options))
            {
                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_with_add_when_no_provider_and_no_sets()
        {
            var serviceCollection = new ServiceCollection();
            new EntityFrameworkServicesBuilder(serviceCollection).TryAddCoreServices();

            var appServiceProivder = serviceCollection
                .AddDbContext<ConstructorTestContext1A>(
                    (p, b) => b.UseInternalServiceProvider(p))
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContext1A>();

                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_with_new_when_no_EF_services_because_parameterless_constructor()
        {
            using (var context = new ConstructorTestContextNoConfigurationWithSets())
            {
                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_with_add_when_no_EF_services_because_parameterless_constructor()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContextNoConfigurationWithSets>()
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextNoConfigurationWithSets>();

                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_with_new_when_no_EF_services_and_no_sets_because_parameterless_constructor()
        {
            using (var context = new ConstructorTestContextNoConfiguration())
            {
                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_with_add_when_no_EF_services_and_no_sets_because_parameterless_constructor()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContextNoConfiguration>()
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextNoConfiguration>();

                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Can_replace_services_in_OnConfiguring()
        {
            object replacedSingleton;
            object replacedScoped;
            object replacedProviderService;

            using (var context = new ReplaceServiceContext1())
            {
                Assert.NotNull(replacedSingleton = context.GetService<IModelCustomizer>());
                Assert.IsType<CustomModelCustomizer>(replacedSingleton);

                Assert.NotNull(replacedScoped = context.GetService<IValueGeneratorSelector>());
                Assert.IsType<CustomInMemoryValueGeneratorSelector>(replacedScoped);

                Assert.NotNull(replacedProviderService = context.GetService<IInMemoryTableFactory>());
                Assert.IsType<CustomInMemoryTableFactory>(replacedProviderService);
            }

            using (var context = new ReplaceServiceContext1())
            {
                Assert.Same(replacedSingleton, context.GetService<IModelCustomizer>());
                Assert.NotSame(replacedScoped, context.GetService<IValueGeneratorSelector>());
                Assert.Same(replacedProviderService, context.GetService<IInMemoryTableFactory>());
            }
        }

        private class ReplaceServiceContext1 : DbContext
        {
            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .ReplaceService<IModelCustomizer, CustomModelCustomizer>()
                    .ReplaceService<IValueGeneratorSelector, CustomInMemoryValueGeneratorSelector>()
                    .ReplaceService<IInMemoryTableFactory, CustomInMemoryTableFactory>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        private class CustomModelCustomizer : ModelCustomizer
        {
            public CustomModelCustomizer(ModelCustomizerDependencies dependencies)
                : base(dependencies)
            {
            }
        }

        private class CustomInMemoryValueGeneratorSelector : InMemoryValueGeneratorSelector
        {
            public CustomInMemoryValueGeneratorSelector(ValueGeneratorSelectorDependencies dependencies)
                : base(dependencies)
            {
            }
        }

        private class CustomInMemoryTableFactory : InMemoryTableFactory
        {
        }

        [Fact]
        public void Can_replace_services_in_passed_options()
        {
            var options = new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ReplaceService<IModelCustomizer, CustomModelCustomizer>()
                .ReplaceService<IValueGeneratorSelector, CustomInMemoryValueGeneratorSelector>()
                .ReplaceService<IInMemoryTableFactory, CustomInMemoryTableFactory>()
                .Options;

            object replacedSingleton;
            object replacedScoped;
            object replacedProviderService;

            using (var context = new ConstructorTestContextWithOC3A(options))
            {
                Assert.NotNull(replacedSingleton = context.GetService<IModelCustomizer>());
                Assert.IsType<CustomModelCustomizer>(replacedSingleton);

                Assert.NotNull(replacedScoped = context.GetService<IValueGeneratorSelector>());
                Assert.IsType<CustomInMemoryValueGeneratorSelector>(replacedScoped);

                Assert.NotNull(replacedProviderService = context.GetService<IInMemoryTableFactory>());
                Assert.IsType<CustomInMemoryTableFactory>(replacedProviderService);
            }

            using (var context = new ConstructorTestContextWithOC3A(options))
            {
                Assert.Same(replacedSingleton, context.GetService<IModelCustomizer>());
                Assert.NotSame(replacedScoped, context.GetService<IValueGeneratorSelector>());
                Assert.Same(replacedProviderService, context.GetService<IInMemoryTableFactory>());
            }
        }

        [Fact]
        public void Can_replace_services_using_AddDbContext()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    b => b.ReplaceService<IModelCustomizer, CustomModelCustomizer>()
                        .ReplaceService<IValueGeneratorSelector, CustomInMemoryValueGeneratorSelector>()
                        .ReplaceService<IInMemoryTableFactory, CustomInMemoryTableFactory>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            object replacedSingleton;
            object replacedScoped;
            object replacedProviderService;

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.NotNull(replacedSingleton = context.GetService<IModelCustomizer>());
                Assert.IsType<CustomModelCustomizer>(replacedSingleton);

                Assert.NotNull(replacedScoped = context.GetService<IValueGeneratorSelector>());
                Assert.IsType<CustomInMemoryValueGeneratorSelector>(replacedScoped);

                Assert.NotNull(replacedProviderService = context.GetService<IInMemoryTableFactory>());
                Assert.IsType<CustomInMemoryTableFactory>(replacedProviderService);
            }

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Same(replacedSingleton, context.GetService<IModelCustomizer>());
                Assert.NotSame(replacedScoped, context.GetService<IValueGeneratorSelector>());
                Assert.Same(replacedProviderService, context.GetService<IInMemoryTableFactory>());
            }
        }

        [Fact]
        public void Throws_replacing_services_in_OnConfiguring_when_UseInternalServiceProvider()
        {
            using (var context = new ReplaceServiceContext2())
            {
                Assert.Equal(
                    CoreStrings.InvalidReplaceService(
                        nameof(DbContextOptionsBuilder.ReplaceService), nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        private class ReplaceServiceContext2 : DbContext
        {
            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .ReplaceService<IModelCustomizer, CustomModelCustomizer>()
                    .UseInternalServiceProvider(new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider())
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        [Fact]
        public void Throws_replacing_services_in_options_when_UseInternalServiceProvider()
        {
            var options = new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider())
                .ReplaceService<IInMemoryTableFactory, CustomInMemoryTableFactory>()
                .Options;

            Assert.Equal(
                CoreStrings.InvalidReplaceService(
                    nameof(DbContextOptionsBuilder.ReplaceService), nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                Assert.Throws<InvalidOperationException>(() => new ConstructorTestContextWithOC3A(options)).Message);
        }

        [Fact]
        public void Throws_replacing_services_with_AddDbContext_when_UseInternalServiceProvider()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    (p, b) => b.ReplaceService<IInMemoryTableFactory, CustomInMemoryTableFactory>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseInternalServiceProvider(p))
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                Assert.Equal(
                    CoreStrings.InvalidReplaceService(
                        nameof(DbContextOptionsBuilder.ReplaceService), nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(
                        () => serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>()).Message);
            }
        }

        [Fact]
        public void Throws_setting_LoggerFactory_in_OnConfiguring_when_UseInternalServiceProvider()
        {
            using (var context = new SetLoggerFactoryContext())
            {
                Assert.Equal(
                    CoreStrings.InvalidUseService(
                        nameof(DbContextOptionsBuilder.UseLoggerFactory),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider),
                        nameof(ILoggerFactory)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        private class SetLoggerFactoryContext : DbContext
        {
            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseLoggerFactory(new FakeLoggerFactory())
                    .UseInternalServiceProvider(new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider())
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        [Fact]
        public void Throws_setting_LoggerFactory_in_options_when_UseInternalServiceProvider()
        {
            var options = new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider())
                .UseLoggerFactory(new FakeLoggerFactory())
                .Options;

            Assert.Equal(
                CoreStrings.InvalidUseService(
                    nameof(DbContextOptionsBuilder.UseLoggerFactory),
                    nameof(DbContextOptionsBuilder.UseInternalServiceProvider),
                    nameof(ILoggerFactory)),
                Assert.Throws<InvalidOperationException>(() => new ConstructorTestContextWithOC3A(options)).Message);
        }

        [Fact]
        public void Throws_setting_LoggerFactory_with_AddDbContext_when_UseInternalServiceProvider()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    (p, b) => b.UseLoggerFactory(new FakeLoggerFactory())
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseInternalServiceProvider(p))
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                Assert.Equal(
                    CoreStrings.InvalidUseService(
                        nameof(DbContextOptionsBuilder.UseLoggerFactory),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider),
                        nameof(ILoggerFactory)),
                    Assert.Throws<InvalidOperationException>(
                        () => serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>()).Message);
            }
        }

        [Fact]
        public void Throws_setting_MemoryCache_in_OnConfiguring_when_UseInternalServiceProvider()
        {
            using (var context = new SetMemoryCacheContext())
            {
                Assert.Equal(
                    CoreStrings.InvalidUseService(
                        nameof(DbContextOptionsBuilder.UseMemoryCache),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider),
                        nameof(IMemoryCache)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        private class SetMemoryCacheContext : DbContext
        {
            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .UseMemoryCache(new FakeMemoryCache())
                    .UseInternalServiceProvider(new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider())
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        [Fact]
        public void Throws_setting_MemoryCache_in_options_when_UseInternalServiceProvider()
        {
            var options = new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider())
                .UseMemoryCache(new FakeMemoryCache())
                .Options;

            Assert.Equal(
                CoreStrings.InvalidUseService(
                    nameof(DbContextOptionsBuilder.UseMemoryCache),
                    nameof(DbContextOptionsBuilder.UseInternalServiceProvider),
                    nameof(IMemoryCache)),
                Assert.Throws<InvalidOperationException>(() => new ConstructorTestContextWithOC3A(options)).Message);
        }

        [Fact]
        public void Throws_setting_MemoryCache_with_AddDbContext_when_UseInternalServiceProvider()
        {
            var appServiceProivder = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    (p, b) => b.UseMemoryCache(new FakeMemoryCache())
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseInternalServiceProvider(p))
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                Assert.Equal(
                    CoreStrings.InvalidUseService(
                        nameof(DbContextOptionsBuilder.UseMemoryCache),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider),
                        nameof(IMemoryCache)),
                    Assert.Throws<InvalidOperationException>(
                        () => serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>()).Message);
            }
        }

        private class FakeMemoryCache : IMemoryCache
        {
            public void Dispose()
            {
            }

            public bool TryGetValue(object key, out object value)
            {
                throw new NotImplementedException();
            }

            public ICacheEntry CreateEntry(object key)
            {
                throw new NotImplementedException();
            }

            public void Remove(object key)
            {
            }
        }

        [Fact]
        public void Throws_changing_sensitive_data_logging_in_OnConfiguring_when_UseInternalServiceProvider()
        {
            using (var context = new ChangeSdlCacheContext(false))
            {
                _ = context.Model;
            }

            using (var context = new ChangeSdlCacheContext(true))
            {
                Assert.Equal(
                    CoreStrings.SingletonOptionChanged(
                        nameof(DbContextOptionsBuilder.EnableSensitiveDataLogging),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        private class ChangeSdlCacheContext : DbContext
        {
            private static readonly IServiceProvider _serviceProvider
                = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

            private readonly bool _on;

            public ChangeSdlCacheContext(bool on)
            {
                _on = on;
            }

            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .EnableSensitiveDataLogging(_on)
                    .UseInternalServiceProvider(_serviceProvider)
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        [Fact]
        public void Throws_changing_sensitive_data_logging_in_options_when_UseInternalServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            using (var context = new ConstructorTestContextWithOC3A(
                new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .UseInternalServiceProvider(serviceProvider)
                    .EnableSensitiveDataLogging()
                    .Options))
            {
                _ = context.Model;
            }

            using (var context = new ConstructorTestContextWithOC3A(
                new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .UseInternalServiceProvider(serviceProvider)
                    .EnableSensitiveDataLogging(false)
                    .Options))
            {
                Assert.Equal(
                    CoreStrings.SingletonOptionChanged(
                        nameof(DbContextOptionsBuilder.EnableSensitiveDataLogging),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_changing_sensitive_data_logging_with_AddDbContext_when_UseInternalServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            using (var serviceScope = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    (p, b) => b.EnableSensitiveDataLogging()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseInternalServiceProvider(serviceProvider))
                .BuildServiceProvider()
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                _ = context.Model;
            }

            using (var serviceScope = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    (p, b) => b.EnableSensitiveDataLogging(false)
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseInternalServiceProvider(serviceProvider))
                .BuildServiceProvider()
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Equal(
                    CoreStrings.SingletonOptionChanged(
                        nameof(DbContextOptionsBuilder.EnableSensitiveDataLogging),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_changing_warnings_default_in_OnConfiguring_when_UseInternalServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            using (var context = new ChangeWarningsCacheContext(serviceProvider, b => b.Default(WarningBehavior.Ignore)))
            {
                _ = context.Model;
            }

            using (var context = new ChangeWarningsCacheContext(serviceProvider, b => b.Default(WarningBehavior.Log)))
            {
                Assert.Equal(
                    CoreStrings.SingletonOptionChanged(
                        nameof(DbContextOptionsBuilder.ConfigureWarnings),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_changing_warnings_in_OnConfiguring_when_UseInternalServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            using (var context = new ChangeWarningsCacheContext(serviceProvider, b => b.Throw(CoreEventId.QueryExecutionPlanned)))
            {
                _ = context.Model;
            }

            using (var context = new ChangeWarningsCacheContext(serviceProvider, b => b.Log(CoreEventId.QueryExecutionPlanned)))
            {
                Assert.Equal(
                    CoreStrings.SingletonOptionChanged(
                        nameof(DbContextOptionsBuilder.ConfigureWarnings),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        private class ChangeWarningsCacheContext : DbContext
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly Action<WarningsConfigurationBuilder> _configAction;

            public ChangeWarningsCacheContext(
                IServiceProvider serviceProvider,
                Action<WarningsConfigurationBuilder> configAction)
            {
                _serviceProvider = serviceProvider;
                _configAction = configAction;
            }

            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder
                    .ConfigureWarnings(_configAction)
                    .UseInternalServiceProvider(_serviceProvider)
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        [Fact]
        public void Throws_changing_warnings_config_in_options_when_UseInternalServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            using (var context = new ConstructorTestContextWithOC3A(
                new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .UseInternalServiceProvider(serviceProvider)
                    .ConfigureWarnings(b => b.Default(WarningBehavior.Throw))
                    .Options))
            {
                _ = context.Model;
            }

            using (var context = new ConstructorTestContextWithOC3A(
                new DbContextOptionsBuilder<ConstructorTestContextWithOC3A>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .UseInternalServiceProvider(serviceProvider)
                    .ConfigureWarnings(b => b.Default(WarningBehavior.Log))
                    .Options))
            {
                Assert.Equal(
                    CoreStrings.SingletonOptionChanged(
                        nameof(DbContextOptionsBuilder.ConfigureWarnings),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        [Fact]
        public void Throws_changing_warnings_config_with_AddDbContext_when_UseInternalServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            using (var serviceScope = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    (p, b) => b.ConfigureWarnings(wb => wb.Default(WarningBehavior.Throw))
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseInternalServiceProvider(serviceProvider))
                .BuildServiceProvider()
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                _ = context.Model;
            }

            using (var serviceScope = new ServiceCollection()
                .AddDbContext<ConstructorTestContextWithOC3A>(
                    (p, b) => b.ConfigureWarnings(wb => wb.Default(WarningBehavior.Ignore))
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .UseInternalServiceProvider(serviceProvider))
                .BuildServiceProvider()
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ConstructorTestContextWithOC3A>();

                Assert.Equal(
                    CoreStrings.SingletonOptionChanged(
                        nameof(DbContextOptionsBuilder.ConfigureWarnings),
                        nameof(DbContextOptionsBuilder.UseInternalServiceProvider)),
                    Assert.Throws<InvalidOperationException>(() => context.Model).Message);
            }
        }

        private class WrappingLoggerFactory : ILoggerFactory
        {
            private readonly ILoggerFactory _loggerFactory;

            public IList<string> CreatedLoggers { get; } = new List<string>();

            public WrappingLoggerFactory(ILoggerFactory loggerFactory)
            {
                _loggerFactory = loggerFactory;
            }

            public void Dispose() => _loggerFactory.Dispose();

            public ILogger CreateLogger(string categoryName)
            {
                CreatedLoggers.Add(categoryName);

                return _loggerFactory.CreateLogger(categoryName);
            }

            public void AddProvider(ILoggerProvider provider)
            {
                _loggerFactory.AddProvider(provider);
            }
        }

        private class ConstructorTestContext1A : DbContext
        {
            public ConstructorTestContext1A(DbContextOptions options)
                : base(options)
            {
            }
        }

        private class ConstructorTestContextWithSets : DbContext
        {
            public ConstructorTestContextWithSets(DbContextOptions options)
                : base(options)
            {
            }

            public DbSet<Product> Products { get; set; }
        }

        private class ConstructorTestContextNoConfiguration : DbContext
        {
        }

        private class ConstructorTestContextNoConfigurationWithSets : DbContext
        {
            public DbSet<Product> Products { get; set; }
        }

        private class ConstructorTestContextWithOCBase : DbContext
        {
            private readonly IServiceProvider _internalServicesProvider;
            private readonly ILoggerFactory _loggerFactory;
            private readonly IMemoryCache _memoryCache;
            private readonly bool _isConfigured;

            protected ConstructorTestContextWithOCBase(
                ILoggerFactory loggerFactory = null,
                IMemoryCache memoryCache = null)
            {
                _loggerFactory = loggerFactory;
                _memoryCache = memoryCache;
            }

            protected ConstructorTestContextWithOCBase(
                IServiceProvider internalServicesProvider,
                ILoggerFactory loggerFactory = null,
                IMemoryCache memoryCache = null)
            {
                _internalServicesProvider = internalServicesProvider;
                _loggerFactory = loggerFactory;
                _memoryCache = memoryCache;
            }

            protected ConstructorTestContextWithOCBase(
                DbContextOptions options,
                ILoggerFactory loggerFactory = null,
                IMemoryCache memoryCache = null)
                : base(options)
            {
                _loggerFactory = loggerFactory;
                _memoryCache = memoryCache;
                _isConfigured = true;
            }

            protected internal override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                Assert.Equal(_isConfigured, optionsBuilder.IsConfigured);

                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
                }

                if (_internalServicesProvider != null)
                {
                    optionsBuilder.UseInternalServiceProvider(_internalServicesProvider);
                }

                if (_memoryCache != null)
                {
                    optionsBuilder.UseMemoryCache(_memoryCache);
                }

                if (_loggerFactory != null)
                {
                    optionsBuilder.UseLoggerFactory(_loggerFactory);
                }
            }
        }

        private class ConstructorTestContextWithOC1A : ConstructorTestContextWithOCBase
        {
        }

        private class ConstructorTestContextWithOC2A : ConstructorTestContextWithOCBase
        {
            public ConstructorTestContextWithOC2A(
                IServiceProvider internalServicesProvider)
                : base(internalServicesProvider)
            {
            }
        }

        private class ConstructorTestContextWithOC3A : ConstructorTestContextWithOCBase
        {
            public ConstructorTestContextWithOC3A(
                DbContextOptions options)
                : base(options)
            {
            }
        }

        private class ConstructorTestContextWithOC1B : ConstructorTestContextWithOCBase
        {
            public ConstructorTestContextWithOC1B(
                ILoggerFactory loggerFactory,
                IMemoryCache memoryCache)
                : base(loggerFactory, memoryCache)
            {
            }
        }

        private class ConstructorTestContextWithOC2B : ConstructorTestContextWithOCBase
        {
            public ConstructorTestContextWithOC2B(
                IServiceProvider internalServicesProvider,
                ILoggerFactory loggerFactory,
                IMemoryCache memoryCache)
                : base(internalServicesProvider, loggerFactory, memoryCache)
            {
            }
        }

        [Fact]
        public void Throws_when_wrong_DbContextOptions_used()
        {
            var options = new DbContextOptionsBuilder<NonGenericOptions1>()
                .UseInternalServiceProvider(new ServiceCollection().BuildServiceProvider())
                .Options;

            Assert.Equal(
                CoreStrings.NonGenericOptions(nameof(NonGenericOptions2)),
                Assert.Throws<InvalidOperationException>(() => new NonGenericOptions2(options)).Message);
        }

        [Fact]
        public void Throws_when_adding_two_contexts_using_non_generic_options()
        {
            var appServiceProivder = new ServiceCollection()
                .AddDbContext<NonGenericOptions2>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddDbContext<NonGenericOptions1>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            using (var serviceScope = appServiceProivder
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                Assert.Equal(
                    CoreStrings.NonGenericOptions(nameof(NonGenericOptions2)),
                    Assert.Throws<InvalidOperationException>(() =>
                        {
                            serviceScope.ServiceProvider.GetService<NonGenericOptions1>();
                            serviceScope.ServiceProvider.GetService<NonGenericOptions2>();
                        }).Message);
            }
        }

        private class NonGenericOptions1 : DbContext
        {
            public NonGenericOptions1(DbContextOptions options)
                : base(options)
            {
            }
        }

        private class NonGenericOptions2 : DbContext
        {
            public NonGenericOptions2(DbContextOptions options)
                : base(options)
            {
            }
        }

        [Fact]
        public void AddDbContext_adds_options_for_all_types()
        {
            var services = new ServiceCollection()
                .AddSingleton<DbContextOptions>(_ => new DbContextOptions<NonGenericOptions1>())
                .AddDbContext<NonGenericOptions1>()
                .AddDbContext<NonGenericOptions2>()
                .BuildServiceProvider();

            Assert.Equal(3, services.GetServices<DbContextOptions>().Count());
            Assert.Equal(2, services.GetServices<DbContextOptions>()
                .Select(o => o.ContextType)
                .Distinct()
                .Count());
        }

        [Fact]
        public void Last_DbContextOptions_in_serviceCollection_selected()
        {
            var services = new ServiceCollection()
                .AddDbContext<NonGenericOptions1>()
                .AddDbContext<NonGenericOptions2>()
                .BuildServiceProvider();

            Assert.Equal(typeof(NonGenericOptions2), services.GetService<DbContextOptions>().ContextType);
        }
    }
}
