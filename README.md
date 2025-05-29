# Q.FilterBuilder

[![.NET Standard](https://img.shields.io/badge/.NET%20Standard-2.1-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A powerful, flexible, and extensible .NET library for building dynamic and complex filter conditions based on runtime data.

## 🚀 Why Q.FilterBuilder is Powerful

### 🔧 **Unlimited Extensibility**
- **Custom Rule Transformers**: Add new operators like `fulltext`, `json_contains`, `array_contains`
- **Custom Type Converters**: Handle specialized data types (currency, phone, UUID, enums)
- **Custom Database Providers**: Support multiple databases with custom formatting rules
- **Runtime Registration**: Add extensions without recompiling

### 🌐 **Universal UI Integration**
- **[jQuery QueryBuilder](https://querybuilder.js.org/)**: Direct JSON conversion support
- **[React QueryBuilder](https://react-querybuilder.js.org/)**: Configurable property mapping
- **Any JSON Query Builder**: Fully customizable property names and structure

### 🏗️ **Enterprise-Grade Architecture**
- **SOLID Principles**: Clean separation of concerns, dependency injection ready
- **SQL Injection Protection**: Automatic parameterization
- **Complex Logic**: Unlimited nested groups with AND/OR combinations

### ⚡ **Developer Experience**
- **Fluent API**: Readable, IntelliSense-friendly syntax
- **Two Building Flows**: Direct code creation OR JSON conversion
- **Automatic Type Conversion**: Smart handling of strings, dates, collections
- **Rich Documentation**: Provider-specific guides and examples

## 📦 Packages

The library is organized into several NuGet packages:

| Package | Description | Use Case |
|---------|-------------|----------|
| `Q.FilterBuilder.Core` | Core functionality and base classes | Foundation for all implementations |
| `Q.FilterBuilder.SqlServer` | SQL Server specific database provider | SQL Server database queries |
| `Q.FilterBuilder.MySql` | MySQL specific database provider | MySQL database queries |
| `Q.FilterBuilder.PostgreSql` | PostgreSQL specific database provider | PostgreSQL database queries |
| `Q.FilterBuilder.Linq` | LINQ expression database provider | In-memory filtering and Entity Framework |
| `Q.FilterBuilder.JsonConverter` | JSON to FilterGroup conversion | Web APIs and jQuery QueryBuilder integration |

## 🔄 How It Works

### Query Building Flows

**Flow 1: Direct FilterGroup Creation**
1. **Create**: Build `FilterGroup` with `FilterRule` conditions directly
2. **Transform**: Rule transformers convert rules to database-specific queries
3. **Format**: Database providers handle field/parameter formatting
4. **Output**: Generated SQL/LINQ with parameterized values

**Flow 2: JSON to FilterGroup**
1. **Parse**: Convert jQuery QueryBuilder JSON to `FilterGroup`
2. **Transform**: Rule transformers convert rules to database-specific queries
3. **Format**: Database providers handle field/parameter formatting
4. **Output**: Generated SQL/LINQ with parameterized values

### Component Responsibilities
- **Database Providers**: Handle database-specific formatting and syntax
- **Rule Transformers**: Convert filter rules to query conditions (replaces old operator system)
- **Type Conversion**: Ensure values match expected data types
- **FilterBuilder**: Orchestrates the entire building process

## 🔧 Database Providers

Choose the provider that matches your database:

| Provider | Package | Field Format | Parameters | Use Case |
|----------|---------|--------------|------------|----------|
| **SQL Server** | `Q.FilterBuilder.SqlServer` | `[Field]` | `@p0, @p1` | SQL Server databases |
| **MySQL** | `Q.FilterBuilder.MySql` | `` `Field` `` | `?, ?` | MySQL databases |
| **PostgreSQL** | `Q.FilterBuilder.PostgreSql` | `"Field"` | `$1, $2` | PostgreSQL databases |
| **LINQ** | `Q.FilterBuilder.Linq` | `Field` | `@p0, @p1` | Entity Framework, in-memory |

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

// 2. Build filters using FluentRuleBuilder (recommended)
var group = new FluentRuleBuilder()
    .Where("Age", "greater_or_equal", 18)
    .Where("Name", "contains", "John")
    .Where("Status", "in", new[] { "Active", "Pending" })
    .Where("CreatedDate", "between", new[] { "2023-01-01", "2023-12-31" })
    .BeginGroup("OR")
        .Where("Department", "equal", "IT")
        .Where("Role", "equal", "Admin")
    .EndGroup()
    .Where("Email", "is_not_null")
    .Build();

// OR build filters manually

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

// 3. Generate query
var (query, parameters) = filterBuilder.Build(group);
// Result: "[Age] >= @p0 AND [Name] LIKE '%' + @p1 + '%' AND [Status] IN (@p2, @p3)
//          AND [CreatedDate] BETWEEN @p4 AND @p5 AND ([Department] = @p6 OR [Role] = @p7)
//          AND [Email] IS NOT NULL"
```

📖 **Complete Reference**: [Core Package Guide](src/Q.FilterBuilder.Core/README.md)

## 🎯 Supported Rule Transformers

| Rule Transformer | Description | Example |
|------------------|-------------|---------|
| `equal`, `not_equal` | Equality/inequality | `Age = 25` |
| `greater`, `less` | Comparisons | `Price > 100` |
| `between`, `not_between` | Range checks | `Date BETWEEN '2023-01-01' AND '2023-12-31'` |
| `in`, `not_in` | Collection membership | `Status IN ('Active', 'Pending')` |
| `contains`, `begins_with`, `ends_with` | String operations | `Name LIKE '%John%'` |
| `is_null`, `is_not_null` | Null checks | `Email IS NOT NULL` |

📖 **Complete Reference**: [Rule Transformers Guide](src/Q.FilterBuilder.Core/README.md#rule-transformers)

## 🧪 Testing

Build and test the solution:

```bash
# Build all packages
dotnet build FilterBuilder.sln

# Run all tests
dotnet test FilterBuilder.sln
```

📖 **Testing Guide**: [Test Documentation](src/Q.FilterBuilder.Core/test/)

## 📚 Documentation

For detailed implementation guidance, refer to the individual project README files:

- **[Core Package](../Q.FilterBuilder.Core/README.md)** - Foundation components and manual setup
- **[SQL Server Provider](../Q.FilterBuilder.SqlServer/README.md)** - SQL Server specific implementation
- **[MySQL Provider](../Q.FilterBuilder.MySql/README.md)** - MySQL specific implementation
- **[PostgreSQL Provider](../Q.FilterBuilder.PostgreSql/README.md)** - PostgreSQL specific implementation
- **[LINQ Provider](../Q.FilterBuilder.Linq/README.md)** - LINQ expressions and Entity Framework
- **[JSON Converter](../Q.FilterBuilder.JsonConverter/README.md)** - jQuery QueryBuilder integration

## 🔌 Extensions

### JSON Integration with Popular UI Libraries

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

### Custom Extensions
- **Custom Rule Transformers**: Add new operators
- **Custom Database Providers**: Support new databases
- **Custom Type Converters**: Handle specialized data types

📖 **Extension Guides**: [Core Extensions](src/Q.FilterBuilder.Core/README.md#extending)

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

```bash
# Clone the repository
git clone https://github.com/wuangnguyen/Q.FilterBuilder.git
cd Q.FilterBuilder

# Build and test
dotnet build FilterBuilder.sln
dotnet test FilterBuilder.sln
```

- Follow C# coding conventions and SOLID, KISS principles
- Add XML documentation for public APIs
- Ensure all tests pass before submitting PRs

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Made with ❤️ by Quang Nguyen**

*Q.FilterBuilder - Powerful, flexible, and extensible dynamic query building for .NET*
