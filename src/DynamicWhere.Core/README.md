# DynamicWhere.Core Library Summary

DynamicWhere.Core is a flexible library for building dynamic WHERE clauses in SQL queries. It provides a type-safe way to construct complex query conditions based on runtime data.

## Key Components

- `DynamicWhereBuilder`: Builds WHERE clauses from `DynamicGroup` inputs.
- `DateTimeHelper`: Parses date and time strings.
- `TypeConversionHelper`: Handles type conversion for various data types.

## Core Classes

### DynamicWhereBuilder
- Implements `IDynamicWhereBuilder`
- Builds dynamic WHERE clauses based on `DynamicGroup` input
- Uses `IOperatorProvider` for operator resolution

### IDynamicWhereBuilder
- Interface for dynamic WHERE clause builders
- Defines `Build` method to generate query and parameters

## Models

### DynamicGroup
- Represents a group of rules and sub-groups in a dynamic query
- Contains `Rules`, `Groups`, and `Condition` properties

### DynamicRule
- Represents a single rule in a dynamic query
- Properties: `FieldName`, `Operator`, `Value`, `Type`, and `Data`

## Operators

### BaseOperator
- Abstract base class for operators
- Implements `IOperator` interface
- Provides default implementation for `GetParametersPart`

### IOperator
- Interface for query operators
- Defines methods for generating query parts and parameters

### SimpleOperator
- Concrete implementation of `BaseOperator`
- Handles simple comparison operators (e.g., ==, !=, <, <=, >, >=)

## Providers

### BaseOperatorProvider
- Implements `IOperatorProvider`
- Provides a default set of operators
- Allows adding custom operators

### IOperatorProvider
- Interface for operator providers
- Defines methods for getting operators and adding custom operators

## Helpers

### DateTimeHelper
- Static class for parsing date and time strings
- Supports various date formats and custom parsing

### TypeConversionHelper
- Static class for type conversion operations
- Handles conversion of values to specified types
- Supports collection and single value conversions

## Key Features
1. Flexible query building with support for nested groups and rules.
2. Extensible operator system.
3. Type conversion and datetime parsing utilities.
4. Parameterized query generation for security.

This library provides a robust framework for building dynamic WHERE clauses in SQL queries, with support for complex conditions, custom operators, and type conversions.

# DynamicWhereBuilder Usage Guide

## Introduction

The DynamicWhereBuilder is a flexible and powerful tool for constructing dynamic WHERE clauses in SQL queries. It allows you to build complex query conditions based on runtime data without writing raw SQL strings.

## Getting Started

1. Add the DynamicWhere.Core namespace to your project.
2. Create an instance of `DynamicWhereBuilder`.

```csharp
using DynamicWhere.Core;
using DynamicWhere.Core.Models;
using DynamicWhere.Core.Providers;
var builder = new DynamicWhereBuilder();
```

## Building a Query

### 1. Create a DynamicGroup

A `DynamicGroup` represents a set of conditions that are combined with a logical operator (AND/OR).

```csharp
var group = new DynamicGroup
{
    Condition = "AND",
    Rules = new List<DynamicRule>(),
    Groups = new List<DynamicGroup>()
};
```

### 2. Add DynamicRules

A `DynamicRule` represents a single condition in the WHERE clause.

```csharp
group.Rules.Add(new DynamicRule
{
    FieldName = "Age",
    Operator = "greater_or_equal",
    Value = 18,
    Type = "int"
});
group.Rules.Add(new DynamicRule
{
    FieldName = "Name",
    Operator = "equal",
    Value = "John",
    Type = "string"
});
```

### 3. Add Nested Groups (Optional)

You can create nested conditions by adding sub-groups to the main group.

```csharp
var subGroup = new DynamicGroup
{
    Condition = "OR",
    Rules = new List<DynamicRule>
    {
        new DynamicRule
        {
            FieldName = "City",
            Operator = "equal",
            Value = "New York",
            Type = "string"
        },
        new DynamicRule
        {
            FieldName = "City",
            Operator = "equal",
            Value = "Los Angeles",
            Type = "string"
        }
    }
};
group.Groups.Add(subGroup);
```

### 4. Build the WHERE Clause

Use the `Build` method of `DynamicWhereBuilder` to generate the WHERE clause and parameters.

```csharp
var (whereClause, parameters) = builder.Build(group);
```

## Using the Result

The `Build` method returns a tuple containing:
1. `whereClause`: A string representing the WHERE clause.
2. `parameters`: An array of parameter values.

You can use these in your database query:

```csharp
using (var connection = new SqlConnection(connectionString))
{
    connection.Open();
    var query = $"SELECT FROM Users WHERE {whereClause}";
    var result = connection.Query(query, parameters);
}
```

## Custom Type Conversions

You can add custom type mappings using the `TypeConversionHelper`:

```csharp
TypeConversionHelper.MergeCustomTypeMapping(new Dictionary<string, Type>
{
    { "custom_type", typeof(YourCustomType) }
});
```

## Best Practices

1. Always validate user input before creating `DynamicRule` objects.
2. Use parameterized queries to prevent SQL injection.
3. Consider performance implications for very complex dynamic queries.
4. Test your queries with various combinations of conditions to ensure correctness.

## Conclusion

The DynamicWhereBuilder provides a flexible and type-safe way to construct dynamic WHERE clauses. By following this guide, you can easily integrate it into your project and create powerful, dynamic database queries.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
