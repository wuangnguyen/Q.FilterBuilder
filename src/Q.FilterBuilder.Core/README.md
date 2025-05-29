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
services.AddFilterBuilder(new SqlServerProvider());

// Or register with custom services
services.AddFilterBuilder(
    new SqlServerProvider(),
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
services.AddFilterBuilder(new SqlServerProvider());
```

#### 2. With Custom Rule Transformer Service

```csharp
using Q.FilterBuilder.Core.RuleTransformers;

var customRuleTransformerService = new RuleTransformerService();
customRuleTransformerService.RegisterTransformer("custom_op", new CustomTransformer());

services.AddFilterBuilder(new SqlServerProvider(), customRuleTransformerService);
```

#### 3. With Configuration Actions

```csharp
services.AddFilterBuilder(
    new SqlServerProvider(),
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

### Custom Query Syntax Provider

Create your own query syntax provider by implementing `IQuerySyntaxProvider`:

```csharp
using Q.FilterBuilder.Core.Providers;

public class CustomQuerySyntaxProvider : IQuerySyntaxProvider
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
services.AddFilterBuilder(new CustomDatabaseProvider());
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
    new SqlServerProvider(),
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
    new PostgreSqlProvider(),
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

// Register query syntax provider
services.AddSingleton<IQuerySyntaxProvider, SqlServerProvider>();

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

## Creating a Custom Provider Package

If you're creating a provider package for a new database:

1. **Create the provider class**:
```csharp
public class MyQuerySyntaxProvider : IQuerySyntaxProvider
{
    // Implement interface methods
}
```

2. **Create extension methods**:
```csharp
public static class MyDatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddMyDatabaseFilterBuilder(this IServiceCollection services)
    {
        return services.AddFilterBuilder(new MyQuerySyntaxProvider());
    }
}
```

3. **Reference the core package**:
```xml
<PackageReference Include="Q.FilterBuilder.Core" Version="1.0.0" />
```

## Complete Custom Provider Example

Here's a complete example of creating a PostgreSQL provider:

```csharp
// 1. Create the provider
public class PostgreSqlProvider : IQuerySyntaxProvider
{
    public string ParameterPrefix => "$";
    public string AndOperator => "AND";
    public string OrOperator => "OR";

    public string FormatFieldName(string fieldName)
    {
        return $"\"{fieldName}\""; // PostgreSQL uses double quotes
    }

    public string FormatParameterName(int parameterIndex)
    {
        return $"${parameterIndex + 1}"; // PostgreSQL uses $1, $2, etc.
    }
}

// 2. Create extension methods
public static class PostgreSqlServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlFilterBuilder(this IServiceCollection services)
    {
        return services.AddFilterBuilder(new PostgreSqlProvider());
    }

    public static IServiceCollection AddPostgreSqlFilterBuilder(
        this IServiceCollection services,
        Action<ITypeConversionService> configureTypeConversion)
    {
        return services.AddFilterBuilder(
            new PostgreSqlProvider(),
            configureTypeConversion);
    }
}

// 3. Usage
services.AddPostgreSqlFilterBuilder();
```

## Testing Your Custom Components

### Testing Custom Providers

```csharp
[Test]
public void PostgreSqlProvider_ShouldFormatFieldsCorrectly()
{
    // Arrange
    var provider = new PostgreSqlProvider();

    // Act
    var result = provider.FormatFieldName("UserName");

    // Assert
    Assert.AreEqual("\"UserName\"", result);
}

[Test]
public void PostgreSqlProvider_ShouldFormatParametersCorrectly()
{
    // Arrange
    var provider = new PostgreSqlProvider();

    // Act
    var result = provider.FormatParameterName(0);

    // Assert
    Assert.AreEqual("$1", result);
}
```

### Testing Custom Converters

```csharp
[Test]
public void CustomDateConverter_ShouldConvertValidDate()
{
    // Arrange
    var converter = new CustomDateConverter();

    // Act
    var result = converter.Convert("31/12/2023", null);

    // Assert
    Assert.AreEqual(new DateTime(2023, 12, 31), result);
}

[Test]
public void CustomDateConverter_ShouldThrowForInvalidDate()
{
    // Arrange
    var converter = new CustomDateConverter();

    // Act & Assert
    Assert.Throws<InvalidOperationException>(() =>
        converter.Convert("invalid-date", null));
}
```

### Integration Testing

```csharp
[Test]
public void FilterBuilder_WithCustomProvider_ShouldGenerateCorrectQuery()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddFilterBuilder(new PostgreSqlProvider());
    var serviceProvider = services.BuildServiceProvider();
    var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

    var group = new FilterGroup("AND");
    group.Rules.Add(new FilterRule("Name", "equal", "John"));

    // Act
    var (query, parameters) = filterBuilder.Build(group);

    // Assert
    Assert.AreEqual("\"Name\" = $1", query);
    Assert.AreEqual(new[] { "John" }, parameters);
}
```

## Best Practices

### Provider Development

1. **Follow Naming Conventions**: Use consistent naming for your provider classes
2. **Handle Edge Cases**: Consider special characters in field names
3. **Document Parameter Format**: Clearly document how parameters are formatted
4. **Test Thoroughly**: Test with various data types and edge cases

### Type Converter Development

1. **Handle Null Values**: Always check for null input values
2. **Provide Clear Error Messages**: Throw descriptive exceptions for invalid conversions
3. **Use Metadata**: Leverage metadata for configuration options
4. **Be Consistent**: Follow consistent patterns across converters

### Rule Transformer Development

1. **Validate Input**: Check for required values and throw appropriate exceptions
2. **Use Base Classes**: Inherit from `BaseRuleTransformer` when possible
3. **Handle Collections**: Consider how your transformer handles array/collection values
4. **Generate Valid SQL**: Ensure your output is valid for your target database

## See Also

- [SQL Server Provider Documentation](../Q.FilterBuilder.SqlServer/README.md)
- [Main Project Documentation](../../README.md)
