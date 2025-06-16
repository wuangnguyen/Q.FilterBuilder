using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

public class EndsWithRuleTransformerTests
{
    private readonly EndsWithRuleTransformer _transformer;

    public EndsWithRuleTransformerTests()
    {
        _transformer = new EndsWithRuleTransformer();
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", ".pdf");
        var fieldName = "\"FileName\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"FileName\" LIKE '%' || $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(".pdf", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateOrConditions()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", new[] { ".jpg", ".png", ".gif" });
        var fieldName = "\"FileName\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("(\"FileName\" LIKE '%' || $1 OR \"FileName\" LIKE '%' || $2 OR \"FileName\" LIKE '%' || $3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(".jpg", parameters[0]);
        Assert.Equal(".png", parameters[1]);
        Assert.Equal(".gif", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateOrConditions()
    {
        // Arrange
        var values = new List<string> { ".com", ".org" };
        var rule = new FilterRule("Domain", "ends_with", values);
        var fieldName = "\"Domain\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("(\"Domain\" LIKE '%' || $1 OR \"Domain\" LIKE '%' || $2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(".com", parameters[0]);
        Assert.Equal(".org", parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Email", "ends_with", "@company.com");
        var fieldName = "\"Email\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Email\" LIKE '%' || $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("@company.com", parameters[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", null);
        var fieldName = "\"FileName\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("ENDS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", new string[0]);
        var fieldName = "\"FileName\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("ENDS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", "");
        var fieldName = "\"FileName\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"FileName\" LIKE '%' || $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
}
