using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

public class NotBeginsWithRuleTransformerTests
{
    private readonly NotBeginsWithRuleTransformer _transformer;

    public NotBeginsWithRuleTransformerTests()
    {
        _transformer = new NotBeginsWithRuleTransformer();
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", "Test");
        var fieldName = "\"Name\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Name\" NOT LIKE $1 || '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("Test", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Code", "not_begins_with", new[] { "TMP", "OLD", "BAK" });
        var fieldName = "\"Code\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("(\"Code\" NOT LIKE $1 || '%' AND \"Code\" NOT LIKE $2 || '%' AND \"Code\" NOT LIKE $3 || '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("TMP", parameters[0]);
        Assert.Equal("OLD", parameters[1]);
        Assert.Equal("BAK", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateAndConditions()
    {
        // Arrange
        var values = new List<string> { "spam_", "junk_" };
        var rule = new FilterRule("Username", "not_begins_with", values);
        var fieldName = "\"Username\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("(\"Username\" NOT LIKE $1 || '%' AND \"Username\" NOT LIKE $2 || '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("spam_", parameters[0]);
        Assert.Equal("junk_", parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Email", "not_begins_with", "noreply@");
        var fieldName = "\"Email\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Email\" NOT LIKE $1 || '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("noreply@", parameters[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", null);
        var fieldName = "\"Name\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("NOT_BEGINS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", new string[0]);
        var fieldName = "\"Name\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("NOT_BEGINS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", "");
        var fieldName = "\"Name\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Name\" NOT LIKE $1 || '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
}
