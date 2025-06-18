using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Npgsql;
using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.IntegrationTests.Database;
using Q.FilterBuilder.IntegrationTests.Database.Models;
using Q.FilterBuilder.MySql.Extensions;
using Q.FilterBuilder.PostgreSql.Extensions;
using Q.FilterBuilder.SqlServer.Extensions;

namespace Q.FilterBuilder.IntegrationTests.Services;

/// <summary>
/// Provider-specific ORM execution service using new core extension methods
/// </summary>
public class OrmExecutionService : IOrmExecutionService
{
    private readonly TestDbContext _context;
    private readonly TestConfiguration _testConfig;
    private readonly string _connectionString;

    public OrmExecutionService(TestDbContext context, TestConfiguration testConfig)
    {
        _context = context;
        _testConfig = testConfig;
        _connectionString = _context.Database.GetConnectionString()!;
    }

    ///<inheritdoc/>
    public async Task<List<User>> ExecuteWithEntityFrameworkAsync(string sql, object[] parameters)
    {
        return await _context.Set<User>().FromSqlRaw(sql, parameters).ToListAsync();
    }

    ///<inheritdoc/>
    public async Task<List<dynamic>> ExecuteWithDapperAsync(string sql, Dictionary<string, object?> parameters)
    {
        var connection = GetDbConnection();
        var results = await connection.QueryAsync(sql, parameters);
        return results.ToList();
    }

    ///<inheritdoc/>
    public async Task<List<dynamic>> ExecuteWithAdoNetAsync(string sql, object[] parameters)
    {
        try
        {
            var connection = GetDbConnection();

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var provider = _testConfig.GetDatabaseProvider();
            switch (provider)
            {
                case DatabaseProvider.SqlServer:
                    SqlServerAdoNetExtensions.AddParameters(command, parameters);
                    break;
                case DatabaseProvider.MySql:
                    MySqlAdoNetExtensions.AddParameters(command, parameters);
                    break;
                case DatabaseProvider.PostgreSql:
                    PostgreSqlAdoNetExtensions.AddParameters(command, parameters);
                    break;
                default:
                    throw new ArgumentException($"Unsupported provider: {provider}");
            }

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            return await ReadResultsAsync(reader);
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// Get database connection based on provider
    /// </summary>
    private DbConnection GetDbConnection()
    {
        var provider = _testConfig.GetDatabaseProvider();
        return provider switch
        {
            DatabaseProvider.SqlServer => new SqlConnection(_connectionString),
            DatabaseProvider.MySql => new MySqlConnection(_connectionString),
            DatabaseProvider.PostgreSql => new NpgsqlConnection(_connectionString),
            _ => throw new ArgumentException($"Unsupported provider: {provider}")
        };
    }

    /// <summary>
    /// Read results from ADO.NET reader into dynamic objects
    /// </summary>
    private static async Task<List<dynamic>> ReadResultsAsync(DbDataReader reader)
    {
        var results = new List<dynamic>();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            results.Add(row);
        }
        return results;
    }
}
