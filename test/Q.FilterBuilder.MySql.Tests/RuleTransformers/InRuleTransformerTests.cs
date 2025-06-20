using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;
using System.Linq;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

public class InRuleTransformerTests
{
    private readonly InRuleTransformer _transformer;

    public InRuleTransformerTests()
    {
        _transformer = new InRuleTransformer();
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", "Active");
        var fieldName = "`Status`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Status` IN (?)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("Active", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", new[] { "Active", "Pending", "Completed" });
        var fieldName = "`Status`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Status` IN (?, ?, ?)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("Active", parameters[0]);
        Assert.Equal("Pending", parameters[1]);
        Assert.Equal("Completed", parameters[2]);
    }

    [Fact]
    public void Transform_WithIntegerValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("CategoryId", "in", new[] { 1, 2, 3, 5 });
        var fieldName = "`CategoryId`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`CategoryId` IN (?, ?, ?, ?)", query);
        Assert.NotNull(parameters);
        Assert.Equal(4, parameters.Length);
        Assert.Equal(1, parameters[0]);
        Assert.Equal(2, parameters[1]);
        Assert.Equal(3, parameters[2]);
        Assert.Equal(5, parameters[3]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var values = new List<string> { "Red", "Blue", "Green" };
        var rule = new FilterRule("Color", "in", values);
        var fieldName = "`Color`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Color` IN (?, ?, ?)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("Red", parameters[0]);
        Assert.Equal("Blue", parameters[1]);
        Assert.Equal("Green", parameters[2]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", null);
        var fieldName = "`Status`";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("IN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", new string[0]);
        var fieldName = "`Status`";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", new List<string>());
        var fieldName = "`Status`";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithMixedTypeCollection_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Data", "in", new object[] { 1, "two", 3.0, true });
        var fieldName = "`Data`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Data` IN (?, ?, ?, ?)", query);
        Assert.Equal([1, "two", 3.0, true], parameters);
    }

    [Fact]
    public void Transform_WithNullElementsInCollection_ShouldIncludeNulls()
    {
        // Arrange
        var rule = new FilterRule("NullableField", "in", new object?[] { 1, null, 3 });
        var fieldName = "`NullableField`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`NullableField` IN (?, ?, ?)", query);
        Assert.Equal(new object?[] { 1, null, 3 }, parameters);
    }

    [Fact]
    public void Transform_WithFieldNameWithDot_ShouldFormatWithBackticks()
    {
        // Arrange
        var rule = new FilterRule("User.Name", "in", new[] { "Alice", "Bob" });
        var fieldName = new MySqlFormatProvider().FormatFieldName("User.Name");

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`User`.`Name` IN (?, ?)", query);
        Assert.Equal(["Alice", "Bob"], parameters!.Cast<string>().ToArray());
    }

    [Fact]
    public void Transform_WithLargeCollection_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var values = new int[150];
        for (int i = 0; i < 150; i++) values[i] = i;
        var rule = new FilterRule("BigList", "in", values);
        var fieldName = "`BigList`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal($"`BigList` IN ({string.Join(", ", Enumerable.Repeat("?", 150))})", query);
        Assert.Equal(values, parameters!.Cast<int>().ToArray());
    }

    [Fact]
    public void GenerateParameterPlaceholders_AlwaysReturnsQuestionMarks()
    {
        // Arrange
        var context = new Core.RuleTransformers.BaseRuleTransformer.TransformContext { ParameterIndex = 0 };

        // Act
        var result = _transformer.GetType()
            .GetMethod("GenerateParameterPlaceholders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_transformer, ["ignored", 5, context]) as string[];

        // Assert
        Assert.All(result!, p => Assert.Equal("?", p));
        Assert.Equal(5, result!.Length);
    }

    [Fact]
    public void Transform_WithCustomFormatProvider_StillUsesQuestionMarks()
    {
        // Arrange
        var rule = new FilterRule("Custom", "in", new[] { 1, 2 });
        var fieldName = "`Custom`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Custom` IN (?, ?)", query);
        Assert.Equal([1, 2], parameters!.Cast<int>().ToArray());
    }
}
