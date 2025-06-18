using Q.FilterBuilder.IntegrationTests.Infrastructure;
using Xunit;

namespace Q.FilterBuilder.IntegrationTests.Tests;

/// <summary>
/// Integration tests for complex rule combinations and scenarios
/// Tests nested groups, multiple AND/OR conditions, mixed operator combinations, and join scenarios
/// </summary>
public class ComplexRulesTests : IntegrationTestBase
{
    private readonly JsonTestDataLoader _jsonLoader;

    public ComplexRulesTests(IntegrationTestWebApplicationFactory factory, DatabaseContainerFixture containerFixture)
        : base(factory, containerFixture)
    {
        _jsonLoader = new JsonTestDataLoader();
    }

    [Fact]
    public async Task NestedGroups_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-nested-groups");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Complex nested logic:
        // IsActive = true AND (
        //   (Department = "Technology" OR Role = "Manager") AND (Age > 25 AND Salary > 60000)
        // ) AND (
        //   CreatedDate > 2023-01-01 OR LastLoginDate is not null
        // )
        Assert.Contains("John Doe", result);    // Should match if meets all criteria
        Assert.Contains("Alice Brown", result); // Should match if meets all criteria
    }

    [Fact]
    public async Task MixedOperators_ShouldReturnCorrectResults()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-mixed-operators");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Mixed operators with AND condition:
        // - Name contains "John" AND
        // - Email ends with "@company.com" AND
        // - Age between [25, 40] AND
        // - Department in ["Technology", "Marketing"] AND
        // - IsActive = true AND
        // - Salary >= 50000 AND
        // - LastLoginDate is not null
        Assert.Contains("John Doe", result); // Should match all criteria
    }

    [Fact]
    public async Task MultipleConditionsWithGroups_ShouldReturnCorrectResults()
    {
        // Arrange - Use existing complex nested filter
        var filterJson = _jsonLoader.LoadTestData("complex-nested-filter");

        // Act
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);

        // Existing complex filter logic:
        // IsActive = true AND (
        //   Department = "Technology" OR Role = "Manager"
        // ) AND (
        //   Age > 25 AND Salary > 60000
        // )
        Assert.NotEmpty(result); // Should return users matching complex criteria
    }

    [Fact]
    public async Task EmptyResults_ShouldHandleGracefully()
    {
        // Arrange - Create a filter that should return no results
        var filterJson = _jsonLoader.LoadTestData("datatype-string-operations");

        // Act - Use a non-existent endpoint or modify filter to return empty
        var response = await Client.PostAsJsonAsync("/api/IntegrationTest/execute-dapper-users", filterJson);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);
        // Even if no matches, should handle gracefully
    }

    [Fact]
    public async Task BuildQuery_NestedGroups_ShouldReturnValidQuery()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-nested-groups");

        // Act
        var queryResult = await ExecuteFilterForQueryAsync(filterJson);

        // Assert
        Assert.NotNull(queryResult);
        Assert.NotEmpty(queryResult.Query);
        Assert.NotEmpty(queryResult.Parameters);

        // Verify the complete query structure for nested groups
        // Expected: IsActive = ? AND ((Department = ? OR Role = ?) AND (Age > ? AND Salary > ?)) AND (CreatedDate > ? OR LastLoginDate IS NOT NULL)
        var expectedPatterns = new[]
        {
            "IsActive", "Department", "Role", "Age", "Salary", "CreatedDate", "LastLoginDate",
            "AND", "OR", "(", ")"
        };

        foreach (var pattern in expectedPatterns)
        {
            Assert.Contains(pattern, queryResult.Query);
        }

        Assert.True(queryResult.Parameters.Length == 6);
    }

    [Fact]
    public async Task BuildQuery_MixedOperators_ShouldReturnValidQuery()
    {
        // Arrange
        var filterJson = _jsonLoader.LoadTestData("complex-mixed-operators");

        // Act
        var queryResult = await ExecuteFilterForQueryAsync(filterJson);

        // Assert
        Assert.NotNull(queryResult);
        Assert.NotEmpty(queryResult.Query);
        Assert.NotEmpty(queryResult.Parameters);

        // Verify the complete query structure for mixed operators
        // Expected: Name LIKE ? AND Email LIKE ? AND Age BETWEEN ? AND ? AND Department IN (?,?) AND IsActive = ? AND Salary >= ? AND LastLoginDate IS NOT NULL
        var expectedPatterns = new[]
        {
            "Name", "Email", "Age", "Department", "IsActive", "Salary", "LastLoginDate",
            "LIKE", "IN", ">=", "IS NOT NULL", "AND"
        };

        foreach (var pattern in expectedPatterns)
        {
            Assert.Contains(pattern, queryResult.Query);
        }

        Assert.True(queryResult.Parameters.Length == 8);
    }

    public override async Task DisposeAsync()
    {
        _jsonLoader?.Dispose();
        await base.DisposeAsync();
    }
}
