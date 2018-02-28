// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable SuggestBaseTypeForParameter

namespace Microsoft.EntityFrameworkCore.Utilities
{
    public class SqlServerTestStore : RelationalTestStore
    {
        private const string Northwind = "Northwind";

        public const int CommandTimeout = 600;

        private static string BaseDirectory => AppContext.BaseDirectory;

        public static readonly string NorthwindConnectionString = CreateConnectionString(Northwind);

        public static SqlServerTestStore GetNorthwindStore()
            => GetOrCreateShared(
                Northwind,
                () => ExecuteScript(
                    Northwind,
                    Path.Combine(
                        Path.GetDirectoryName(typeof(SqlServerTestStore).GetTypeInfo().Assembly.Location),
                        "Northwind.sql")),
                cleanDatabase: false);

        public static SqlServerTestStore GetOrCreateShared(string name, Action initializeDatabase, bool cleanDatabase = true)
            => new SqlServerTestStore(name, cleanDatabase: cleanDatabase).CreateShared(initializeDatabase);

        public static SqlServerTestStore Create(string name, bool deleteDatabase = false)
            => new SqlServerTestStore(name).CreateTransient(true, deleteDatabase);

        public static SqlServerTestStore CreateScratch(bool createDatabase = true, bool useFileName = false)
            => new SqlServerTestStore(GetScratchDbName(), useFileName).CreateTransient(createDatabase, true);

        private SqlConnection _connection;
        private readonly string _fileName;
        private readonly bool _cleanDatabase;
        private string _connectionString;
        private bool _deleteDatabase;

        public string Name { get; }
        public override string ConnectionString => _connectionString;

        private SqlServerTestStore(string name, bool useFileName = false, bool cleanDatabase = true)
        {
            Name = name;

            if (useFileName)
            {
                _fileName = Path.Combine(BaseDirectory, name + ".mdf");
            }

            _cleanDatabase = cleanDatabase;
        }

        private static string GetScratchDbName()
        {
            string name;
            do
            {
                name = "Scratch_" + Guid.NewGuid();
            }
            while (DatabaseExists(name)
                   || DatabaseFilesExist(name));

            return name;
        }

        private SqlServerTestStore CreateShared(Action initializeDatabase)
        {
            _connectionString = CreateConnectionString(Name, _fileName);
            _connection = new SqlConnection(_connectionString);

            CreateShared(typeof(SqlServerTestStore).Name + Name,
                () =>
                    {
                        if (CreateDatabase())
                        {
                            initializeDatabase?.Invoke();
                        }
                    });

            return this;
        }

        private bool CreateDatabase()
        {
            using (var master = new SqlConnection(CreateConnectionString("master", false)))
            {
                if (DatabaseExists(Name))
                {
                    if (!_cleanDatabase)
                    {
                        return false;
                    }

                    Clean(Name);
                }
                else
                {
                    ExecuteNonQuery(master, GetCreateDatabaseStatement(Name, _fileName));
                    WaitForExists(_connection);
                }
            }

            return true;
        }

        public static void ExecuteScript(string databaseName, string scriptPath)
        {
            // HACK: Probe for script file as current dir
            // is different between k build and VS run.
            if (File.Exists(@"..\..\" + scriptPath))
            {
                //executing in VS - so path is relative to bin\<config> dir
                scriptPath = @"..\..\" + scriptPath;
            }
            else
            {
                scriptPath = Path.Combine(BaseDirectory, scriptPath);
            }

            var script = File.ReadAllText(scriptPath);
            using (var connection = new SqlConnection(CreateConnectionString(databaseName)))
            {
                Execute(connection, command =>
                    {
                        foreach (var batch in
                            new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(1000.0))
                                .Split(script).Where(b => !string.IsNullOrEmpty(b)))
                        {
                            command.CommandText = batch;
                            command.ExecuteNonQuery();
                        }
                        return 0;
                    }, "");
            }
        }

        private static void WaitForExists(SqlConnection connection)
        {
            if (TestEnvironment.IsSqlAzure)
            {
                new TestSqlServerRetryingExecutionStrategy().Execute(connection,
                    connectionScoped => WaitForExistsImplementation(connectionScoped));
            }
            else
            {
                WaitForExistsImplementation(connection);
            }
        }

        private static void WaitForExistsImplementation(SqlConnection connection)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }

                    SqlConnection.ClearPool(connection);

                    connection.Open();
                    connection.Close();
                    return;
                }
                catch (SqlException e)
                {
                    if (++retryCount >= 30
                        || e.Number != 233 && e.Number != -2 && e.Number != 4060 && e.Number != 1832 && e.Number != 5120)
                    {
                        throw;
                    }

                    Thread.Sleep(100);
                }
            }
        }

        private SqlServerTestStore CreateTransient(bool createDatabase, bool deleteDatabase)
        {
            _connectionString = CreateConnectionString(Name, _fileName);
            _connection = new SqlConnection(_connectionString);

            if (createDatabase)
            {
                CreateDatabase();

                OpenConnection();
            }
            else if (DatabaseExists(Name))
            {
                DeleteDatabase(Name);
            }

            _deleteDatabase = deleteDatabase;
            return this;
        }

        private static void Clean(string name)
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(CreateConnectionString(name), b => b.ApplyConfiguration())
                .UseInternalServiceProvider(
                    new ServiceCollection()
                        .AddEntityFrameworkSqlServer()
                        .BuildServiceProvider())
                .Options;

            using (var context = new DbContext(options))
            {
                context.Database.EnsureClean();
            }
        }

        private static string GetCreateDatabaseStatement(string name, string fileName)
        {
            var result = $"CREATE DATABASE [{name}]";

            if (TestEnvironment.IsSqlAzure)
            {
                var elasticGroupName = TestEnvironment.ElasticPoolName;
                result += Environment.NewLine +
                          (string.IsNullOrEmpty(elasticGroupName)
                              ? " ( Edition = 'basic' )"
                              : $" ( SERVICE_OBJECTIVE = ELASTIC_POOL ( name = {elasticGroupName} ) )");
            }
            else
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    var logFileName = Path.ChangeExtension(fileName, ".ldf");
                    result += Environment.NewLine +
                              $" ON (NAME = '{name}', FILENAME = '{fileName}')" +
                              $" LOG ON (NAME = '{name}_log', FILENAME = '{logFileName}')";
                }
            }
            return result;
        }

        private static bool DatabaseExists(string name)
        {
            using (var master = new SqlConnection(CreateConnectionString("master")))
            {
                return ExecuteScalar<int>(master, $@"SELECT COUNT(*) FROM sys.databases WHERE name = N'{name}'") > 0;
            }
        }

        private static bool DatabaseFilesExist(string name)
        {
            var userFolder = Environment.GetEnvironmentVariable("USERPROFILE") ?? Environment.GetEnvironmentVariable("HOME");
            return userFolder != null
                   && (File.Exists(Path.Combine(userFolder, name + ".mdf"))
                       || File.Exists(Path.Combine(userFolder, name + "_log.ldf")));
        }

        private static void DeleteDatabase(string name)
        {
            using (var master = new SqlConnection(CreateConnectionString("master")))
            {
                ExecuteNonQuery(master, GetDeleteDatabaseSql(name));

                SqlConnection.ClearAllPools();
            }
        }

        private static string GetDeleteDatabaseSql(string name)
            // SET SINGLE_USER will close any open connections that would prevent the drop
            => string.Format(@"IF EXISTS (SELECT * FROM sys.databases WHERE name = N'{0}')
                                          BEGIN
                                              ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                                              DROP DATABASE [{0}];
                                          END", name);

        public override DbConnection Connection => _connection;

        public override DbTransaction Transaction => null;

        public override void OpenConnection()
        {
            if (TestEnvironment.IsSqlAzure)
            {
                new TestSqlServerRetryingExecutionStrategy().Execute(_connection, connection => connection.Open());
            }
            else
            {
                _connection.Open();
            }
        }

        public Task OpenConnectionAsync()
            => TestEnvironment.IsSqlAzure
                ? new TestSqlServerRetryingExecutionStrategy().ExecuteAsync(_connection, connection => connection.OpenAsync())
                : _connection.OpenAsync();

        public T ExecuteScalar<T>(string sql, params object[] parameters)
            => ExecuteScalar<T>(_connection, sql, parameters);

        private static T ExecuteScalar<T>(SqlConnection connection, string sql, params object[] parameters)
            => Execute(connection, command => (T)command.ExecuteScalar(), sql, false, parameters);

        public Task<T> ExecuteScalarAsync<T>(string sql, params object[] parameters)
            => ExecuteScalarAsync<T>(_connection, sql, parameters);

        private static Task<T> ExecuteScalarAsync<T>(SqlConnection connection, string sql, IReadOnlyList<object> parameters = null)
            => ExecuteAsync(connection, async command => (T)await command.ExecuteScalarAsync(), sql, false, parameters);

        public int ExecuteNonQuery(string sql, params object[] parameters)
            => ExecuteNonQuery(_connection, sql, parameters);

        private static int ExecuteNonQuery(SqlConnection connection, string sql, object[] parameters = null)
            => Execute(connection, command => command.ExecuteNonQuery(), sql, false, parameters);

        public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
            => ExecuteNonQueryAsync(_connection, sql, parameters);

        private static Task<int> ExecuteNonQueryAsync(SqlConnection connection, string sql, IReadOnlyList<object> parameters = null)
            => ExecuteAsync(connection, command => command.ExecuteNonQueryAsync(), sql, false, parameters);

        public IEnumerable<T> Query<T>(string sql, params object[] parameters)
            => Query<T>(_connection, sql, parameters);

        private static IEnumerable<T> Query<T>(SqlConnection connection, string sql, object[] parameters = null)
            => Execute(connection, command =>
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        var results = Enumerable.Empty<T>();
                        while (dataReader.Read())
                        {
                            results = results.Concat(new[] { dataReader.GetFieldValue<T>(0) });
                        }
                        return results;
                    }
                }, sql, false, parameters);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] parameters)
            => QueryAsync<T>(_connection, sql, parameters);

        private static Task<IEnumerable<T>> QueryAsync<T>(SqlConnection connection, string sql, object[] parameters = null)
            => ExecuteAsync(connection, async command =>
                {
                    using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        var results = Enumerable.Empty<T>();
                        while (await dataReader.ReadAsync())
                        {
                            results = results.Concat(new[] { await dataReader.GetFieldValueAsync<T>(0) });
                        }
                        return results;
                    }
                }, sql, false, parameters);

        private static T Execute<T>(
            SqlConnection connection, Func<DbCommand, T> execute, string sql,
            bool useTransaction = false, object[] parameters = null)
            => TestEnvironment.IsSqlAzure
                ? new TestSqlServerRetryingExecutionStrategy().Execute(new { connection, execute, sql, useTransaction, parameters },
                    state => ExecuteCommand(state.connection, state.execute, state.sql, state.useTransaction, state.parameters))
                : ExecuteCommand(connection, execute, sql, useTransaction, parameters);

        private static T ExecuteCommand<T>(
            SqlConnection connection, Func<DbCommand, T> execute, string sql, bool useTransaction, object[] parameters)
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
            connection.Open();
            try
            {
                using (var transaction = useTransaction ? connection.BeginTransaction() : null)
                {
                    T result;
                    using (var command = CreateCommand(connection, sql, parameters))
                    {
                        command.Transaction = transaction;
                        result = execute(command);
                    }
                    transaction?.Commit();

                    return result;
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Closed
                    && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private static Task<T> ExecuteAsync<T>(
            SqlConnection connection, Func<DbCommand, Task<T>> executeAsync, string sql,
            bool useTransaction = false, IReadOnlyList<object> parameters = null)
            => TestEnvironment.IsSqlAzure
                ? new TestSqlServerRetryingExecutionStrategy().ExecuteAsync(
                    new { connection, executeAsync, sql, useTransaction, parameters },
                    state => ExecuteCommandAsync(state.connection, state.executeAsync, state.sql, state.useTransaction, state.parameters))
                : ExecuteCommandAsync(connection, executeAsync, sql, useTransaction, parameters);

        private static async Task<T> ExecuteCommandAsync<T>(
            SqlConnection connection, Func<DbCommand, Task<T>> executeAsync, string sql, bool useTransaction, IReadOnlyList<object> parameters)
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
            await connection.OpenAsync();
            try
            {
                using (var transaction = useTransaction ? connection.BeginTransaction() : null)
                {
                    T result;
                    using (var command = CreateCommand(connection, sql, parameters))
                    {
                        result = await executeAsync(command);
                    }
                    transaction?.Commit();

                    return result;
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Closed
                    && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private static DbCommand CreateCommand(
            SqlConnection connection, string commandText, IReadOnlyList<object> parameters = null)
        {
            var command = connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = CommandTimeout;

            if (parameters != null)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    command.Parameters.AddWithValue("p" + i, parameters[i]);
                }
            }

            return command;
        }

        public override void Dispose()
        {
            _connection.Dispose();

            if (_deleteDatabase)
            {
                DeleteDatabase(Name);
            }
        }

        public static string CreateConnectionString(string name)
            => CreateConnectionString(name, null, new Random().Next(0, 2) == 1);

        public static string CreateConnectionString(string name, string fileName)
            => CreateConnectionString(name, fileName, new Random().Next(0, 2) == 1);

        private static string CreateConnectionString(string name, bool multipleActiveResultSets)
            => CreateConnectionString(name, null, multipleActiveResultSets);

        private static string CreateConnectionString(string name, string fileName, bool multipleActiveResultSets)
        {
            var builder = new SqlConnectionStringBuilder(TestEnvironment.DefaultConnection)
            {
                MultipleActiveResultSets = multipleActiveResultSets,
                InitialCatalog = name
            };
            if (fileName != null)
            {
                builder.AttachDBFilename = fileName;
            }

            return builder.ToString();
        }
    }
}
