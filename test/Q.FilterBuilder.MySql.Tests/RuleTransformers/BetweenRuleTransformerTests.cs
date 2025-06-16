using System;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

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
        var fieldName = "`Age`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Age` BETWEEN ? AND ?", query);
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
        var fieldName = "`CreatedDate`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`CreatedDate` BETWEEN ? AND ?", query);
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
        var fieldName = "`Age`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("BETWEEN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new[] { 18 });
        var fieldName = "`Age`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new[] { 18, 25, 65 });
        var fieldName = "`Age`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new int[0]);
        var fieldName = "`Age`";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }
}
