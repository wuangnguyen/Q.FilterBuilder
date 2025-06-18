using System.Collections.Generic;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

namespace Q.FilterBuilder.PostgreSql.Extensions;

/// <summary>
/// PostgreSQL-specific Dapper extension methods for IFilterBuilder.
/// These extensions provide optimized parameter handling for PostgreSQL with Dapper.
/// </summary>
public static class PostgreSqlDapperExtensions
{
    /// <summary>
    /// Builds a query result specifically formatted for Dapper with PostgreSQL.
    /// Converts PostgreSQL's $1, $2, etc. parameters to @p0, @p1, etc. for Dapper compatibility.
    /// </summary>
    /// <param name="filterBuilder">The filter builder instance.</param>
    /// <param name="group">The FilterGroup to build the query from.</param>
    /// <returns>A tuple with the query and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (query, parameters) = filterBuilder.BuildForDapper(filterGroup);
    /// var users = await connection.QueryAsync($"SELECT * FROM Users WHERE {query}", parameters);
    /// </code>
    /// </example>
    public static (string query, Dictionary<string, object?> parameters) BuildForDapper(this IFilterBuilder filterBuilder, FilterGroup group)
    {
        var (query, parameters) = filterBuilder.Build(group);
        var (dapperQuery, dapperParameters) = ConvertToDapperFormat(query, parameters);
        return (dapperQuery, dapperParameters);
    }

    /// <summary>
    /// Converts PostgreSQL query format to Dapper-compatible format.
    /// Changes $1, $2, etc. to @p0, @p1, etc. and creates appropriate parameter dictionary.
    /// </summary>
    /// <param name="query">The PostgreSQL query with $1, $2, etc. parameters</param>
    /// <param name="parameterValues">The parameter values</param>
    /// <returns>A tuple with Dapper-compatible query and parameter dictionary</returns>
    private static (string query, Dictionary<string, object?> parameters) ConvertToDapperFormat(
        string query, object[] parameterValues)
    {
        var paramDict = new Dictionary<string, object?>();
        var dapperQuery = query;
        
        for (var i = 0; i < parameterValues.Length; i++)
        {
            var postgresParam = $"${i + 1}"; // PostgreSQL uses 1-based indexing
            var dapperParam = $"@p{i}";      // Dapper uses 0-based indexing
            
            dapperQuery = dapperQuery.Replace(postgresParam, dapperParam);
            paramDict[dapperParam] = parameterValues[i];
        }
        
        return (dapperQuery, paramDict);
    }
}
