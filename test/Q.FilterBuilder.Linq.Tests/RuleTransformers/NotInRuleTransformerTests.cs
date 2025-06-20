using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class NotInRuleTransformerTests
{
    private readonly NotInRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", "Inactive");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!@p0.Contains(Status)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        var arr = Assert.IsType<object[]>(parameters);
        var collection = Assert.IsType<object[]>(arr[0]);
        Assert.Single(collection);
        Assert.Equal("Inactive", collection[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new[] { "Inactive", "Deleted", "Archived" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!@p0.Contains(Status)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        var arr = Assert.IsType<object[]>(parameters);
        var collection = Assert.IsType<List<object>>(arr[0]);
        Assert.Equal(3, collection.Count);
        Assert.Equal("Inactive", collection[0]);
        Assert.Equal("Deleted", collection[1]);
        Assert.Equal("Archived", collection[2]);
    }

    [Fact]
    public void Transform_WithIntegerList_ShouldHandleIntegerValues()
    {
        // Arrange
        var rule = new FilterRule("Id", "not_in", new List<int> { 1, 2, 3 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Id", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!@p0.Contains(Id)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        var arr = Assert.IsType<object[]>(parameters);
        var collection = Assert.IsType<List<object>>(arr[0]);
        Assert.Equal(3, collection.Count);
        Assert.Equal(1, collection[0]);
        Assert.Equal(2, collection[1]);
        Assert.Equal(3, collection[2]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", null);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Status", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_IN operator requires a non-null value", ex.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new string[0]);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Status", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_IN operator requires at least one value", ex.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new List<string>());

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Status", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_IN operator requires at least one value", ex.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Status", "not_in", new[] { "Inactive", "Deleted" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "User.Status", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!@p0.Contains(User.Status)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
    }
    
    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new[] { "Inactive", "Deleted" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", 2, new LinqFormatProvider());

        // Assert
        Assert.Equal("!@p2.Contains(Status)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
    }
} 
