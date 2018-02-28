// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class RelationalEntityTypeAttributeConventionTest
    {
        [Fact]
        public void TableAttribute_sets_column_name_order_and_type_with_conventional_builder()
        {
            var modelBuilder = new ModelBuilder(TestRelationalConventionSetBuilder.Build());

            var entityBuilder = modelBuilder.Entity<A>();

            Assert.Equal("MyTable", entityBuilder.Metadata.Relational().TableName);
            Assert.Equal("MySchema", entityBuilder.Metadata.Relational().Schema);
        }

        [Fact]
        public void TableAttribute_overrides_configuration_from_convention_source()
        {
            var entityBuilder = CreateInternalEntityTypeBuilder<A>();

            entityBuilder.HasAnnotation(RelationalAnnotationNames.TableName, "ConventionalName", ConfigurationSource.Convention);
            entityBuilder.HasAnnotation(RelationalAnnotationNames.Schema, "ConventionalSchema", ConfigurationSource.Convention);

            new RelationalTableAttributeConvention().Apply(entityBuilder);

            Assert.Equal("MyTable", entityBuilder.Metadata.Relational().TableName);
            Assert.Equal("MySchema", entityBuilder.Metadata.Relational().Schema);
        }

        [Fact]
        public void TableAttribute_does_not_override_configuration_from_explicit_source()
        {
            var entityBuilder = CreateInternalEntityTypeBuilder<A>();

            entityBuilder.HasAnnotation(RelationalAnnotationNames.TableName, "ExplicitName", ConfigurationSource.Explicit);
            entityBuilder.HasAnnotation(RelationalAnnotationNames.Schema, "ExplicitName", ConfigurationSource.Explicit);

            new RelationalTableAttributeConvention().Apply(entityBuilder);

            Assert.Equal("ExplicitName", entityBuilder.Metadata.Relational().TableName);
            Assert.Equal("ExplicitName", entityBuilder.Metadata.Relational().Schema);
        }

        private InternalEntityTypeBuilder CreateInternalEntityTypeBuilder<T>()
        {
            var conventionSet = new ConventionSet();
            conventionSet.EntityTypeAddedConventions.Add(new PropertyDiscoveryConvention(new CoreTypeMapper(new CoreTypeMapperDependencies())));

            var modelBuilder = new InternalModelBuilder(new Model(conventionSet));

            return modelBuilder.Entity(typeof(T), ConfigurationSource.Explicit);
        }

        [Table("MyTable", Schema = "MySchema")]
        private class A
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
