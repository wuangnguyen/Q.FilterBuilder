using Q.FilterBuilder.IntegrationTests.Infrastructure;
using Xunit;

namespace Q.FilterBuilder.IntegrationTests.Tests;

/// <summary>
/// Integration tests for comprehensive complex scenario executed specifically with Entity Framework Core
/// Tests complete flow from JSON input through FilterBuilder to EF Core query execution
/// </summary>
public class EfIntegrationTests : IntegrationTestBase
{
    private readonly JsonTestDataLoader _jsonLoader;

    public EfIntegrationTests(IntegrationTestWebApplicationFactory factory, DatabaseContainerFixture containerFixture)
        : base(factory, containerFixture)
    {
        _jsonLoader = new JsonTestDataLoader();
    }

    [Fact]
    public async Task SimpleUserFilter_EFCore_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-string-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-efcore-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should execute string operations through EF Core
        Assert.Contains("John Doe", result);
        Assert.Contains("@company.com", result);
    }

    [Fact]
    public async Task ComplexNestedFilter_EFCore_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-nested-groups");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-efcore-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle complex nested groups through EF Core
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task NumericOperations_EFCore_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-numeric-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-efcore-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle numeric comparisons through EF Core
        Assert.Contains("John Doe", result);
        Assert.Contains("Alice Brown", result);
    }

    [Fact]
    public async Task DateTimeOperations_EFCore_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-datetime-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-efcore-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle DateTime operations through EF Core
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ArrayOperations_EFCore_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-array-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-efcore-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle IN/NOT IN operations through EF Core
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task BooleanAndNullOperations_EFCore_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-boolean-null-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-efcore-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should handle boolean and null checks through EF Core
        Assert.Contains("John Doe", result);
        Assert.Contains("Jane Smith", result);
        Assert.Contains("Bob Johnson", result);
        Assert.Contains("Alice Brown", result);
    }

    [Fact]
    public async Task PerformanceTest_EFCore_ShouldExecuteWithinReasonableTime()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-mixed-operators");
        var startTime = DateTime.UtcNow;

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-efcore-users", filterJson);

        // Assert
        var endTime = DateTime.UtcNow;
        var executionTime = endTime - startTime;

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should execute within reasonable time (adjust threshold as needed)
        Assert.True(executionTime.TotalSeconds < 30, $"EF Core execution took too long: {executionTime.TotalSeconds} seconds");
    }

    public override async Task DisposeAsync()
    {
        _jsonLoader?.Dispose();
        await base.DisposeAsync();
    }
}
