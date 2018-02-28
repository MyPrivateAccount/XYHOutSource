// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class LoggingRelationalTest<TBuilder, TExtension> : LoggingTestBase
        where TBuilder : RelationalDbContextOptionsBuilder<TBuilder, TExtension>
        where TExtension : RelationalOptionsExtension, new()
    {
        [Fact]
        public void Logs_context_initialization_max_batch_size()
        {
            Assert.Equal(
                ExpectedMessage("MaxBatchSize=10 " + DefaultOptions),
                ActualMessage(CreateOptionsBuilder(b => b.MaxBatchSize(10))));
        }

        [Fact]
        public void Logs_context_initialization_command_timeout()
        {
            Assert.Equal(
                ExpectedMessage("CommandTimeout=10 " + DefaultOptions),
                ActualMessage(CreateOptionsBuilder(b => b.CommandTimeout(10))));
        }

        [Fact]
        public void Logs_context_initialization_relational_nulls()
        {
            Assert.Equal(
                ExpectedMessage("UseRelationalNulls " + DefaultOptions),
                ActualMessage(CreateOptionsBuilder(b => b.UseRelationalNulls())));
        }

        [Fact]
        public void Logs_context_initialization_migrations_assembly()
        {
            Assert.Equal(
                ExpectedMessage("MigrationsAssembly=A.B.C " + DefaultOptions),
                ActualMessage(CreateOptionsBuilder(b => b.MigrationsAssembly("A.B.C"))));
        }

        [Fact]
        public void Logs_context_initialization_migrations_history_table()
        {
            Assert.Equal(
                ExpectedMessage("MigrationsHistoryTable=MyHistory " + DefaultOptions),
                ActualMessage(CreateOptionsBuilder(b => b.MigrationsHistoryTable("MyHistory"))));
        }

        [Fact]
        public void Logs_context_initialization_migrations_history_table_schema()
        {
            Assert.Equal(
                ExpectedMessage("MigrationsHistoryTable=mySchema.MyHistory " + DefaultOptions),
                ActualMessage(CreateOptionsBuilder(b => b.MigrationsHistoryTable("MyHistory", "mySchema"))));
        }

        protected abstract DbContextOptionsBuilder CreateOptionsBuilder(
            Action<RelationalDbContextOptionsBuilder<TBuilder, TExtension>> relationalAction);

        protected override DbContextOptionsBuilder CreateOptionsBuilder() => CreateOptionsBuilder(null);
    }
}