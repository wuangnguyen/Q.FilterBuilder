using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

public class NotEndsWithRuleTransformerTests
{
    private readonly NotEndsWithRuleTransformer _transformer;

    public NotEndsWithRuleTransformerTests()
    {
        _transformer = new NotEndsWithRuleTransformer();
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("FileName", "not_ends_with", ".tmp");
        var fieldName = "`FileName`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`FileName` NOT LIKE CONCAT('%', ?)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(".tmp", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateAndConditions()
    {
        // Arrange
        var rule = new FilterRule("FileName", "not_ends_with", new[] { ".bak", ".old", ".temp" });
        var fieldName = "`FileName`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("(`FileName` NOT LIKE CONCAT('%', ?) AND `FileName` NOT LIKE CONCAT('%', ?) AND `FileName` NOT LIKE CONCAT('%', ?))", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(".bak", parameters[0]);
        Assert.Equal(".old", parameters[1]);
        Assert.Equal(".temp", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateAndConditions()
    {
        // Arrange
        var values = new List<string> { ".log", ".cache" };
        var rule = new FilterRule("File", "not_ends_with", values);
        var fieldName = "`File`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("(`File` NOT LIKE CONCAT('%', ?) AND `File` NOT LIKE CONCAT('%', ?))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(".log", parameters[0]);
        Assert.Equal(".cache", parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Email", "not_ends_with", "@spam.com");
        var fieldName = "`Email`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Email` NOT LIKE CONCAT('%', ?)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("@spam.com", parameters[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("FileName", "not_ends_with", null);
        var fieldName = "`FileName`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("NOT_ENDS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("FileName", "not_ends_with", new string[0]);
        var fieldName = "`FileName`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("NOT_ENDS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("FileName", "not_ends_with", "");
        var fieldName = "`FileName`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`FileName` NOT LIKE CONCAT('%', ?)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
}
