using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.Models;
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
        var (query, parameters) = transformer.Transform(rule, "Name", "@0");

        // Assert
        Assert.Equal("Name = @0", query);
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
        var exception = Assert.Throws<ArgumentException>(() => transformer.Transform(rule, "CategoryId", "@0"));
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
        var exception = Assert.Throws<ArgumentException>(() => transformer.Transform(rule, "Values", "@0"));
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
        var (query, parameters) = transformer.Transform(rule, "Description", "@0");

        // Assert
        Assert.Equal("Description != @0", query);
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
        var (query, parameters) = transformer.Transform(rule, "Status", "@0");

        // Assert
        Assert.Equal("Status = @0", query);
        Assert.Null(parameters);
    }
}
