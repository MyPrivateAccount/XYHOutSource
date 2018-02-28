// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.Design.Internal
{
    public class DbContextOperations
    {
        // TODO: Flow in from tools (issue #8332)
        private static readonly string[] _args = Array.Empty<string>();

        private readonly IOperationReporter _reporter;
        private readonly Assembly _assembly;
        private readonly Assembly _startupAssembly;
        private readonly AppServiceProviderFactory _appServicesFactory;

        // NB: Used by Scaffolding. Break with care.
        public DbContextOperations(
            [NotNull] IOperationReporter reporter,
            [NotNull] Assembly assembly,
            [NotNull] Assembly startupAssembly)
            : this(reporter, assembly, startupAssembly, new AppServiceProviderFactory(startupAssembly, reporter))
        {
        }

        protected DbContextOperations(
            [NotNull] IOperationReporter reporter,
            [NotNull] Assembly assembly,
            [NotNull] Assembly startupAssembly,
            [NotNull] AppServiceProviderFactory appServicesFactory)
        {
            Check.NotNull(reporter, nameof(reporter));
            Check.NotNull(assembly, nameof(assembly));
            Check.NotNull(startupAssembly, nameof(startupAssembly));

            _reporter = reporter;
            _assembly = assembly;
            _startupAssembly = startupAssembly;
            _appServicesFactory = appServicesFactory;
        }

        public virtual void DropDatabase([CanBeNull] string contextType)
        {
            using (var context = CreateContext(contextType))
            {
                var connection = context.Database.GetDbConnection();
                _reporter.WriteInformation(DesignStrings.DroppingDatabase(connection.Database));
                if (context.Database.EnsureDeleted())
                {
                    _reporter.WriteInformation(DesignStrings.DatabaseDropped(connection.Database));
                }
                else
                {
                    _reporter.WriteInformation(DesignStrings.NotExistDatabase(connection.Database));
                }
            }
        }

        public virtual DbContext CreateContext([CanBeNull] string contextType)
            => CreateContext(FindContextType(contextType).Value);

        private DbContext CreateContext(Func<DbContext> factory)
        {
            var context = factory();
            _reporter.WriteVerbose(DesignStrings.UseContext(context.GetType().ShortDisplayName()));

            var loggerFactory = context.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new OperationLoggerProvider(_reporter));

            return context;
        }

        public virtual IEnumerable<Type> GetContextTypes()
            => FindContextTypes().Keys;

        public virtual Type GetContextType([CanBeNull] string name)
            => FindContextType(name).Key;

        private IDictionary<Type, Func<DbContext>> FindContextTypes()
        {
            _reporter.WriteVerbose(DesignStrings.FindingContexts);

            var contexts = new Dictionary<Type, Func<DbContext>>();

            // Look for IDesignTimeDbContextFactory implementations
            _reporter.WriteVerbose(DesignStrings.FindingContextFactories);
            var contextFactories = _startupAssembly.GetConstructableTypes()
                .Where(t => typeof(IDesignTimeDbContextFactory<DbContext>).GetTypeInfo().IsAssignableFrom(t));
            foreach (var factory in contextFactories)
            {
                _reporter.WriteVerbose(DesignStrings.FoundContextFactory(factory.ShortDisplayName()));
                var manufacturedContexts =
                    from i in factory.ImplementedInterfaces
                    where i.GetTypeInfo().IsGenericType
                          && i.GetGenericTypeDefinition() == typeof(IDesignTimeDbContextFactory<>)
                    select i.GenericTypeArguments[0];
                foreach (var context in manufacturedContexts)
                {
                    _reporter.WriteVerbose(DesignStrings.FoundDbContext(context.ShortDisplayName()));
                    contexts.Add(
                        context,
                        () => CreateContextFromFactory(factory.AsType()));
                }
            }

            // Look for DbContext classes registered in the service provider
            var appServices = _appServicesFactory.Create(_args);
            var registeredContexts = appServices.GetServices<DbContextOptions>()
                .Select(o => o.ContextType);
            foreach (var context in registeredContexts.Where(c => !contexts.ContainsKey(c)))
            {
                _reporter.WriteVerbose(DesignStrings.FoundDbContext(context.ShortDisplayName()));
                contexts.Add(
                    context,
                    FindContextFactory(context)
                        ?? (() => (DbContext)ActivatorUtilities.GetServiceOrCreateInstance(appServices, context)));
            }

            // Look for DbContext classes in assemblies
            _reporter.WriteVerbose(DesignStrings.FindingReferencedContexts);
            var types = _startupAssembly.GetConstructableTypes()
                .Concat(_assembly.GetConstructableTypes())
                .ToList();
            var contextTypes = types.Where(t => typeof(DbContext).GetTypeInfo().IsAssignableFrom(t)).Select(
                    t => t.AsType())
                .Concat(
                    types.Where(t => typeof(Migration).GetTypeInfo().IsAssignableFrom(t))
                        .Select(t => t.GetCustomAttribute<DbContextAttribute>()?.ContextType)
                        .Where(t => t != null))
                .Distinct();
            foreach (var context in contextTypes.Where(c => !contexts.ContainsKey(c)))
            {
                _reporter.WriteVerbose(DesignStrings.FoundDbContext(context.ShortDisplayName()));
                contexts.Add(
                    context,
                    FindContextFactory(context) ?? (() =>
                        {
                            try
                            {
                                return (DbContext)Activator.CreateInstance(context);
                            }
                            catch (MissingMethodException ex)
                            {
                                throw new OperationException(DesignStrings.NoParameterlessConstructor(context.Name), ex);
                            }
                        }));
            }

            return contexts;
        }

        public virtual ContextInfo GetContextInfo([CanBeNull] string contextType)
        {
            using (var context = CreateContext(contextType))
            {
                var info = new ContextInfo();

                var provider = context.GetService<IDatabaseProvider>();
                info.ProviderName = provider.Name;

                var connection = context.Database.GetDbConnection();
                info.DataSource = connection.DataSource;
                info.DatabaseName = connection.Database;

                var options = context.GetService<IDbContextOptions>();
                info.Options = options.BuildOptionsFragment().Trim();

                return info;
            }
        }

        private Func<DbContext> FindContextFactory(Type contextType)
        {
            var factoryInterface = typeof(IDesignTimeDbContextFactory<>).MakeGenericType(contextType).GetTypeInfo();
            var factory = contextType.GetTypeInfo().Assembly.GetConstructableTypes()
                .Where(t => factoryInterface.IsAssignableFrom(t))
                .FirstOrDefault();
            if (factory == null)
            {
                return null;
            }

            return () => CreateContextFromFactory(factory.AsType());
        }

        private DbContext CreateContextFromFactory(Type factory)
        {
            _reporter.WriteVerbose(DesignStrings.UsingDbContextFactory(factory.ShortDisplayName()));

            return ((IDesignTimeDbContextFactory<DbContext>)Activator.CreateInstance(factory))
                .CreateDbContext(_args);
        }

        private KeyValuePair<Type, Func<DbContext>> FindContextType(string name)
        {
            var types = FindContextTypes();

            if (string.IsNullOrEmpty(name))
            {
                if (types.Count == 0)
                {
                    throw new OperationException(DesignStrings.NoContext(_assembly.GetName().Name));
                }
                if (types.Count == 1)
                {
                    return types.First();
                }

                throw new OperationException(DesignStrings.MultipleContexts);
            }

            var candidates = FilterTypes(types, name, ignoreCase: true);
            if (candidates.Count == 0)
            {
                throw new OperationException(DesignStrings.NoContextWithName(name));
            }
            if (candidates.Count == 1)
            {
                return candidates.First();
            }

            // Disambiguate using case
            candidates = FilterTypes(candidates, name);
            if (candidates.Count == 0)
            {
                throw new OperationException(DesignStrings.MultipleContextsWithName(name));
            }
            if (candidates.Count == 1)
            {
                return candidates.First();
            }

            // Allow selecting types in the default namespace
            candidates = candidates.Where(t => t.Key.Namespace == null).ToDictionary(t => t.Key, t => t.Value);
            if (candidates.Count == 0)
            {
                throw new OperationException(DesignStrings.MultipleContextsWithQualifiedName(name));
            }

            Debug.Assert(candidates.Count == 1, "candidates.Count is not 1.");

            return candidates.First();
        }

        private static IDictionary<Type, Func<DbContext>> FilterTypes(
            IDictionary<Type, Func<DbContext>> types,
            string name,
            bool ignoreCase = false)
        {
            var comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return types
                .Where(
                    t => string.Equals(t.Key.Name, name, comparisonType)
                         || string.Equals(t.Key.FullName, name, comparisonType)
                         || string.Equals(t.Key.AssemblyQualifiedName, name, comparisonType))
                .ToDictionary(t => t.Key, t => t.Value);
        }
    }
}
