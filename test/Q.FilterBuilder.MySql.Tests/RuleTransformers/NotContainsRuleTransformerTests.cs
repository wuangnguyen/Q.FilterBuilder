using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

public class NotContainsRuleTransformerTests
{
    private readonly NotContainsRuleTransformer _transformer;

    public NotContainsRuleTransformerTests()
    {
        _transformer = new NotContainsRuleTransformer();
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", "spam");
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Name` NOT LIKE CONCAT('%', ?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("spam", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Description", "not_contains", new[] { "spam", "junk", "fake" });
        var fieldName = "`Description`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("(`Description` NOT LIKE CONCAT('%', ?, '%') AND `Description` NOT LIKE CONCAT('%', ?, '%') AND `Description` NOT LIKE CONCAT('%', ?, '%'))", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("spam", parameters[0]);
        Assert.Equal("junk", parameters[1]);
        Assert.Equal("fake", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateAndConditions()
    {
        // Arrange
        var values = new List<string> { "bad", "evil" };
        var rule = new FilterRule("Content", "not_contains", values);
        var fieldName = "`Content`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("(`Content` NOT LIKE CONCAT('%', ?, '%') AND `Content` NOT LIKE CONCAT('%', ?, '%'))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("bad", parameters[0]);
        Assert.Equal("evil", parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Email", "not_contains", "@spam.com");
        var fieldName = "`Email`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Email` NOT LIKE CONCAT('%', ?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("@spam.com", parameters[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", null);
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("NOT_CONTAINS operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", new string[0]);
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("NOT_CONTAINS operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", "");
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Name` NOT LIKE CONCAT('%', ?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
}
