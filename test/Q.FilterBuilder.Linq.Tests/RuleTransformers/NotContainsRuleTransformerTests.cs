using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class NotContainsRuleTransformerTests
{
    private readonly NotContainsRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!Name.Contains(@p0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(!Name.Contains(@p0) && !Name.Contains(@p1))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("test1", parameters[0]);
        Assert.Equal("test2", parameters[1]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Email", "not_contains", new List<string> { "@spam.com", "@fake.org" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Email", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(!Email.Contains(@p0) && !Email.Contains(@p1))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("@spam.com", parameters[0]);
        Assert.Equal("@fake.org", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", null);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Name", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_CONTAINS operator requires a non-null value", ex.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", new string[0]);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_CONTAINS operator requires at least one value", ex.Message);
    }
}
