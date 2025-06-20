using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;
using System.Linq;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

public class NotInRuleTransformerTests
{
    private readonly NotInRuleTransformer _transformer;

    public NotInRuleTransformerTests()
    {
        _transformer = new NotInRuleTransformer();
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", "Inactive");
        var fieldName = "`Status`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Status` NOT IN (?)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("Inactive", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new[] { "Inactive", "Deleted", "Archived" });
        var fieldName = "`Status`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Status` NOT IN (?, ?, ?)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("Inactive", parameters[0]);
        Assert.Equal("Deleted", parameters[1]);
        Assert.Equal("Archived", parameters[2]);
    }

    [Fact]
    public void Transform_WithIntegerValues_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("CategoryId", "not_in", new[] { 4, 6, 7 });
        var fieldName = "`CategoryId`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`CategoryId` NOT IN (?, ?, ?)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(4, parameters[0]);
        Assert.Equal(6, parameters[1]);
        Assert.Equal(7, parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var values = new List<string> { "Yellow", "Purple" };
        var rule = new FilterRule("Color", "not_in", values);
        var fieldName = "`Color`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Color` NOT IN (?, ?)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("Yellow", parameters[0]);
        Assert.Equal("Purple", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", null);
        var fieldName = "`Status`";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("NOT_IN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new string[0]);
        var fieldName = "`Status`";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("NOT_IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new List<string>());
        var fieldName = "`Status`";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider()));
        Assert.Contains("NOT_IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithMixedTypeCollection_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Data", "not_in", new object[] { 1, "two", 3.0, true });
        var fieldName = "`Data`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Data` NOT IN (?, ?, ?, ?)", query);
        Assert.Equal([1, "two", 3.0, true], parameters);
    }

    [Fact]
    public void Transform_WithNullElementsInCollection_ShouldIncludeNulls()
    {
        // Arrange
        var rule = new FilterRule("NullableField", "not_in", new object?[] { 1, null, 3 });
        var fieldName = "`NullableField`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`NullableField` NOT IN (?, ?, ?)", query);
        Assert.Equal(new object?[] { 1, null, 3 }, parameters);
    }

    [Fact]
    public void Transform_WithFieldNameWithDot_ShouldFormatWithBackticks()
    {
        // Arrange
        var rule = new FilterRule("User.Name", "not_in", new[] { "Alice", "Bob" });
        var fieldName = new MySqlFormatProvider().FormatFieldName("User.Name");

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`User`.`Name` NOT IN (?, ?)", query);
        Assert.Equal(["Alice", "Bob"], parameters);
    }

    [Fact]
    public void Transform_WithLargeCollection_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var values = new int[120];
        for (int i = 0; i < 120; i++) values[i] = i;
        var rule = new FilterRule("BigList", "not_in", values);
        var fieldName = "`BigList`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal($"`BigList` NOT IN ({string.Join(", ", Enumerable.Repeat("?", 120))})", query);
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
        var rule = new FilterRule("Custom", "not_in", new[] { 1, 2 });
        var fieldName = "`Custom`";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Custom` NOT IN (?, ?)", query);
        Assert.Equal([1, 2], parameters!.Cast<int>().ToArray());
    }
}
