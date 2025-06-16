using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

public class NotInRuleTransformerTests
{
    private readonly NotInRuleTransformer _transformer;

    public NotInRuleTransformerTests()
    {
        _transformer = new NotInRuleTransformer();
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", "Inactive");
        var fieldName = "\"Status\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Status\" NOT IN ($1)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("Inactive", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new[] { "Inactive", "Deleted", "Archived" });
        var fieldName = "\"Status\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Status\" NOT IN ($1, $2, $3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("Inactive", parameters[0]);
        Assert.Equal("Deleted", parameters[1]);
        Assert.Equal("Archived", parameters[2]);
    }

    [Fact]
    public void Transform_WithIntegerValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("CategoryId", "not_in", new[] { 4, 6, 7 });
        var fieldName = "\"CategoryId\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"CategoryId\" NOT IN ($1, $2, $3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(4, parameters[0]);
        Assert.Equal(6, parameters[1]);
        Assert.Equal(7, parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var values = new List<string> { "Yellow", "Purple" };
        var rule = new FilterRule("Color", "not_in", values);
        var fieldName = "\"Color\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Color\" NOT IN ($1, $2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("Yellow", parameters[0]);
        Assert.Equal("Purple", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", null);
        var fieldName = "\"Status\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("NOT_IN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new string[0]);
        var fieldName = "\"Status\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("NOT_IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new List<string>());
        var fieldName = "\"Status\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("NOT_IN operator requires at least one value", exception.Message);
    }
}
