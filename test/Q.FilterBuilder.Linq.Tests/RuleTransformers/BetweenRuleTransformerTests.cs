using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class BetweenRuleTransformerTests
{
    private readonly BetweenRuleTransformer _transformer = new();
    private readonly LinqFormatProvider _formatProvider = new();

    [Fact]
    public void BuildParameters_WithNullValue_ThrowsArgumentNullException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(new FilterRule("Field", "between", null), "Field", 0, _formatProvider));
        Assert.Contains("BETWEEN operator requires a non-null value", ex.Message);
    }

    [Fact]
    public void BuildParameters_WithNonCollectionValue_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(new FilterRule("Field", "between", 123), "Field", 0, _formatProvider));
        Assert.Contains("BETWEEN operator requires an array or collection with exactly 2 values", ex.Message);
    }

    [Fact]
    public void BuildParameters_WithEmptyArray_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(new FilterRule("Field", "between", new object[0]), "Field", 0, _formatProvider));
        Assert.Contains("BETWEEN operator requires exactly 2 values", ex.Message);
    }

    [Fact]
    public void BuildParameters_WithOneElement_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(new FilterRule("Field", "between", new object[] { 1 }), "Field", 0, _formatProvider));
        Assert.Contains("BETWEEN operator requires exactly 2 values", ex.Message);
    }

    [Fact]
    public void BuildParameters_WithThreeElements_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(new FilterRule("Field", "between", new object[] { 1, 2, 3 }), "Field", 0, _formatProvider));
        Assert.Contains("BETWEEN operator requires exactly 2 values", ex.Message);
    }

    [Fact]
    public void BuildParameters_WithTwoElements_ReturnsArray()
    {
        // Arrange
        var rule = new FilterRule("Field", "between", new object[] { 10, 20 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Field", 0, _formatProvider);

        // Assert
        Assert.Equal("Field >= @p0 && Field <= @p1", query);
        Assert.Equal([10, 20], parameters);
    }

    [Fact]
    public void BuildParameters_WithTwoDateElementsAndDateMetadata_NormalizesDates()
    {
        // Arrange
        var start = new DateTime(2023, 1, 1, 10, 0, 0);
        var end = new DateTime(2023, 1, 2, 23, 59, 59);
        var rule = new FilterRule("Field", "between", new object[] { start, end });
        rule.Metadata = new Dictionary<string, object?> { { "type", "date" } };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Field", 0, _formatProvider);

        // Assert
        Assert.Equal("Field >= @p0 && Field <= @p1", query);
        Assert.Equal(DateTime.SpecifyKind(start.Date, DateTimeKind.Unspecified), parameters![0]);
        Assert.Equal(DateTime.SpecifyKind(end.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified), parameters![1]);
    }

    [Fact]
    public void BuildQuery_UsesCorrectParameterNames()
    {
        // Arrange
        var rule = new FilterRule("Field", "between", new object[] { 1, 2 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Field", 5, _formatProvider);

        // Assert
        Assert.Equal("Field >= @p5 && Field <= @p6", query);
    }
}
