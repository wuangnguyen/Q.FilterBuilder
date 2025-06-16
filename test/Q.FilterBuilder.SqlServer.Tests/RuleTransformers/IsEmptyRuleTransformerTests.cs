using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class IsEmptyRuleTransformerTests
{
    private readonly IsEmptyRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithAnyValue_ShouldGenerateIsEmptyQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "is_empty", "any_value");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name = ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldGenerateIsEmptyQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "is_empty", null);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name = ''", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "is_empty", null);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Name]", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("[User].[Profile].[Name] = ''", query);
        Assert.Null(parameters);
    }
}
