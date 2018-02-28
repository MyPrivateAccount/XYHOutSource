// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    public class SqliteConventionSetBuilder : RelationalConventionSetBuilder
    {
        public SqliteConventionSetBuilder([NotNull] RelationalConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        public static ConventionSet Build()
        {
            var relationalTypeMapper = new SqliteTypeMapper(new RelationalTypeMapperDependencies());

            return new SqliteConventionSetBuilder(
                    new RelationalConventionSetBuilderDependencies(relationalTypeMapper, null, null))
                .AddConventions(
                    new CoreConventionSetBuilder(
                        new CoreConventionSetBuilderDependencies(relationalTypeMapper)).CreateConventionSet());
        }
    }
}
