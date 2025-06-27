# Rule Transformers Documentation

Rule transformers are the core components that convert `FilterRule` objects into database-specific query conditions. They replace the old operator system and provide a more flexible, extensible architecture for handling different query operations.

## Overview

Rule transformers follow the operator isolation pattern where each transformer handles its own parameter normalization and query generation logic internally. This eliminates the need for if/else conditions based on operator names in the framework.

## Core Architecture

### IRuleTransformer Interface

The base interface that all rule transformers must implement.

```csharp
public interface IRuleTransformer
{
    (string query, object[]? parameters) Transform(FilterRule rule, string fieldName, int parameterIndex, IQueryFormatProvider formatProvider);
}
```

**Parameters:**
- `rule`: The FilterRule to transform
- `fieldName`: The formatted field name (e.g., `[Name]`, `"Name"`, `` `Name` ``)
- `parameterName`: The formatted parameter name (e.g., `@p0`, `$1`, `?`)

**Returns:**
- `query`: The generated query string
- `parameters`: Array of parameter values, or null if no parameters needed

### BaseRuleTransformer Abstract Class

Provides common orchestration logic and default parameter building behavior.

```csharp
public abstract class BaseRuleTransformer : IRuleTransformer
{
    public class TransformContext
    {
        public object[]? Parameters { get; set; }
        public Dictionary<string, object?>? Metadata { get; set; }
        public int ParameterIndex { get; set; }
        public IQueryFormatProvider? FormatProvider { get; set; }
    }

    public virtual (string query, object[]? parameters) Transform(FilterRule rule, string fieldName, int parameterIndex, IQueryFormatProvider formatProvider)
    {
        var metadata = rule.Metadata != null
            ? new Dictionary<string, object?>(rule.Metadata)
            : new Dictionary<string, object?>();

        if (!metadata.ContainsKey("type"))
        {
            metadata["type"] = rule.Type;
        }

        rule.Metadata = metadata;

        var context = new TransformContext
        {
            Metadata = rule.Metadata,
            ParameterIndex = parameterIndex,
            FormatProvider = formatProvider
        };

        context.Parameters = BuildParameters(rule.Value, rule.Metadata);

        var query = BuildQuery(fieldName, context);

        return (query, context.Parameters);
    }

    protected virtual object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            return null;
        }

        return new[] { value };
    }

    protected abstract string BuildQuery(string fieldName, TransformContext context);
}
```

## Built-in Transformers

### BasicRuleTransformer

Handles simple comparison operators that work the same across all databases.

```csharp
public class BasicRuleTransformer : BaseRuleTransformer
{
    private readonly string _operator;

    public BasicRuleTransformer(string @operator)
    {
        _operator = @operator;
    }

    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null) return null;

        if (value is IEnumerable && value is not string)
        {
            throw new ArgumentException($"Basic operator '{_operator}' cannot compare with collections. Use collection-specific operators instead.", nameof(value));
        }

        return new[] { value };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        return $"{fieldName} {_operator} {parameterName}";
    }
}
```

**Registered Operators:**
- `equal` → `=`
- `not_equal` → `!=`
- `less` → `<`
- `less_or_equal` → `<=`
- `greater` → `>`
- `greater_or_equal` → `>=`

**Usage:**
```csharp
var rule = new FilterRule("Age", "greater", 25);
// Generates: [Age] > @p0 (SQL Server)
// Parameters: [25]
```

### SimpleNoParameterTransformer

Base class for operators that don't require parameters.

```csharp
public abstract class SimpleNoParameterTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        return null;
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        return BuildSimpleQuery(fieldName);
    }

    protected abstract string BuildSimpleQuery(string fieldName);
}
```

**Example Implementation:**
```csharp
public class IsNullTransformer : SimpleNoParameterTransformer
{
    protected override string BuildSimpleQuery(string fieldName)
    {
        return $"{fieldName} IS NULL";
    }
}
```

### InTransformerBase

Base class for IN and NOT IN operators that handle collections.

```csharp
public abstract class InTransformerBase : BaseRuleTransformer
{
    private readonly string _operatorName;

    protected InTransformerBase(string operatorName)
    {
        _operatorName = operatorName ?? throw new ArgumentNullException(nameof(operatorName));
    }

    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), $"{_operatorName} operator requires a non-null value");
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var values = new List<object>();
            foreach (var item in enumerable)
            {
                values.Add(item);
            }

            if (values.Count == 0)
            {
                throw new ArgumentException($"{_operatorName} operator requires at least one value", nameof(value));
            }

            return values.ToArray();
        }

        return new[] { value };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        if (context.Parameters == null || context.Parameters.Length == 0)
        {
            throw new InvalidOperationException($"{_operatorName} operator requires parameters");
        }

        var parameterPlaceholders = Enumerable.Range(context.ParameterIndex, context.Parameters.Length)
            .Select(index => context.FormatProvider!.FormatParameterName(index))
            .ToArray();
        var parameterList = string.Join(", ", parameterPlaceholders);

        return BuildInQuery(fieldName, parameterList);
    }

    protected abstract string BuildInQuery(string fieldName, string parameterList);
}
```

## Custom Rule Transformers

### Creating Custom Transformers

```csharp
// Example: Full-text search transformer
public class FullTextSearchTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        // Process search terms
        var searchTerms = value.ToString()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var processedTerms = searchTerms.Select(term => $"\"{term}\"").ToArray();

        return new[] { string.Join(" AND ", processedTerms) };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        return $"CONTAINS({fieldName}, {parameterName})";
    }
}

// Example: JSON path transformer
public class JsonContainsTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null) return null;

        // Get JSON path from metadata
        var jsonPath = metadata?.TryGetValue("jsonPath", out var pathValue) == true
            ? pathValue?.ToString()
            : "$";

        return new[] { jsonPath, value };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName0 = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        var parameterName1 = context.FormatProvider!.FormatParameterName(context.ParameterIndex + 1);
        return $"JSON_CONTAINS({fieldName}, {parameterName1}, {parameterName0})";
    }
}
```

### Advanced Custom Transformer

```csharp
// Example: Date range with automatic normalization
public class DateRangeTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (value is not IEnumerable enumerable || value is string)
            throw new ArgumentException("Date range requires an array of two dates");

        var dates = enumerable.Cast<object>().ToArray();
        if (dates.Length != 2)
            throw new ArgumentException("Date range requires exactly two dates");

        // Parse and normalize dates
        var startDate = DateTime.Parse(dates[0].ToString()!).Date; // Start of day
        var endDate = DateTime.Parse(dates[1].ToString()!).Date.AddDays(1).AddTicks(-1); // End of day

        return new object[] { startDate, endDate };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName0 = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        var parameterName1 = context.FormatProvider!.FormatParameterName(context.ParameterIndex + 1);
        return $"{fieldName} >= {parameterName0} AND {fieldName} <= {parameterName1}";
    }
}

```
```

## Registration and Configuration

### Service Registration

```csharp
// Register during DI configuration
services.AddFilterBuilder(
    new SqlServerFormatProvider(),
    ruleTransformers =>
    {
        ruleTransformers.RegisterTransformer("fulltext", new FullTextSearchTransformer());
        ruleTransformers.RegisterTransformer("json_contains", new JsonContainsTransformer());
        ruleTransformers.RegisterTransformer("date_range", new DateRangeTransformer());
    });
```

### Provider-Specific Registration

```csharp
// SQL Server specific transformers
public class SqlServerRuleTransformerService : RuleTransformerService
{
    public SqlServerRuleTransformerService()
    {
        RegisterSqlServerTransformers();
    }

    private void RegisterSqlServerTransformers()
    {
        // Basic operators (inherited from base)

        // SQL Server specific operators
        RegisterTransformer("in", new InRuleTransformer());
        RegisterTransformer("not_in", new NotInRuleTransformer());
        RegisterTransformer("between", new BetweenRuleTransformer());
        RegisterTransformer("contains", new ContainsRuleTransformer());
        RegisterTransformer("is_null", new IsNullTransformer());
        RegisterTransformer("is_not_null", new IsNotNullTransformer());
    }
}
```

## Usage Examples

### Basic Usage

```csharp
// Simple comparison
var rule1 = new FilterRule("Age", "greater", 25);

// Collection operations
var rule2 = new FilterRule("Status", "in", new[] { "Active", "Pending" });

// Range operations
var rule3 = new FilterRule("Date", "between", new[] { "2023-01-01", "2023-12-31" });

// String operations
var rule4 = new FilterRule("Name", "contains", "John");

// Null checks
var rule5 = new FilterRule("Email", "is_not_null");
```

### Custom Transformer Usage

```csharp
// Full-text search
var rule1 = new FilterRule("Content", "fulltext", "search terms here");

// JSON operations with metadata
var rule2 = new FilterRule("JsonData", "json_contains", "value");
rule2.Metadata = new Dictionary<string, object?>
{
    ["jsonPath"] = "$.user.name"
};

// Date range with automatic normalization
var rule3 = new FilterRule("CreatedDate", "date_range", new[] { "2023-01-01", "2023-01-31" });
```

## Error Handling

### Common Exceptions

1. **ArgumentException**: Invalid parameters for operator
```csharp
// Basic operator with collection - throws ArgumentException
var rule = new FilterRule("Age", "equal", new[] { 1, 2, 3 });
```

2. **ArgumentNullException**: Required value is null
```csharp
// IN operator with null value - throws ArgumentNullException
var rule = new FilterRule("Status", "in", null);
```

3. **NotImplementedException**: Unregistered operator
```csharp
// Unknown operator - throws NotImplementedException
var rule = new FilterRule("Field", "unknown_operator", "value");
```

## Best Practices

### 1. Validate Parameters Early

```csharp
protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
{
    if (value == null)
        throw new ArgumentNullException(nameof(value), "Operator requires a non-null value");

    // Additional validation...
    return new[] { value };
}
```

### 2. Use Metadata for Configuration

```csharp
protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
{
    var caseSensitive = metadata?.TryGetValue("caseSensitive", out var caseValue) == true
        && bool.Parse(caseValue?.ToString() ?? "false");

    var processedValue = caseSensitive ? value?.ToString() : value?.ToString()?.ToLowerInvariant();
    return new[] { processedValue };
}
```

### 3. Handle Collections Appropriately

```csharp
protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
{
    // Check if operator supports collections
    if (value is IEnumerable && value is not string && !SupportsCollections)
    {
        throw new ArgumentException($"Operator '{OperatorName}' does not support collections");
    }

    // Process accordingly...
}
```

### 4. Provider-Specific Implementation

```csharp
// Create provider-specific transformers when syntax differs
public class SqlServerContainsTransformer : BaseRuleTransformer
{
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"{fieldName} LIKE '%' + {parameterName} + '%'"; // SQL Server concatenation
    }
}

public class MySqlContainsTransformer : BaseRuleTransformer
{
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"{fieldName} LIKE CONCAT('%', {parameterName}, '%')"; // MySQL concatenation
    }
}
```

## Advanced Scenarios

### Conditional Query Building

```csharp
public class SmartLikeTransformer : BaseRuleTransformer
{
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        var useWildcards = context.Metadata?.TryGetValue("useWildcards", out var wildcardValue) == true
            && bool.Parse(wildcardValue?.ToString() ?? "true");

        if (useWildcards)
        {
            return $"{fieldName} LIKE {parameterName}";
        }
        else
        {
            return $"{fieldName} = {parameterName}";
        }
    }
}
```

### Multi-Parameter Transformers

```csharp
public class ProximitySearchTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        var searchTerm = value?.ToString() ?? throw new ArgumentNullException(nameof(value));
        var distance = metadata?.TryGetValue("distance", out var distValue) == true
            ? int.Parse(distValue?.ToString() ?? "5")
            : 5;

        return new object[] { searchTerm, distance };
    }

    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"CONTAINS({fieldName}, 'NEAR(({parameterName}0), {parameterName}1)')";
    }
}
```

## Complete Transformer Examples

### String Operations Transformers

```csharp
// LIKE with wildcards
public class LikeTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null) return null;

        var pattern = value.ToString()!;
        var addWildcards = metadata?.TryGetValue("addWildcards", out var wildcardValue) == true
            && bool.Parse(wildcardValue?.ToString() ?? "true");

        if (addWildcards && !pattern.Contains('%') && !pattern.Contains('_'))
        {
            pattern = $"%{pattern}%";
        }

        return new[] { pattern };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        return $"{fieldName} LIKE {parameterName}";
    }
}

// Case-insensitive contains
public class IContainsTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null) return null;
        return new[] { $"%{value.ToString()!.ToLowerInvariant()}%" };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        return $"LOWER({fieldName}) LIKE {parameterName}";
    }
}
```

### Numeric Range Transformers

```csharp
// Range with tolerance
public class RangeWithToleranceTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var numericValue = Convert.ToDouble(value);
        var tolerance = metadata?.TryGetValue("tolerance", out var tolValue) == true
            ? Convert.ToDouble(tolValue)
            : 0.1;

        var minValue = numericValue - tolerance;
        var maxValue = numericValue + tolerance;

        return new object[] { minValue, maxValue };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName0 = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        var parameterName1 = context.FormatProvider!.FormatParameterName(context.ParameterIndex + 1);
        return $"{fieldName} BETWEEN {parameterName0} AND {parameterName1}";
    }
}
```

### Date-Specific Transformers

```csharp
// Date within last N days
public class WithinLastDaysTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var days = Convert.ToInt32(value);
        var cutoffDate = DateTime.Now.AddDays(-days);

        return new[] { cutoffDate };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        return $"{fieldName} >= {parameterName}";
    }
}

// Same day comparison
public class SameDayTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var date = DateTime.Parse(value.ToString()!).Date;
        var startOfDay = date;
        var endOfDay = date.AddDays(1).AddTicks(-1);

        return new object[] { startOfDay, endOfDay };
    }

    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName0 = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        var parameterName1 = context.FormatProvider!.FormatParameterName(context.ParameterIndex + 1);
        return $"{fieldName} >= {parameterName0} AND {fieldName} <= {parameterName1}";
    }
}
```
```
