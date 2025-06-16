using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.SqlServer.Extensions;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests;

public class SqlServerIntegrationTests
{
    [Fact]
    public void SqlServerFilterBuilder_WithSimpleRule_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("[Name] = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithMultipleRules_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Age", "greater", 25));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("[Name] = @p0 AND [Age] > @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal(25, parameters[1]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithNestedGroups_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var mainGroup = new FilterGroup("AND");
        mainGroup.Rules.Add(new FilterRule("Status", "equal", "Active"));

        var nestedGroup = new FilterGroup("OR");
        nestedGroup.Rules.Add(new FilterRule("Name", "equal", "John"));
        nestedGroup.Rules.Add(new FilterRule("Name", "equal", "Jane"));
        mainGroup.Groups.Add(nestedGroup);

        // Act
        var (query, parameters) = filterBuilder.Build(mainGroup);

        // Assert
        Assert.Equal("[Status] = @p0 AND ([Name] = @p1 OR [Name] = @p2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("Active", parameters[0]);
        Assert.Equal("John", parameters[1]);
        Assert.Equal("Jane", parameters[2]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithBetweenOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Age", "between", new[] { 18, 65 }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("[Age] BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(18, parameters[0]);
        Assert.Equal(65, parameters[1]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithInOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Status", "in", new[] { "Active", "Pending", "Approved" }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("[Status] IN (@p0, @p1, @p2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("Active", parameters[0]);
        Assert.Equal("Pending", parameters[1]);
        Assert.Equal("Approved", parameters[2]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithContainsOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Description", "contains", "test"));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("[Description] LIKE '%' + @p0 + '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithDateDiffOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        var rule = new FilterRule("CreatedDate", "date_diff", 30);
        rule.Metadata = new Dictionary<string, object?> { { "intervalType", "day" } };
        group.Rules.Add(rule);

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("DATEDIFF(day, [CreatedDate], GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(30, parameters[0]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithIsNullOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("DeletedDate", "is_null", null));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("[DeletedDate] IS NULL", query);
        Assert.NotNull(parameters);
        Assert.Empty(parameters);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithIsEmptyOperator_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Description", "is_empty", null));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("[Description] = ''", query);
        Assert.NotNull(parameters);
        Assert.Empty(parameters);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithComplexNestedQuery_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var mainGroup = new FilterGroup("AND");

        // Add simple rule
        mainGroup.Rules.Add(new FilterRule("Status", "equal", "Active"));

        // Add nested OR group
        var orGroup = new FilterGroup("OR");
        orGroup.Rules.Add(new FilterRule("Name", "contains", "John"));
        orGroup.Rules.Add(new FilterRule("Email", "ends_with", "@company.com"));
        mainGroup.Groups.Add(orGroup);

        // Add another simple rule
        mainGroup.Rules.Add(new FilterRule("Age", "between", new[] { 25, 65 }));

        // Act
        var (query, parameters) = filterBuilder.Build(mainGroup);

        // Assert
        Assert.Contains("[Status] = @p0", query);
        Assert.Contains("[Name] LIKE '%' + @p3 + '%'", query);
        Assert.Contains("[Age] BETWEEN @p1 AND @p2", query);
        Assert.NotNull(parameters);
        Assert.Equal(5, parameters.Length);
        Assert.Equal("Active", parameters[0]);
        Assert.Equal(25, parameters[1]);
        Assert.Equal(65, parameters[2]);
        Assert.Equal("John", parameters[3]);
        Assert.Equal("@company.com", parameters[4]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithEmptyGroup_ShouldReturnEmptyQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("", query);
        Assert.NotNull(parameters);
        Assert.Empty(parameters);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithAllSqlServerOperators_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var ruleTransformerService = serviceProvider.GetRequiredService<IRuleTransformerService>();

        // Act & Assert - Verify all SQL Server operators are registered
        var sqlServerOperators = new[]
        {
            "equal", "not_equal", "less", "less_or_equal", "greater", "greater_or_equal",
            "between", "not_between", "in", "not_in", "contains", "not_contains",
            "begins_with", "not_begins_with", "ends_with", "not_ends_with",
            "is_null", "is_not_null", "is_empty", "is_not_empty", "date_diff"
        };

        foreach (var operatorName in sqlServerOperators)
        {
            Assert.True(HasTransformer(ruleTransformerService, operatorName),
                $"Operator '{operatorName}' should be registered");
        }
    }

    // Helper method to check if a transformer exists
    private static bool HasTransformer(IRuleTransformerService service, string operatorName)
    {
        try
        {
            var transformer = service.GetRuleTransformer(operatorName);
            return transformer != null;
        }
        catch (NotImplementedException)
        {
            return false;
        }
    }
}
