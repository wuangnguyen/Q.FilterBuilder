
[![banner](banner.png)](https://github.com/wuangnguyen/Q.FilterBuilder)

---

[![.NET Standard](https://img.shields.io/badge/.NET%20Standard-2.1-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)
[![Cross Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux%20%7C%20macOS-blue.svg)](https://dotnet.microsoft.com/)
[![Multiple Databases](https://img.shields.io/badge/Supported%20Databases-SQL%20Server%20%7C%20MySQL%20%7C%20PostgreSQL-blue.svg)](#-packages)
[![Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/wuangnguyen/0ad369a5370256450204a3f97397cb22/raw/filter-builder-code-coverage.json)](https://github.com/wuangnguyen/Q.FilterBuilder)

Q.FilterBuilder is a powerful, flexible, and extensible .NET library that simplifies the process of building dynamic `WHERE` clauses for your data queries. It provides a fluent API that allows you to create complex filtering rules with ease. The library is designed to be highly extensible, allowing you to add custom type converters, rule transformers, and database providers to match your specific business logic. It also offers built-in support for popular UI query builders like jQuery QueryBuilder and React QueryBuilder, making it easy to integrate with your existing applications.

Give it a ⭐ if you find it useful! ❤️❤️❤️

## 🚀 Why Q.FilterBuilder is Powerful?

### 🛸 **Universal UI Integration**
- Compatible with universal UI query builders like **[jQuery QueryBuilder](https://querybuilder.js.org/)** - the most popular JavaScript query builder and **[React QueryBuilder](https://react-querybuilder.js.org/)** - modern React-based query builder
- Direct JSON conversion support and configurable property mapping

### ⚡ **Extensible Architecture & Developer Experience**
- **Fully Extensible**: Easily add custom type converters, rule transformers, and database providers to match your business logic. Offers complete customization.
- **SOLID Principles**: Clean separation of concerns, dependency injection ready
- **Developer Experience**: Clean, fluent API, and SQL injection protection

### 🌏 Multiple Platform Support
- **.NET Framework 4.6.1+** - Windows-based .NET Framework
- **.NET Core 3.1+** - Cross-platform .NET Core
- **.NET 5.0+** - Modern cross-platform .NET
- **Mono** - Cross-platform open-source implementation

### 📎 Multiple ORM Support
- **Entity Framework**: Full support for LINQ and database queries
- **Dapper**: Dynamic SQL execution with micro-ORM capabilities
- **ADO.NET**: Low-level database access for maximum flexibility
- **Any Other ORM**: Any ORM that can execute raw SQL queries

## 🚀 Quick Start

### Installation

```bash
# Install your database provider (includes Core automatically)
dotnet add package Q.FilterBuilder.SqlServer
# OR dotnet add package Q.FilterBuilder.MySql
# OR dotnet add package Q.FilterBuilder.PostgreSql
# OR dotnet add package Q.FilterBuilder.Linq

# Optional: JSON support
dotnet add package Q.FilterBuilder.JsonConverter
```

### Basic Usage

```csharp
using Q.FilterBuilder.Core;
using Q.FilterBuilder.SqlServer.Extensions;

// 1. Register with DI
services.AddSqlServerFilterBuilder();

// If you want to register your custom type converters and rule transformers, you can do so like this:
// services.AddSqlServerFilterBuilder(options => options
//     .ConfigureTypeConversion(tc => {
//         tc.RegisterConverter("custom", new CustomConverter());
//         tc.RegisterConverter("custom2", new CustomConverter2());
//         tc.RegisterConverter("custom3", new CustomConverter3());
//     }))
//     .ConfigureRuleTransformers(rt => {
//         rt.RegisterTransformer("custom_op", new CustomTransformer());
//         rt.RegisterTransformer("custom_op2", new CustomTransformer2());
//         rt.RegisterTransformer("custom_op3", new CustomTransformer3());
//     }));

// 2. Build filter rules

// Using JSON converter (this requires the additional Q.FilterBuilder.JsonConverter package)
var json = "<json string from UI>";
var group = JsonSerializer.Deserialize<FilterGroup>(json, new JsonSerializerOptions
{
    Converters = { new QueryBuilderConverter() }
});

// OR using built-in fluent API rule builder
// var group = new FluentRuleBuilder()
//     .Where("Age", "greater_or_equal", 18)
//     .Where("Name", "contains", "John")
//     .Where("Status", "in", new[] { "Active", "Pending" })
//     .Where("CreatedDate", "between", new[] { "2023-01-01", "2023-12-31" })
//     .BeginGroup("OR")
//         .Where("Department", "equal", "IT")
//         .Where("Role", "equal", "Admin")
//     .EndGroup()
//     .Where("Email", "is_not_null")
//     .Build();

// OR
// var group = new FilterGroup("AND");
// group.Rules.Add(new FilterRule("Age", "greater_or_equal", 18));
// group.Rules.Add(new FilterRule("Name", "contains", "John"));
// group.Rules.Add(new FilterRule("Status", "in", new[] { "Active", "Pending" }));
// group.Rules.Add(new FilterRule("CreatedDate", "between", new[] { "2023-01-01", "2023-12-31" }));

// var nestedGroup = new FilterGroup("OR");
// nestedGroup.Rules.Add(new FilterRule("Department", "equal", "IT"));
// nestedGroup.Rules.Add(new FilterRule("Role", "equal", "Admin"));

// group.Groups.Add(nestedGroup);
// group.Rules.Add(new FilterRule("Email", "is_not_null"));



// 3. Generate where clause
var (whereClause, parameters) = _filterBuilder.BuildForEf(group); // see more example for other ORMs in the intergration tests project

// whereClause: "[Age] >= {0} AND [Name] LIKE '%' + {1} + '%' AND [Status] IN ({2}, {3})
//          AND [CreatedDate] BETWEEN {4} AND {5} AND ([Department] = {6} OR [Role] = {7})
//          AND [Email] IS NOT NULL"
// parameters: [18, "John", "Active", "Pending", "2023-01-01", "2023-12-31", "IT", "Admin"]

// 4. Execute query
var sql = $"SELECT * FROM Users WHERE {whereClause}";
var users = await _context.Set<User>().FromSqlRaw(sql, parameters).ToListAsync()

// 5. Done!
```

📖 **Complete Reference**: [Core Package Guide](src/Q.FilterBuilder.Core/README.md)

## 📦 Packages

The library is organized into several NuGet packages:

| Package | Description | Use Case |
|---------|-------------|----------|
| `Q.FilterBuilder.Core` | Core functionality and base classes | Foundation for all implementations |
| `Q.FilterBuilder.SqlServer` | SQL Server specific database provider | SQL Server database queries |
| `Q.FilterBuilder.MySql` | MySQL specific database provider | MySQL database queries |
| `Q.FilterBuilder.PostgreSql` | PostgreSQL specific database provider | PostgreSQL database queries |
| `Q.FilterBuilder.Linq` | LINQ expression database provider | Integrate with [Dynamic LINQ](https://dynamic-linq.net/) |
| `Q.FilterBuilder.JsonConverter` | JSON to FilterGroup conversion | Web APIs and jQuery QueryBuilder integration |

## 🎯 Built-In Operators, Plus Support For Your Own Custom Logic

| Rule Transformer | Description | Example |
|------------------|-------------|---------|
| `equal`, `not_equal` | Equality/inequality | `Age = 25` |
| `greater`, `less`, `greater_or_equal`, `less_or_equal` | Comparisons | `Price > 100`, `Age >= 18` |
| `between`, `not_between` | Range checks | `Date BETWEEN '2023-01-01' AND '2023-12-31'` |
| `in`, `not_in` | Collection membership | `Status IN ('Active', 'Pending')` |
| `contains`, `begins_with`, `ends_with` | String operations | `Name LIKE '%John%'` |
| `is_null`, `is_not_null` | Null checks | `Email IS NOT NULL` |
| `date_diff` | Date difference | `DATEDIFF(day, CreatedDate, GETDATE()) = 30` |

📖 **Complete Reference**: [Rule Transformers Guide](src/Q.FilterBuilder.Core/README.md#custom-rule-transformers)

## 📚 Documentation

### Core Features Documentation

Comprehensive guides for Q.FilterBuilder's core features and capabilities:

- **[FluentRuleBuilder](docs/fluent-rule-builder.md)** - Complete API reference for the fluent rule building system with examples and best practices
- **[Type Conversion](docs/type-conversion.md)** - Type conversion system, built-in converters, custom converters, and collection handling
- **[Rule Transformers](docs/rule-transformers.md)** - Rule transformer architecture, built-in transformers, and creating custom transformers
- **[Database Providers](docs/providers.md)** - All database providers, provider architecture, and creating custom providers
- **[Helpers & Utilities](docs/helpers.md)** - DateTimeHelper, extension methods, and utility functions

### Provider-Specific Documentation

Detailed implementation guides for each database provider:

- **[Core Package](src/Q.FilterBuilder.Core/README.md)** - Foundation components and manual setup
- **[SQL Server Provider](src/Q.FilterBuilder.SqlServer/README.md)** - SQL Server specific implementation
- **[MySQL Provider](src/Q.FilterBuilder.MySql/README.md)** - MySQL specific implementation
- **[PostgreSQL Provider](src/Q.FilterBuilder.PostgreSql/README.md)** - PostgreSQL specific implementation
- **[LINQ Provider](src/Q.FilterBuilder.Linq/README.md)** - LINQ expressions and Entity Framework
- **[JSON Converter](src/Q.FilterBuilder.JsonConverter/README.md)** - jQuery QueryBuilder integration

## 🔌 JSON Integration with Popular UI Libraries

**Supported Query Builder Libraries:**
- **[jQuery QueryBuilder](https://querybuilder.js.org/)** - The most popular JavaScript query builder
- **[React QueryBuilder](https://react-querybuilder.js.org/)** - Modern React-based query builder
- **Any Custom JSON Query Builder** - Fully configurable property mapping

```csharp
using Q.FilterBuilder.JsonConverter;

// jQuery QueryBuilder (default configuration)
var jqueryOptions = new JsonSerializerOptions
{
    Converters = { new QueryBuilderConverter() }
};

// React QueryBuilder (custom property names)
var reactOptions = new QueryBuilderOptions
{
    ConditionPropertyName = "combinator",  // "combinator" instead of "condition"
    RulesPropertyName = "children",        // "children" instead of "rules"
    FieldPropertyName = "id"               // "id" instead of "field"
};
var reactJsonOptions = new JsonSerializerOptions
{
    Converters = { new QueryBuilderConverter(reactOptions) }
};

var group = JsonSerializer.Deserialize<FilterGroup>(json, jqueryOptions);
```

📖 **JSON Guide**: [JSON Converter Documentation](src/Q.FilterBuilder.JsonConverter/README.md)

## 🧪 Testing

Q.FilterBuilder includes comprehensive integration tests that validate the complete workflow from JSON input to database execution:

### Integration Test Features
- **Multi-Provider Testing**: SQL Server, MySQL, PostgreSQL using Docker containers
- **Multi-ORM Support**: Entity Framework, Dapper, ADO.NET compatibility
- **Real Database Execution**: Tests against actual database instances via Testcontainers
- **Complete Workflow Validation**: JSON → FilterGroup → SQL → Database Results

### Running Integration Tests

```bash
# Requires Docker Desktop running
dotnet test test/Q.FilterBuilder.IntegrationTests/

# Test specific provider
$env:DatabaseProvider="SqlServer"
dotnet test test/Q.FilterBuilder.IntegrationTests/
```

📖 **Integration Test Guide**: [Integration Tests Documentation](test/Q.FilterBuilder.IntegrationTests/README.md)

## 🤝 Contributing

Contributions are welcome! If you'd like to contribute, please follow these steps:

1.  Fork the repository.
2.  Create a new branch for your feature or bug fix.
3.  Make your changes and commit them with a descriptive message.
4.  Push your changes to your fork.
5.  Create a pull request to the main repository.

Please ensure that your code adheres to the existing coding style and that all tests pass before submitting a pull request.

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.