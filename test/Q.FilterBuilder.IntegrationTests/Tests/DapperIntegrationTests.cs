using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.IntegrationTests.Infrastructure;
using Xunit;

namespace Q.FilterBuilder.IntegrationTests.Tests;

/// <summary>
/// Integration tests for comprehensive complex scenario executed specifically with Dapper
/// Tests the same comprehensive complex scenario as EFCoreIntegrationTests but executed with Dapper
/// </summary>
public class DapperIntegrationTests : IntegrationTestBase
{
    private readonly JsonTestDataLoader _jsonLoader;

    public DapperIntegrationTests(IntegrationTestWebApplicationFactory factory, DatabaseContainerFixture containerFixture)
        : base(factory, containerFixture)
    {
        _jsonLoader = new JsonTestDataLoader();
    }

    [Fact]
    public async Task SimpleUserFilter_Dapper_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-string-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should execute string operations through Dapper
        Assert.Contains("John Doe", result);
        Assert.Contains("@company.com", result);
    }

    [Fact]
    public async Task ComplexNestedFilter_Dapper_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-nested-groups");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle complex nested groups through Dapper
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task NumericOperations_Dapper_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-numeric-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle numeric comparisons through Dapper
        Assert.Contains("John Doe", result);
        Assert.Contains("Alice Brown", result);
    }

    [Fact]
    public async Task DateTimeOperations_Dapper_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-datetime-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle DateTime operations through Dapper
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ArrayOperations_Dapper_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-array-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle IN/NOT IN operations through Dapper
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task BooleanAndNullOperations_Dapper_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-boolean-null-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle boolean and null checks through Dapper
        Assert.Contains("John Doe", result);
        Assert.Contains("Jane Smith", result);
        Assert.Contains("Bob Johnson", result);
        Assert.Contains("Alice Brown", result);
    }

    [Fact]
    public async Task ParameterBinding_Dapper_ShouldHandleCorrectly()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-mixed-operators");

        // Act
        var queryResult = await ExecuteFilterForQueryAsync(filterJson);

        // Assert
        Assert.NotNull(queryResult);
        Assert.NotEmpty(queryResult.Query);
        Assert.NotEmpty(queryResult.Parameters);

        // Verify parameter placeholders in SQL (provider-specific)
        var expectedParameterSymbol = Provider switch
        {
            DatabaseProvider.MySql => "?",
            DatabaseProvider.PostgreSql => "$",
            _ => "@" // SQL Server and others
        };
        Assert.Contains(expectedParameterSymbol, queryResult.Query);

        // Verify parameter count matches expected operations
        Assert.True(queryResult.Parameters.Length >= 6);
    }

    [Fact]
    public async Task PerformanceTest_Dapper_ShouldExecuteWithinReasonableTime()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-mixed-operators");
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        var endTime = DateTime.UtcNow;
        var executionTime = endTime - startTime;

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should execute within reasonable time (adjust threshold as needed)
        Assert.True(executionTime.TotalSeconds < 30, $"Dapper execution took too long: {executionTime.TotalSeconds} seconds");
    }

    public override async Task DisposeAsync()
    {
        _jsonLoader?.Dispose();
        await base.DisposeAsync();
    }
}
