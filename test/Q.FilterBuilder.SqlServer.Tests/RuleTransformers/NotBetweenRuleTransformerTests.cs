using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class NotBetweenRuleTransformerTests
{
    private readonly NotBetweenRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithTwoIntegerValues_ShouldGenerateNotBetweenQuery()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18, 65 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Age", "@param");

        // Assert
        Assert.Equal("Age NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(18, parameters[0]);
        Assert.Equal(65, parameters[1]);
    }

    [Fact]
    public void Transform_WithTwoDecimalValues_ShouldGenerateNotBetweenQuery()
    {
        // Arrange
        var rule = new FilterRule("Price", "not_between", new[] { 10.50m, 99.99m });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Price", "@param");

        // Assert
        Assert.Equal("Price NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(10.50m, parameters[0]);
        Assert.Equal(99.99m, parameters[1]);
    }

    [Fact]
    public void Transform_WithDateValues_ShouldNormalizeDates()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 15, 10, 30, 0);
        var endDate = new DateTime(2023, 1, 20, 14, 45, 0);
        var rule = new FilterRule("CreatedDate", "not_between", new[] { startDate, endDate })
        {
            Metadata = new Dictionary<string, object?> { { "type", "date" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", "@param");

        // Assert
        Assert.Equal("CreatedDate NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        
        var normalizedStart = (DateTime)parameters[0];
        var normalizedEnd = (DateTime)parameters[1];
        
        Assert.Equal(new DateTime(2023, 1, 15, 0, 0, 0), normalizedStart);
        Assert.Equal(new DateTime(2023, 1, 20, 23, 59, 59, 999).AddTicks(9999), normalizedEnd);
    }

    [Fact]
    public void Transform_WithNonDateValues_ShouldNotNormalize()
    {
        // Arrange
        var rule = new FilterRule("Price", "not_between", new[] { 10.5, 20.5 })
        {
            Metadata = new Dictionary<string, object?> { { "type", "number" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Price", "@param");

        // Assert
        Assert.Equal("Price NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(10.5, parameters[0]);
        Assert.Equal(20.5, parameters[1]);
    }

    [Fact]
    public void Transform_WithStringValues_ShouldHandleStringComparison()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_between", new[] { "A", "M" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("Name NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("A", parameters[0]);
        Assert.Equal("M", parameters[1]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Score", "not_between", new List<int> { 70, 100 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Score", "@param");

        // Assert
        Assert.Equal("Score NOT BETWEEN @param0 AND @param1", query);
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

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Age", "@param"));
        Assert.Contains("NOT_BETWEEN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18 });

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", "@param"));
        Assert.Contains("NOT_BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18, 30, 65 });

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", "@param"));
        Assert.Contains("NOT_BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new int[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", "@param"));
        Assert.Contains("NOT_BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithNonCollectionValue_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", 25);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", "@param"));
        Assert.Contains("NOT_BETWEEN operator requires an array or collection with exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Age", "not_between", new[] { 18, 65 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Age]", "@param");

        // Assert
        Assert.Equal("[User].[Profile].[Age] NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 18, 65 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Age", "@customParam");

        // Assert
        Assert.Equal("Age NOT BETWEEN @customParam0 AND @customParam1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithDateTimeValues_ShouldHandleDateTime()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 1, 0, 0, 0);
        var endDate = new DateTime(2023, 12, 31, 23, 59, 59);
        var rule = new FilterRule("CreatedDate", "not_between", new[] { startDate, endDate });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", "@param");

        // Assert
        Assert.Equal("CreatedDate NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(startDate, parameters[0]);
        Assert.Equal(endDate, parameters[1]);
    }

    [Fact]
    public void Transform_WithMixedNumericTypes_ShouldHandleMixedTypes()
    {
        // Arrange
        var rule = new FilterRule("Value", "not_between", new object[] { 10, 20.5 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Value", "@param");

        // Assert
        Assert.Equal("Value NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(10, parameters[0]);
        Assert.Equal(20.5, parameters[1]);
    }

    [Fact]
    public void Transform_WithNegativeValues_ShouldHandleNegativeNumbers()
    {
        // Arrange
        var rule = new FilterRule("Temperature", "not_between", new[] { -10, 5 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Temperature", "@param");

        // Assert
        Assert.Equal("Temperature NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(-10, parameters[0]);
        Assert.Equal(5, parameters[1]);
    }

    [Fact]
    public void Transform_WithZeroValues_ShouldHandleZeroValues()
    {
        // Arrange
        var rule = new FilterRule("Balance", "not_between", new[] { 0, 0 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Balance", "@param");

        // Assert
        Assert.Equal("Balance NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(0, parameters[0]);
        Assert.Equal(0, parameters[1]);
    }

    [Fact]
    public void Transform_WithReversedValues_ShouldPreserveOrder()
    {
        // Arrange
        var rule = new FilterRule("Age", "not_between", new[] { 65, 18 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Age", "@param");

        // Assert
        Assert.Equal("Age NOT BETWEEN @param0 AND @param1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(65, parameters[0]);
        Assert.Equal(18, parameters[1]);
    }
}
