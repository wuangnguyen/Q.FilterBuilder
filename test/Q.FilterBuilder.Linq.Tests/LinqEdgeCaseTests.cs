using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.Extensions;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests;

public class LinqEdgeCaseTests
{
    private readonly IFilterBuilder _filterBuilder;

    public LinqEdgeCaseTests()
    {
        var services = new ServiceCollection();
        services.AddLinqFilterBuilder();
        var serviceProvider = services.BuildServiceProvider();
        _filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();
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
    public void Build_WithSpecialCharactersInFieldName_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("User.Profile.Name", "equal", "John"));
        group.Rules.Add(new FilterRule("Data[0].Value", "equal", "Test"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("User.Profile.Name = @p0", query);
        Assert.Contains("Data[0].Value = @p1", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal("Test", parameters[1]);
    }

    [Fact]
    public void Build_WithNullValue_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", null));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Equal("Name = @p0", query);
        // LINQ provider optimizes away null parameters
        Assert.Empty(parameters);
    }

    [Fact]
    public void Build_WithEmptyStringValues_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("OR");
        group.Rules.Add(new FilterRule("Title", "equal", ""));
        group.Rules.Add(new FilterRule("Description", "contains", ""));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Title = @p0", query);
        Assert.Contains("Description.Contains(@@p10)", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("", parameters[0]);
        Assert.Equal("", parameters[1]);
    }

    [Fact]
    public void Build_WithWhitespaceValues_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "   "));
        group.Rules.Add(new FilterRule("Code", "contains", "\t\n\r"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Name = @p0", query);
        Assert.Contains("Code.Contains(@@p10)", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("   ", parameters[0]);
        Assert.Equal("\t\n\r", parameters[1]);
    }

    [Fact]
    public void Build_WithUnicodeCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Name", "equal", "José"));
        group.Rules.Add(new FilterRule("City", "contains", "北京"));
        group.Rules.Add(new FilterRule("Symbol", "equal", "€"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Name = @p0", query);
        Assert.Contains("City.Contains(@@p10)", query);
        Assert.Contains("Symbol = @p2", query);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("José", parameters[0]);
        Assert.Equal("北京", parameters[1]);
        Assert.Equal("€", parameters[2]);
    }

    [Fact]
    public void Build_WithVeryLongFieldNames_ShouldHandleCorrectly()
    {
        // Arrange
        var longFieldName = new string('A', 500);
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule(longFieldName, "equal", "value"));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains($"{longFieldName} = @p0", query);
        Assert.Single(parameters);
        Assert.Equal("value", parameters[0]);
    }

    [Fact]
    public void Build_WithVeryLongStringValues_ShouldHandleCorrectly()
    {
        // Arrange
        var longValue = new string('B', 5000);
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Content", "contains", longValue));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Content.Contains(@@p00)", query);
        Assert.Single(parameters);
        Assert.Equal(longValue, parameters[0]);
    }

    [Fact]
    public void Build_WithSingleElementArrays_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("CategoryId", "in", new[] { 42 }));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("@@p0.Contains(CategoryId)", query);
        Assert.Single(parameters);
        Assert.IsType<System.Collections.Generic.List<object>>(parameters[0]);
        var list = (System.Collections.Generic.List<object>)parameters[0];
        Assert.Single(list);
        Assert.Equal(42, list[0]);
    }

    [Fact]
    public void Build_WithSameBetweenValues_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Score", "between", new[] { 100, 100 }));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Score >= @p00 && Score <= @p01", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(100, parameters[0]);
        Assert.Equal(100, parameters[1]);
    }

    [Fact]
    public void Build_WithExtremeNumericValues_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("MinValue", "equal", int.MinValue));
        group.Rules.Add(new FilterRule("MaxValue", "equal", int.MaxValue));
        group.Rules.Add(new FilterRule("LongMin", "equal", long.MinValue));
        group.Rules.Add(new FilterRule("LongMax", "equal", long.MaxValue));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("MinValue = @p0", query);
        Assert.Contains("MaxValue = @p1", query);
        Assert.Contains("LongMin = @p2", query);
        Assert.Contains("LongMax = @p3", query);
        Assert.Equal(4, parameters.Length);
        Assert.Equal(int.MinValue, parameters[0]);
        Assert.Equal(int.MaxValue, parameters[1]);
        Assert.Equal(long.MinValue, parameters[2]);
        Assert.Equal(long.MaxValue, parameters[3]);
    }

    [Fact]
    public void Build_WithExtremeDecimalValues_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("MinDecimal", "equal", decimal.MinValue));
        group.Rules.Add(new FilterRule("MaxDecimal", "equal", decimal.MaxValue));
        group.Rules.Add(new FilterRule("MinDouble", "equal", double.MinValue));
        group.Rules.Add(new FilterRule("MaxDouble", "equal", double.MaxValue));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("MinDecimal = @p0", query);
        Assert.Contains("MaxDecimal = @p1", query);
        Assert.Contains("MinDouble = @p2", query);
        Assert.Contains("MaxDouble = @p3", query);
        Assert.Equal(4, parameters.Length);
        Assert.Equal(decimal.MinValue, parameters[0]);
        Assert.Equal(decimal.MaxValue, parameters[1]);
        Assert.Equal(double.MinValue, parameters[2]);
        Assert.Equal(double.MaxValue, parameters[3]);
    }

    [Fact]
    public void Build_WithExtremeDateValues_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("MinDate", "equal", DateTime.MinValue));
        group.Rules.Add(new FilterRule("MaxDate", "equal", DateTime.MaxValue));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("MinDate = @p0", query);
        Assert.Contains("MaxDate = @p1", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(DateTime.MinValue, parameters[0]);
        Assert.Equal(DateTime.MaxValue, parameters[1]);
    }

    [Fact]
    public void Build_WithBooleanValues_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("IsActive", "equal", true));
        group.Rules.Add(new FilterRule("IsDeleted", "equal", false));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("IsActive = @p0", query);
        Assert.Contains("IsDeleted = @p1", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(true, parameters[0]);
        Assert.Equal(false, parameters[1]);
    }

    [Fact]
    public void Build_WithGuidValues_ShouldHandleCorrectly()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var group = new FilterGroup("OR");
        group.Rules.Add(new FilterRule("UserId", "equal", guid1));
        group.Rules.Add(new FilterRule("UserId", "equal", guid2));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("UserId = @p0", query);
        Assert.Contains("UserId = @p1", query);
        Assert.Contains("||", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(guid1, parameters[0]);
        Assert.Equal(guid2, parameters[1]);
    }

    [Fact]
    public void Build_WithCharValues_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Grade", "equal", 'A'));
        group.Rules.Add(new FilterRule("Section", "not_equal", 'Z'));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Grade = @p0", query);
        Assert.Contains("Section != @p1", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal('A', parameters[0]);
        Assert.Equal('Z', parameters[1]);
    }

    [Fact]
    public void Build_WithByteValues_ShouldHandleCorrectly()
    {
        // Arrange
        var group = new FilterGroup("AND");
        group.Rules.Add(new FilterRule("Level", "greater", (byte)5));
        group.Rules.Add(new FilterRule("Priority", "less_or_equal", (byte)10));

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Contains("Level > @p0", query);
        Assert.Contains("Priority <= @p1", query);
        Assert.Equal(2, parameters.Length);
        Assert.Equal((byte)5, parameters[0]);
        Assert.Equal((byte)10, parameters[1]);
    }
}
