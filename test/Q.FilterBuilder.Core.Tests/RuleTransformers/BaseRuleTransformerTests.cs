using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.Providers;
using Q.FilterBuilder.Core.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.RuleTransformers;

public class BaseRuleTransformerTests
{
    [Fact]
    public void Transform_WithValidRule_ShouldCallBuildParametersAndBuildQuery()
    {
        // Arrange
        var transformer = new TestBaseRuleTransformer();
        var rule = new FilterRule("TestField", "test_op", "test_value");
        var fieldName = "FormattedField";

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, fieldName, 0, formatProvider);

        // Assert
        Assert.Equal("FormattedField TEST @p0", query);
        Assert.Single(parameters!);
        Assert.Equal("test_value", parameters![0]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldReturnNullParameters()
    {
        // Arrange
        var transformer = new TestBaseRuleTransformer();
        var rule = new FilterRule("TestField", "test_op", null);
        var fieldName = "FormattedField";

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, fieldName, 0, formatProvider);

        // Assert
        Assert.Equal("FormattedField TEST @p0", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithExistingMetadata_ShouldPreserveMetadata()
    {
        // Arrange
        var transformer = new TestBaseRuleTransformer();
        var rule = new FilterRule("TestField", "test_op", "test_value", "string");
        rule.Metadata = new Dictionary<string, object?> { ["custom"] = "value" };
        var fieldName = "FormattedField";

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, fieldName, 0, formatProvider);

        // Assert
        Assert.Equal("FormattedField TEST @p0", query);
        Assert.Contains("custom", rule.Metadata.Keys);
        Assert.Equal("value", rule.Metadata["custom"]);
        Assert.Contains("type", rule.Metadata.Keys);
        Assert.Equal("string", rule.Metadata["type"]);
    }

    [Fact]
    public void Transform_WithNullMetadata_ShouldCreateMetadataWithType()
    {
        // Arrange
        var transformer = new TestBaseRuleTransformer();
        var rule = new FilterRule("TestField", "test_op", "test_value", "int");
        var fieldName = "FormattedField";

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, fieldName, 0, formatProvider);

        // Assert
        Assert.NotNull(rule.Metadata);
        Assert.Contains("type", rule.Metadata.Keys);
        Assert.Equal("int", rule.Metadata["type"]);
    }

    [Fact]
    public void Transform_WithEmptyType_ShouldSetTypeInMetadata()
    {
        // Arrange
        var transformer = new TestBaseRuleTransformer();
        var rule = new FilterRule("TestField", "test_op", "test_value");
        var fieldName = "FormattedField";

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, fieldName, 0, formatProvider);

        // Assert
        Assert.NotNull(rule.Metadata);
        Assert.Contains("type", rule.Metadata.Keys);
        Assert.Equal(string.Empty, rule.Metadata["type"]);
    }

    [Fact]
    public void Transform_WithExistingTypeInMetadata_ShouldNotOverrideType()
    {
        // Arrange
        var transformer = new TestBaseRuleTransformer();
        var rule = new FilterRule("TestField", "test_op", "test_value", "string");
        rule.Metadata = new Dictionary<string, object?> { ["type"] = "custom_type" };
        var fieldName = "FormattedField";

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, fieldName, 0, formatProvider);

        // Assert
        Assert.Equal("custom_type", rule.Metadata["type"]);
    }

    [Fact]
    public void BuildParameters_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        var transformer = new TestBaseRuleTransformer();

        // Act
        var result = transformer.TestBuildParameters(null, null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void BuildParameters_WithValue_ShouldReturnSingleElementArray()
    {
        // Arrange
        var transformer = new TestBaseRuleTransformer();
        var value = "test_value";

        // Act
        var result = transformer.TestBuildParameters(value, null);

        // Assert
        Assert.Single(result!);
        Assert.Equal(value, result![0]);
    }

    [Fact]
    public void BuildParameters_WithComplexValue_ShouldReturnValueAsIs()
    {
        // Arrange
        var transformer = new TestBaseRuleTransformer();
        var value = new { Name = "Test", Value = 42 };

        // Act
        var result = transformer.TestBuildParameters(value, null);

        // Assert
        Assert.Single(result!);
        Assert.Same(value, result![0]);
    }

    private class TestBaseRuleTransformer : BaseRuleTransformer
    {
        protected override string BuildQuery(string fieldName, TransformContext context)
        {
            var parameterName = context.FormatProvider?.FormatParameterName(context.ParameterIndex) ?? "@p0";
            return $"{fieldName} TEST {parameterName}";
        }

        // Expose protected method for testing
        public object[]? TestBuildParameters(object? value, Dictionary<string, object?>? metadata)
        {
            return BuildParameters(value, metadata);
        }
    }

    private class TestFormatProvider : IQueryFormatProvider
    {
        public string FormatFieldName(string fieldName) => fieldName;
        public string FormatParameterName(int index) => $"@p{index}";
        public string ParameterPrefix => "@";
        public string AndOperator => " AND ";
        public string OrOperator => " OR ";
    }
}
