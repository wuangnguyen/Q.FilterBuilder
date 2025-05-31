using System;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

public class BetweenRuleTransformerTests
{
    private readonly BetweenRuleTransformer _transformer;

    public BetweenRuleTransformerTests()
    {
        _transformer = new BetweenRuleTransformer();
    }

    [Fact]
    public void Transform_WithTwoValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new[] { 18, 65 });
        var fieldName = "\"Age\"";
        var parameterName = "$1";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("\"Age\" BETWEEN $10 AND $11", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(18, parameters[0]);
        Assert.Equal(65, parameters[1]);
    }

    [Fact]
    public void Transform_WithDateValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 31);
        var rule = new FilterRule("CreatedDate", "between", new[] { startDate, endDate });
        var fieldName = "\"CreatedDate\"";
        var parameterName = "$5";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("\"CreatedDate\" BETWEEN $50 AND $51", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(startDate, parameters[0]);
        Assert.Equal(endDate, parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", null);
        var fieldName = "\"Age\"";
        var parameterName = "$1";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("BETWEEN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new[] { 18 });
        var fieldName = "\"Age\"";
        var parameterName = "$1";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new[] { 18, 25, 65 });
        var fieldName = "\"Age\"";
        var parameterName = "$1";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new int[0]);
        var fieldName = "\"Age\"";
        var parameterName = "$1";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, parameterName));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }
}
