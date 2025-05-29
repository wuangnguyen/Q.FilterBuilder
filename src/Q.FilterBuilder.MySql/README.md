# Q.FilterBuilder.MySql

MySQL provider for Q.FilterBuilder that provides MySQL-specific query generation and field formatting.

## Installation

```bash
dotnet add package Q.FilterBuilder.MySql
```

> **Note**: The core `Q.FilterBuilder.Core` package will be automatically installed as a dependency.

## Quick Start

### 1. Basic Usage

Register the MySQL FilterBuilder in your DI container:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.MySql.Extensions;

// In your Startup.cs or Program.cs
services.AddMySqlFilterBuilder();
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
// Result: "`Name` = ? AND `Age` > ?"
// Parameters: ["John", 25]
```

### 3. MySQL Specific Features

The MySQL provider generates MySQL-specific syntax:

```csharp
var group = new FilterGroup("AND");
group.Rules.Add(new FilterRule("Name", "equal", "John"));
group.Rules.Add(new FilterRule("Age", "greater_or_equal", 18));
group.Rules.Add(new FilterRule("Status", "not_equal", "Inactive"));

var (query, parameters) = _filterBuilder.Build(group);
// Result: "`Name` = ? AND `Age` >= ? AND `Status` != ?"
// Parameters: ["John", 18, "Inactive"]
```

## Advanced Usage

### Custom Type Converters

Register custom type converters for specialized data transformations:

```csharp
using Q.FilterBuilder.Core.TypeConversion;

services.AddMySqlFilterBuilder(typeConversion =>
{
    // Register a custom converter for currency formatting
    typeConversion.RegisterConverter("currency", new CurrencyConverter());
    
    // Register a custom converter for phone number formatting
    typeConversion.RegisterConverter("phone", new PhoneNumberConverter());
});

// Example custom converter
public class CurrencyConverter : ITypeConverter
{
    public object? Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null) return null;
        
        if (decimal.TryParse(value.ToString(), out var amount))
        {
            return Math.Round(amount, 2);
        }
        
        throw new InvalidOperationException($"Cannot convert '{value}' to currency");
    }
}
```

### Custom Rule Transformers

Register custom rule transformers for specialized query operations:

```csharp
using Q.FilterBuilder.Core.RuleTransformers;

services.AddMySqlFilterBuilder(ruleTransformers =>
{
    // Register a custom transformer for full-text search
    ruleTransformers.RegisterTransformer("fulltext", new MySqlFullTextSearchTransformer());
    
    // Register a custom transformer for JSON queries (MySQL 5.7+)
    ruleTransformers.RegisterTransformer("json_contains", new MySqlJsonContainsTransformer());
});

// Example custom transformer for MySQL full-text search
public class MySqlFullTextSearchTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Full-text search requires a value");
            
        return new[] { value };
    }

    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"MATCH({fieldName}) AGAINST(? IN BOOLEAN MODE)";
    }
}
```

### Both Custom Type Converters and Rule Transformers

```csharp
services.AddMySqlFilterBuilder(
    typeConversion =>
    {
        typeConversion.RegisterConverter("currency", new CurrencyConverter());
        typeConversion.RegisterConverter("phone", new PhoneNumberConverter());
    },
    ruleTransformers =>
    {
        ruleTransformers.RegisterTransformer("fulltext", new MySqlFullTextSearchTransformer());
        ruleTransformers.RegisterTransformer("json_contains", new MySqlJsonContainsTransformer());
    });
```

## Supported Operators

The MySQL provider supports all basic operators:

| Operator | MySQL Output | Description |
|----------|--------------|-------------|
| `equal` | `` `field` = ? `` | Equality |
| `not_equal` | `` `field` != ? `` | Inequality |
| `greater` | `` `field` > ? `` | Greater than |
| `greater_or_equal` | `` `field` >= ? `` | Greater than or equal |
| `less` | `` `field` < ? `` | Less than |
| `less_or_equal` | `` `field` <= ? `` | Less than or equal |

> **Note**: For advanced operators like `contains`, `in`, `between`, etc., you can register custom rule transformers or use the SQL Server provider which includes these operators.

## MySQL-Specific Features

### Field Name Formatting
- MySQL field names are wrapped in backticks: `` `field_name` ``
- Supports field names with spaces and special characters

### Parameter Formatting
- Uses MySQL's positional parameter format: `?`
- All parameters are positional and passed in order

### Data Types
- Supports all MySQL data types through the type conversion system
- Automatic handling of MySQL-specific date/time formats
- Support for MySQL boolean values (0/1)

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

### Dapper Integration

```csharp
public class ProductService
{
    private readonly IDbConnection _connection;
    private readonly IFilterBuilder _filterBuilder;

    public ProductService(IDbConnection connection, IFilterBuilder filterBuilder)
    {
        _connection = connection;
        _filterBuilder = filterBuilder;
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(FilterGroup filters)
    {
        var (whereClause, parameters) = _filterBuilder.Build(filters);
        
        var sql = $@"
            SELECT p.*, c.Name as CategoryName 
            FROM Products p 
            INNER JOIN Categories c ON p.CategoryId = c.Id 
            WHERE {whereClause}
            ORDER BY p.Name";
            
        return await _connection.QueryAsync<Product>(sql, parameters.ToDynamicParameters());
    }
}
```

### Complex Nested Queries

```csharp
var mainGroup = new FilterGroup("AND");
mainGroup.Rules.Add(new FilterRule("IsActive", "equal", true));

var nameGroup = new FilterGroup("OR");
nameGroup.Rules.Add(new FilterRule("FirstName", "equal", "John"));
nameGroup.Rules.Add(new FilterRule("LastName", "equal", "Smith"));
mainGroup.Groups.Add(nameGroup);

var (query, parameters) = _filterBuilder.Build(mainGroup);
// Result: "`IsActive` = ? AND (`FirstName` = ? OR `LastName` = ?)"
```

## Troubleshooting

### Common Issues

1. **Missing Extension Method**: Ensure you have `using Q.FilterBuilder.MySql.Extensions;`
2. **Parameter Mismatch**: Check that your parameter count matches the generated query
3. **Type Conversion Errors**: Register appropriate type converters for custom data types
4. **MySQL Syntax Errors**: Verify that custom rule transformers generate valid MySQL syntax

## See Also

- [Q.FilterBuilder.Core Documentation](../Q.FilterBuilder.Core/README.md)
- [Main Project Documentation](../../README.md)
