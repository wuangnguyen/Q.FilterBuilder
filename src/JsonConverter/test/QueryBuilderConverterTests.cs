using System;
using System.Text.Json;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.JsonConverter;
using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests;

public class QueryBuilderConverterTests
{
    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new QueryBuilderConverter(null!));
    }

    [Fact]
    public void Constructor_WithValidOptions_ShouldCreateInstance()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
    }

    [Fact]
    public void Constructor_Parameterless_ShouldCreateInstanceWithDefaultOptions()
    {
        // Act
        var converter = new QueryBuilderConverter();

        // Assert
        Assert.NotNull(converter);
    }

    [Fact]
    public void Read_SimpleAndGroup_ShouldDeserializeCorrectly()
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
                    "value": "John",
                    "type": "string"
                },
                {
                    "field": "Age",
                    "operator": "greater",
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
        Assert.Equal("AND", result.Condition);
        Assert.Equal(2, result.Rules.Count);
        Assert.Empty(result.Groups);

        var firstRule = result.Rules[0];
        Assert.Equal("Name", firstRule.FieldName);
        Assert.Equal("equal", firstRule.Operator);
        Assert.Equal("John", firstRule.Value);
        Assert.Equal("string", firstRule.Type);

        var secondRule = result.Rules[1];
        Assert.Equal("Age", secondRule.FieldName);
        Assert.Equal("greater", secondRule.Operator);
        Assert.Equal(25, secondRule.Value);
        Assert.Equal("int", secondRule.Type);
    }

    [Fact]
    public void Read_SimpleOrGroup_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "OR",
            "rules": [
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
        Assert.Empty(result.Groups);

        var rule = result.Rules[0];
        Assert.Equal("Status", rule.FieldName);
        Assert.Equal("equal", rule.Operator);
        Assert.Equal("Active", rule.Value);
        Assert.Equal("string", rule.Type);
    }

    [Fact]
    public void Read_NestedGroups_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "Category",
                    "operator": "equal",
                    "value": "Electronics",
                    "type": "string"
                },
                {
                    "condition": "OR",
                    "rules": [
                        {
                            "field": "Price",
                            "operator": "less",
                            "value": 100,
                            "type": "decimal"
                        },
                        {
                            "field": "OnSale",
                            "operator": "equal",
                            "value": true,
                            "type": "bool"
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
        Assert.Equal("AND", result.Condition);
        Assert.Single(result.Rules);
        Assert.Single(result.Groups);

        // Check main rule
        var mainRule = result.Rules[0];
        Assert.Equal("Category", mainRule.FieldName);
        Assert.Equal("equal", mainRule.Operator);
        Assert.Equal("Electronics", mainRule.Value);

        // Check nested group
        var nestedGroup = result.Groups[0];
        Assert.Equal("OR", nestedGroup.Condition);
        Assert.Equal(2, nestedGroup.Rules.Count);
        Assert.Empty(nestedGroup.Groups);

        var nestedRule1 = nestedGroup.Rules[0];
        Assert.Equal("Price", nestedRule1.FieldName);
        Assert.Equal("less", nestedRule1.Operator);
        Assert.Equal(100, nestedRule1.Value);

        var nestedRule2 = nestedGroup.Rules[1];
        Assert.Equal("OnSale", nestedRule2.FieldName);
        Assert.Equal("equal", nestedRule2.Operator);
        Assert.Equal(true, nestedRule2.Value);
    }

    [Fact]
    public void Read_DeeplyNestedGroups_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "condition": "OR",
                    "rules": [
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
                    ]
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("AND", result.Condition);
        Assert.Empty(result.Rules);
        Assert.Single(result.Groups);

        var level1Group = result.Groups[0];
        Assert.Equal("OR", level1Group.Condition);
        Assert.Empty(level1Group.Rules);
        Assert.Single(level1Group.Groups);

        var level2Group = level1Group.Groups[0];
        Assert.Equal("AND", level2Group.Condition);
        Assert.Single(level2Group.Rules);
        Assert.Empty(level2Group.Groups);

        var deepRule = level2Group.Rules[0];
        Assert.Equal("Name", deepRule.FieldName);
        Assert.Equal("equal", deepRule.Operator);
        Assert.Equal("Test", deepRule.Value);
    }

    [Fact]
    public void Read_RuleWithMetadata_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "CreatedDate",
                    "operator": "between",
                    "value": ["2023-01-01", "2023-12-31"],
                    "type": "datetime",
                    "data": {
                        "dateFormat": "yyyy-MM-dd",
                        "timezone": "UTC",
                        "customProperty": "value"
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

        var rule = result.Rules[0];
        Assert.Equal("CreatedDate", rule.FieldName);
        Assert.Equal("between", rule.Operator);
        Assert.Equal("datetime", rule.Type);

        Assert.NotNull(rule.Metadata);
        Assert.Equal(3, rule.Metadata.Count);
        Assert.Equal("yyyy-MM-dd", rule.Metadata["dateFormat"]);
        Assert.Equal("UTC", rule.Metadata["timezone"]);
        Assert.Equal("value", rule.Metadata["customProperty"]);
    }

    [Fact]
    public void Read_RuleWithoutMetadata_ShouldHaveNullMetadata()
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
        Assert.Single(result.Rules);
        Assert.Null(result.Rules[0].Metadata);
    }

    [Fact]
    public void Write_ShouldThrowNotImplementedException()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var filterGroup = new FilterGroup("AND");
        var options = new JsonSerializerOptions { Converters = { converter } };

        // Act & Assert
        Assert.Throws<NotImplementedException>(() =>
            JsonSerializer.Serialize(filterGroup, options));
    }
}
