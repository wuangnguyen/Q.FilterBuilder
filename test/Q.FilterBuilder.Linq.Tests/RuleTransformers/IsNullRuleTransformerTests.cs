using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class IsNullRuleTransformerTests
{
    private readonly IsNullRuleTransformer _transformer = new();

    [Fact]
    public void Transform_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "is_null", null);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("Name == null", query);
        Assert.Null(parameters);
    }
}
