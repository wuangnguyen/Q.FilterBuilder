using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

public class BeginsWithRuleTransformerTests
{
    private readonly BeginsWithRuleTransformer _transformer;

    public BeginsWithRuleTransformerTests()
    {
        _transformer = new BeginsWithRuleTransformer();
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", "John");
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Name` LIKE CONCAT(?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateOrConditions()
    {
        // Arrange
        var rule = new FilterRule("Code", "begins_with", new[] { "ABC", "DEF", "GHI" });
        var fieldName = "`Code`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("(`Code` LIKE CONCAT(?, '%') OR `Code` LIKE CONCAT(?, '%') OR `Code` LIKE CONCAT(?, '%'))", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("ABC", parameters[0]);
        Assert.Equal("DEF", parameters[1]);
        Assert.Equal("GHI", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateOrConditions()
    {
        // Arrange
        var values = new List<string> { "Mr.", "Dr." };
        var rule = new FilterRule("Title", "begins_with", values);
        var fieldName = "`Title`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("(`Title` LIKE CONCAT(?, '%') OR `Title` LIKE CONCAT(?, '%'))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("Mr.", parameters[0]);
        Assert.Equal("Dr.", parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Email", "begins_with", "admin@");
        var fieldName = "`Email`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Email` LIKE CONCAT(?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("admin@", parameters[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", null);
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("BEGINS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", new string[0]);
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("BEGINS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", "");
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Name` LIKE CONCAT(?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
}
