// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public class ModelSourceTest
    {
        private readonly ModelValidator _coreModelValidator
            = new ModelValidator(
                new ModelValidatorDependencies(
                    new DiagnosticsLogger<DbLoggerCategory.Model.Validation>(
                        new LoggerFactory(),
                        new LoggingOptions(),
                        new DiagnosticListener("Fake"))));

        private readonly NullConventionSetBuilder _nullConventionSetBuilder
            = new NullConventionSetBuilder();

        [Fact]
        public void Adds_all_entities_based_on_all_distinct_entity_types_found()
        {
            var setFinderMock = new Mock<IDbSetFinder>();
            setFinderMock.Setup(m => m.FindSets(It.IsAny<DbContext>())).Returns(
                new[]
                {
                    new DbSetProperty("One", typeof(SetA), setter: null),
                    new DbSetProperty("Two", typeof(SetB), setter: null),
                    new DbSetProperty("Three", typeof(SetA), setter: null)
                });

            var model = CreateDefaultModelSource(setFinderMock.Object)
                .GetModel(new Mock<DbContext>().Object, _nullConventionSetBuilder, _coreModelValidator);

            Assert.Equal(
                new[] { typeof(SetA).DisplayName(), typeof(SetB).DisplayName() },
                model.GetEntityTypes().Select(e => e.Name).ToArray());
        }

        private class JustAClass
        {
            public DbSet<Random> One { get; set; }
            protected DbSet<object> Two { get; set; }
            private DbSet<string> Three { get; set; }
            private DbSet<string> Four { get; set; }
        }

        private class SetA
        {
            public int Id { get; set; }
        }

        private class SetB
        {
            public int Id { get; set; }
        }

        [Fact]
        public void Caches_model_by_context_type()
        {
            var modelSource = CreateDefaultModelSource(new DbSetFinder());

            var model1 = modelSource.GetModel(new Context1(), _nullConventionSetBuilder, _coreModelValidator);
            var model2 = modelSource.GetModel(new Context2(), _nullConventionSetBuilder, _coreModelValidator);

            Assert.NotSame(model1, model2);
            Assert.Same(model1, modelSource.GetModel(new Context1(), _nullConventionSetBuilder, _coreModelValidator));
            Assert.Same(model2, modelSource.GetModel(new Context2(), _nullConventionSetBuilder, _coreModelValidator));
        }

        [Fact]
        public void Stores_model_version_information_as_annotation_on_model()
        {
            var modelSource = CreateDefaultModelSource(new DbSetFinder());

            var model = modelSource.GetModel(new Context1(), _nullConventionSetBuilder, _coreModelValidator);

            Assert.StartsWith("2.0.0", model.GetProductVersion(), StringComparison.OrdinalIgnoreCase);
        }

        private class Context1 : DbContext
        {
        }

        private class Context2 : DbContext
        {
        }

        private IModelSource CreateDefaultModelSource(IDbSetFinder setFinder)
            => new ConcreteModelSource(setFinder);


        private class ConcreteModelSource : ModelSource
        {
            public ConcreteModelSource(IDbSetFinder setFinder)
                : base(new ModelSourceDependencies(
                    new CoreConventionSetBuilder(new CoreConventionSetBuilderDependencies(new CoreTypeMapper(new CoreTypeMapperDependencies()))),
                    new ModelCustomizer(new ModelCustomizerDependencies(setFinder)),
                    new ModelCacheKeyFactory(new ModelCacheKeyFactoryDependencies())))
            {
            }
        }
    }
}
