using Q.FilterBuilder.IntegrationTests.Database.Models;

namespace Q.FilterBuilder.IntegrationTests.Services;

/// <summary>
/// Interface for ORM-specific query execution using new extension method results
/// </summary>
public interface IOrmExecutionService
{
    /// <summary>
    /// Execute query using Entity Framework Core with raw SQL and parameters
    /// </summary>
    Task<List<User>> ExecuteWithEntityFrameworkAsync(string formattedQuery, object[] parameters);

    /// <summary>
    /// Execute query using Dapper with parameter dictionary
    /// </summary>
    Task<List<dynamic>> ExecuteWithDapperAsync(string whereClause, Dictionary<string, object?> parameters);

    /// <summary>
    /// Execute query using ADO.NET with parameter values
    /// </summary>
    Task<List<dynamic>> ExecuteWithAdoNetAsync(string whereClause, object[] parameters);
}
