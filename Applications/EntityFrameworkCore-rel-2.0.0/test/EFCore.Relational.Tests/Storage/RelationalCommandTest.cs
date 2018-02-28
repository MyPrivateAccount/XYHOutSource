﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Storage
{
    using CommandAction = Action<
        IRelationalConnection,
        IRelationalCommand,
        IReadOnlyDictionary<string, object>>;
    using CommandFunc = Func<
        IRelationalConnection,
        IRelationalCommand,
        IReadOnlyDictionary<string, object>,
        Task>;

    public class RelationalCommandTest
    {
        private const string FileLineEnding = @"
";

        [Fact]
        public void Configures_DbCommand()
        {
            var fakeConnection = CreateConnection();

            var relationalCommand = CreateRelationalCommand(commandText: "CommandText");

            relationalCommand.ExecuteNonQuery(fakeConnection);

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);

            var command = fakeConnection.DbConnections[0].DbCommands[0];

            Assert.Equal("CommandText", command.CommandText);
            Assert.Null(command.Transaction);
            Assert.Equal(FakeDbCommand.DefaultCommandTimeout, command.CommandTimeout);
        }

        [Fact]
        public void Configures_DbCommand_with_transaction()
        {
            var fakeConnection = CreateConnection();

            var relationalTransaction = fakeConnection.BeginTransaction();

            var relationalCommand = CreateRelationalCommand();

            relationalCommand.ExecuteNonQuery(fakeConnection);

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);

            var command = fakeConnection.DbConnections[0].DbCommands[0];

            Assert.Same(relationalTransaction.GetDbTransaction(), command.Transaction);
        }

        [Fact]
        public void Configures_DbCommand_with_timeout()
        {
            var optionsExtension = new FakeRelationalOptionsExtension()
                .WithConnectionString(ConnectionString)
                .WithCommandTimeout(42);

            var fakeConnection = CreateConnection(CreateOptions(optionsExtension));

            var relationalCommand = CreateRelationalCommand();

            relationalCommand.ExecuteNonQuery(fakeConnection);

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);

            var command = fakeConnection.DbConnections[0].DbCommands[0];

            Assert.Equal(42, command.CommandTimeout);
        }

        [Fact]
        public void Can_ExecuteNonQuery()
        {
            var executeNonQueryCount = 0;
            var disposeCount = -1;

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeNonQuery: c =>
                        {
                            executeNonQueryCount++;
                            disposeCount = c.DisposeCount;
                            return 1;
                        }));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var relationalCommand = CreateRelationalCommand();

            var result = relationalCommand.ExecuteNonQuery(
                new FakeRelationalConnection(options));

            Assert.Equal(1, result);

            var expectedCount = 1;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);

            // Durring command execution
            Assert.Equal(1, executeNonQueryCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Fact]
        public virtual async Task Can_ExecuteNonQueryAsync()
        {
            var executeNonQueryCount = 0;
            var disposeCount = -1;

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeNonQueryAsync: (c, ct) =>
                        {
                            executeNonQueryCount++;
                            disposeCount = c.DisposeCount;
                            return Task.FromResult(1);
                        }));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var relationalCommand = CreateRelationalCommand();

            var result = await relationalCommand.ExecuteNonQueryAsync(
                new FakeRelationalConnection(options));

            Assert.Equal(1, result);

            var expectedCount = 1;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);

            // Durring command execution
            Assert.Equal(1, executeNonQueryCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Fact]
        public void Can_ExecuteScalar()
        {
            var executeScalarCount = 0;
            var disposeCount = -1;

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeScalar: c =>
                        {
                            executeScalarCount++;
                            disposeCount = c.DisposeCount;
                            return "ExecuteScalar Result";
                        }));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var relationalCommand = CreateRelationalCommand();

            var result = (string)relationalCommand.ExecuteScalar(
                new FakeRelationalConnection(options));

            Assert.Equal("ExecuteScalar Result", result);

            var expectedCount = 1;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);

            // Durring command execution
            Assert.Equal(1, executeScalarCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Fact]
        public async Task Can_ExecuteScalarAsync()
        {
            var executeScalarCount = 0;
            var disposeCount = -1;

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeScalarAsync: (c, ct) =>
                        {
                            executeScalarCount++;
                            disposeCount = c.DisposeCount;
                            return Task.FromResult<object>("ExecuteScalar Result");
                        }));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var relationalCommand = CreateRelationalCommand();

            var result = (string)await relationalCommand.ExecuteScalarAsync(
                new FakeRelationalConnection(options));

            Assert.Equal("ExecuteScalar Result", result);

            var expectedCount = 1;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);

            // Durring command execution
            Assert.Equal(1, executeScalarCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Fact]
        public void Can_ExecuteReader()
        {
            var executeReaderCount = 0;
            var disposeCount = -1;

            var dbDataReader = new FakeDbDataReader();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeReader: (c, b) =>
                        {
                            executeReaderCount++;
                            disposeCount = c.DisposeCount;
                            return dbDataReader;
                        }));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var relationalCommand = CreateRelationalCommand();

            var result = relationalCommand.ExecuteReader(
                new FakeRelationalConnection(options));

            Assert.Same(dbDataReader, result.DbDataReader);
            Assert.Equal(0, fakeDbConnection.CloseCount);

            var expectedCount = 1;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);

            // Durring command execution
            Assert.Equal(1, executeReaderCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(0, dbDataReader.DisposeCount);
            Assert.Equal(0, fakeDbConnection.DbCommands[0].DisposeCount);

            // After reader dispose
            result.Dispose();
            Assert.Equal(1, dbDataReader.DisposeCount);
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);
        }

        [Fact]
        public async Task Can_ExecuteReaderAsync()
        {
            var executeReaderCount = 0;
            var disposeCount = -1;

            var dbDataReader = new FakeDbDataReader();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    executeReaderAsync: (c, b, ct) =>
                        {
                            executeReaderCount++;
                            disposeCount = c.DisposeCount;
                            return Task.FromResult<DbDataReader>(dbDataReader);
                        }));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var relationalCommand = CreateRelationalCommand();

            var result = await relationalCommand.ExecuteReaderAsync(
                new FakeRelationalConnection(options));

            Assert.Same(dbDataReader, result.DbDataReader);
            Assert.Equal(0, fakeDbConnection.CloseCount);

            var expectedCount = 1;
            Assert.Equal(expectedCount, fakeDbConnection.OpenCount);

            // Durring command execution
            Assert.Equal(1, executeReaderCount);
            Assert.Equal(0, disposeCount);

            // After command execution
            Assert.Equal(0, dbDataReader.DisposeCount);
            Assert.Equal(0, fakeDbConnection.DbCommands[0].DisposeCount);

            // After reader dispose
            result.Dispose();
            Assert.Equal(1, dbDataReader.DisposeCount);
            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
            Assert.Equal(expectedCount, fakeDbConnection.CloseCount);
        }

        public static TheoryData CommandActions
            => new TheoryData<Delegate, DbCommandMethod, bool>
            {
                {
                    new CommandAction(
                        (connection, command, parameterValues)
                            => command.ExecuteNonQuery(connection, parameterValues)),
                    DbCommandMethod.ExecuteNonQuery,
                    false
                },
                {
                    new CommandAction(
                        (connection, command, parameterValues)
                            => command.ExecuteScalar(connection, parameterValues)),
                    DbCommandMethod.ExecuteScalar,
                    false
                },
                {
                    new CommandAction(
                        (connection, command, parameterValues)
                            => command.ExecuteReader(connection, parameterValues)),
                    DbCommandMethod.ExecuteReader,
                    false
                },
                {
                    new CommandFunc(
                        (connection, command, parameterValues)
                            => command.ExecuteNonQueryAsync(connection, parameterValues)),
                    DbCommandMethod.ExecuteNonQuery,
                    true
                },
                {
                    new CommandFunc(
                        (connection, command, parameterValues)
                            => command.ExecuteScalarAsync(connection, parameterValues)),
                    DbCommandMethod.ExecuteScalar,
                    true
                },
                {
                    new CommandFunc(
                        (connection, command, parameterValues)
                            => command.ExecuteReaderAsync(connection, parameterValues)),
                    DbCommandMethod.ExecuteReader,
                    true
                }
            };

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Throws_when_parameters_are_configured_and_parameter_values_is_null(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var relationalCommand = CreateRelationalCommand(
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false),
                    new TypeMappedRelationalParameter("SecondInvariant", "SecondParameter", new LongTypeMapping("long", DbType.Int64), true),
                    new TypeMappedRelationalParameter("ThirdInvariant", "ThirdParameter", RelationalTypeMapping.NullMapping, null)
                });

            if (async)
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("FirstInvariant"),
                    (await Assert.ThrowsAsync<InvalidOperationException>(
                        async ()
                            => await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, null))).Message);
            }
            else
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("FirstInvariant"),
                    Assert.Throws<InvalidOperationException>(
                            ()
                                => ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, null))
                        .Message);
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Throws_when_parameters_are_configured_and_value_is_missing(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var relationalCommand = CreateRelationalCommand(
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false),
                    new TypeMappedRelationalParameter("SecondInvariant", "SecondParameter", new LongTypeMapping("long", DbType.Int64), true),
                    new TypeMappedRelationalParameter("ThirdInvariant", "ThirdParameter", RelationalTypeMapping.NullMapping, null)
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "FirstInvariant", 17 },
                { "SecondInvariant", 18L }
            };

            if (async)
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("ThirdInvariant"),
                    (await Assert.ThrowsAsync<InvalidOperationException>(
                        async ()
                            => await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues))).Message);
            }
            else
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("ThirdInvariant"),
                    Assert.Throws<InvalidOperationException>(
                            ()
                                => ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues))
                        .Message);
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Configures_DbCommand_with_type_mapped_parameters(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var relationalCommand = CreateRelationalCommand(
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false),
                    new TypeMappedRelationalParameter("SecondInvariant", "SecondParameter", new LongTypeMapping("long", DbType.Int64), true),
                    new TypeMappedRelationalParameter("ThirdInvariant", "ThirdParameter", RelationalTypeMapping.NullMapping, null)
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "FirstInvariant", 17 },
                { "SecondInvariant", 18L },
                { "ThirdInvariant", null }
            };

            if (async)
            {
                await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }
            else
            {
                ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);
            Assert.Equal(3, fakeConnection.DbConnections[0].DbCommands[0].Parameters.Count);

            var parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[0];

            Assert.Equal("FirstParameter", parameter.ParameterName);
            Assert.Equal(17, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.False(parameter.IsNullable);
            Assert.Equal(DbType.Int32, parameter.DbType);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[1];

            Assert.Equal("SecondParameter", parameter.ParameterName);
            Assert.Equal(18L, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.True(parameter.IsNullable);
            Assert.Equal(DbType.Int64, parameter.DbType);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[2];

            Assert.Equal("ThirdParameter", parameter.ParameterName);
            Assert.Equal(DBNull.Value, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.Equal(FakeDbParameter.DefaultIsNullable, parameter.IsNullable);
            Assert.Equal(FakeDbParameter.DefaultDbType, parameter.DbType);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Configures_DbCommand_with_dynamic_parameters(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var typeMapper = new FakeRelationalTypeMapper(new RelationalTypeMapperDependencies());

            var dbParameter = new FakeDbParameter { ParameterName = "FirstParameter", Value = 17, DbType = DbType.Int32 };

            var relationalCommand = CreateRelationalCommand(
                parameters: new[]
                {
                    new DynamicRelationalParameter("FirstInvariant", "FirstParameter", typeMapper),
                    new DynamicRelationalParameter("SecondInvariant", "SecondParameter", typeMapper),
                    new DynamicRelationalParameter("ThirdInvariant", "ThirdParameter", typeMapper)
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "FirstInvariant", dbParameter },
                { "SecondInvariant", 18L },
                { "ThirdInvariant", null }
            };

            if (async)
            {
                await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }
            else
            {
                ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);
            Assert.Equal(3, fakeConnection.DbConnections[0].DbCommands[0].Parameters.Count);

            var parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[0];

            Assert.Equal(parameter, fakeConnection.DbConnections[0].DbCommands[0].Parameters[0]);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[1];
            var mapping = typeMapper.GetMapping(18L.GetType());

            Assert.Equal("SecondParameter", parameter.ParameterName);
            Assert.Equal(18L, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.False(parameter.IsNullable);
            Assert.Equal(mapping.DbType, parameter.DbType);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[2];

            Assert.Equal("ThirdParameter", parameter.ParameterName);
            Assert.Equal(DBNull.Value, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.Equal(FakeDbParameter.DefaultIsNullable, parameter.IsNullable);
            Assert.Equal(FakeDbParameter.DefaultDbType, parameter.DbType);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Configures_DbCommand_with_composite_parameters(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var relationalCommand = CreateRelationalCommand(
                parameters: new[]
                {
                    new CompositeRelationalParameter(
                        "CompositeInvariant",
                        new[]
                        {
                            new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false),
                            new TypeMappedRelationalParameter("SecondInvariant", "SecondParameter", new LongTypeMapping("long", DbType.Int64), true),
                            new TypeMappedRelationalParameter("ThirdInvariant", "ThirdParameter", RelationalTypeMapping.NullMapping, null)
                        })
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "CompositeInvariant", new object[] { 17, 18L, null } }
            };

            if (async)
            {
                await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }
            else
            {
                ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }

            Assert.Equal(1, fakeConnection.DbConnections.Count);
            Assert.Equal(1, fakeConnection.DbConnections[0].DbCommands.Count);
            Assert.Equal(3, fakeConnection.DbConnections[0].DbCommands[0].Parameters.Count);

            var parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[0];

            Assert.Equal("FirstParameter", parameter.ParameterName);
            Assert.Equal(17, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.False(parameter.IsNullable);
            Assert.Equal(DbType.Int32, parameter.DbType);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[1];

            Assert.Equal("SecondParameter", parameter.ParameterName);
            Assert.Equal(18L, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.True(parameter.IsNullable);
            Assert.Equal(DbType.Int64, parameter.DbType);

            parameter = fakeConnection.DbConnections[0].DbCommands[0].Parameters[2];

            Assert.Equal("ThirdParameter", parameter.ParameterName);
            Assert.Equal(DBNull.Value, parameter.Value);
            Assert.Equal(ParameterDirection.Input, parameter.Direction);
            Assert.Equal(FakeDbParameter.DefaultIsNullable, parameter.IsNullable);
            Assert.Equal(FakeDbParameter.DefaultDbType, parameter.DbType);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Throws_when_composite_parameters_are_configured_and_value_is_missing(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var relationalCommand = CreateRelationalCommand(
                parameters: new[]
                {
                    new CompositeRelationalParameter(
                        "CompositeInvariant",
                        new[]
                        {
                            new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false),
                            new TypeMappedRelationalParameter("SecondInvariant", "SecondParameter", new LongTypeMapping("long", DbType.Int64), true),
                            new TypeMappedRelationalParameter("ThirdInvariant", "ThirdParameter", RelationalTypeMapping.NullMapping, null)
                        })
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "CompositeInvariant", new object[] { 17, 18L } }
            };

            if (async)
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("ThirdInvariant"),
                    (await Assert.ThrowsAsync<InvalidOperationException>(
                        async ()
                            => await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues))).Message);
            }
            else
            {
                Assert.Equal(
                    RelationalStrings.MissingParameterValue("ThirdInvariant"),
                    Assert.Throws<InvalidOperationException>(
                            ()
                                => ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues))
                        .Message);
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Throws_when_composite_parameters_are_configured_and_value_is_not_object_array(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var fakeConnection = CreateConnection();

            var relationalCommand = CreateRelationalCommand(
                parameters: new[]
                {
                    new CompositeRelationalParameter(
                        "CompositeInvariant",
                        new[]
                        {
                            new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false)
                        })
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "CompositeInvariant", 17 }
            };

            if (async)
            {
                Assert.Equal(
                    RelationalStrings.ParameterNotObjectArray("CompositeInvariant"),
                    (await Assert.ThrowsAsync<InvalidOperationException>(
                        async ()
                            => await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues))).Message);
            }
            else
            {
                Assert.Equal(
                    RelationalStrings.ParameterNotObjectArray("CompositeInvariant"),
                    Assert.Throws<InvalidOperationException>(
                            ()
                                => ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues))
                        .Message);
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Disposes_command_on_exception(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var exception = new InvalidOperationException();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    c => throw exception,
                    c => throw exception,
                    (c, cb) => throw exception,
                    (c, ct) => throw exception,
                    (c, ct) => throw exception,
                    (c, cb, ct) => throw exception));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var fakeConnection = new FakeRelationalConnection(options);

            var relationalCommand = CreateRelationalCommand();

            if (async)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async ()
                        => await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, null));
            }
            else
            {
                Assert.Throws<InvalidOperationException>(
                    ()
                        => ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, null));
            }

            Assert.Equal(1, fakeDbConnection.DbCommands[0].DisposeCount);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Closes_managed_connections_on_exception(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var exception = new InvalidOperationException();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    c => throw exception,
                    c => throw exception,
                    (c, cb) => throw exception,
                    (c, ct) => throw exception,
                    (c, ct) => throw exception,
                    (c, cb, ct) => throw exception));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var fakeConnection = new FakeRelationalConnection(options);

            var relationalCommand = CreateRelationalCommand();

            if (async)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async ()
                        => await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, null));

                Assert.Equal(1, fakeDbConnection.OpenAsyncCount);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(
                    ()
                        => ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, null));

                Assert.Equal(1, fakeDbConnection.OpenCount);
            }

            Assert.Equal(1, fakeDbConnection.CloseCount);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Does_not_close_unmanaged_connections_on_exception(
            Delegate commandDelegate,
            string telemetryName,
            bool async)
        {
            var exception = new InvalidOperationException();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    c => throw exception,
                    c => throw exception,
                    (c, cb) => throw exception,
                    (c, ct) => throw exception,
                    (c, ct) => throw exception,
                    (c, cb, ct) => throw exception));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var fakeConnection = new FakeRelationalConnection(options);

            var relationalCommand = CreateRelationalCommand();

            if (async)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async ()
                        => await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, null));

                Assert.Equal(1, fakeDbConnection.OpenAsyncCount);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(
                    ()
                        => ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, null));

                Assert.Equal(1, fakeDbConnection.OpenCount);
            }

            Assert.Equal(1, fakeDbConnection.CloseCount);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Logs_commands_without_parameter_values(
            Delegate commandDelegate,
            string diagnosticName,
            bool async)
        {
            var options = CreateOptions();

            var log = new List<Tuple<LogLevel, EventId, string>>();

            var fakeConnection = new FakeRelationalConnection(options);

            var relationalCommand = CreateRelationalCommand(
                new DiagnosticsLogger<DbLoggerCategory.Database.Command>(
                    new ListLoggerFactory(log),
                    new FakeLoggingOptions(false),
                    new DiagnosticListener("Fake")),
                commandText: "Logged Command",
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false)
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "FirstInvariant", 17 }
            };

            if (async)
            {
                await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }
            else
            {
                ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }

            Assert.Equal(2, log.Count);

            Assert.Equal(LogLevel.Debug, log[0].Item1);
            Assert.Equal(LogLevel.Information, log[1].Item1);

            foreach (var item in log)
            {
                Assert.EndsWith(
                    @"[Parameters=[FirstParameter='?'], CommandType='0', CommandTimeout='30']
Logged Command",
                    item.Item3.Replace(Environment.NewLine, FileLineEnding));
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Logs_commands_parameter_values(
            Delegate commandDelegate,
            string diagnosticName,
            bool async)
        {
            var optionsExtension = new FakeRelationalOptionsExtension().WithConnectionString(ConnectionString);

            var options = CreateOptions(optionsExtension);

            var log = new List<Tuple<LogLevel, EventId, string>>();

            var fakeConnection = new FakeRelationalConnection(options);

            var relationalCommand = CreateRelationalCommand(
                new DiagnosticsLogger<DbLoggerCategory.Database.Command>(
                    new ListLoggerFactory(log),
                    new FakeLoggingOptions(true),
                    new DiagnosticListener("Fake")),
                commandText: "Logged Command",
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false)
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "FirstInvariant", 17 }
            };

            if (async)
            {
                await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }
            else
            {
                ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }

            Assert.Equal(3, log.Count);
            Assert.Equal(LogLevel.Warning, log[0].Item1);
            Assert.Equal(CoreStrings.LogSensitiveDataLoggingEnabled.GenerateMessage(), log[0].Item3);

            Assert.Equal(LogLevel.Debug, log[1].Item1);
            Assert.Equal(LogLevel.Information, log[2].Item1);

            foreach (var item in log.Skip(1))
            {
                Assert.EndsWith(
                    @"[Parameters=[FirstParameter='17'], CommandType='0', CommandTimeout='30']
Logged Command",
                    item.Item3.Replace(Environment.NewLine, FileLineEnding));
            }
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Reports_command_diagnostic(
            Delegate commandDelegate,
            DbCommandMethod diagnosticName,
            bool async)
        {
            var options = CreateOptions();

            var fakeConnection = new FakeRelationalConnection(options);

            var diagnostic = new List<Tuple<string, object>>();

            var relationalCommand = CreateRelationalCommand(
                new DiagnosticsLogger<DbLoggerCategory.Database.Command>(
                    new ListLoggerFactory(new List<Tuple<LogLevel, EventId, string>>()),
                    new FakeLoggingOptions(false),
                    new ListDiagnosticSource(diagnostic)),
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false)
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "FirstInvariant", 17 }
            };

            if (async)
            {
                await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }
            else
            {
                ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues);
            }

            Assert.Equal(2, diagnostic.Count);
            Assert.Equal(RelationalEventId.CommandExecuting.Name, diagnostic[0].Item1);
            Assert.Equal(RelationalEventId.CommandExecuted.Name, diagnostic[1].Item1);

            var beforeData = (CommandEventData)diagnostic[0].Item2;
            var afterData = (CommandExecutedEventData)diagnostic[1].Item2;

            Assert.Equal(fakeConnection.DbConnections[0].DbCommands[0], beforeData.Command);
            Assert.Equal(fakeConnection.DbConnections[0].DbCommands[0], afterData.Command);

            Assert.Equal(diagnosticName, beforeData.ExecuteMethod);
            Assert.Equal(diagnosticName, afterData.ExecuteMethod);

            Assert.Equal(async, beforeData.IsAsync);
            Assert.Equal(async, afterData.IsAsync);
        }

        [Theory]
        [MemberData(nameof(CommandActions))]
        public async Task Reports_command_diagnostic_on_exception(
            Delegate commandDelegate,
            DbCommandMethod diagnosticName,
            bool async)
        {
            var exception = new InvalidOperationException();

            var fakeDbConnection = new FakeDbConnection(
                ConnectionString,
                new FakeCommandExecutor(
                    c => throw exception,
                    c => throw exception,
                    (c, cb) => throw exception,
                    (c, ct) => throw exception,
                    (c, ct) => throw exception,
                    (c, cb, ct) => throw exception));

            var optionsExtension = new FakeRelationalOptionsExtension().WithConnection(fakeDbConnection);

            var options = CreateOptions(optionsExtension);

            var diagnostic = new List<Tuple<string, object>>();

            var fakeConnection = new FakeRelationalConnection(options);

            var relationalCommand = CreateRelationalCommand(
                new DiagnosticsLogger<DbLoggerCategory.Database.Command>(
                    new ListLoggerFactory(new List<Tuple<LogLevel, EventId, string>>()),
                    new FakeLoggingOptions(false),
                    new ListDiagnosticSource(diagnostic)),
                parameters: new[]
                {
                    new TypeMappedRelationalParameter("FirstInvariant", "FirstParameter", new IntTypeMapping("int", DbType.Int32), false)
                });

            var parameterValues = new Dictionary<string, object>
            {
                { "FirstInvariant", 17 }
            };

            if (async)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async ()
                        => await ((CommandFunc)commandDelegate)(fakeConnection, relationalCommand, parameterValues));
            }
            else
            {
                Assert.Throws<InvalidOperationException>(
                    ()
                        => ((CommandAction)commandDelegate)(fakeConnection, relationalCommand, parameterValues));
            }

            Assert.Equal(2, diagnostic.Count);
            Assert.Equal(RelationalEventId.CommandExecuting.Name, diagnostic[0].Item1);
            Assert.Equal(RelationalEventId.CommandError.Name, diagnostic[1].Item1);

            var beforeData = (CommandEventData)diagnostic[0].Item2;
            var afterData = (CommandErrorEventData)diagnostic[1].Item2;

            Assert.Equal(fakeDbConnection.DbCommands[0], beforeData.Command);
            Assert.Equal(fakeDbConnection.DbCommands[0], afterData.Command);

            Assert.Equal(diagnosticName, beforeData.ExecuteMethod);
            Assert.Equal(diagnosticName, afterData.ExecuteMethod);

            Assert.Equal(async, beforeData.IsAsync);
            Assert.Equal(async, afterData.IsAsync);

            Assert.Equal(exception, afterData.Exception);
        }

        private const string ConnectionString = "Fake Connection String";

        private static FakeRelationalConnection CreateConnection(IDbContextOptions options = null)
            => new FakeRelationalConnection(options ?? CreateOptions());

        private static IDbContextOptions CreateOptions(
            RelationalOptionsExtension optionsExtension = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(
                    optionsExtension
                    ?? new FakeRelationalOptionsExtension().WithConnectionString(ConnectionString));

            return optionsBuilder.Options;
        }

        private class FakeLoggingOptions : ILoggingOptions
        {
            public FakeLoggingOptions(bool sensitiveDataLoggingEnabled)
            {
                IsSensitiveDataLoggingEnabled = sensitiveDataLoggingEnabled;
            }

            public void Initialize(IDbContextOptions options)
            {
            }

            public void Validate(IDbContextOptions options)
            {
            }

            public bool IsSensitiveDataLoggingEnabled { get; }
            public bool IsSensitiveDataLoggingWarned { get; set; }
            public WarningsConfiguration WarningsConfiguration => null;
        }

        private IRelationalCommand CreateRelationalCommand(
            IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger = null,
            string commandText = "Command Text",
            IReadOnlyList<IRelationalParameter> parameters = null)
            => new RelationalCommand(
                logger ?? new FakeDiagnosticsLogger<DbLoggerCategory.Database.Command>(),
                commandText,
                parameters ?? new IRelationalParameter[0]);
    }
}
