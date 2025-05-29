# Q.FilterBuilder.JsonConverter

JSON converter for Q.FilterBuilder that enables seamless integration with various query builder libraries including jQuery QueryBuilder, React QueryBuilder, and other JSON-based query builders.

## Installation

```bash
dotnet add package Q.FilterBuilder.JsonConverter
```

## Quick Start

### Basic Usage

```csharp
using System.Text.Json;
using Q.FilterBuilder.JsonConverter;

var options = new JsonSerializerOptions
{
    Converters = { new QueryBuilderConverter() }
};

string json = """
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

var filterGroup = JsonSerializer.Deserialize<FilterGroup>(json, options);
```

### Custom Property Names

Configure custom JSON property names to match your specific JSON structure:

```csharp
using Q.FilterBuilder.JsonConverter;

var customOptions = new QueryBuilderOptions
{
    ConditionPropertyName = "combinator",  // Instead of "condition"
    RulesPropertyName = "children",        // Instead of "rules"
    FieldPropertyName = "id",              // Instead of "field"
    OperatorPropertyName = "operator",     // Default
    ValuePropertyName = "value",           // Default
    TypePropertyName = "type",             // Default
    DataPropertyName = "data"              // Default
};

var jsonOptions = new JsonSerializerOptions
{
    Converters = { new QueryBuilderConverter(customOptions) }
};

// Now supports JSON like:
string customJson = """
{
    "combinator": "AND",
    "children": [
        {
            "id": "Name",
            "operator": "equal",
            "value": "John",
            "type": "string"
        }
    ]
}
""";

var filterGroup = JsonSerializer.Deserialize<FilterGroup>(customJson, jsonOptions);
```

## Supported Query Builders

This converter supports various query builder libraries through configurable property names:

### jQuery QueryBuilder
- **Website**: https://querybuilder.js.org/
- **Default Configuration**: Uses default property names
- **Example**: See Basic Usage above

### React QueryBuilder
- **Website**: https://react-querybuilder.js.org/
- **Configuration**: Uses "combinator" instead of "condition"
- **Example**: See React QueryBuilder Integration below

### Custom Query Builders
- **Configuration**: Fully configurable property names
- **Flexibility**: Supports any JSON structure through options

## Configuration Options

The `QueryBuilderOptions` class allows you to customize all JSON property names:

| Property | Default Value | Description |
|----------|---------------|-------------|
| `ConditionPropertyName` | `"condition"` | Logical operator for groups (AND/OR) |
| `RulesPropertyName` | `"rules"` | Array of rules and nested groups |
| `FieldPropertyName` | `"field"` | Field name in rules |
| `OperatorPropertyName` | `"operator"` | Operator name in rules |
| `ValuePropertyName` | `"value"` | Value in rules |
| `TypePropertyName` | `"type"` | Explicit type in rules |
| `DataPropertyName` | `"data"` | Metadata/additional data in rules |

## Supported JSON Structure

### Default Structure

```json
{
    "condition": "AND",
    "rules": [
        {
            "field": "Name",
            "operator": "equal",
            "value": "John",
            "type": "string",
            "data": {
                "customProperty": "value"
            }
        },
        {
            "condition": "OR",
            "rules": [
                {
                    "field": "Status",
                    "operator": "in",
                    "value": ["Active", "Pending"],
                    "type": "string"
                }
            ]
        }
    ]
}
```

### Custom Structure Example

```json
{
    "combinator": "AND",
    "children": [
        {
            "id": "Name",
            "operator": "equal",
            "value": "John",
            "type": "string"
        }
    ]
}
```

## Integration Examples

### React QueryBuilder Integration

```csharp
using Q.FilterBuilder.JsonConverter;

// React QueryBuilder uses "combinator" instead of "condition"
var reactOptions = new QueryBuilderOptions
{
    ConditionPropertyName = "combinator"
};

var jsonOptions = new JsonSerializerOptions
{
    Converters = { new QueryBuilderConverter(reactOptions) }
};

string reactJson = """
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
        }
    ]
}
""";

var filterGroup = JsonSerializer.Deserialize<FilterGroup>(reactJson, jsonOptions);
```

### With ASP.NET Core

```csharp
// Startup.cs or Program.cs
services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new QueryBuilderConverter());
});

// Controller
[HttpPost("filter")]
public IActionResult ApplyFilter([FromBody] FilterGroup filterGroup)
{
    // filterGroup is automatically deserialized from JSON
    return Ok(filterGroup);
}
```

### With Custom Property Names in ASP.NET Core

```csharp
services.Configure<JsonOptions>(options =>
{
    var customOptions = new QueryBuilderOptions
    {
        ConditionPropertyName = "combinator",
        RulesPropertyName = "children"
    };
    options.SerializerOptions.Converters.Add(new QueryBuilderConverter(customOptions));
});
```

## Advanced Usage

### Nested Groups

The converter supports unlimited nesting of groups:

```json
{
    "condition": "AND",
    "rules": [
        {
            "field": "Category",
            "operator": "equal",
            "value": "Electronics"
        },
        {
            "condition": "OR",
            "rules": [
                {
                    "field": "Price",
                    "operator": "less",
                    "value": 100
                },
                {
                    "condition": "AND",
                    "rules": [
                        {
                            "field": "Brand",
                            "operator": "equal",
                            "value": "Samsung"
                        },
                        {
                            "field": "OnSale",
                            "operator": "equal",
                            "value": true
                        }
                    ]
                }
            ]
        }
    ]
}
```

### Metadata Support

Rules can include additional metadata through the data property:

```json
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
```

## Error Handling

The converter will throw exceptions for:
- Missing required properties (condition, rules, field, operator, value)
- Invalid JSON structure
- Null options parameter

```csharp
try
{
    var filterGroup = JsonSerializer.Deserialize<FilterGroup>(json, options);
}
catch (JsonException ex)
{
    // Handle JSON parsing errors
}
catch (ArgumentNullException ex)
{
    // Handle null options
}
```

## Compatibility

- **Target Framework**: .NET Standard 2.1
- **JSON Library**: System.Text.Json