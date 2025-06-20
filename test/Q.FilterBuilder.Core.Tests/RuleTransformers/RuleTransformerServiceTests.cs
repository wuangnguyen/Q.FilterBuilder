using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.Providers;
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

    [Fact]
    public void RegisterTransformer_WithNullOperatorName_ShouldThrowArgumentException()
    {
        // Arrange
        var customTransformer = new TestRuleTransformer();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.RegisterTransformer(null!, customTransformer));
    }

    [Fact]
    public void RegisterTransformer_WithEmptyOperatorName_ShouldThrowArgumentException()
    {
        // Arrange
        var customTransformer = new TestRuleTransformer();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.RegisterTransformer(string.Empty, customTransformer));
    }

    [Fact]
    public void RegisterTransformer_WithNullTransformer_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.RegisterTransformer("custom", null!));
    }

    private class TestRuleTransformer : IRuleTransformer
    {
        public (string query, object[]? parameters) Transform(FilterRule rule, string fieldName, int parameterIndex, IQueryFormatProvider formatProvider)
        {
            return ("test query", new object[] { "test" });
        }
    }
}
