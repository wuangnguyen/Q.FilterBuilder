# Q.FilterBuilder.SqlServer

SQL Server provider for Q.FilterBuilder that provides SQL Server-specific query generation and operators.

## Installation

```bash
dotnet add package Q.FilterBuilder.SqlServer
```

> **Note**: The core `Q.FilterBuilder.Core` package will be automatically installed as a dependency.

## Quick Start

### 1. Basic Usage

Register the SQL Server FilterBuilder in your DI container:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.SqlServer.Extensions;

// In your Startup.cs or Program.cs
services.AddSqlServerFilterBuilder();
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
// Result: "[Name] = @p0 AND [Age] > @p1"
// Parameters: ["John", 25]
```

### 3. SQL Server Specific Features

The SQL Server provider generates SQL Server-specific syntax:

```csharp
var group = new FilterGroup("AND");
group.Rules.Add(new FilterRule("Name", "contains", "John"));
group.Rules.Add(new FilterRule("CategoryId", "in", new[] { 1, 2, 3 }));
group.Rules.Add(new FilterRule("CreatedDate", "between", new[] { "2023-01-01", "2023-12-31" }));

var (query, parameters) = _filterBuilder.Build(group);
// Result: "[Name] LIKE '%' + @p0 + '%' AND [CategoryId] IN (@p1, @p2, @p3) AND [CreatedDate] BETWEEN @p4 AND @p5"
```

## Advanced Usage

### Custom Type Converters

Register custom type converters for specialized data transformations:

```csharp
using Q.FilterBuilder.Core.TypeConversion;

services.AddSqlServerFilterBuilder(typeConversion =>
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

services.AddSqlServerFilterBuilder(ruleTransformers =>
{
    // Register a custom transformer for full-text search
    ruleTransformers.RegisterTransformer("fulltext", new FullTextSearchTransformer());

    // Register a custom transformer for JSON queries
    ruleTransformers.RegisterTransformer("json_contains", new JsonContainsTransformer());
});

// Example custom transformer
public class FullTextSearchTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Full-text search requires a value");

        return new[] { value };
    }

    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"CONTAINS({fieldName}, {parameterName}0)";
    }
}
```

### Both Custom Type Converters and Rule Transformers

```csharp
services.AddSqlServerFilterBuilder(
    typeConversion =>
    {
        typeConversion.RegisterConverter("currency", new CurrencyConverter());
        typeConversion.RegisterConverter("phone", new PhoneNumberConverter());
    },
    ruleTransformers =>
    {
        ruleTransformers.RegisterTransformer("fulltext", new FullTextSearchTransformer());
        ruleTransformers.RegisterTransformer("json_contains", new JsonContainsTransformer());
    });
```

## Supported Operators

The SQL Server provider supports all standard operators plus SQL Server-specific ones:

| Operator | SQL Output | Description |
|----------|------------|-------------|
| `equal` | `[field] = @p0` | Equality |
| `not_equal` | `[field] <> @p0` | Inequality |
| `greater` | `[field] > @p0` | Greater than |
| `greater_or_equal` | `[field] >= @p0` | Greater than or equal |
| `less` | `[field] < @p0` | Less than |
| `less_or_equal` | `[field] <= @p0` | Less than or equal |
| `contains` | `[field] LIKE '%' + @p0 + '%'` | String contains |
| `not_contains` | `[field] NOT LIKE '%' + @p0 + '%'` | String doesn't contain |
| `begins_with` | `[field] LIKE @p0 + '%'` | String starts with |
| `not_begins_with` | `[field] NOT LIKE @p0 + '%'` | String doesn't start with |
| `ends_with` | `[field] LIKE '%' + @p0` | String ends with |
| `not_ends_with` | `[field] NOT LIKE '%' + @p0` | String doesn't end with |
| `in` | `[field] IN (@p0, @p1, @p2)` | Value in list |
| `not_in` | `[field] NOT IN (@p0, @p1, @p2)` | Value not in list |
| `between` | `[field] BETWEEN @p0 AND @p1` | Value between range |
| `not_between` | `[field] NOT BETWEEN @p0 AND @p1` | Value not between range |
| `is_null` | `[field] IS NULL` | Null check |
| `is_not_null` | `[field] IS NOT NULL` | Not null check |
| `is_empty` | `[field] = ''` | Empty string check |
| `is_not_empty` | `[field] <> ''` | Non-empty string check |

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

### Complex Nested Queries

```csharp
var mainGroup = new FilterGroup("AND");
mainGroup.Rules.Add(new FilterRule("IsActive", "equal", true));

var nameGroup = new FilterGroup("OR");
nameGroup.Rules.Add(new FilterRule("FirstName", "contains", "John"));
nameGroup.Rules.Add(new FilterRule("LastName", "contains", "Smith"));
mainGroup.Groups.Add(nameGroup);

var (query, parameters) = _filterBuilder.Build(mainGroup);
// Result: "[IsActive] = @p0 AND ([FirstName] LIKE '%' + @p1 + '%' OR [LastName] LIKE '%' + @p2 + '%')"
```

## Real-World Examples

### Entity Framework Integration

```csharp
public class UserRepository
{
    private readonly DbContext _context;
    private readonly IFilterBuilder _filterBuilder;

    public UserRepository(DbContext context, IFilterBuilder filterBuilder)
    {
        _context = context;
        _filterBuilder = filterBuilder;
    }

    public async Task<List<User>> GetFilteredUsersAsync(FilterGroup filters)
    {
        var (whereClause, parameters) = _filterBuilder.Build(filters);

        return await _context.Users
            .FromSqlRaw($"SELECT * FROM Users WHERE {whereClause}", parameters)
            .ToListAsync();
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

### Advanced Custom Converters

```csharp
// Enum converter
public class StatusEnumConverter : ITypeConverter
{
    public object? Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null) return null;

        if (Enum.TryParse<UserStatus>(value.ToString(), true, out var status))
        {
            return (int)status; // Convert to int for database
        }

        throw new InvalidOperationException($"Invalid status: {value}");
    }
}

// JSON converter for complex objects
public class JsonConverter : ITypeConverter
{
    public object? Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null) return null;

        // Convert object to JSON string for database storage
        return JsonSerializer.Serialize(value);
    }
}

// Registration
services.AddSqlServerFilterBuilder(typeConversion =>
{
    typeConversion.RegisterConverter("status", new StatusEnumConverter());
    typeConversion.RegisterConverter("json", new JsonConverter());
});
```

### Advanced Custom Rule Transformers

```csharp
// SQL Server full-text search
public class FullTextSearchTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        // Format for SQL Server full-text search
        var searchTerm = value.ToString()!.Replace("'", "''");
        return new[] { $"\"{searchTerm}\"" };
    }

    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"CONTAINS({fieldName}, {parameterName}0)";
    }
}

// JSON path query for SQL Server 2016+
public class JsonPathTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var jsonPath = metadata?.GetValueOrDefault("path")?.ToString() ?? "$.value";
        return new[] { jsonPath, value };
    }

    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"JSON_VALUE({fieldName}, {parameterName}0) = {parameterName}1";
    }
}

// Registration
services.AddSqlServerFilterBuilder(ruleTransformers =>
{
    ruleTransformers.RegisterTransformer("fulltext", new FullTextSearchTransformer());
    ruleTransformers.RegisterTransformer("json_path", new JsonPathTransformer());
});
```

### Usage with Custom Operators

```csharp
// Using custom full-text search
var filters = new FilterGroup("AND");
filters.Rules.Add(new FilterRule("Description", "fulltext", "SQL Server database"));

// Using JSON path queries
var jsonRule = new FilterRule("Settings", "json_path", "dark");
jsonRule.Data["path"] = "$.theme.mode";
filters.Rules.Add(jsonRule);

var (query, parameters) = _filterBuilder.Build(filters);
// Result: "CONTAINS([Description], @p0) AND JSON_VALUE([Settings], @p1) = @p2"
```

## Troubleshooting

### Common Issues

1. **Missing Extension Method**: Ensure you have `using Q.FilterBuilder.SqlServer.Extensions;`
2. **Parameter Mismatch**: Check that your parameter count matches the generated query
3. **Type Conversion Errors**: Register appropriate type converters for custom data types
4. **SQL Syntax Errors**: Verify that custom rule transformers generate valid SQL Server syntax

## See Also

- [Q.FilterBuilder.Core Documentation](../Q.FilterBuilder.Core/README.md)
- [Main Project Documentation](../../README.md)
