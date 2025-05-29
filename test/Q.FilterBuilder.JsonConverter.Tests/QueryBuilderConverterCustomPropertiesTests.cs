using System.Text.Json;
using Q.FilterBuilder.Core.Models;
using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests;

public class QueryBuilderConverterCustomPropertiesTests
{
    [Fact]
    public void Read_WithCustomConditionProperty_ShouldDeserializeCorrectly()
    {
        // Arrange
        var customOptions = new QueryBuilderOptions
        {
            ConditionPropertyName = "combinator"
        };
        var converter = new QueryBuilderConverter(customOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };
        
        var json = """
        {
            "combinator": "AND",
            "rules": [
                {
                    "field": "Name",
                    "operator": "equal",
                    "value": "Test",
                    "type": "string"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("AND", result.Condition);
        Assert.Single(result.Rules);
    }

    [Fact]
    public void Read_WithCustomRulesProperty_ShouldDeserializeCorrectly()
    {
        // Arrange
        var customOptions = new QueryBuilderOptions
        {
            RulesPropertyName = "children"
        };
        var converter = new QueryBuilderConverter(customOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };
        
        var json = """
        {
            "condition": "OR",
            "children": [
                {
                    "field": "Status",
                    "operator": "equal",
                    "value": "Active",
                    "type": "string"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OR", result.Condition);
        Assert.Single(result.Rules);
        Assert.Equal("Status", result.Rules[0].FieldName);
    }

    [Fact]
    public void Read_WithCustomFieldProperty_ShouldDeserializeCorrectly()
    {
        // Arrange
        var customOptions = new QueryBuilderOptions
        {
            FieldPropertyName = "id"
        };
        var converter = new QueryBuilderConverter(customOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };
        
        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "id": "UserName",
                    "operator": "contains",
                    "value": "admin",
                    "type": "string"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.Equal("UserName", result.Rules[0].FieldName);
        Assert.Equal("contains", result.Rules[0].Operator);
    }

    [Fact]
    public void Read_WithCustomOperatorProperty_ShouldDeserializeCorrectly()
    {
        // Arrange
        var customOptions = new QueryBuilderOptions
        {
            OperatorPropertyName = "op"
        };
        var converter = new QueryBuilderConverter(customOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };
        
        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Price",
                    "op": "greater_than",
                    "value": 100,
                    "type": "decimal"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.Equal("Price", result.Rules[0].FieldName);
        Assert.Equal("greater_than", result.Rules[0].Operator);
    }

    [Fact]
    public void Read_WithCustomValueProperty_ShouldDeserializeCorrectly()
    {
        // Arrange
        var customOptions = new QueryBuilderOptions
        {
            ValuePropertyName = "val"
        };
        var converter = new QueryBuilderConverter(customOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };
        
        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Count",
                    "operator": "equal",
                    "val": 42,
                    "type": "int"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.Equal("Count", result.Rules[0].FieldName);
        Assert.Equal(42, result.Rules[0].Value);
    }

    [Fact]
    public void Read_WithCustomTypeProperty_ShouldDeserializeCorrectly()
    {
        // Arrange
        var customOptions = new QueryBuilderOptions
        {
            TypePropertyName = "dataType"
        };
        var converter = new QueryBuilderConverter(customOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };
        
        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "IsActive",
                    "operator": "equal",
                    "value": true,
                    "dataType": "boolean"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.Equal("IsActive", result.Rules[0].FieldName);
        Assert.Equal("boolean", result.Rules[0].Type);
    }

    [Fact]
    public void Read_WithCustomDataProperty_ShouldDeserializeCorrectly()
    {
        // Arrange
        var customOptions = new QueryBuilderOptions
        {
            DataPropertyName = "metadata"
        };
        var converter = new QueryBuilderConverter(customOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };
        
        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Date",
                    "operator": "between",
                    "value": ["2023-01-01", "2023-12-31"],
                    "type": "datetime",
                    "metadata": {
                        "format": "yyyy-MM-dd",
                        "timezone": "UTC"
                    }
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.NotNull(result.Rules[0].Metadata);
        Assert.Equal(2, result.Rules[0].Metadata.Count);
        Assert.Equal("yyyy-MM-dd", result.Rules[0].Metadata["format"]);
        Assert.Equal("UTC", result.Rules[0].Metadata["timezone"]);
    }

    [Fact]
    public void Read_WithAllCustomProperties_ShouldDeserializeCorrectly()
    {
        // Arrange
        var customOptions = new QueryBuilderOptions
        {
            ConditionPropertyName = "combinator",
            RulesPropertyName = "children",
            FieldPropertyName = "id",
            OperatorPropertyName = "op",
            ValuePropertyName = "val",
            TypePropertyName = "dataType",
            DataPropertyName = "metadata"
        };
        var converter = new QueryBuilderConverter(customOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };
        
        var json = """
        {
            "combinator": "OR",
            "children": [
                {
                    "id": "ProductName",
                    "op": "starts_with",
                    "val": "iPhone",
                    "dataType": "text",
                    "metadata": {
                        "caseSensitive": false
                    }
                },
                {
                    "combinator": "AND",
                    "children": [
                        {
                            "id": "Price",
                            "op": "greater",
                            "val": 500,
                            "dataType": "number"
                        }
                    ]
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OR", result.Condition);
        Assert.Single(result.Rules);
        Assert.Single(result.Groups);

        // Check main rule
        var mainRule = result.Rules[0];
        Assert.Equal("ProductName", mainRule.FieldName);
        Assert.Equal("starts_with", mainRule.Operator);
        Assert.Equal("iPhone", mainRule.Value);
        Assert.Equal("text", mainRule.Type);
        Assert.NotNull(mainRule.Metadata);
        Assert.Equal(false, mainRule.Metadata["caseSensitive"]);

        // Check nested group
        var nestedGroup = result.Groups[0];
        Assert.Equal("AND", nestedGroup.Condition);
        Assert.Single(nestedGroup.Rules);

        var nestedRule = nestedGroup.Rules[0];
        Assert.Equal("Price", nestedRule.FieldName);
        Assert.Equal("greater", nestedRule.Operator);
        Assert.Equal(500, nestedRule.Value);
        Assert.Equal("number", nestedRule.Type);
    }

    [Fact]
    public void Read_ReactQueryBuilderFormat_ShouldDeserializeCorrectly()
    {
        // Arrange - React QueryBuilder uses different property names
        var reactOptions = new QueryBuilderOptions
        {
            ConditionPropertyName = "combinator",
            RulesPropertyName = "rules", // Same as default
            FieldPropertyName = "field", // Same as default
            OperatorPropertyName = "operator", // Same as default
            ValuePropertyName = "value", // Same as default
            TypePropertyName = "valueSource", // Different
            DataPropertyName = "data" // Same as default
        };
        var converter = new QueryBuilderConverter(reactOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };
        
        var json = """
        {
            "combinator": "and",
            "rules": [
                {
                    "field": "firstName",
                    "operator": "=",
                    "value": "Steve",
                    "valueSource": "value"
                },
                {
                    "combinator": "or",
                    "rules": [
                        {
                            "field": "lastName",
                            "operator": "=",
                            "value": "Vai"
                        }
                    ]
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("and", result.Condition);
        Assert.Single(result.Rules);
        Assert.Single(result.Groups);

        var mainRule = result.Rules[0];
        Assert.Equal("firstName", mainRule.FieldName);
        Assert.Equal("=", mainRule.Operator);
        Assert.Equal("Steve", mainRule.Value);
        Assert.Equal("value", mainRule.Type);

        var nestedGroup = result.Groups[0];
        Assert.Equal("or", nestedGroup.Condition);
        Assert.Single(nestedGroup.Rules);

        var nestedRule = nestedGroup.Rules[0];
        Assert.Equal("lastName", nestedRule.FieldName);
        Assert.Equal("=", nestedRule.Operator);
        Assert.Equal("Vai", nestedRule.Value);
    }
}
