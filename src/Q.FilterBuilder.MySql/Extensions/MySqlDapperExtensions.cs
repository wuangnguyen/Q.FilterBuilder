using System.Collections.Generic;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

namespace Q.FilterBuilder.MySql.Extensions;

/// <summary>
/// MySQL-specific extension methods for Dapper integration.
/// These extensions handle MySQL's positional parameter requirements while maintaining Dapper compatibility.
/// </summary>
public static class MySqlDapperExtensions
{
    /// <summary>
    /// Builds a query result specifically optimized for MySQL Dapper operations.
    /// Handles MySQL's positional parameter requirements by converting the query to use positional parameters
    /// while providing unique parameter names in the dictionary for Dapper compatibility.
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
        
        // For MySQL, we need to convert the query to use positional parameters
        // but still provide unique parameter names for Dapper
        return ConvertToDapperFormat(query, parameters);
    }

    /// <summary>
    /// Converts a query with named parameters to MySQL positional parameters
    /// while maintaining unique parameter names for Dapper compatibility.
    /// </summary>
    /// <param name="query">The original query with ? placeholders</param>
    /// <param name="parameters">The parameter values</param>
    /// <returns>A tuple with the MySQL-compatible query and parameter dictionary</returns>
    private static (string query, Dictionary<string, object?> parameters) ConvertToDapperFormat(
        string query, object[] parameters)
    {
        var paramDict = new Dictionary<string, object?>();
        
        // MySQL uses positional parameters (?), so the query should already be correct
        // We just need to create unique parameter names for Dapper
        for (var i = 0; i < parameters.Length; i++)
        {
            paramDict[$"p{i}"] = parameters[i];
        }
        
        return (query, paramDict);
    }
}
