using System;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

public class NotBetweenRuleTransformerTests
{
    private readonly NotBetweenRuleTransformer _transformer;

    public NotBetweenRuleTransformerTests()
    {
        _transformer = new NotBetweenRuleTransformer();
    }

    [Fact]
    public void Transform_WithTwoValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18, 65 });
        var fieldName = "\"Age\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Age\" NOT BETWEEN $1 AND $2", query);
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
        var rule = new FilterRule("CreatedDate", "not_between", new[] { startDate, endDate });
        var fieldName = "\"CreatedDate\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"CreatedDate\" NOT BETWEEN $1 AND $2", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(startDate, parameters[0]);
        Assert.Equal(endDate, parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", null);
        var fieldName = "\"Age\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("NOT_BETWEEN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18 });
        var fieldName = "\"Age\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("NOT_BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18, 25, 65 });
        var fieldName = "\"Age\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("NOT_BETWEEN operator requires exactly 2 values", exception.Message);
    }
}
