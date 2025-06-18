using Microsoft.AspNetCore.Mvc;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.IntegrationTests.Services;
using Q.FilterBuilder.MySql.Extensions;
using Q.FilterBuilder.PostgreSql.Extensions;
using Q.FilterBuilder.SqlServer.Extensions;

namespace Q.FilterBuilder.IntegrationTests.Controllers;

/// <summary>
/// controller for integration testing of FilterBuilder functionality
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class IntegrationTestController : ControllerBase
{
    private readonly IFilterBuilder _filterBuilder;
    private readonly IOrmExecutionService _ormService;
    private readonly TestConfiguration _testConfig;

    public IntegrationTestController(
        IFilterBuilder filterBuilder,
        IOrmExecutionService ormService,
        TestConfiguration testConfig)
    {
        _filterBuilder = filterBuilder;
        _ormService = ormService;
        _testConfig = testConfig;
    }

    /// <summary>
    /// Execute filter using Entity Framework Core
    /// </summary>
    [HttpPost("execute-efcore-users")]
    public async Task<IActionResult> ExecuteEFCoreUsers([FromBody] FilterGroup filterGroup)
    {
        try
        {
            var (query, parameters) = BuildForEf(filterGroup);
            var tableName = FormatFieldName("Users");
            var sql = $"SELECT * FROM {tableName} WHERE {query}";
            var results = await _ormService.ExecuteWithEntityFrameworkAsync(sql, parameters);

            return Ok(new
            {
                Query = query,
                Parameters = parameters,
                Results = results,
                results.Count,
                ORM = "EntityFramework"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message, ex.StackTrace });
        }
    }

    /// <summary>
    /// Execute filter using Dapper
    /// </summary>
    [HttpPost("execute-dapper-users")]
    public async Task<IActionResult> ExecuteDapperUsers([FromBody] FilterGroup filterGroup)
    {
        try
        {
            var (query, parameters) = BuildForDapper(filterGroup);
            var tableName = FormatFieldName("Users");
            var sql = $"SELECT * FROM {tableName} WHERE {query}";
            var results = await _ormService.ExecuteWithDapperAsync(sql, parameters);

            return Ok(new
            {
                WhereClause = query,
                Parameters = parameters,
                Results = results,
                results.Count,
                ORM = "Dapper"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message,
                ex.StackTrace,
                ExceptionType = ex.GetType().Name,
                InnerException = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Execute filter using ADO.NET
    /// </summary>
    [HttpPost("execute-adonet-users")]
    public async Task<IActionResult> ExecuteAdoNetUsers([FromBody] FilterGroup filterGroup)
    {
        try
        {
            // Use the new ADO.NET extension method
            var (query, parameters) = _filterBuilder.Build(filterGroup);
            var tableName = FormatFieldName("Users");
            var sql = $"SELECT * FROM {tableName} WHERE {query}";
            var results = await _ormService.ExecuteWithAdoNetAsync(sql, parameters);

            return Ok(new
            {
                Results = results,
                results.Count,
                ORM = "ADO.NET"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message, ex.StackTrace });
        }
    }

    /// <summary>
    /// Execute filter using ADO.NET with cross-table join
    /// </summary>
    [HttpPost("execute-adonet-users-cross-table")]
    public async Task<IActionResult> ExecuteAdoNetUsersCrossTable([FromBody] FilterGroup filterGroup)
    {
        try
        {
            // Build the filter for the cross-table join
            var (query, parameters) = _filterBuilder.Build(filterGroup);
            var usersTable = FormatFieldName("Users");
            var productsTable = FormatFieldName("Products");

            // Always join Products for this endpoint
            var sql = $@"SELECT DISTINCT {usersTable}.* FROM {usersTable}
                INNER JOIN {productsTable} ON {FormatFieldName("Products.CreatedByUserId")} = {FormatFieldName("Users.Id")}
                WHERE {query}";
            var results = await _ormService.ExecuteWithAdoNetAsync(sql, parameters);
            return Ok(new
            {
                Sql = sql,
                Query = query,
                Parameters = parameters,
                Results = results,
                results.Count,
                ORM = "ADO.NET"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message, ex.StackTrace });
        }
    }

    /// <summary>
    /// Build query and return query string and parameters without execution
    /// </summary>
    [HttpPost("build-query")]
    public IActionResult BuildQuery([FromBody] FilterGroup filterGroup)
    {
        try
        {
            if (filterGroup == null)
            {
                return BadRequest(new { Error = "FilterGroup is null", Details = "Request body could not be deserialized to FilterGroup" });
            }

            var (query, parameters) = _filterBuilder.Build(filterGroup);

            return Ok(new
            {
                Query = query,
                WhereClause = query,
                Parameters = parameters
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message,
                ex.StackTrace,
                ExceptionType = ex.GetType().Name,
                InnerException = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Formats the specified field name according to the query format provider's rules.
    /// </summary>
    /// <param name="fieldName">The name of the field to format. Cannot be null or empty.</param>
    /// <returns>A string representing the formatted field name.</returns>
    private string FormatFieldName(string fieldName)
    {
        // Use the FilterBuilder's QueryFormatProvider to format filed names consistently
        return _filterBuilder.QueryFormatProvider.FormatFieldName(fieldName);
    }

    /// <summary>
    /// Build query for Dapper using appropriate provider-specific extension method
    /// </summary>
    /// <param name="filterGroup"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private (string query, Dictionary<string, object?> parameters) BuildForDapper(FilterGroup filterGroup)
    {
        var provider = _testConfig.GetDatabaseProvider();
        return provider switch
        {
            DatabaseProvider.MySql => MySqlDapperExtensions.BuildForDapper(_filterBuilder, filterGroup),
            DatabaseProvider.PostgreSql => PostgreSqlDapperExtensions.BuildForDapper(_filterBuilder, filterGroup),
            DatabaseProvider.SqlServer => SqlServerDapperExtensions.BuildForDapper(_filterBuilder, filterGroup),
            _ => throw new ArgumentException($"Unsupported provider: {provider}")
        };
    }

    private (string query, object[] parameters) BuildForEf(FilterGroup filterGroup)
    {
        var provider = _testConfig.GetDatabaseProvider();
        return provider switch
        {
            DatabaseProvider.MySql => MySqlEfExtensions.BuildForEf(_filterBuilder, filterGroup),
            DatabaseProvider.PostgreSql => PostgreSqlEfExtensions.BuildForEf(_filterBuilder, filterGroup),
            DatabaseProvider.SqlServer => SqlServerEfExtensions.BuildForEf(_filterBuilder, filterGroup),
            _ => throw new ArgumentException($"Unsupported provider: {provider}")
        };
    }
}
