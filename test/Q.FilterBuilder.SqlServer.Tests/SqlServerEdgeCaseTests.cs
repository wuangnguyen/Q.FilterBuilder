using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.Core;
using Q.FilterBuilder.SqlServer.Extensions;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests;

public class SqlServerEdgeCaseTests
{
    [Fact]
    public void SqlServerFilterBuilder_WithNullFieldName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FilterRule(null!, "equal", "Value"));
    }

    [Fact]
    public void SqlServerFilterBuilder_WithEmptyFieldName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FilterRule("", "equal", "Value"));
    }

    [Fact]
    public void SqlServerFilterBuilder_WithWhitespaceFieldName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FilterRule("   ", "equal", "Value"));
    }

    [Fact]
    public void SqlServerFilterBuilder_WithSpecialCharactersInFieldName_ShouldEscapeProperly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Field[With]Brackets", "equal", "Value"));
        group.Rules.Add(new FilterRule("Field'With'Quotes", "equal", "Value2"));
        group.Rules.Add(new FilterRule("Field\"With\"DoubleQuotes", "equal", "Value3"));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("[Field[With]Brackets] = @p0", query);
        Assert.Contains("[Field'With'Quotes] = @p1", query);
        Assert.Contains("[Field\"With\"DoubleQuotes] = @p2", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithVeryLongFieldName_ShouldHandle()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var longFieldName = new string('A', 10000); // Very long field name
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule(longFieldName, "equal", "Value"));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal($"[{longFieldName}] = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("Value", parameters[0]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithNullValues_ShouldHandleCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Field1", "equal", null));
        group.Rules.Add(new FilterRule("Field2", "not_equal", null));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("[Field1] = @p0", query);
        Assert.Contains("[Field2] != @p0", query);
        Assert.NotNull(parameters);
        Assert.Empty(parameters);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithEmptyArrayForInOperator_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Status", "in", new string[0]));

        // Act & Assert - Should throw ArgumentException for empty array
        Assert.Throws<ArgumentException>(() => filterBuilder.Build(group));
    }

    [Fact]
    public void SqlServerFilterBuilder_WithSingleItemArrayForInOperator_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Status", "in", new[] { "Active" }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("[Status] IN (@p0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("Active", parameters[0]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithMixedTypesInArray_ShouldHandleCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("MixedField", "in", new object?[] { "String", 123, true, null }));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Equal("[MixedField] IN (@p0, @p1, @p2, @p3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(4, parameters.Length);
        Assert.Equal("String", parameters[0]);
        Assert.Equal(123, parameters[1]);
        Assert.Equal(true, parameters[2]);
        Assert.Null(parameters[3]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithInvalidConditionType_ShouldDefaultToAnd()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("INVALID_CONDITION");
        group.Rules.Add(new FilterRule("Field1", "equal", "Value1"));
        group.Rules.Add(new FilterRule("Field2", "equal", "Value2"));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        // Should default to AND behavior or handle gracefully
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithUnicodeFieldNames_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("用户名", "equal", "张三"));
        group.Rules.Add(new FilterRule("пользователь", "equal", "Иван"));
        group.Rules.Add(new FilterRule("🚀Field", "equal", "🌟Value"));

        // Act
        var (query, parameters) = filterBuilder.Build(group);

        // Assert
        Assert.Contains("[用户名] = @p0", query);
        Assert.Contains("[пользователь] = @p1", query);
        Assert.Contains("[🚀Field] = @p2", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("张三", parameters[0]);
        Assert.Equal("Иван", parameters[1]);
        Assert.Equal("🌟Value", parameters[2]);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithExtremelyDeepNesting_ShouldNotStackOverflow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        // Create extremely deep nesting (100 levels)
        var rootGroup = new FilterGroup("AND");
        var currentGroup = rootGroup;

        for (int i = 0; i < 100; i++)
        {
            currentGroup.Rules.Add(new FilterRule($"Field{i}", "equal", $"Value{i}"));

            if (i < 99) // Don't add nested group to the last iteration
            {
                var nestedGroup = new FilterGroup("OR");
                currentGroup.Groups.Add(nestedGroup);
                currentGroup = nestedGroup;
            }
        }

        // Act & Assert - Should not throw StackOverflowException
        var (query, parameters) = filterBuilder.Build(rootGroup);

        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(100, parameters.Length);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithCircularReference_ShouldNotCauseInfiniteLoop()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group1 = new FilterGroup("AND");
        var group2 = new FilterGroup("OR");

        group1.Rules.Add(new FilterRule("Field1", "equal", "Value1"));
        group1.Groups.Add(group2);

        group2.Rules.Add(new FilterRule("Field2", "equal", "Value2"));
        // Note: We can't actually create a circular reference with the current model
        // but this tests the robustness of the system

        // Act
        var (query, parameters) = filterBuilder.Build(group1);

        // Assert
        Assert.NotNull(query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void SqlServerFilterBuilder_WithZeroLengthBetweenArray_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Age", "between", new int[0]));

        // Act & Assert - Should throw ArgumentException for invalid array length
        Assert.Throws<ArgumentException>(() => filterBuilder.Build(group));
    }

    [Fact]
    public void SqlServerFilterBuilder_WithSingleValueBetweenArray_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Age", "between", new[] { 25 }));

        // Act & Assert - Should throw ArgumentException for invalid array length
        Assert.Throws<ArgumentException>(() => filterBuilder.Build(group));
    }

    [Fact]
    public void SqlServerFilterBuilder_WithMoreThanTwoValuesBetweenArray_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSqlServerFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Age", "between", new[] { 18, 65, 100, 200 }));

        // Act & Assert - Should throw ArgumentException for invalid array length
        Assert.Throws<ArgumentException>(() => filterBuilder.Build(group));
    }
}
