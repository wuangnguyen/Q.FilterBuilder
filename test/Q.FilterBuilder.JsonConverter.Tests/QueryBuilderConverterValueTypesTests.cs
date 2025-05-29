using System.Collections.Generic;
using System.Text.Json;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.JsonConverter;
using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests;

public class QueryBuilderConverterValueTypesTests
{
    [Fact]
    public void Read_StringValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Name",
                    "operator": "equal",
                    "value": "John Doe",
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
        Assert.Equal("John Doe", result.Rules[0].Value);
        Assert.IsType<string>(result.Rules[0].Value);
    }

    [Fact]
    public void Read_IntegerValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Age",
                    "operator": "equal",
                    "value": 25,
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
        Assert.Equal(25, result.Rules[0].Value);
        Assert.IsType<int>(result.Rules[0].Value);
    }

    [Fact]
    public void Read_DecimalValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Price",
                    "operator": "greater",
                    "value": 99.99,
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
        Assert.Equal(99.99, result.Rules[0].Value);
        Assert.IsType<double>(result.Rules[0].Value);
    }

    [Fact]
    public void Read_BooleanTrueValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "IsActive",
                    "operator": "equal",
                    "value": true,
                    "type": "bool"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.Equal(true, result.Rules[0].Value);
        Assert.IsType<bool>(result.Rules[0].Value);
    }

    [Fact]
    public void Read_BooleanFalseValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "IsDeleted",
                    "operator": "equal",
                    "value": false,
                    "type": "bool"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.Equal(false, result.Rules[0].Value);
        Assert.IsType<bool>(result.Rules[0].Value);
    }

    [Fact]
    public void Read_NullValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Description",
                    "operator": "is_null",
                    "value": null,
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
        Assert.Null(result.Rules[0].Value);
    }

    [Fact]
    public void Read_ArrayValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Status",
                    "operator": "in",
                    "value": ["Active", "Pending", "Approved"],
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

        var value = result.Rules[0].Value;
        Assert.IsType<List<object?>>(value);

        var list = (List<object?>)value;
        Assert.Equal(3, list.Count);
        Assert.Equal("Active", list[0]);
        Assert.Equal("Pending", list[1]);
        Assert.Equal("Approved", list[2]);
    }

    [Fact]
    public void Read_MixedArrayValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Values",
                    "operator": "in",
                    "value": [1, "text", true, null],
                    "type": "mixed"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);

        var value = result.Rules[0].Value;
        Assert.IsType<List<object?>>(value);

        var list = (List<object?>)value;
        Assert.Equal(4, list.Count);
        Assert.Equal(1, list[0]); // Numbers are preserved as numbers
        Assert.Equal("text", list[1]);
        Assert.Equal(true, list[2]);
        Assert.Null(list[3]);
    }

    [Fact]
    public void Read_ObjectValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Settings",
                    "operator": "contains",
                    "value": {
                        "theme": "dark",
                        "notifications": true,
                        "maxItems": 10
                    },
                    "type": "object"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);

        var value = result.Rules[0].Value;
        Assert.IsType<Dictionary<string, object?>>(value);

        var dict = (Dictionary<string, object?>)value;
        Assert.Equal(3, dict.Count);
        Assert.Equal("dark", dict["theme"]);
        Assert.Equal(true, dict["notifications"]);
        Assert.Equal(10, dict["maxItems"]); // Numbers are preserved as numbers
    }

    [Fact]
    public void Read_EmptyArrayValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Tags",
                    "operator": "not_in",
                    "value": [],
                    "type": "array"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);

        var value = result.Rules[0].Value;
        Assert.IsType<List<object?>>(value);

        var list = (List<object?>)value;
        Assert.Empty(list);
    }

    [Fact]
    public void Read_EmptyObjectValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Config",
                    "operator": "equal",
                    "value": {},
                    "type": "object"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);

        var value = result.Rules[0].Value;
        Assert.IsType<Dictionary<string, object?>>(value);

        var dict = (Dictionary<string, object?>)value;
        Assert.Empty(dict);
    }

    [Fact]
    public void Read_NestedObjectValue_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "UserProfile",
                    "operator": "contains",
                    "value": {
                        "preferences": {
                            "theme": "light",
                            "language": "en"
                        },
                        "permissions": ["read", "write"]
                    },
                    "type": "object"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);

        var value = result.Rules[0].Value;
        Assert.IsType<Dictionary<string, object?>>(value);

        var dict = (Dictionary<string, object?>)value;
        Assert.Equal(2, dict.Count);

        // Check nested object
        var preferences = dict["preferences"] as Dictionary<string, object?>;
        Assert.NotNull(preferences);
        Assert.Equal("light", preferences["theme"]);
        Assert.Equal("en", preferences["language"]);

        // Check nested array
        var permissions = dict["permissions"] as List<object?>;
        Assert.NotNull(permissions);
        Assert.Equal(2, permissions.Count);
        Assert.Equal("read", permissions[0]);
        Assert.Equal("write", permissions[1]);
    }
}
