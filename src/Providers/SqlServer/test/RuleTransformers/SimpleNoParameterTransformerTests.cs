using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class SimpleNoParameterTransformerTests
{
    private class TestSimpleNoParameterTransformer : SimpleNoParameterTransformer
    {
        protected override string BuildSimpleQuery(string fieldName)
        {
            return $"{fieldName} IS TEST";
        }
    }

    [Fact]
    public void Transform_WithAnyValue_ShouldReturnNullParameters()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var rule = new FilterRule("TestField", "test", "any_value");

        // Act
        var (query, parameters) = transformer.Transform(rule, "TestField", "@param");

        // Assert
        Assert.Equal("TestField IS TEST", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldReturnNullParameters()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var rule = new FilterRule("TestField", "test", null);

        // Act
        var (query, parameters) = transformer.Transform(rule, "TestField", "@param");

        // Assert
        Assert.Equal("TestField IS TEST", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithMetadata_ShouldIgnoreMetadataAndReturnNullParameters()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var rule = new FilterRule("TestField", "test", "value")
        {
            Metadata = new Dictionary<string, object?> { { "key", "value" } }
        };

        // Act
        var (query, parameters) = transformer.Transform(rule, "TestField", "@param");

        // Assert
        Assert.Equal("TestField IS TEST", query);
        Assert.Null(parameters);
    }

    [Fact]
    public void Transform_WithDifferentFieldName_ShouldUseProvidedFieldName()
    {
        // Arrange
        var transformer = new TestSimpleNoParameterTransformer();
        var rule = new FilterRule("OriginalField", "test", "value");

        // Act
        var (query, parameters) = transformer.Transform(rule, "CustomFieldName", "@param");

        // Assert
        Assert.Equal("CustomFieldName IS TEST", query);
        Assert.Null(parameters);
    }
}
