using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class IsNotEmptyRuleTransformerTests
{
    private readonly IsNotEmptyRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithAnyValue_ShouldGenerateIsNotEmptyQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "is_not_empty", "any_value");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name <> ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldGenerateIsNotEmptyQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "is_not_empty", null);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name <> ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "is_not_empty", null);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Name]", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("[User].[Profile].[Name] <> ''", query);
        Assert.Null(parameters);
    }
}
