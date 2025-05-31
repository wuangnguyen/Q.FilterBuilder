using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

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
        var rule = new FilterRule("Name", "not_begins_with", "Admin");
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Name` NOT LIKE CONCAT(?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("Admin", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Code", "not_begins_with", new[] { "TEMP", "TEST", "DEMO" });
        var fieldName = "`Code`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("(`Code` NOT LIKE CONCAT(?, '%') AND `Code` NOT LIKE CONCAT(?, '%') AND `Code` NOT LIKE CONCAT(?, '%'))", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("TEMP", parameters[0]);
        Assert.Equal("TEST", parameters[1]);
        Assert.Equal("DEMO", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateAndConditions()
    {
        // Arrange
        var values = new List<string> { "SYS", "TMP" };
        var rule = new FilterRule("Prefix", "not_begins_with", values);
        var fieldName = "`Prefix`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("(`Prefix` NOT LIKE CONCAT(?, '%') AND `Prefix` NOT LIKE CONCAT(?, '%'))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("SYS", parameters[0]);
        Assert.Equal("TMP", parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Email", "not_begins_with", "noreply@");
        var fieldName = "`Email`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Email` NOT LIKE CONCAT(?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("noreply@", parameters[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", null);
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("NOT_BEGINS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", new string[0]);
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("NOT_BEGINS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", "");
        var fieldName = "`Name`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Name` NOT LIKE CONCAT(?, '%')", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
}
