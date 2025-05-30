# Q.FilterBuilder.Core

Core library for Q.FilterBuilder that provides the foundation for dynamic query building with pluggable database providers.

## Installation

```bash
dotnet add package Q.FilterBuilder.Core
```

> **Note**: If you're using a specific database, consider installing a provider package instead (e.g., `Q.FilterBuilder.SqlServer`) which includes this core package automatically.

## When to Use Core Package Only

Use the core package directly when:
- You want to create a custom database provider
- You need maximum control over the registration process
- You're working with a database not covered by existing providers
- You want to use FilterBuilder with a custom query engine

## Quick Start

### 1. Basic Usage with Manual Provider Registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Extensions;
using Q.FilterBuilder.Core.Providers;

// Register with a specific database provider
services.AddFilterBuilder(new SqlServerFormatProvider());

// Or register with custom services
services.AddFilterBuilder(
    new SqlServerFormatProvider(),
    typeConversion => {
        // Configure custom type converters
    },
    ruleTransformers => {
        // Configure custom rule transformers
    });
```

### 2. Using the FilterBuilder

```csharp
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

public class DataService
{
    private readonly IFilterBuilder _filterBuilder;

    public DataService(IFilterBuilder filterBuilder)
    {
        _filterBuilder = filterBuilder;
    }

    public async Task<List<T>> GetFilteredDataAsync<T>(FilterGroup filters)
    {
        var (query, parameters) = _filterBuilder.Build(filters);

        // Use with your data access layer
        var sql = $"SELECT * FROM {typeof(T).Name} WHERE {query}";
        return await _repository.QueryAsync<T>(sql, parameters);
    }
}
```

## Core Extension Methods

### AddFilterBuilder Overloads

#### 1. Basic Registration

```csharp
using Q.FilterBuilder.Core.Extensions;

// Register with a database provider
services.AddFilterBuilder(new SqlServerFormatProvider());
```

#### 2. With Custom Rule Transformer Service

```csharp
using Q.FilterBuilder.Core.RuleTransformers;

var customRuleTransformerService = new RuleTransformerService();
customRuleTransformerService.RegisterTransformer("custom_op", new CustomTransformer());

services.AddFilterBuilder(new SqlServerFormatProvider(), customRuleTransformerService);
```

#### 3. With Configuration Actions

```csharp
services.AddFilterBuilder(
    new SqlServerFormatProvider(),
    typeConversion =>
    {
        // Configure type conversion
        typeConversion.RegisterConverter("currency", new CurrencyConverter());
    },
    ruleTransformers =>
    {
        // Configure rule transformers
        ruleTransformers.RegisterTransformer("fulltext", new FullTextTransformer());
    });
```

## Advanced Usage

### Custom Query Format Provider

Create your own query format provider by implementing `IQueryFormatProvider`:

```csharp
using Q.FilterBuilder.Core.Providers;

public class CustomQueryFormatProvider : IQueryFormatProvider
{
    public string ParameterPrefix => "$";
    public string AndOperator => "AND";
    public string OrOperator => "OR";

    public string FormatFieldName(string fieldName)
    {
        return $"`{fieldName}`"; // Custom field formatting
    }

    public string FormatParameterName(int parameterIndex)
    {
        return $"{ParameterPrefix}param{parameterIndex}";
    }
}

// Register your custom provider
services.AddFilterBuilder(new CustomQueryFormatProvider());
```

### Custom Type Converters

```csharp
using Q.FilterBuilder.Core.TypeConversion;

public class CustomDateConverter : ITypeConverter
{
    public object? Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null) return null;

        // Custom date parsing logic
        if (DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new InvalidOperationException($"Cannot convert '{value}' to date");
    }
}

// Register the converter
services.AddFilterBuilder(
    new SqlServerFormatProvider(),
    typeConversion =>
    {
        typeConversion.RegisterConverter("custom_date", new CustomDateConverter());
    });
```

### Custom Rule Transformers

```csharp
using Q.FilterBuilder.Core.RuleTransformers;

public class CustomLikeTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        // Custom parameter processing
        var processedValue = $"%{value}%";
        return new[] { processedValue };
    }

    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        // Custom query generation
        return $"{fieldName} ILIKE {parameterName}0"; // Case-insensitive LIKE
    }
}

// Register the transformer
services.AddFilterBuilder(
    new PostgreSqlFormatProvider(),
    ruleTransformers =>
    {
        ruleTransformers.RegisterTransformer("ilike", new CustomLikeTransformer());
    });
```

### Standalone Service Registration

If you prefer to register services individually:

```csharp
using Q.FilterBuilder.Core.Extensions;

// Register individual services
services.AddTypeConversion(typeConversion =>
{
    typeConversion.RegisterConverter("currency", new CurrencyConverter());
});

services.AddRuleTransformers(ruleTransformers =>
{
    ruleTransformers.RegisterTransformer("custom", new CustomTransformer());
});

// Register query format provider
services.AddSingleton<IQueryFormatProvider, SqlServerFormatProvider>();

// Register FilterBuilder
services.AddSingleton<IFilterBuilder, FilterBuilder>();
```

## Core Models

### FilterGroup

```csharp
var group = new FilterGroup("AND"); // or "OR"
group.Rules.Add(new FilterRule("Name", "equal", "John"));
group.Rules.Add(new FilterRule("Age", "greater", 25));

// Nested groups
var nestedGroup = new FilterGroup("OR");
nestedGroup.Rules.Add(new FilterRule("City", "equal", "New York"));
nestedGroup.Rules.Add(new FilterRule("City", "equal", "London"));
group.Groups.Add(nestedGroup);
```

### FilterRule

```csharp
// Basic rule
var rule = new FilterRule("FieldName", "operator", value);

// Rule with explicit type
var typedRule = new FilterRule("DateField", "greater", "2023-01-01", "DateTime");

// Rule with metadata
var ruleWithMetadata = new FilterRule("CustomField", "custom_op", value);
ruleWithMetadata.Data["custom_setting"] = "special_value";
```

## Built-in Services

### Type Conversion Service

```csharp
// Access the service directly
var typeConversionService = serviceProvider.GetRequiredService<ITypeConversionService>();

// Convert values
var convertedValue = typeConversionService.ConvertValue("123", "int");
var dateValue = typeConversionService.ConvertValue("2023-01-01", "DateTime");
```

### Rule Transformer Service

```csharp
// Access the service directly
var ruleTransformerService = serviceProvider.GetRequiredService<IRuleTransformerService>();

// Get a transformer
var transformer = ruleTransformerService.GetRuleTransformer("equal");
```

## Creating Custom Components

### Custom Database Providers

Create your own database provider by implementing `IQueryFormatProvider`:

```csharp
using Q.FilterBuilder.Core.Providers;

public class OracleFormatProvider : IQueryFormatProvider
{
    public string ParameterPrefix => ":";
    public string AndOperator => "AND";
    public string OrOperator => "OR";

    public string FormatFieldName(string fieldName)
    {
        return fieldName.ToUpperInvariant(); // Oracle convention
    }

    public string FormatParameterName(int parameterIndex)
    {
        return $":p{parameterIndex}";
    }
}

// Create extension method
public static class OracleServiceCollectionExtensions
{
    public static IServiceCollection AddOracleFilterBuilder(this IServiceCollection services)
    {
        return services.AddFilterBuilder(new OracleFormatProvider());
    }
}

// Usage
services.AddOracleFilterBuilder();
```

📖 **Complete Reference**: [Database Providers Documentation](../../docs/providers.md)

### Custom Type Converters

```csharp
using Q.FilterBuilder.Core.TypeConversion;

public class CurrencyConverter : ITypeConverter<decimal>
{
    public decimal Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var stringValue = value.ToString()!;
        var cleanValue = stringValue.Replace("$", "").Replace(",", "").Trim();

        return decimal.Parse(cleanValue, CultureInfo.InvariantCulture);
    }
}

// Register the converter
services.AddFilterBuilder(
    new SqlServerFormatProvider(),
    typeConversion =>
    {
        typeConversion.RegisterConverter("currency", new CurrencyConverter());
    });
```

📖 **Complete Reference**: [Type Conversion Documentation](../../docs/type-conversion.md)

### Custom Rule Transformers

```csharp
using Q.FilterBuilder.Core.RuleTransformers;

public class FullTextTransformer : BaseRuleTransformer
{
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var searchTerms = value.ToString()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var processedTerms = searchTerms.Select(term => $"\"{term}\"").ToArray();

        return new[] { string.Join(" AND ", processedTerms) };
    }

    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"CONTAINS({fieldName}, {parameterName})";
    }
}

// Register the transformer
services.AddFilterBuilder(
    new SqlServerFormatProvider(),
    ruleTransformers =>
    {
        ruleTransformers.RegisterTransformer("fulltext", new FullTextTransformer());
    });
```

📖 **Complete Reference**: [Rule Transformers Documentation](../../docs/rule-transformers.md)

## Testing Your Custom Components

### Basic Testing Examples

```csharp
[Test]
public void OracleFormatProvider_ShouldFormatFieldsCorrectly()
{
    // Arrange
    var provider = new OracleFormatProvider();

    // Act
    var result = provider.FormatFieldName("UserName");

    // Assert
    Assert.AreEqual("USERNAME", result);
}

[Test]
public void CurrencyConverter_ShouldConvertValidCurrency()
{
    // Arrange
    var converter = new CurrencyConverter();

    // Act
    var result = converter.Convert("$1,234.56", null);

    // Assert
    Assert.AreEqual(1234.56m, result);
}

[Test]
public void FullTextTransformer_ShouldGenerateCorrectQuery()
{
    // Arrange
    var transformer = new FullTextTransformer();
    var rule = new FilterRule("Content", "fulltext", "search terms");

    // Act
    var (query, parameters) = transformer.Transform(rule, "[Content]", "@p0");

    // Assert
    Assert.AreEqual("CONTAINS([Content], @p0)", query);
    Assert.AreEqual(new[] { "\"search\" AND \"terms\"" }, parameters);
}
```

### Integration Testing

```csharp
[Test]
public void FilterBuilder_WithCustomComponents_ShouldWork()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddFilterBuilder(
        new OracleFormatProvider(),
        typeConversion =>
        {
            typeConversion.RegisterConverter("currency", new CurrencyConverter());
        },
        ruleTransformers =>
        {
            ruleTransformers.RegisterTransformer("fulltext", new FullTextTransformer());
        });

    var serviceProvider = services.BuildServiceProvider();
    var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

    var group = new FilterGroup("AND");
    group.Rules.Add(new FilterRule("PRICE", "greater", "$100.00", "currency"));

    // Act
    var (query, parameters) = filterBuilder.Build(group);

    // Assert
    Assert.AreEqual("PRICE > :p0", query);
    Assert.AreEqual(new object[] { 100.00m }, parameters);
}
```

## Best Practices

### 1. Follow Established Patterns
- Use consistent naming conventions across your components
- Inherit from base classes (`BaseRuleTransformer`) when possible
- Follow the same error handling patterns as built-in components

### 2. Handle Edge Cases
- Always validate input parameters
- Consider null values and empty collections
- Test with special characters in field names

### 3. Provide Clear Documentation
- Document your custom operators and their expected input formats
- Include usage examples in your documentation
- Specify any limitations or requirements

### 4. Test Thoroughly
- Write unit tests for each custom component
- Include integration tests with FilterBuilder
- Test with various data types and edge cases

## See Also

- **[Main Project Documentation](../../README.md)** - Overview and getting started
- **[Database Providers Documentation](../../docs/providers.md)** - Complete provider reference
- **[Type Conversion Documentation](../../docs/type-conversion.md)** - Type conversion system
- **[Rule Transformers Documentation](../../docs/rule-transformers.md)** - Rule transformer architecture
- **[SQL Server Provider](../Q.FilterBuilder.SqlServer/README.md)** - Example provider implementation
