using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

public class ContainsRuleTransformerTests
{
    private readonly ContainsRuleTransformer _transformer;

    public ContainsRuleTransformerTests()
    {
        _transformer = new ContainsRuleTransformer();
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", "John");
        var fieldName = "`Name`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Name` LIKE CONCAT('%', ?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateOrConditions()
    {
        // Arrange
        var rule = new FilterRule("Description", "contains", new[] { "test", "demo", "sample" });
        var fieldName = "`Description`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("(`Description` LIKE CONCAT('%', ?, '%') OR `Description` LIKE CONCAT('%', ?, '%') OR `Description` LIKE CONCAT('%', ?, '%'))", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("test", parameters[0]);
        Assert.Equal("demo", parameters[1]);
        Assert.Equal("sample", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateOrConditions()
    {
        // Arrange
        var values = new List<string> { "alpha", "beta" };
        var rule = new FilterRule("Code", "contains", values);
        var fieldName = "`Code`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("(`Code` LIKE CONCAT('%', ?, '%') OR `Code` LIKE CONCAT('%', ?, '%'))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("alpha", parameters[0]);
        Assert.Equal("beta", parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Text", "contains", "test@example.com");
        var fieldName = "`Text`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Text` LIKE CONCAT('%', ?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test@example.com", parameters[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", null);
        var fieldName = "`Name`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("CONTAINS operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", new string[0]);
        var fieldName = "`Name`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("CONTAINS operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", "");
        var fieldName = "`Name`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Name` LIKE CONCAT('%', ?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
}
