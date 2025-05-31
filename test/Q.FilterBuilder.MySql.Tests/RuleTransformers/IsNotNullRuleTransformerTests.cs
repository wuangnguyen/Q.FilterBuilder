using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

public class IsNotNullRuleTransformerTests
{
    private readonly IsNotNullRuleTransformer _transformer;

    public IsNotNullRuleTransformerTests()
    {
        _transformer = new IsNotNullRuleTransformer();
    }

    [Fact]
    public void Transform_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Description", "is_not_null", null);
        var fieldName = "`Description`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Description` IS NOT NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Notes", "is_not_null", "some value");
        var fieldName = "`Notes`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`Notes` IS NOT NULL", query);
        Assert.Null(parameters); // IS NOT NULL doesn't use parameters regardless of rule value
    }

    [Fact]
    public void Transform_WithDifferentFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("RequiredField", "is_not_null", null);
        var fieldName = "`RequiredField`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`RequiredField` IS NOT NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Email", "is_not_null", null);
        var fieldName = "`User`.`Profile`.`Email`";
        var parameterName = "?";

        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("`User`.`Profile`.`Email` IS NOT NULL", query);
        Assert.Null(parameters);
    }
}
