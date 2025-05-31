using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

public class IsNullRuleTransformerTests
{
    private readonly IsNullRuleTransformer _transformer;

    public IsNullRuleTransformerTests()
    {
        _transformer = new IsNullRuleTransformer();
    }

    [Fact]
    public void Transform_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Description", "is_null", null);
        var fieldName = "\"Description\"";
        var parameterName = "$1";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("\"Description\" IS NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Notes", "is_null", "some value");
        var fieldName = "\"Notes\"";
        var parameterName = "$2";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("\"Notes\" IS NULL", query);
        Assert.Null(parameters); // IS NULL doesn't use parameters regardless of rule value
    }

    [Fact]
    public void Transform_WithDifferentFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("OptionalField", "is_null", null);
        var fieldName = "\"OptionalField\"";
        var parameterName = "$5";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("\"OptionalField\" IS NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Avatar", "is_null", null);
        var fieldName = "\"User\".\"Profile\".\"Avatar\"";
        var parameterName = "$10";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("\"User\".\"Profile\".\"Avatar\" IS NULL", query);
        Assert.Null(parameters);
    }
}
