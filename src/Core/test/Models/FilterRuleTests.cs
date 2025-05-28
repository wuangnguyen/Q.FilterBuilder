using Q.FilterBuilder.Core.Models;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.Models;

public class FilterRuleTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var fieldName = "Age";
        var operatorName = "greater";
        var value = 25;
        var type = "int";

        // Act
        var rule = new FilterRule(fieldName, operatorName, value, type);

        // Assert
        Assert.Equal(fieldName, rule.FieldName);
        Assert.Equal(operatorName, rule.Operator);
        Assert.Equal(value, rule.Value);
        Assert.Equal(type, rule.Type);
        Assert.Null(rule.Metadata);
    }

    [Fact]
    public void Constructor_WithoutExplicitType_ShouldSetEmptyType()
    {
        // Arrange
        var fieldName = "Name";
        var operatorName = "equal";
        var value = "John";

        // Act
        var rule = new FilterRule(fieldName, operatorName, value);

        // Assert
        Assert.Equal(fieldName, rule.FieldName);
        Assert.Equal(operatorName, rule.Operator);
        Assert.Equal(value, rule.Value);
        Assert.Equal(string.Empty, rule.Type);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidFieldName_ShouldThrowArgumentNullException(string? fieldName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new FilterRule(fieldName!, "equal", "value"));

        Assert.Equal("fieldName", exception.ParamName);
        Assert.Contains("FieldName cannot be null or whitespace", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidOperator_ShouldThrowArgumentNullException(string? operatorName)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new FilterRule("FieldName", operatorName!, "value"));

        Assert.Equal("operator", exception.ParamName);
        Assert.Contains("Operator cannot be null or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_WithNullValue_ShouldAllowNullValue()
    {
        // Act
        var rule = new FilterRule("FieldName", "is_null", null);

        // Assert
        Assert.Null(rule.Value);
    }

    [Fact]
    public void Value_ShouldBeSettable()
    {
        // Arrange
        var rule = new FilterRule("FieldName", "equal", "initial");
        var newValue = "updated";

        // Act
        rule.Value = newValue;

        // Assert
        Assert.Equal(newValue, rule.Value);
    }

    [Fact]
    public void Metadata_ShouldBeSettable()
    {
        // Arrange
        var rule = new FilterRule("FieldName", "equal", "value");
        var metadata = new Dictionary<string, object?> { ["key"] = "value" };

        // Act
        rule.Metadata = metadata;

        // Assert
        Assert.Same(metadata, rule.Metadata);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Constructor_WithNullOrEmptyType_ShouldSetEmptyString(string? type)
    {
        // Act
        var rule = new FilterRule("FieldName", "equal", "value", type);

        // Assert
        Assert.Equal(string.Empty, rule.Type);
    }

    [Fact]
    public void Constructor_WithComplexValue_ShouldStoreValue()
    {
        // Arrange
        var complexValue = new { Name = "John", Age = 25 };

        // Act
        var rule = new FilterRule("User", "equal", complexValue);

        // Assert
        Assert.Same(complexValue, rule.Value);
    }

    [Fact]
    public void Constructor_WithArrayValue_ShouldStoreArray()
    {
        // Arrange
        var arrayValue = new[] { 1, 2, 3, 4, 5 };

        // Act
        var rule = new FilterRule("Numbers", "in", arrayValue, "int");

        // Assert
        Assert.Same(arrayValue, rule.Value);
    }
}
