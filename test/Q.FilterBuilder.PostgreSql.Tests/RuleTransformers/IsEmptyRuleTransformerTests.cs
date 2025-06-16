using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

public class IsEmptyRuleTransformerTests
{
    private readonly IsEmptyRuleTransformer _transformer;

    public IsEmptyRuleTransformerTests()
    {
        _transformer = new IsEmptyRuleTransformer();
    }

    [Fact]
    public void Transform_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Description", "is_empty", null);
        var fieldName = "\"Description\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Description\" = ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Notes", "is_empty", "some value");
        var fieldName = "\"Notes\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Notes\" = ''", query);
        Assert.Null(parameters); // IS EMPTY doesn't use parameters regardless of rule value
    }

    [Fact]
    public void Transform_WithDifferentFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Comment", "is_empty", null);
        var fieldName = "\"Comment\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"Comment\" = ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Bio", "is_empty", null);
        var fieldName = "\"User\".\"Profile\".\"Bio\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("\"User\".\"Profile\".\"Bio\" = ''", query);
        Assert.Null(parameters);
    }
}
