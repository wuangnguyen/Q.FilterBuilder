using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.Extensions;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests;

public class LinqFunctionalTests
{
    private readonly IFilterBuilder _filterBuilder;

    public LinqFunctionalTests()
    {
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        _filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();
    }

    [Fact]
    public void Build_WithBasicEqualOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Equal("Name = @p0", query);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void Build_WithMultipleBasicOperators_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "greater", 25));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Name = @p0", query);
        Assert.Contains("Age > @p1", query);
        Assert.Contains("&&", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(25, parameters[1]);
    }

    [Fact]
    public void Build_WithOrOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("OR");
        group.Rules.Add(new FilterRule("Status", "equal", "Active"));
        group.Rules.Add(new FilterRule("Status", "equal", "Pending"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Status = @p0", query);
        Assert.Contains("Status = @p1", query);
        Assert.Contains("||", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("Active", parameters[0]);
        Assert.Equal("Pending", parameters[1]);
    }

    [Fact]
    public void Build_WithContainsOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "contains", "John"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Name.Contains(@@p00)", query);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void Build_WithBetweenOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Age", "between", new[] { 18, 65 }));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Age >= @p00 && Age <= @p01", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(18, parameters[0]);
        Assert.Equal(65, parameters[1]);
    }

    [Fact]
    public void Build_WithInOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("CategoryId", "in", new[] { 1, 2, 3 }));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("@@p0.Contains(CategoryId)", query);
        Assert.Single(parameters);
        Assert.IsType<List<object>>(parameters[0]);
        var list = (List<object>)parameters[0];
        Assert.Equal(3, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }

    [Fact]
    public void Build_WithNullChecks_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Description", "is_null", null));
        group.Rules.Add(new FilterRule("Notes", "is_not_null", null));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Description == null", query);
        Assert.Contains("Notes != null", query);
        Assert.Contains("&&", query);
        Assert.Empty(parameters);
    }

    [Fact]
    public void Build_WithStringOperators_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("FirstName", "begins_with", "Jo"));
        group.Rules.Add(new FilterRule("Email", "ends_with", "@test.com"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("FirstName.StartsWith(@@p00)", query);
        Assert.Contains("Email.EndsWith(@@p10)", query);
        Assert.Contains("&&", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("Jo", parameters[0]);
        Assert.Equal("@test.com", parameters[1]);
    }

    [Fact]
    public void Build_WithNestedGroups_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var mainGroup = new FilterGroup("AND");
        mainGroup.Rules.Add(new FilterRule("IsActive", "equal", true));

        var nestedGroup = new FilterGroup("OR");
        nestedGroup.Rules.Add(new FilterRule("Department", "equal", "IT"));
        nestedGroup.Rules.Add(new FilterRule("Department", "equal", "HR"));
        mainGroup.Groups.Add(nestedGroup);

        // Act
        var (query, parameters) = _filterBuilder.Build(mainGroup);

        // Assert
        Assert.Contains("IsActive = @p0", query);
        Assert.Contains("Department = @p1", query);
        Assert.Contains("Department = @p2", query);
        Assert.Contains("&&", query);
        Assert.Contains("||", query);
        Assert.Contains("(", query);
        Assert.Contains(")", query);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(true, parameters[0]);
        Assert.Equal("IT", parameters[1]);
        Assert.Equal("HR", parameters[2]);
    }

    [Fact]
    public void Build_WithEmptyGroup_ShouldReturnEmptyQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Equal("", query);
        Assert.Empty(parameters);
    }

    [Fact]
    public void Build_WithSingleRule_ShouldNotIncludeLogicalOperators()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Equal("Name = @p0", query);
        Assert.DoesNotContain("&&", query);
        Assert.DoesNotContain("||", query);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void Build_WithNotEqualOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Status", "not_equal", "Deleted"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Status != @p0", query);
        Assert.Single(parameters);
        Assert.Equal("Deleted", parameters[0]);
    }

    [Fact]
    public void Build_WithComparisonOperators_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Age", "greater_or_equal", 18));
        group.Rules.Add(new FilterRule("Score", "less_or_equal", 100));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Age >= @p0", query);
        Assert.Contains("Score <= @p1", query);
        Assert.Contains("&&", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(18, parameters[0]);
        Assert.Equal(100, parameters[1]);
    }

    [Fact]
    public void Build_WithMixedDataTypes_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "Test"));
        group.Rules.Add(new FilterRule("Count", "equal", 42));
        group.Rules.Add(new FilterRule("IsEnabled", "equal", true));
        group.Rules.Add(new FilterRule("Price", "equal", 19.99m));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Name = @p0", query);
        Assert.Contains("Count = @p1", query);
        Assert.Contains("IsEnabled = @p2", query);
        Assert.Contains("Price = @p3", query);
        Assert.Equal(4, parameters.Length);
        Assert.Equal("Test", parameters[0]);
        Assert.Equal(42, parameters[1]);
        Assert.Equal(true, parameters[2]);
        Assert.Equal(19.99m, parameters[3]);
    }

    [Fact]
    public void Build_WithComplexFieldNames_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("User.Profile.Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Data.Value", "equal", "Test"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("User.Profile.Name = @p0", query);
        Assert.Contains("Data.Value = @p1", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal("Test", parameters[1]);
    }

    [Fact]
    public void Build_WithNotContainsOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Content", "not_contains", "spam"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("!Content.Contains(@@p00)", query);
        Assert.Single(parameters);
        Assert.Equal("spam", parameters[0]);
    }

    [Fact]
    public void Build_WithDateTimeValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var date = DateTime.Parse("2023-01-01");
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("CreatedDate", "greater", date));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("CreatedDate > @p0", query);
        Assert.Single(parameters);
        Assert.Equal(date, parameters[0]);
    }
}
