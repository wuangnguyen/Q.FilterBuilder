using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.RuleTransformers;

public class SimpleNoParameterTransformerTests
{
    [Fact]
    public void BuildParameters_WithAnyValue_ShouldReturnNull()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();

        // Act
        var result1 = transformer.TestBuildParameters("some_value", null);
        var result2 = transformer.TestBuildParameters(123, null);
        var result3 = transformer.TestBuildParameters(null, null);
        var result4 = transformer.TestBuildParameters(new object[] { 1, 2, 3 }, null);

        // Assert
        Assert.Null(result1);
        Assert.Null(result2);
        Assert.Null(result3);
        Assert.Null(result4);
    }

    [Fact]
    public void BuildQuery_ShouldCallBuildSimpleQuery()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var fieldName = "[TestField]";
        var parameterName = "@p0";

        // Act
        var result = transformer.TestBuildQuery(fieldName, parameterName);

        // Assert
        Assert.Equal("[TestField] IS NULL", result);
    }

    [Fact]
    public void Transform_WithValidRule_ShouldReturnQueryWithNullParameters()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var rule = new FilterRule("TestField", "is_null", null);
        var fieldName = "[TestField]";
        var parameterName = "@p0";

        // Act
        var (query, parameters) = transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("[TestField] IS NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithNonNullValue_ShouldStillReturnNullParameters()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var rule = new FilterRule("TestField", "is_null", "ignored_value");
        var fieldName = "[TestField]";
        var parameterName = "@p0";

        // Act
        var (query, parameters) = transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("[TestField] IS NULL", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithMetadata_ShouldPreserveMetadata()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var rule = new FilterRule("TestField", "is_null", null, "string");
        rule.Metadata = new Dictionary<string, object?> { ["custom"] = "value" };
        var fieldName = "[TestField]";
        var parameterName = "@p0";

        // Act
        var (query, parameters) = transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("[TestField] IS NULL", query);
        Assert.Null(parameters);
        Assert.Contains("custom", rule.Metadata.Keys);
        Assert.Equal("value", rule.Metadata["custom"]);
    }

    [Fact]
    public void BuildParameters_WithMetadata_ShouldStillReturnNull()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var metadata = new Dictionary<string, object?> { ["key"] = "value" };

        // Act
        var result = transformer.TestBuildParameters("value", metadata);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void BuildQuery_WithDifferentFieldNames_ShouldReturnCorrectQuery()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();

        // Act
        var result1 = transformer.TestBuildQuery("[Name]", "@p0");
        var result2 = transformer.TestBuildQuery("`email`", "?");
        var result3 = transformer.TestBuildQuery("\"age\"", ":p1");

        // Assert
        Assert.Equal("[Name] IS NULL", result1);
        Assert.Equal("`email` IS NULL", result2);
        Assert.Equal("\"age\" IS NULL", result3);
    }

    [Fact]
    public void BuildQuery_ParameterNameShouldBeIgnored()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var fieldName = "[TestField]";

        // Act
        var result1 = transformer.TestBuildQuery(fieldName, "@p0");
        var result2 = transformer.TestBuildQuery(fieldName, "@p999");
        var result3 = transformer.TestBuildQuery(fieldName, "?");

        // Assert - All should return the same result since parameter name is ignored
        Assert.Equal("[TestField] IS NULL", result1);
        Assert.Equal("[TestField] IS NULL", result2);
        Assert.Equal("[TestField] IS NULL", result3);
    }

    private class TestSimpleNoParameterTransformer : SimpleNoParameterTransformer
    {
        protected override string BuildSimpleQuery(string fieldName)
        {
            return $"{fieldName} IS NULL";
        }

        // Expose protected methods for testing
        public object[]? TestBuildParameters(object? value, Dictionary<string, object?>? metadata)
        {
            return BuildParameters(value, metadata);
        }

        public string TestBuildQuery(string fieldName, string parameterName)
        {
            var context = new TransformContext
            {
                Parameters = null,
                Metadata = new Dictionary<string, object?>()
            };
            return BuildQuery(fieldName, parameterName, context);
        }
    }
}
