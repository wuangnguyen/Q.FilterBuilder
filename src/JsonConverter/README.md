# Q.FilterBuilder.JsonConverter

A System.Text.Json converter for Q.FilterBuilder that enables seamless JSON deserialization of query builder structures from popular JavaScript libraries like jQuery QueryBuilder and React Query Builder.

## Features

- **Universal JSON Support**: Works with multiple query builder libraries (jQuery QueryBuilder, React Query Builder, etc.)
- **Configurable Property Names**: Customize JSON property names to match your frontend library
- **Type Preservation**: Maintains original JSON types (numbers, booleans, strings, arrays, objects)
- **Dependency Injection**: Full DI support for ASP.NET Core applications
- **Null Value Handling**: Properly handles null values in arrays and objects
- **Metadata Support**: Preserves additional metadata from query builder rules

## Installation

```bash
dotnet add package Q.FilterBuilder.JsonConverter
```

## Quick Start

### Basic Usage

```csharp
using Q.FilterBuilder.JsonConverter;
using Q.FilterBuilder.Core.Models;
using System.Text.Json;

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

var filterGroup = JsonSerializer.Deserialize<FilterGroup>(json, options);
```

### Custom Property Names

```csharp
var options = new QueryBuilderOptions
{
    ConditionPropertyName = "combinator",  // React Query Builder uses "combinator"
    FieldPropertyName = "field",
    OperatorPropertyName = "operator",
    ValuePropertyName = "value",
    TypePropertyName = "type"
};

var converter = new QueryBuilderConverter(options);
```

## Dependency Injection Integration

### ASP.NET Core Setup

Add to your `Program.cs`:

```csharp
using Q.FilterBuilder.JsonConverter.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Option 1: Basic registration with default options
builder.Services.AddQueryBuilderJsonConverter();

// Option 2: Custom configuration for different query builder libraries
builder.Services.AddQueryBuilderJsonConverter(options =>
{
    options.ConditionPropertyName = "combinator";  // For React Query Builder
    options.FieldPropertyName = "field";
    options.OperatorPropertyName = "operator";
    options.ValuePropertyName = "value";
    options.TypePropertyName = "type";
});
```

### Controller Usage

#### Simple Usage with Dependency Injection

```csharp
[ApiController]
[Route("api/[controller]")]
public class FilterController : ControllerBase
{
    private readonly QueryBuilderConverter _converter;

    public FilterController(QueryBuilderConverter converter)
    {
        _converter = converter;
    }

    [HttpPost("parse")]
    public IActionResult ParseFilter([FromBody] JsonElement filterJson)
    {
        var options = new JsonSerializerOptions { Converters = { _converter } };
        var filterGroup = JsonSerializer.Deserialize<FilterGroup>(filterJson.GetRawText(), options);

        return Ok(filterGroup);
    }
}
```

## Configuration Options

| Property | Default | Description |
|----------|---------|-------------|
| `ConditionPropertyName` | `"condition"` | JSON property name for group condition (AND/OR) |
| `RulesPropertyName` | `"rules"` | JSON property name for rules array |
| `FieldPropertyName` | `"field"` | JSON property name for rule field |
| `OperatorPropertyName` | `"operator"` | JSON property name for rule operator |
| `ValuePropertyName` | `"value"` | JSON property name for rule value |
| `TypePropertyName` | `"type"` | JSON property name for rule type |
| `DataPropertyName` | `"data"` | JSON property name for rule metadata |

## Supported Query Builder Libraries

### jQuery QueryBuilder

Default configuration works out of the box:

```json
{
    "condition": "AND",
    "rules": [
        {
            "field": "name",
            "operator": "equal",
            "value": "John"
        }
    ]
}
```

### React Query Builder

Configure for React Query Builder format:

```csharp
builder.Services.AddQueryBuilderJsonConverter(options =>
{
    options.ConditionPropertyName = "combinator";
});
```

```json
{
    "combinator": "and",
    "rules": [
        {
            "field": "name",
            "operator": "=",
            "value": "John"
        }
    ]
}
```

## Advanced Examples

### Complex Nested Structure

```json
{
    "condition": "AND",
    "rules": [
        {
            "field": "category",
            "operator": "equal",
            "value": "Electronics"
        },
        {
            "condition": "OR",
            "rules": [
                {
                    "field": "price",
                    "operator": "less",
                    "value": 100
                },
                {
                    "field": "on_sale",
                    "operator": "equal",
                    "value": true
                }
            ]
        }
    ]
}
```

### With Metadata

```json
{
    "condition": "AND",
    "rules": [
        {
            "field": "created_date",
            "operator": "between",
            "value": ["2023-01-01", "2023-12-31"],
            "type": "datetime",
            "data": {
                "dateFormat": "yyyy-MM-dd",
                "timezone": "UTC"
            }
        }
    ]
}
```

## Type Handling

The converter preserves JSON types:

- **Numbers**: Integers remain as `int`, decimals as `double`
- **Booleans**: Preserved as `bool`
- **Strings**: Preserved as `string`
- **Arrays**: Converted to `List<object?>`
- **Objects**: Converted to `Dictionary<string, object?>`
- **Null**: Preserved as `null`

## Error Handling

The converter throws `JsonException` for invalid JSON structures:

```csharp
try
{
    var filterGroup = JsonSerializer.Deserialize<FilterGroup>(json, options);
}
catch (JsonException ex)
{
    // Handle invalid JSON format
    Console.WriteLine($"Invalid filter format: {ex.Message}");
}
```

## Integration with FilterBuilder

Use the parsed `FilterGroup` with Q.FilterBuilder.Core:

```csharp
// Parse JSON to FilterGroup
var filterGroup = JsonSerializer.Deserialize<FilterGroup>(json, options);

// Use with FilterBuilder
var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();
var (query, parameters) = filterBuilder.Build(filterGroup);

// Execute query
var sql = $"SELECT * FROM Users WHERE {query}";
var users = await repository.QueryAsync<User>(sql, parameters);
```

## Testing

The package includes comprehensive test coverage. Run tests with:

```bash
dotnet test src/JsonConverter/test/
```

## Contributing

Contributions are welcome! Please ensure all tests pass and follow the existing code style.

## License

This project is licensed under the MIT License.
