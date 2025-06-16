using System;
using System.Collections.Generic;
using System.Text.Json;
using Q.FilterBuilder.Core.Models;

using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests;

public class QueryBuilderConverterErrorTests
{
    [Fact]
    public void Read_MissingConditionProperty_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
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

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_MissingRulesProperty_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND"
        }
        """;

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_MissingFieldProperty_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "operator": "equal",
                    "value": "Test",
                    "type": "string"
                }
            ]
        }
        """;

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_MissingOperatorProperty_ShouldThrowJsonException()
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
                    "value": "Test",
                    "type": "string"
                }
            ]
        }
        """;

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_MissingTypeProperty_ShouldNotThrow()
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
                    "value": "Test"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.Equal(string.Empty, result.Rules[0].Type);
    }

    [Fact]
    public void Read_InvalidJson_ShouldThrowJsonException()
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
                    "value": "Test"
                    // Missing closing brace
            ]
        }
        """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_EmptyJson_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = "";

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_NullJson_ShouldReturnNull()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = "null";

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Read_EmptyObject_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = "{}";

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_ArrayInsteadOfObject_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        [
            {
                "condition": "AND",
                "rules": []
            }
        ]
        """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_StringInsteadOfObject_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = "\"not an object\"";

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithCustomPropertyNames_MissingCustomCondition_ShouldThrowJsonException()
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
            "condition": "AND",
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

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithCustomPropertyNames_MissingCustomRules_ShouldThrowJsonException()
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
            "condition": "AND",
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

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithCustomPropertyNames_MissingCustomField_ShouldThrowJsonException()
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
                    "field": "Name",
                    "operator": "equal",
                    "value": "Test",
                    "type": "string"
                }
            ]
        }
        """;

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_NullConditionValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": null,
            "rules": []
        }
        """;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_NullFieldValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": null,
                    "operator": "equal",
                    "value": "Test",
                    "type": "string"
                }
            ]
        }
        """;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_NullOperatorValue_ShouldThrowArgumentNullException()
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
                    "operator": null,
                    "value": "Test",
                    "type": "string"
                }
            ]
        }
        """;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithInvalidRulesStructure_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": "not_an_array"
        }
        """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithNumberAsCondition_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": 123,
            "rules": []
        }
        """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithBooleanAsCondition_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": true,
            "rules": []
        }
        """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithObjectAsCondition_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": {"type": "AND"},
            "rules": []
        }
        """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithNumberAsFieldValue_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": 123,
                    "operator": "equal",
                    "value": "Test",
                    "type": "string"
                }
            ]
        }
        """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithArrayAsOperatorValue_ShouldThrowJsonException()
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
                    "operator": ["equal", "not_equal"],
                    "value": "Test",
                    "type": "string"
                }
            ]
        }
        """;

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }

    [Fact]
    public void Read_WithMalformedNestedGroup_ShouldThrowJsonException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "condition": "OR"
                }
            ]
        }
        """;

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<FilterGroup>(json, options));
    }
}
