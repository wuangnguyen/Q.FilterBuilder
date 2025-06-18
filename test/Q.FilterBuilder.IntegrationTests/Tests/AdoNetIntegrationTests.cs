using Q.FilterBuilder.IntegrationTests.Infrastructure;
using Xunit;

namespace Q.FilterBuilder.IntegrationTests.Tests;

/// <summary>
/// Integration tests for comprehensive complex scenario executed specifically with ADO.NET
/// Tests the same comprehensive complex scenario as EFCoreIntegrationTests but executed with ADO.NET
/// </summary>
public class AdoNetIntegrationTests : IntegrationTestBase
{
    private readonly JsonTestDataLoader _jsonLoader;

    public AdoNetIntegrationTests(IntegrationTestWebApplicationFactory factory, DatabaseContainerFixture containerFixture)
        : base(factory, containerFixture)
    {
        _jsonLoader = new JsonTestDataLoader();
    }

    [Fact]
    public async Task SimpleUserFilter_AdoNet_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-string-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-adonet-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should execute string operations through ADO.NET
        Assert.Contains("John Doe", result);
        Assert.Contains("@company.com", result);
    }

    [Fact]
    public async Task ComplexNestedFilter_AdoNet_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-nested-groups");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-adonet-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle complex nested groups through ADO.NET
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public async Task NumericOperations_AdoNet_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-numeric-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-adonet-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle numeric comparisons through ADO.NET
        Assert.Contains("John Doe", result);
        Assert.Contains("Alice Brown", result);
    }

    [Fact]
    public async Task DateTimeOperations_AdoNet_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-datetime-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-adonet-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle DateTime operations through ADO.NET
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ArrayOperations_AdoNet_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-array-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-adonet-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle IN/NOT IN operations through ADO.NET
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task BooleanAndNullOperations_AdoNet_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-boolean-null-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-adonet-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle boolean and null checks through ADO.NET
        Assert.Contains("John Doe", result);
        Assert.Contains("Jane Smith", result);
        Assert.Contains("Bob Johnson", result);
        Assert.Contains("Alice Brown", result);
    }

    [Fact]
    public async Task BuildQuery_AdoNet_ShouldReturnValidSqlQuery()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("comprehensive-orm-test");

        // Act
        var queryResult = await ExecuteFilterForQueryAsync(filterJson);

        // Assert
        Assert.NotNull(queryResult);
        Assert.NotEmpty(queryResult.Query);
        Assert.NotEmpty(queryResult.Parameters);
        Assert.True(queryResult.Parameters.Length == 8);
    }

    [Fact]
    public async Task ParameterBinding_AdoNet_ShouldHandleCorrectly()
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
        Assert.True(queryResult.Query.Contains("@") || queryResult.Query.Contains("?") || queryResult.Query.Contains("$"));

        // Verify parameter count matches expected operations
        Assert.True(queryResult.Parameters.Length >= 6);
    }

    [Fact]
    public async Task ConnectionManagement_AdoNet_ShouldHandleCorrectly()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-string-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-adonet-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle connection lifecycle properly
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task PerformanceTest_AdoNet_ShouldExecuteWithinReasonableTime()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-mixed-operators");
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-adonet-users", filterJson);

        // Assert
        var endTime = DateTime.UtcNow;
        var executionTime = endTime - startTime;

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should execute within reasonable time (adjust threshold as needed)
        Assert.True(executionTime.TotalSeconds < 30, $"ADO.NET execution took too long: {executionTime.TotalSeconds} seconds");
    }

    [Fact]
    public async Task CrossTableJoin_UserWithProductName_AdoNet_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("user-product-cross-table");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-adonet-users-cross-table", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should return users who have at least one product named 'Test Product A'
        Assert.Contains("Test Product A", result); // The result should include the product name in the user data
    }

    public override async Task DisposeAsync()
    {
        _jsonLoader?.Dispose();
        await base.DisposeAsync();
    }
}
