using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.Models;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.RuleTransformers;

public class RuleTransformerServiceTests
{
    private readonly RuleTransformerService _service;

    public RuleTransformerServiceTests()
    {
        _service = new RuleTransformerService();
    }

    [Fact]
    public void GetTransformer_WithValidOperator_ShouldReturnTransformer()
    {
        // Act
        var transformer = _service.GetRuleTransformer("equal");

        // Assert
        Assert.NotNull(transformer);
        Assert.IsAssignableFrom<IRuleTransformer>(transformer);
    }

    [Fact]
    public void GetTransformer_WithInvalidOperator_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        var exception = Assert.Throws<NotImplementedException>(() => _service.GetRuleTransformer("invalid_operator"));
        Assert.Contains("invalid_operator", exception.Message);
    }

    [Fact]
    public void GetTransformer_WithNullOperator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.GetRuleTransformer(null!));
    }

    [Fact]
    public void RegisterTransformer_WithValidParameters_ShouldRegisterSuccessfully()
    {
        // Arrange
        var customTransformer = new TestRuleTransformer();

        // Act
        _service.RegisterTransformer("custom", customTransformer);

        // Assert
        var retrievedTransformer = _service.GetRuleTransformer("custom");
        Assert.Same(customTransformer, retrievedTransformer);
    }



    private class TestRuleTransformer : IRuleTransformer
    {
        public (string query, object[]? parameters) Transform(FilterRule rule, string fieldName, string parameterName)
        {
            return ("test query", new object[] { "test" });
        }
    }
}
