using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.MySql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests.RuleTransformers;

public class IsNotEmptyRuleTransformerTests
{
    private readonly IsNotEmptyRuleTransformer _transformer;

    public IsNotEmptyRuleTransformerTests()
    {
        _transformer = new IsNotEmptyRuleTransformer();
    }

    [Fact]
    public void Transform_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Description", "is_not_empty", null);
        var fieldName = "`Description`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Description` != ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Notes", "is_not_empty", "some value");
        var fieldName = "`Notes`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Notes` != ''", query);
        Assert.Null(parameters); // IS NOT EMPTY doesn't use parameters regardless of rule value
    }

    [Fact]
    public void Transform_WithDifferentFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Title", "is_not_empty", null);
        var fieldName = "`Title`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`Title` != ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "is_not_empty", null);
        var fieldName = "`User`.`Profile`.`Name`";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new MySqlFormatProvider());

        // Assert
        Assert.Equal("`User`.`Profile`.`Name` != ''", query);
        Assert.Null(parameters);
    }
}
