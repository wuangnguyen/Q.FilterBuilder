using Q.FilterBuilder.IntegrationTests.Configuration;
using Xunit;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure;

/// <summary>
/// Base class for integration tests using shared database container and web test server
/// </summary>
[Collection("DatabaseCollection")]
public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebApplicationFactory>, IAsyncLifetime
{
    protected DatabaseProvider Provider => _containerFixture.Provider;
    protected string ConnectionString => _containerFixture.ConnectionString;
    protected HttpClient Client { get; private set; } = null!;
    protected IntegrationTestWebApplicationFactory Factory { get; private set; }

    private readonly DatabaseContainerFixture _containerFixture;

    protected IntegrationTestBase(IntegrationTestWebApplicationFactory factory, DatabaseContainerFixture containerFixture)
    {
        Factory = factory;
        _containerFixture = containerFixture;
    }

    public virtual async Task InitializeAsync()
    {
        // Configure the factory with the provider and connection string from shared container
        Factory.ConfigureProvider(Provider, ConnectionString);

        // Create HTTP client
        Client = Factory.CreateClient();

        await Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        Client?.Dispose();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Execute complete integration workflow via web API
    /// </summary>
    protected async Task<T?> ExecuteFilterAsync<T>(object filterJson)
    {
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    /// <summary>
    /// Execute filter and return raw query and parameters
    /// </summary>
    protected async Task<QueryResult?> ExecuteFilterForQueryAsync(object filterJson)
    {
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/build-query", filterJson);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<QueryResult>();
    }
}

/// <summary>
/// Result of query building operation
/// </summary>
public class QueryResult
{
    public string Query { get; set; } = string.Empty;
    public string WhereClause { get; set; } = string.Empty;
    public object[] Parameters { get; set; } = Array.Empty<object>();
    public bool IsValidParameterFormat { get; set; }
}
