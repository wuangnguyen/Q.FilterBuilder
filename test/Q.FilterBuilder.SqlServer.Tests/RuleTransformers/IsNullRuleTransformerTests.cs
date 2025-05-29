using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class IsNullRuleTransformerTests
{
    private readonly IsNullRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithAnyValue_ShouldGenerateIsNullQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "is_null", "any_value");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("Name IS NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldGenerateIsNullQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "is_null", null);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("Name IS NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "is_null", null);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Name]", "@param");

        // Assert
        Assert.Equal("[User].[Profile].[Name] IS NULL", query);
        Assert.Null(parameters);
    }
}
