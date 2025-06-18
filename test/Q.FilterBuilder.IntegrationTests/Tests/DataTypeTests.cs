using Q.FilterBuilder.IntegrationTests.Infrastructure;
using Xunit;

namespace Q.FilterBuilder.IntegrationTests.Tests;

/// <summary>
/// Integration tests for all data type scenarios with various operators
/// Tests string, int, DateTime, bool, decimal, etc. with equals, contains, greater than, less than, etc.
/// </summary>
public class DataTypeTests : IntegrationTestBase
{
    private readonly JsonTestDataLoader _jsonLoader;

    public DataTypeTests(IntegrationTestWebApplicationFactory factory, DatabaseContainerFixture containerFixture)
        : base(factory, containerFixture)
    {
        _jsonLoader = new JsonTestDataLoader();
    }

    [Fact]
    public async Task StringOperations_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-string-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status {response.StatusCode}. Response: {errorContent}");
        }

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should match users based on string operations:
        // - Name equals "John Doe" OR
        // - Email contains "@company.com" OR
        // - Department starts with "Tech" OR
        // - Role ends with "Manager"
        Assert.Contains("John Doe", result); // Name match
        // All users should match email contains "@company.com"
        Assert.Contains("Jane Smith", result);
        Assert.Contains("Bob Johnson", result);
        Assert.Contains("Alice Brown", result);
    }

    [Fact]
    public async Task NumericOperations_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-numeric-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should match users with:
        // - Age >= 25 AND Age <= 40 AND
        // - Salary > 50000 AND Salary < 100000
        Assert.Contains("John Doe", result);    // Age: 30, Salary: 75000
        Assert.Contains("Alice Brown", result); // Age: 32, Salary: 85000
        // Should not contain users outside these ranges
    }

    [Fact]
    public async Task DateTimeOperations_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-datetime-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should match users with:
        // - CreatedDate >= 2023-01-01 AND CreatedDate < 2024-01-01 AND
        // - LastLoginDate is not null
        Assert.Contains("John Doe", result);
        Assert.Contains("Jane Smith", result);
        Assert.Contains("Alice Brown", result);
        Assert.DoesNotContain("Bob Johnson", result); // LastLoginDate is null
    }

    [Fact]
    public async Task BooleanAndNullOperations_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-boolean-null-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should match users with:
        // - IsActive = true OR
        // - LastLoginDate is null OR
        // - Department is not null
        Assert.Contains("John Doe", result);    // IsActive = true
        Assert.Contains("Jane Smith", result);  // IsActive = true
        Assert.Contains("Bob Johnson", result); // LastLoginDate is null
        Assert.Contains("Alice Brown", result); // IsActive = true
    }

    [Fact]
    public async Task ArrayOperations_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-array-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status {response.StatusCode}. Response: {errorContent}");
        }

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Should match users with:
        // - Department in ["Technology", "Marketing", "Finance"] AND
        // - Age in [25, 30, 35] AND
        // - Role not in ["Intern", "Contractor"]
        Assert.Contains("John Doe", result);    // Technology, Age 30
        Assert.Contains("Bob Johnson", result); // Technology, Age 35
    }

    [Fact]
    public async Task BuildQuery_StringOperations_ShouldReturnValidQuery()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-string-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/build-query", filterJson);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status {response.StatusCode}. Response: {errorContent}");
        }

        response.EnsureSuccessStatusCode();
        var queryResult = await response.Content.ReadFromJsonAsync<QueryResult>();

        Assert.NotNull(queryResult);
        Assert.NotEmpty(queryResult.Query);
        Assert.NotEmpty(queryResult.Parameters);
        Assert.Equal(4, queryResult.Parameters.Length); // 4 string operations

        // Verify the complete query structure for string operations
        // Expected: Name = ? OR Email LIKE ? OR Department LIKE ? OR Role LIKE ?
        var expectedPatterns = new[]
        {
            "Name", "Email", "Department", "Role", "OR"
        };

        foreach (var pattern in expectedPatterns)
        {
            Assert.Contains(pattern, queryResult.Query);
        }
    }

    [Fact]
    public async Task BuildQuery_NumericOperations_ShouldReturnValidQuery()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("datatype-numeric-operations");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/build-query", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var queryResult = await response.Content.ReadFromJsonAsync<QueryResult>();

        Assert.NotNull(queryResult);
        Assert.NotEmpty(queryResult.Query);
        Assert.NotEmpty(queryResult.Parameters);
        Assert.Equal(4, queryResult.Parameters.Length); // 4 numeric operations

        // Verify the complete query structure for numeric operations
        // Expected: Age >= ? AND Age <= ? AND Salary > ? AND Salary < ?
        var expectedPatterns = new[]
        {
            "Age", "Salary", ">=", "<=", ">", "<", "AND"
        };

        foreach (var pattern in expectedPatterns)
        {
            Assert.Contains(pattern, queryResult.Query);
        }
    }

    public override async Task DisposeAsync()
    {
        _jsonLoader?.Dispose();
        await base.DisposeAsync();
    }
}
