using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class IsNotEmptyRuleTransformerTests
{
    private readonly IsNotEmptyRuleTransformer _transformer = new();

    [Fact]
    public void Transform_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "is_not_empty", null);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("Name != string.Empty", query);
        Assert.Null(parameters);
    }
}
