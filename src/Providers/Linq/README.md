# Q.FilterBuilder.Linq

LINQ provider for Q.FilterBuilder that provides LINQ expression generation for in-memory filtering and Entity Framework integration.

## Installation

```bash
dotnet add package Q.FilterBuilder.Linq
```

> **Note**: The core `Q.FilterBuilder.Core` package will be automatically installed as a dependency.

## Quick Start

### 1. Basic Usage

Register the LINQ FilterBuilder in your DI container:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Linq.Extensions;

// In your Startup.cs or Program.cs
services.AddLinqFilterBuilder();
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

    public List<User> GetFilteredUsers(List<User> users, FilterGroup filters)
    {
        var (expression, parameters) = _filterBuilder.Build(filters);
        
        // Use with LINQ to Objects
        return users.Where(u => EvaluateExpression(u, expression, parameters)).ToList();
    }
}
```

### 2. Creating Filter Groups

```csharp
// Simple AND group
var group = new FilterGroup("AND");
group.Rules.Add(new FilterRule("Name", "equal", "John"));
group.Rules.Add(new FilterRule("Age", "greater", 25));

var (expression, parameters) = _filterBuilder.Build(group);
// Result: "Name == @p0 && Age > @p1"
// Parameters: ["John", 25]
```

### 3. LINQ Specific Features

The LINQ provider generates LINQ-compatible expressions:

```csharp
var group = new FilterGroup("AND");
group.Rules.Add(new FilterRule("Name", "contains", "John"));
group.Rules.Add(new FilterRule("CategoryId", "in", new[] { 1, 2, 3 }));
group.Rules.Add(new FilterRule("IsActive", "equal", true));

var (expression, parameters) = _filterBuilder.Build(group);
// Result: "Name.Contains(@p0) && @p1.Contains(CategoryId) && IsActive == @p2"
// Parameters: ["John", [1, 2, 3], true]
```

## Advanced Usage

### Custom Type Converters

Register custom type converters for specialized data transformations:

```csharp
using Q.FilterBuilder.Core.TypeConversion;

services.AddLinqFilterBuilder(typeConversion =>
{
    // Register a custom converter for enum handling
    typeConversion.RegisterConverter("enum", new EnumConverter());
    
    // Register a custom converter for complex objects
    typeConversion.RegisterConverter("complex", new ComplexObjectConverter());
});

// Example custom converter for enums
public class EnumConverter : ITypeConverter
{
    public object? Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null) return null;
        
        var enumType = metadata?.GetValueOrDefault("enumType") as Type;
        if (enumType != null && enumType.IsEnum)
        {
            return Enum.Parse(enumType, value.ToString()!);
        }
        
        throw new InvalidOperationException($"Cannot convert '{value}' to enum");
    }
}
```

### Custom Rule Transformers

Register custom rule transformers for specialized LINQ operations:

```csharp
using Q.FilterBuilder.Core.RuleTransformers;

services.AddLinqFilterBuilder(ruleTransformers =>
{
    // Register a custom transformer for string operations
    ruleTransformers.RegisterTransformer("starts_with", new StartsWithTransformer());
    
    // Register a custom transformer for date operations
    ruleTransformers.RegisterTransformer("date_range", new DateRangeTransformer());
});

// Example custom transformer for StartsWith
public class StartsWithTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "StartsWith requires a value");
            
        return new[] { value };
    }

    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"{fieldName}.StartsWith({parameterName}0)";
    }
}
```

### Both Custom Type Converters and Rule Transformers

```csharp
services.AddLinqFilterBuilder(
    typeConversion =>
    {
        typeConversion.RegisterConverter("enum", new EnumConverter());
        typeConversion.RegisterConverter("complex", new ComplexObjectConverter());
    },
    ruleTransformers =>
    {
        ruleTransformers.RegisterTransformer("starts_with", new StartsWithTransformer());
        ruleTransformers.RegisterTransformer("date_range", new DateRangeTransformer());
    });
```

## Supported Operators

The LINQ provider supports all basic operators plus LINQ-specific ones:

| Operator | LINQ Output | Description |
|----------|-------------|-------------|
| `equal` | `field == @p0` | Equality |
| `not_equal` | `field != @p0` | Inequality |
| `greater` | `field > @p0` | Greater than |
| `greater_or_equal` | `field >= @p0` | Greater than or equal |
| `less` | `field < @p0` | Less than |
| `less_or_equal` | `field <= @p0` | Less than or equal |
| `contains` | `field.Contains(@p0)` | String contains |
| `not_contains` | `!field.Contains(@p0)` | String doesn't contain |
| `begins_with` | `field.StartsWith(@p0)` | String starts with |
| `not_begins_with` | `!field.StartsWith(@p0)` | String doesn't start with |
| `ends_with` | `field.EndsWith(@p0)` | String ends with |
| `not_ends_with` | `!field.EndsWith(@p0)` | String doesn't end with |
| `in` | `@p0.Contains(field)` | Value in collection |
| `not_in` | `!@p0.Contains(field)` | Value not in collection |
| `between` | `field >= @p0 && field <= @p1` | Value between range |
| `not_between` | `field < @p0 \|\| field > @p1` | Value not between range |
| `is_null` | `field == null` | Null check |
| `is_not_null` | `field != null` | Not null check |
| `is_empty` | `string.IsNullOrEmpty(field)` | Empty string check |
| `is_not_empty` | `!string.IsNullOrEmpty(field)` | Non-empty string check |

## LINQ-Specific Features

### Field Name Formatting
- LINQ field names are used as-is (no special formatting)
- Supports property navigation: `User.Profile.Name`
- Case-sensitive property names

### Parameter Formatting
- Uses LINQ parameter format: `@p0`, `@p1`, `@p2`, etc.
- Parameters are referenced by index

### Logical Operators
- Uses LINQ logical operators: `&&` for AND, `||` for OR
- Supports complex nested expressions with proper parentheses

## Examples

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
        var (expression, parameters) = _filterBuilder.Build(filters);
        
        // Convert to Entity Framework expression
        var predicate = BuildLinqExpression<User>(expression, parameters);
        
        return await _context.Users
            .Where(predicate)
            .ToListAsync();
    }
    
    private Expression<Func<T, bool>> BuildLinqExpression<T>(string expression, object[] parameters)
    {
        // Implementation to convert string expression to LINQ Expression<Func<T, bool>>
        // This would typically use a library like System.Linq.Dynamic.Core
        return DynamicExpressionParser.ParseLambda<T, bool>(expression, parameters);
    }
}
```

### In-Memory Collections

```csharp
public class ProductService
{
    private readonly IFilterBuilder _filterBuilder;
    private readonly List<Product> _products;

    public ProductService(IFilterBuilder filterBuilder, List<Product> products)
    {
        _filterBuilder = filterBuilder;
        _products = products;
    }

    public List<Product> SearchProducts(FilterGroup filters)
    {
        var (expression, parameters) = _filterBuilder.Build(filters);
        
        // Use with LINQ to Objects
        return _products
            .Where(p => EvaluateLinqExpression(p, expression, parameters))
            .ToList();
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

var (expression, parameters) = _filterBuilder.Build(mainGroup);
// Result: "IsActive == @p0 && (FirstName.Contains(@p1) || LastName.Contains(@p2))"
```

### Dynamic LINQ with System.Linq.Dynamic.Core

```csharp
// Install-Package System.Linq.Dynamic.Core

public class DynamicQueryService
{
    private readonly IFilterBuilder _filterBuilder;

    public DynamicQueryService(IFilterBuilder filterBuilder)
    {
        _filterBuilder = filterBuilder;
    }

    public IQueryable<T> ApplyFilters<T>(IQueryable<T> query, FilterGroup filters)
    {
        var (expression, parameters) = _filterBuilder.Build(filters);
        
        // Use System.Linq.Dynamic.Core for dynamic LINQ
        return query.Where(expression, parameters);
    }
}
```

## Troubleshooting

### Common Issues

1. **Missing Extension Method**: Ensure you have `using Q.FilterBuilder.Linq.Extensions;`
2. **Expression Compilation Errors**: Verify that property names match exactly
3. **Type Conversion Errors**: Register appropriate type converters for custom types
4. **Performance Issues**: Consider using compiled expressions for frequently used filters
5. **Entity Framework Issues**: Ensure expressions are compatible with your EF provider

## Integration Libraries

### Recommended Packages

- **System.Linq.Dynamic.Core**: For dynamic LINQ expression parsing
- **Entity Framework Core**: For database integration
- **AutoMapper**: For object mapping in complex scenarios

## See Also

- [Q.FilterBuilder.Core Documentation](../../Core/README.md)
- [Main Project Documentation](../../../README.md)
- [System.Linq.Dynamic.Core Documentation](https://dynamic-linq.net/)
