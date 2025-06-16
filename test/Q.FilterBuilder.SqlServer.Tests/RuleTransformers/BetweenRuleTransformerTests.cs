using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class BetweenRuleTransformerTests
{
    private readonly BetweenRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithTwoIntegerValues_ShouldGenerateBetweenQuery()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new[] { 18, 65 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Age", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Age BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(18, parameters[0]);
        Assert.Equal(65, parameters[1]);
    }

    [Fact]
    public void Transform_WithTwoDecimalValues_ShouldGenerateBetweenQuery()
    {
        // Arrange
        var rule = new FilterRule("Price", "between", new[] { 10.50m, 99.99m });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Price", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Price BETWEEN @p0 AND @p1", query);
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
        var rule = new FilterRule("CreatedDate", "between", new[] { startDate, endDate })
        {
            Metadata = new Dictionary<string, object?> { { "type", "date" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("CreatedDate BETWEEN @p0 AND @p1", query);
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
        var rule = new FilterRule("Price", "between", new[] { 10.5, 20.5 })
        {
            Metadata = new Dictionary<string, object?> { { "type", "number" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Price", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Price BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(10.5, parameters[0]);
        Assert.Equal(20.5, parameters[1]);
    }

    [Fact]
    public void Transform_WithStringValues_ShouldHandleStringComparison()
    {
        // Arrange
        var rule = new FilterRule("Name", "between", new[] { "A", "M" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("A", parameters[0]);
        Assert.Equal("M", parameters[1]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Score", "between", new List<int> { 70, 100 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Score", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Score BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(70, parameters[0]);
        Assert.Equal(100, parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Age", 0, new SqlServerFormatProvider()));
        Assert.Contains("BETWEEN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new[] { 18 });

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", 0, new SqlServerFormatProvider()));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new[] { 18, 30, 65 });

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", 0, new SqlServerFormatProvider()));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new int[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", 0, new SqlServerFormatProvider()));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithNonCollectionValue_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", 25);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Age", 0, new SqlServerFormatProvider()));
        Assert.Contains("BETWEEN operator requires an array or collection with exactly 2 values", exception.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Age", "between", new[] { 18, 65 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Age]", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("[User].[Profile].[Age] BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Age", "between", new[] { 18, 65 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Age", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Age BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithDateTimeValues_ShouldHandleDateTime()
    {
        // Arrange
        var startDate = new DateTime(2023, 1, 1, 0, 0, 0);
        var endDate = new DateTime(2023, 12, 31, 23, 59, 59);
        var rule = new FilterRule("CreatedDate", "between", new[] { startDate, endDate });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("CreatedDate BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(startDate, parameters[0]);
        Assert.Equal(endDate, parameters[1]);
    }

    [Fact]
    public void Transform_WithMixedNumericTypes_ShouldHandleMixedTypes()
    {
        // Arrange
        var rule = new FilterRule("Value", "between", new object[] { 10, 20.5 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Value", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Value BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(10, parameters[0]);
        Assert.Equal(20.5, parameters[1]);
    }

    [Fact]
    public void Transform_WithNegativeValues_ShouldHandleNegativeNumbers()
    {
        // Arrange
        var rule = new FilterRule("Temperature", "between", new[] { -10, 5 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Temperature", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Temperature BETWEEN @p0 AND @p1", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(-10, parameters[0]);
        Assert.Equal(5, parameters[1]);
    }
}
