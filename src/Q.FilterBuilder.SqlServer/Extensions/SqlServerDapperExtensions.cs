using System.Collections.Generic;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

namespace Q.FilterBuilder.SqlServer.Extensions;

/// <summary>
/// SQL Server-specific Dapper extension methods for IFilterBuilder.
/// These extensions provide optimized parameter handling for SQL Server with Dapper.
/// </summary>
public static class SqlServerDapperExtensions
{
    /// <summary>
    /// Builds a query result specifically formatted for Dapper with SQL Server.
    /// SQL Server parameters (@p0, @p1, etc.) are already compatible with Dapper.
    /// </summary>
    /// <param name="filterBuilder">The filter builder instance.</param>
    /// <param name="group">The FilterGroup to build the query from.</param>
    /// <returns>A tuple with the query and parameter dictionary</returns>
    /// <example>
    /// <code>
    /// var (query, parameters) = filterBuilder.BuildForSqlServerDapper(filterGroup);
    /// var users = await connection.QueryAsync($"SELECT * FROM Users WHERE {query}", parameters);
    /// </code>
    /// </example>
    public static (string query, Dictionary<string, object?> parameters) BuildForDapper(this IFilterBuilder filterBuilder, FilterGroup group)
    {
        var (query, parameters) = filterBuilder.Build(group);
        var dapperParameters = ConvertToDapperParameters(parameters);
        return (query, dapperParameters);
    }

    /// <summary>
    /// Converts SQL Server parameters to Dapper-compatible format.
    /// </summary>
    /// <param name="parameterValues"></param>
    /// <returns></returns>
    private static Dictionary<string, object?> ConvertToDapperParameters(object[] parameterValues)
    {
        var paramDict = new Dictionary<string, object?>();
        
        for (var i = 0; i < parameterValues.Length; i++)
        {
            paramDict[$"@p{i}"] = parameterValues[i];
        }
        
        return paramDict;
    }
}
