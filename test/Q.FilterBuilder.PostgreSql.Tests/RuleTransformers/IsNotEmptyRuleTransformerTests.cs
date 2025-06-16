using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

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
        var fieldName = "\"Description\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Description\" != ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Title", "is_not_empty", "some value");
        var fieldName = "\"Title\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Title\" != ''", query);
        Assert.Null(parameters); // IS NOT EMPTY doesn't use parameters regardless of rule value
    }

    [Fact]
    public void Transform_WithDifferentFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Content", "is_not_empty", null);
        var fieldName = "\"Content\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Content\" != ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Article.Body.Text", "is_not_empty", null);
        var fieldName = "\"Article\".\"Body\".\"Text\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Article\".\"Body\".\"Text\" != ''", query);
        Assert.Null(parameters);
    }
}
