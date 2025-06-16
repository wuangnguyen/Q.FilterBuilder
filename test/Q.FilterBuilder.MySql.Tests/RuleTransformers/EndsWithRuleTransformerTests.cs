using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

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
        var fieldName = "`FileName`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`FileName` LIKE CONCAT('%', ?)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(".pdf", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateOrConditions()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", new[] { ".jpg", ".png", ".gif" });
        var fieldName = "`FileName`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("(`FileName` LIKE CONCAT('%', ?) OR `FileName` LIKE CONCAT('%', ?) OR `FileName` LIKE CONCAT('%', ?))", query);
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
        var values = new List<string> { ".doc", ".docx" };
        var rule = new FilterRule("Document", "ends_with", values);
        var fieldName = "`Document`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("(`Document` LIKE CONCAT('%', ?) OR `Document` LIKE CONCAT('%', ?))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(".doc", parameters[0]);
        Assert.Equal(".docx", parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Email", "ends_with", "@company.com");
        var fieldName = "`Email`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Email` LIKE CONCAT('%', ?)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("@company.com", parameters[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", null);
        var fieldName = "`FileName`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("ENDS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", new string[0]);
        var fieldName = "`FileName`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("ENDS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", "");
        var fieldName = "`FileName`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`FileName` LIKE CONCAT('%', ?)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
}
