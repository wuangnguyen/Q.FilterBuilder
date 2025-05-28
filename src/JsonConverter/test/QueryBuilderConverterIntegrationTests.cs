using System.Text.Json;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.JsonConverter;
using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests;

public class QueryBuilderConverterIntegrationTests
{
    [Fact]
    public void Read_JQueryQueryBuilderFormat_ShouldDeserializeCorrectly()
    {
        // Arrange - Standard jQuery QueryBuilder format
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "id": "name",
                    "field": "name",
                    "type": "string",
                    "input": "text",
                    "operator": "equal",
                    "value": "Mistic"
                },
                {
                    "id": "category",
                    "field": "category",
                    "type": "integer",
                    "input": "select",
                    "operator": "in",
                    "value": [1, 2]
                },
                {
                    "condition": "OR",
                    "rules": [
                        {
                            "id": "in_stock",
                            "field": "in_stock",
                            "type": "integer",
                            "input": "radio",
                            "operator": "equal",
                            "value": 1
                        },
                        {
                            "id": "price",
                            "field": "price",
                            "type": "double",
                            "input": "number",
                            "operator": "less",
                            "value": 10.25
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
        Assert.Equal(2, result.Rules.Count);
        Assert.Single(result.Groups);

        // Check first rule
        var nameRule = result.Rules[0];
        Assert.Equal("name", nameRule.FieldName);
        Assert.Equal("equal", nameRule.Operator);
        Assert.Equal("Mistic", nameRule.Value);
        Assert.Equal("string", nameRule.Type);

        // Check second rule
        var categoryRule = result.Rules[1];
        Assert.Equal("category", categoryRule.FieldName);
        Assert.Equal("in", categoryRule.Operator);
        Assert.Equal("integer", categoryRule.Type);

        // Check nested group
        var nestedGroup = result.Groups[0];
        Assert.Equal("OR", nestedGroup.Condition);
        Assert.Equal(2, nestedGroup.Rules.Count);

        var stockRule = nestedGroup.Rules[0];
        Assert.Equal("in_stock", stockRule.FieldName);
        Assert.Equal("equal", stockRule.Operator);
        Assert.Equal(1, stockRule.Value);

        var priceRule = nestedGroup.Rules[1];
        Assert.Equal("price", priceRule.FieldName);
        Assert.Equal("less", priceRule.Operator);
        Assert.Equal(10.25, priceRule.Value);
    }

    [Fact]
    public void Read_ReactQueryBuilderFormat_ShouldDeserializeCorrectly()
    {
        // Arrange - React QueryBuilder format with combinator
        var reactOptions = new QueryBuilderOptions
        {
            ConditionPropertyName = "combinator"
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
                    "value": "Steve"
                },
                {
                    "field": "lastName",
                    "operator": "=",
                    "value": "Vai"
                },
                {
                    "combinator": "or",
                    "rules": [
                        {
                            "field": "age",
                            "operator": ">",
                            "value": "28"
                        },
                        {
                            "field": "isMusician",
                            "operator": "=",
                            "value": true
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
        Assert.Equal(2, result.Rules.Count);
        Assert.Single(result.Groups);

        var firstNameRule = result.Rules[0];
        Assert.Equal("firstName", firstNameRule.FieldName);
        Assert.Equal("=", firstNameRule.Operator);
        Assert.Equal("Steve", firstNameRule.Value);

        var lastNameRule = result.Rules[1];
        Assert.Equal("lastName", lastNameRule.FieldName);
        Assert.Equal("=", lastNameRule.Operator);
        Assert.Equal("Vai", lastNameRule.Value);

        var nestedGroup = result.Groups[0];
        Assert.Equal("or", nestedGroup.Condition);
        Assert.Equal(2, nestedGroup.Rules.Count);

        var ageRule = nestedGroup.Rules[0];
        Assert.Equal("age", ageRule.FieldName);
        Assert.Equal(">", ageRule.Operator);
        Assert.Equal("28", ageRule.Value);

        var musicianRule = nestedGroup.Rules[1];
        Assert.Equal("isMusician", musicianRule.FieldName);
        Assert.Equal("=", musicianRule.Operator);
        Assert.Equal(true, musicianRule.Value);
    }

    [Fact]
    public void Read_CustomQueryBuilderFormat_ShouldDeserializeCorrectly()
    {
        // Arrange - Custom format with all different property names
        var customOptions = new QueryBuilderOptions
        {
            ConditionPropertyName = "logic",
            RulesPropertyName = "filters",
            FieldPropertyName = "column",
            OperatorPropertyName = "comparison",
            ValuePropertyName = "criteria",
            TypePropertyName = "datatype",
            DataPropertyName = "options"
        };
        var converter = new QueryBuilderConverter(customOptions);
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "logic": "ALL",
            "filters": [
                {
                    "column": "user_name",
                    "comparison": "starts_with",
                    "criteria": "admin",
                    "datatype": "text",
                    "options": {
                        "caseSensitive": false,
                        "trimWhitespace": true
                    }
                },
                {
                    "column": "created_date",
                    "comparison": "between",
                    "criteria": ["2023-01-01T00:00:00Z", "2023-12-31T23:59:59Z"],
                    "datatype": "datetime",
                    "options": {
                        "timezone": "UTC",
                        "format": "ISO8601"
                    }
                },
                {
                    "logic": "ANY",
                    "filters": [
                        {
                            "column": "status",
                            "comparison": "in",
                            "criteria": ["active", "pending", "approved"],
                            "datatype": "enum"
                        },
                        {
                            "column": "priority",
                            "comparison": "greater_equal",
                            "criteria": 5,
                            "datatype": "integer"
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
        Assert.Equal("ALL", result.Condition);
        Assert.Equal(2, result.Rules.Count);
        Assert.Single(result.Groups);

        // Check first rule with metadata
        var userNameRule = result.Rules[0];
        Assert.Equal("user_name", userNameRule.FieldName);
        Assert.Equal("starts_with", userNameRule.Operator);
        Assert.Equal("admin", userNameRule.Value);
        Assert.Equal("text", userNameRule.Type);
        Assert.NotNull(userNameRule.Metadata);
        Assert.Equal(false, userNameRule.Metadata["caseSensitive"]);
        Assert.Equal(true, userNameRule.Metadata["trimWhitespace"]);

        // Check second rule with array value and metadata
        var dateRule = result.Rules[1];
        Assert.Equal("created_date", dateRule.FieldName);
        Assert.Equal("between", dateRule.Operator);
        Assert.Equal("datetime", dateRule.Type);
        Assert.NotNull(dateRule.Metadata);
        Assert.Equal("UTC", dateRule.Metadata["timezone"]);
        Assert.Equal("ISO8601", dateRule.Metadata["format"]);

        // Check nested group
        var nestedGroup = result.Groups[0];
        Assert.Equal("ANY", nestedGroup.Condition);
        Assert.Equal(2, nestedGroup.Rules.Count);

        var statusRule = nestedGroup.Rules[0];
        Assert.Equal("status", statusRule.FieldName);
        Assert.Equal("in", statusRule.Operator);
        Assert.Equal("enum", statusRule.Type);

        var priorityRule = nestedGroup.Rules[1];
        Assert.Equal("priority", priorityRule.FieldName);
        Assert.Equal("greater_equal", priorityRule.Operator);
        Assert.Equal(5, priorityRule.Value);
        Assert.Equal("integer", priorityRule.Type);
    }

    [Fact]
    public void Read_ComplexNestedStructure_ShouldDeserializeCorrectly()
    {
        // Arrange - Complex nested structure with multiple levels
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": [
                {
                    "field": "company",
                    "operator": "equal",
                    "value": "Acme Corp",
                    "type": "string"
                },
                {
                    "condition": "OR",
                    "rules": [
                        {
                            "field": "department",
                            "operator": "in",
                            "value": ["Engineering", "Product", "Design"],
                            "type": "string"
                        },
                        {
                            "condition": "AND",
                            "rules": [
                                {
                                    "field": "role",
                                    "operator": "equal",
                                    "value": "Manager",
                                    "type": "string"
                                },
                                {
                                    "field": "experience_years",
                                    "operator": "greater_equal",
                                    "value": 5,
                                    "type": "int"
                                },
                                {
                                    "condition": "OR",
                                    "rules": [
                                        {
                                            "field": "has_certification",
                                            "operator": "equal",
                                            "value": true,
                                            "type": "bool"
                                        },
                                        {
                                            "field": "education_level",
                                            "operator": "in",
                                            "value": ["Masters", "PhD"],
                                            "type": "string"
                                        }
                                    ]
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
        Assert.Single(result.Rules);
        Assert.Single(result.Groups);

        // Check main rule
        var companyRule = result.Rules[0];
        Assert.Equal("company", companyRule.FieldName);
        Assert.Equal("equal", companyRule.Operator);
        Assert.Equal("Acme Corp", companyRule.Value);

        // Check level 1 group (OR)
        var level1Group = result.Groups[0];
        Assert.Equal("OR", level1Group.Condition);
        Assert.Single(level1Group.Rules);
        Assert.Single(level1Group.Groups);

        var departmentRule = level1Group.Rules[0];
        Assert.Equal("department", departmentRule.FieldName);
        Assert.Equal("in", departmentRule.Operator);

        // Check level 2 group (AND)
        var level2Group = level1Group.Groups[0];
        Assert.Equal("AND", level2Group.Condition);
        Assert.Equal(2, level2Group.Rules.Count);
        Assert.Single(level2Group.Groups);

        var roleRule = level2Group.Rules[0];
        Assert.Equal("role", roleRule.FieldName);
        Assert.Equal("equal", roleRule.Operator);
        Assert.Equal("Manager", roleRule.Value);

        var experienceRule = level2Group.Rules[1];
        Assert.Equal("experience_years", experienceRule.FieldName);
        Assert.Equal("greater_equal", experienceRule.Operator);
        Assert.Equal(5, experienceRule.Value);

        // Check level 3 group (OR)
        var level3Group = level2Group.Groups[0];
        Assert.Equal("OR", level3Group.Condition);
        Assert.Equal(2, level3Group.Rules.Count);
        Assert.Empty(level3Group.Groups);

        var certificationRule = level3Group.Rules[0];
        Assert.Equal("has_certification", certificationRule.FieldName);
        Assert.Equal("equal", certificationRule.Operator);
        Assert.Equal(true, certificationRule.Value);

        var educationRule = level3Group.Rules[1];
        Assert.Equal("education_level", educationRule.FieldName);
        Assert.Equal("in", educationRule.Operator);
    }

    [Fact]
    public void Read_EmptyRulesArray_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "AND",
            "rules": []
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<FilterGroup>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("AND", result.Condition);
        Assert.Empty(result.Rules);
        Assert.Empty(result.Groups);
    }

    [Fact]
    public void Read_OnlyNestedGroups_ShouldDeserializeCorrectly()
    {
        // Arrange
        var converter = new QueryBuilderConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };

        var json = """
        {
            "condition": "OR",
            "rules": [
                {
                    "condition": "AND",
                    "rules": [
                        {
                            "field": "name",
                            "operator": "equal",
                            "value": "Test1",
                            "type": "string"
                        }
                    ]
                },
                {
                    "condition": "AND",
                    "rules": [
                        {
                            "field": "name",
                            "operator": "equal",
                            "value": "Test2",
                            "type": "string"
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
        Assert.Empty(result.Rules);
        Assert.Equal(2, result.Groups.Count);

        var group1 = result.Groups[0];
        Assert.Equal("AND", group1.Condition);
        Assert.Single(group1.Rules);
        Assert.Equal("Test1", group1.Rules[0].Value);

        var group2 = result.Groups[1];
        Assert.Equal("AND", group2.Condition);
        Assert.Single(group2.Rules);
        Assert.Equal("Test2", group2.Rules[0].Value);
    }
}
