using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class InRuleTransformerTests
{
    private readonly InRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithArray_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("CategoryId", "in", new[] { 1, 2, 3 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CategoryId", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("@p0.Contains(CategoryId)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.IsType<List<object>>(parameters[0]);
        var list = (List<object>)parameters[0];
        Assert.Equal(new object[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", new List<string> { "Active", "Pending" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("@p0.Contains(Status)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.IsType<List<object>>(parameters[0]);
        var list = (List<object>)parameters[0];
        Assert.Equal(new object[] { "Active", "Pending" }, list);
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldWrapInList()
    {
        // Arrange
        var rule = new FilterRule("Id", "in", 42);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Id", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("@p0.Contains(Id)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        var arr = (object[])parameters[0];
        Assert.Single(arr);
        Assert.Equal(42, arr[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Id", "in", null);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Id", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("IN operator requires a non-null value", ex.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Id", "in", new int[0]);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Id", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("IN operator requires at least one value", ex.Message);
    }
}
