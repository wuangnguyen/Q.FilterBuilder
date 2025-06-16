using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

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
        var fieldName = "\"Description\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Description\" IS NOT NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("RequiredField", "is_not_null", "some value");
        var fieldName = "\"RequiredField\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"RequiredField\" IS NOT NULL", query);
        Assert.Null(parameters); // IS NOT NULL doesn't use parameters regardless of rule value
    }

    [Fact]
    public void Transform_WithDifferentFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("MandatoryField", "is_not_null", null);
        var fieldName = "\"MandatoryField\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"MandatoryField\" IS NOT NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Order.Customer.Email", "is_not_null", null);
        var fieldName = "\"Order\".\"Customer\".\"Email\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Order\".\"Customer\".\"Email\" IS NOT NULL", query);
        Assert.Null(parameters);
    }
}
