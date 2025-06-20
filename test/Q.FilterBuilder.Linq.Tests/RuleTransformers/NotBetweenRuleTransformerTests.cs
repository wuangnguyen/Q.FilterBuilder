using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class NotBetweenRuleTransformerTests
{
    private readonly NotBetweenRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithTwoIntegerValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18, 65 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Age", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(Age < @p0 || Age > @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(18, parameters[0]);
        Assert.Equal(65, parameters[1]);
    }

    [Fact]
    public void Transform_WithTwoDecimalValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Price", "not_between", new[] { 10.5m, 99.99m });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Price", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(Price < @p0 || Price > @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(10.5m, parameters[0]);
        Assert.Equal(99.99m, parameters[1]);
    }

    [Fact]
    public void Transform_WithDateValuesAndMetadata_ShouldNormalizeDates()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 15, 10, 30, 0);
        var endDate = new DateTime(2023, 1, 20, 14, 45, 0);
        var rule = new FilterRule("CreatedDate", "not_between", new[] { startDate, endDate })
        {
            Metadata = new Dictionary<string, object?> { { "type", "date" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(CreatedDate < @p0 || CreatedDate > @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(startDate.Date, parameters[0]);
        Assert.Equal(endDate.Date.AddDays(1).AddTicks(-1), parameters[1]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Score", "not_between", new List<int> { 70, 100 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Score", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(Score < @p0 || Score > @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(70, parameters[0]);
        Assert.Equal(100, parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", null);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Age", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_BETWEEN operator requires a non-null value", ex.Message);
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18 });

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_BETWEEN operator requires exactly 2 values", ex.Message);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18, 30, 65 });

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_BETWEEN operator requires exactly 2 values", ex.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new int[0]);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_BETWEEN operator requires exactly 2 values", ex.Message);
    }

    [Fact]
    public void Transform_WithNonCollectionValue_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", 25);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_BETWEEN operator requires an array or collection with exactly 2 values", ex.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Age", "not_between", new[] { 18, 65 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "User.Profile.Age", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(User.Profile.Age < @p0 || User.Profile.Age > @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }
    
    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18, 65 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Age", 2, new LinqFormatProvider());

        // Assert
        Assert.Equal("(Age < @p2 || Age > @p3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }
}
