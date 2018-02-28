// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.ReverseEngineering
{
    public class SqliteAttributesInsteadOfFluentApiE2ETest : SqliteE2ETestBase
    {
        public SqliteAttributesInsteadOfFluentApiE2ETest(ITestOutputHelper output)
            : base(output)
        {
        }

        protected override string DbSuffix { get; } = "Attributes";
        protected override bool UseDataAnnotations { get; } = true;
        protected override string ExpectedResultsParentDir { get; } = Path.Combine("ReverseEngineering", "Expected", "Attributes");

        protected override string ProviderName => "Microsoft.EntityFrameworkCore.Sqlite.Design";
    }
}
