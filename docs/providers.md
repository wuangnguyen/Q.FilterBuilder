# Database Providers Documentation

Database providers handle database-specific formatting and syntax for Q.FilterBuilder. Each provider implements the `IQueryFormatProvider` interface to provide database-specific formatting rules for parameters, field names, and logical operators.

## Overview

The provider system enables Q.FilterBuilder to generate queries for different databases while maintaining a consistent API. Each provider handles:

- **Parameter formatting** (e.g., `@p0` for SQL Server, `$1` for PostgreSQL)
- **Field name formatting** (e.g., `[Field]` for SQL Server, `"Field"` for PostgreSQL)
- **Logical operator representation** (e.g., `AND`/`OR` for SQL, `&&`/`||` for LINQ)

## Core Interface

### IQueryFormatProvider

The base interface that all database providers must implement:

```csharp
public interface IQueryFormatProvider
{
    string ParameterPrefix { get; }
    string AndOperator { get; }
    string OrOperator { get; }

    string FormatFieldName(string fieldName);
    string FormatParameterName(int parameterIndex);
}
```

**Properties:**
- `ParameterPrefix`: The character used to prefix parameters (e.g., "@", "$", "?")
- `AndOperator`: The logical AND operator representation
- `OrOperator`: The logical OR operator representation

**Methods:**
- `FormatFieldName`: Formats field names with database-specific delimiters
- `FormatParameterName`: Generates parameter names based on index

## Built-in Providers

### SQL Server Provider

Provides SQL Server-specific formatting with square brackets for field names and @ prefixed parameters.

```csharp
public class SqlServerFormatProvider : IQueryFormatProvider
{
    public string ParameterPrefix => "@";
    public string AndOperator => "AND";
    public string OrOperator => "OR";

    public string FormatFieldName(string fieldName)
    {
        return $"[{fieldName}]";
    }

    public string FormatParameterName(int parameterIndex)
    {
        return $"{ParameterPrefix}p{parameterIndex}";
    }
}
```

**Package:** `Q.FilterBuilder.SqlServer`

**Usage:**
```csharp
services.AddSqlServerFilterBuilder();

// Generates: [Name] = @p0 AND [Age] > @p1
```

**Features:**
- Square bracket field delimiters: `[FieldName]`
- Named parameters: `@p0`, `@p1`, `@p2`
- Standard SQL logical operators: `AND`, `OR`

### MySQL Provider

Provides MySQL-specific formatting with backticks for field names and positional parameters.

```csharp
public class MySqlFormatProvider : IQueryFormatProvider
{
    public string ParameterPrefix => "?";
    public string AndOperator => "AND";
    public string OrOperator => "OR";

    public string FormatFieldName(string fieldName)
    {
        return $"`{fieldName}`";
    }

    public string FormatParameterName(int parameterIndex)
    {
        return ParameterPrefix; // MySQL uses positional parameters
    }
}
```

**Package:** `Q.FilterBuilder.MySql`

**Usage:**
```csharp
services.AddMySqlFilterBuilder();

// Generates: `Name` = ? AND `Age` > ?
```

**Features:**
- Backtick field delimiters: `` `FieldName` ``
- Positional parameters: `?` (all parameters use the same placeholder)
- Standard SQL logical operators: `AND`, `OR`

### PostgreSQL Provider

Provides PostgreSQL-specific formatting with double quotes for field names and numbered parameters.

```csharp
public class PostgreSqlFormatProvider : IQueryFormatProvider
{
    public string ParameterPrefix => "$";
    public string AndOperator => "AND";
    public string OrOperator => "OR";

    public string FormatFieldName(string fieldName)
    {
        return $"\"{fieldName}\"";
    }

    public string FormatParameterName(int parameterIndex)
    {
        return $"{ParameterPrefix}{parameterIndex + 1}"; // PostgreSQL uses $1, $2, etc.
    }
}
```

**Package:** `Q.FilterBuilder.PostgreSql`

**Usage:**
```csharp
services.AddPostgreSqlFilterBuilder();

// Generates: "Name" = $1 AND "Age" > $2
```

**Features:**
- Double quote field delimiters: `"FieldName"`
- Numbered parameters: `$1`, `$2`, `$3` (1-based indexing)
- Standard SQL logical operators: `AND`, `OR`

### LINQ Provider

Provides LINQ expression formatting for use with Entity Framework and in-memory collections.

```csharp
public class LinqFormatProvider : IQueryFormatProvider
{
    public string ParameterPrefix => "";
    public string AndOperator => "&&";
    public string OrOperator => "||";

    public string FormatFieldName(string fieldName) => fieldName;

    public string FormatParameterName(int parameterIndex) => $"@p{parameterIndex}";
}
```

**Package:** `Q.FilterBuilder.Linq`

**Usage:**
```csharp
services.AddLinqFilterBuilder();

// Generates: Name == @p0 && Age > @p1
```

**Features:**
- No field delimiters: `FieldName`
- Named parameters: `@p0`, `@p1`, `@p2`
- C# logical operators: `&&`, `||`

## Provider Comparison

| Provider | Package | Field Format | Parameters | Logical Ops | Use Case |
|----------|---------|--------------|------------|-------------|----------|
| **SQL Server** | `Q.FilterBuilder.SqlServer` | `[Field]` | `@p0, @p1` | `AND, OR` | SQL Server databases |
| **MySQL** | `Q.FilterBuilder.MySql` | `` `Field` `` | `?, ?` | `AND, OR` | MySQL databases |
| **PostgreSQL** | `Q.FilterBuilder.PostgreSql` | `"Field"` | `$1, $2` | `AND, OR` | PostgreSQL databases |
| **LINQ** | `Q.FilterBuilder.Linq` | `Field` | `@p0, @p1` | `&&, \|\|` | Entity Framework, in-memory |

## Installation and Setup

### Package Installation

```bash
# Choose your database provider
dotnet add package Q.FilterBuilder.SqlServer
# OR
dotnet add package Q.FilterBuilder.MySql
# OR
dotnet add package Q.FilterBuilder.PostgreSql
# OR
dotnet add package Q.FilterBuilder.Linq
```

### Service Registration

```csharp
// SQL Server
services.AddSqlServerFilterBuilder();

// MySQL
services.AddMySqlFilterBuilder();

// PostgreSQL
services.AddPostgreSqlFilterBuilder();

// LINQ
services.AddLinqFilterBuilder();
```

### Advanced Configuration

```csharp
// With custom type converters
services.AddSqlServerFilterBuilder(typeConversion =>
{
    typeConversion.RegisterConverter("currency", new CurrencyConverter());
});

// With custom rule transformers
services.AddSqlServerFilterBuilder(
    typeConversion => { /* type converters */ },
    ruleTransformers =>
    {
        ruleTransformers.RegisterTransformer("fulltext", new FullTextTransformer());
    });
```

## Creating Custom Providers

### Basic Custom Provider

```csharp
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
```

### Extension Method for Custom Provider

```csharp
public static class OracleServiceCollectionExtensions
{
    public static IServiceCollection AddOracleFilterBuilder(this IServiceCollection services)
    {
        return services.AddFilterBuilder(new OracleFormatProvider());
    }

    public static IServiceCollection AddOracleFilterBuilder(
        this IServiceCollection services,
        Action<ITypeConversionService> configureTypeConversion)
    {
        return services.AddFilterBuilder(new OracleFormatProvider(), configureTypeConversion);
    }

    public static IServiceCollection AddOracleFilterBuilder(
        this IServiceCollection services,
        Action<ITypeConversionService> configureTypeConversion,
        Action<IRuleTransformerService> configureRuleTransformers)
    {
        return services.AddFilterBuilder(
            new OracleFormatProvider(),
            configureTypeConversion,
            configureRuleTransformers);
    }
}
```

### Usage of Custom Provider

```csharp
// Register the custom provider
services.AddOracleFilterBuilder();

// Use with FilterBuilder
var (query, parameters) = filterBuilder.Build(group);
// Generates: NAME = :p0 AND AGE > :p1
```

## Provider Characteristics

### SQL Server
- **Parameter Style**: Named parameters (`@p0`, `@p1`)
- **Field Delimiters**: Square brackets `[FieldName]`
- **Best For**: Enterprise applications using SQL Server

### MySQL
- **Parameter Style**: Positional parameters (all use `?`)
- **Field Delimiters**: Backticks `` `FieldName` ``
- **Best For**: Web applications, cross-platform deployments

### PostgreSQL
- **Parameter Style**: Numbered parameters (`$1`, `$2`) with 1-based indexing
- **Field Delimiters**: Double quotes `"FieldName"`
- **Best For**: Applications requiring advanced data types and JSON operations

### LINQ
- **Parameter Style**: Named parameters (`@p0`, `@p1`)
- **Field Delimiters**: None (direct field names)
- **Logical Operators**: C# operators (`&&`, `||`)
- **Best For**: Entity Framework applications, in-memory filtering

## Testing Custom Providers

### Basic Provider Tests

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
public void OracleFormatProvider_ShouldFormatParametersCorrectly()
{
    // Arrange
    var provider = new OracleFormatProvider();

    // Act
    var result = provider.FormatParameterName(0);

    // Assert
    Assert.AreEqual(":p0", result);
}
```

### Integration Tests

```csharp
[Test]
public void FilterBuilder_WithOracleProvider_ShouldGenerateCorrectQuery()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddOracleFilterBuilder();
    var serviceProvider = services.BuildServiceProvider();
    var filterBuilder = serviceProvider.GetRequiredService<IFilterBuilder>();

    var group = new FilterGroup("AND");
    group.Rules.Add(new FilterRule("Name", "equal", "John"));

    // Act
    var (query, parameters) = filterBuilder.Build(group);

    // Assert
    Assert.AreEqual("NAME = :p0", query);
    Assert.AreEqual(new[] { "John" }, parameters);
}
```

## See Also

- **[Rule Transformers Documentation](rule-transformers.md)** - Provider-specific rule transformers
- **[Type Conversion Documentation](type-conversion.md)** - Type conversion system
- **[Core Package Documentation](../src/Q.FilterBuilder.Core/README.md)** - Manual provider setup
- **[Main Project Documentation](../README.md)** - Getting started guide
