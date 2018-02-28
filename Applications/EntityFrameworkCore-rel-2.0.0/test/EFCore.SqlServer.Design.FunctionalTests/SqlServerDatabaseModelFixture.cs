// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore
{
    public class SqlServerDatabaseModelFixture : IDisposable
    {
        public SqlServerDatabaseModelFixture()
        {
            TestStore = SqlServerTestStore.CreateScratch();
        }

        public TestDesignLoggerFactory TestDesignLoggerFactory { get; } = new TestDesignLoggerFactory();

        public DatabaseModel CreateModel(string createSql, IEnumerable<string> tables = null, ILogger logger = null)
        {
            TestStore.ExecuteNonQuery(createSql);

            return new SqlServerDatabaseModelFactory(
                    new DiagnosticsLogger<DbLoggerCategory.Scaffolding>(
                        TestDesignLoggerFactory,
                        new LoggingOptions(),
                        new DiagnosticListener("Fake")))
                .Create(TestStore.ConnectionString, tables ?? Enumerable.Empty<string>(), Enumerable.Empty<string>());
        }

        public SqlServerTestStore TestStore { get; }

        public void ExecuteNonQuery(string sql) => TestStore.ExecuteNonQuery(sql);

        public void Dispose() => TestStore.Dispose();
    }
}
