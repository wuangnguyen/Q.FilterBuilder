# Q.FilterBuilder.PostgreSql

PostgreSQL provider for Q.FilterBuilder that provides PostgreSQL-specific query generation and field formatting.

## Installation

```bash
dotnet add package Q.FilterBuilder.PostgreSql
```

> **Note**: The core `Q.FilterBuilder.Core` package will be automatically installed as a dependency.

## Quick Start

### 1. Basic Usage

Register the PostgreSQL FilterBuilder in your DI container:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.PostgreSql.Extensions;

// In your Startup.cs or Program.cs
services.AddPostgreSqlFilterBuilder();
```

Use in your services:

```csharp
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

public class UserService
{
    private readonly IFilterBuilder _filterBuilder;

    public UserService(IFilterBuilder filterBuilder)
    {
        _filterBuilder = filterBuilder;
    }

    public async Task<List<User>> GetFilteredUsersAsync(FilterGroup filters)
    {
        var (query, parameters) = _filterBuilder.Build(filters);
        
        // Use with your data access layer
        var sql = $"SELECT * FROM Users WHERE {query}";
        return await _repository.QueryAsync<User>(sql, parameters);
    }
}
```

### 2. Creating Filter Groups

```csharp
// Simple AND group
var group = new FilterGroup("AND");
group.Rules.Add(new FilterRule("Name", "equal", "John"));
group.Rules.Add(new FilterRule("Age", "greater", 25));

var (query, parameters) = _filterBuilder.Build(group);
// Result: "\"Name\" = $1 AND \"Age\" > $2"
// Parameters: ["John", 25]
```

### 3. PostgreSQL Specific Features

The PostgreSQL provider generates PostgreSQL-specific syntax:

```csharp
var group = new FilterGroup("AND");
group.Rules.Add(new FilterRule("Name", "equal", "John"));
group.Rules.Add(new FilterRule("Age", "greater_or_equal", 18));
group.Rules.Add(new FilterRule("Status", "not_equal", "Inactive"));

var (query, parameters) = _filterBuilder.Build(group);
// Result: "\"Name\" = $1 AND \"Age\" >= $2 AND \"Status\" != $3"
// Parameters: ["John", 18, "Inactive"]
```

## Advanced Usage

### Custom Type Converters

Register custom type converters for specialized data transformations:

```csharp
using Q.FilterBuilder.Core.TypeConversion;

services.AddPostgreSqlFilterBuilder(typeConversion =>
{
    // Register a custom converter for UUID formatting
    typeConversion.RegisterConverter("uuid", new UuidConverter());
    
    // Register a custom converter for PostgreSQL arrays
    typeConversion.RegisterConverter("array", new PostgreSqlArrayConverter());
});

// Example custom converter for UUID
public class UuidConverter : ITypeConverter
{
    public object? Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null) return null;
        
        if (Guid.TryParse(value.ToString(), out var guid))
        {
            return guid;
        }
        
        throw new InvalidOperationException($"Cannot convert '{value}' to UUID");
    }
}
```

### Custom Rule Transformers

Register custom rule transformers for specialized query operations:

```csharp
using Q.FilterBuilder.Core.RuleTransformers;

services.AddPostgreSqlFilterBuilder(ruleTransformers =>
{
    // Register a custom transformer for full-text search
    ruleTransformers.RegisterTransformer("fulltext", new PostgreSqlFullTextSearchTransformer());
    
    // Register a custom transformer for JSON queries
    ruleTransformers.RegisterTransformer("json_contains", new PostgreSqlJsonContainsTransformer());
    
    // Register a custom transformer for array operations
    ruleTransformers.RegisterTransformer("array_contains", new PostgreSqlArrayContainsTransformer());
});

// Example custom transformer for PostgreSQL full-text search
public class PostgreSqlFullTextSearchTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Full-text search requires a value");
            
        return new[] { value };
    }

    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"{fieldName} @@ to_tsquery($1)";
    }
}
```

### Both Custom Type Converters and Rule Transformers

```csharp
services.AddPostgreSqlFilterBuilder(
    typeConversion =>
    {
        typeConversion.RegisterConverter("uuid", new UuidConverter());
        typeConversion.RegisterConverter("array", new PostgreSqlArrayConverter());
    },
    ruleTransformers =>
    {
        ruleTransformers.RegisterTransformer("fulltext", new PostgreSqlFullTextSearchTransformer());
        ruleTransformers.RegisterTransformer("json_contains", new PostgreSqlJsonContainsTransformer());
    });
```

## Supported Operators

The PostgreSQL provider supports all basic operators:

| Operator | PostgreSQL Output | Description |
|----------|-------------------|-------------|
| `equal` | `"field" = $1` | Equality |
| `not_equal` | `"field" != $1` | Inequality |
| `greater` | `"field" > $1` | Greater than |
| `greater_or_equal` | `"field" >= $1` | Greater than or equal |
| `less` | `"field" < $1` | Less than |
| `less_or_equal` | `"field" <= $1` | Less than or equal |

> **Note**: For advanced operators like `contains`, `in`, `between`, etc., you can register custom rule transformers or use the SQL Server provider which includes these operators.

## PostgreSQL-Specific Features

### Field Name Formatting
- PostgreSQL field names are wrapped in double quotes: `"field_name"`
- Supports field names with spaces and special characters
- Case-sensitive field name handling

### Parameter Formatting
- Uses PostgreSQL's numbered parameter format: `$1`, `$2`, `$3`, etc.
- Parameters are numbered starting from 1
- Supports unlimited parameters

### Data Types
- Supports all PostgreSQL data types through the type conversion system
- Automatic handling of PostgreSQL-specific date/time formats
- Support for PostgreSQL boolean values (true/false)
- Support for PostgreSQL arrays and JSON types

## Examples

### Web API Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IFilterBuilder _filterBuilder;
    private readonly IUserRepository _userRepository;

    public UsersController(IFilterBuilder filterBuilder, IUserRepository userRepository)
    {
        _filterBuilder = filterBuilder;
        _userRepository = userRepository;
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<User>>> SearchUsers([FromBody] FilterGroup filters)
    {
        var (whereClause, parameters) = _filterBuilder.Build(filters);
        var users = await _userRepository.SearchAsync(whereClause, parameters);
        return Ok(users);
    }
}
```

### Npgsql Integration

```csharp
public class ProductService
{
    private readonly NpgsqlConnection _connection;
    private readonly IFilterBuilder _filterBuilder;

    public ProductService(NpgsqlConnection connection, IFilterBuilder filterBuilder)
    {
        _connection = connection;
        _filterBuilder = filterBuilder;
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(FilterGroup filters)
    {
        var (whereClause, parameters) = _filterBuilder.Build(filters);
        
        var sql = $@"
            SELECT p.*, c.name as category_name 
            FROM products p 
            INNER JOIN categories c ON p.category_id = c.id 
            WHERE {whereClause}
            ORDER BY p.name";
            
        using var cmd = new NpgsqlCommand(sql, _connection);
        for (int i = 0; i < parameters.Length; i++)
        {
            cmd.Parameters.AddWithValue($"${i + 1}", parameters[i]);
        }
        
        // Execute and return results
        return await ExecuteQueryAsync<Product>(cmd);
    }
}
```

### Complex Nested Queries

```csharp
var mainGroup = new FilterGroup("AND");
mainGroup.Rules.Add(new FilterRule("is_active", "equal", true));

var nameGroup = new FilterGroup("OR");
nameGroup.Rules.Add(new FilterRule("first_name", "equal", "John"));
nameGroup.Rules.Add(new FilterRule("last_name", "equal", "Smith"));
mainGroup.Groups.Add(nameGroup);

var (query, parameters) = _filterBuilder.Build(mainGroup);
// Result: "\"is_active\" = $1 AND (\"first_name\" = $2 OR \"last_name\" = $3)"
```

## Troubleshooting

### Common Issues

1. **Missing Extension Method**: Ensure you have `using Q.FilterBuilder.PostgreSql.Extensions;`
2. **Parameter Mismatch**: Check that your parameter count matches the generated query
3. **Type Conversion Errors**: Register appropriate type converters for custom data types
4. **PostgreSQL Syntax Errors**: Verify that custom rule transformers generate valid PostgreSQL syntax
5. **Case Sensitivity**: Remember that PostgreSQL is case-sensitive for quoted identifiers

## See Also

- [Q.FilterBuilder.Core Documentation](../../Core/README.md)
- [Main Project Documentation](../../../README.md)
