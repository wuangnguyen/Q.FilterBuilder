using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.Providers;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.RuleTransformers;

public class BasicRuleTransformerTests
{
    [Fact]
    public void Constructor_WithValidOperator_ShouldCreateInstance()
    {
        // Act
        var transformer = new BasicRuleTransformer("=");

        // Assert
        Assert.NotNull(transformer);
    }

    [Fact]
    public void Constructor_WithNullOperator_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BasicRuleTransformer(null!));
    }

    [Fact]
    public void Constructor_WithEmptyOperator_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BasicRuleTransformer(""));
    }

    [Fact]
    public void Transform_WithSingleValue_ShouldReturnCorrectQuery()
    {
        // Arrange
        var transformer = new BasicRuleTransformer("=");
        var rule = new FilterRule("Name", "equal", "John");

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, "Name", 0, formatProvider);

        // Assert
        Assert.Equal("Name = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("John", parameters[0]);
    }

    [Fact]
    public void Transform_WithCollectionValue_ShouldThrowArgumentException()
    {
        // Arrange
        var transformer = new BasicRuleTransformer("=");
        var rule = new FilterRule("CategoryId", "equal", new[] { 1, 2, 3 });

        // Act & Assert
        var formatProvider = new TestFormatProvider();
        var exception = Assert.Throws<ArgumentException>(() => transformer.Transform(rule, "CategoryId", 0, formatProvider));
        Assert.Contains("cannot compare with collections", exception.Message);
        Assert.Contains("=", exception.Message);
    }

    [Fact]
    public void Transform_WithListValue_ShouldThrowArgumentException()
    {
        // Arrange
        var transformer = new BasicRuleTransformer(">");
        var rule = new FilterRule("Values", "greater", new List<int> { 1, 2, 3 });

        // Act & Assert
        var formatProvider = new TestFormatProvider();
        var exception = Assert.Throws<ArgumentException>(() => transformer.Transform(rule, "Values", 0, formatProvider));
        Assert.Contains("cannot compare with collections", exception.Message);
        Assert.Contains(">", exception.Message);
    }

    [Fact]
    public void Transform_WithStringValue_ShouldNotThrowException()
    {
        // Arrange
        var transformer = new BasicRuleTransformer("!=");
        var rule = new FilterRule("Description", "not_equal", "test string");

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, "Description", 0, formatProvider);

        // Assert
        Assert.Equal("Description != @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test string", parameters[0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldReturnNullParameters()
    {
        // Arrange
        var transformer = new BasicRuleTransformer("=");
        var rule = new FilterRule("Status", "equal", null);

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, "Status", 0, formatProvider);

        // Assert
        Assert.Equal("Status = @p0", query);
        Assert.Null(parameters);
    }

    private class TestFormatProvider : IQueryFormatProvider
    {
        public string FormatFieldName(string fieldName) => fieldName;
        public string FormatParameterName(int index) => $"@p{index}";
        public string ParameterPrefix => "@";
        public string AndOperator => " AND ";
        public string OrOperator => " OR ";
    }
}
