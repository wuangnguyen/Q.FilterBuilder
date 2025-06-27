# FluentRuleBuilder Documentation

The `FluentRuleBuilder` class provides a fluent API for creating dynamic filter rule structures. It's designed to build `FilterGroup` objects that are then passed to `FilterBuilder` for query generation.

## Overview

FluentRuleBuilder is responsible for building rule structures, not generating queries. It creates a hierarchical structure of `FilterGroup` and `FilterRule` objects that represent the logical conditions for your filters.

## Core Methods

### Where Method

Adds a filter condition to the current rule structure.

```csharp
public FluentRuleBuilder Where(string fieldName, string @operator, object? value, string? explicitType = null)
```

**Parameters:**
- `fieldName`: The field name to filter on
- `operator`: The operator to use (e.g., "equal", "greater", "contains")
- `value`: The value to compare against
- `explicitType`: The explicit type for the value. This allows you to specify the target type for value conversion. This is particularly useful when the value is a string that needs to be converted to a specific data type, such as a `DateTime` or a number.

**Example:**
```csharp
var builder = new FluentRuleBuilder()
    .Where("Age", "greater_or_equal", 18)
    .Where("Name", "contains", "John")
    .Where("Status", "in", new[] { "Active", "Pending" })
    .Where("CreatedDate", "between", new[] { "2023-01-01", "2023-12-31" })
    .Where("Price", "greater", 100.50, "decimal");
```

### BeginGroup Method

Starts a logical group with the specified condition.

```csharp
public FluentRuleBuilder BeginGroup(string condition = "AND")
```

**Parameters:**
- `condition`: The logical condition ("AND" or "OR"). Defaults to "AND"

**Example:**
```csharp
var builder = new FluentRuleBuilder()
    .Where("Age", "greater", 18)
    .BeginGroup("OR")
        .Where("Department", "equal", "IT")
        .Where("Role", "equal", "Admin")
    .EndGroup();
```

### EndGroup Method

Ends the current logical group.

```csharp
public FluentRuleBuilder EndGroup()
```

**Throws:**
- `InvalidOperationException`: If no group is currently open

**Example:**
```csharp
var builder = new FluentRuleBuilder()
    .BeginGroup("OR")
        .Where("City", "equal", "New York")
        .Where("City", "equal", "Los Angeles")
    .EndGroup(); // Closes the OR group
```

### Build Method

Builds the final `FilterGroup` structure from all rules and groups.

```csharp
public FilterGroup Build(string condition = "AND")
```

**Parameters:**
- `condition`: The root condition for combining top-level rules and groups. Defaults to "AND"

**Returns:**
- `FilterGroup`: The complete filter structure

**Throws:**
- `InvalidOperationException`: If there are unclosed groups

**Example:**
```csharp
var group = builder.Build(); // Uses "AND" as root condition
var orGroup = builder.Build("OR"); // Uses "OR" as root condition
```

### AddGroup Method

Adds an existing `FilterGroup` to the current builder.

```csharp
public FluentRuleBuilder AddGroup(FilterGroup group)
```

**Parameters:**
- `group`: The FilterGroup to add

**Throws:**
- `ArgumentNullException`: If group is null

**Example:**
```csharp
var existingGroup = new FilterGroup("OR");
existingGroup.Rules.Add(new FilterRule("Status", "equal", "Active"));

var builder = new FluentRuleBuilder()
    .Where("Age", "greater", 18)
    .AddGroup(existingGroup);
```

### Clear Method

Removes all rules and groups, resetting the builder to its initial state.

```csharp
public FluentRuleBuilder Clear()
```

**Returns:**
- `FluentRuleBuilder`: The same instance for method chaining

**Example:**
```csharp
var builder = new FluentRuleBuilder()
    .Where("Field1", "equal", "value1")
    .Clear() // Removes all rules
    .Where("Field2", "equal", "value2"); // Start fresh
```

## Usage Patterns

### Simple Conditions

```csharp
var group = new FluentRuleBuilder()
    .Where("Name", "equal", "John")
    .Where("Age", "greater", 25)
    .Where("Email", "is_not_null")
    .Build();

// Generates: Name = 'John' AND Age > 25 AND Email IS NOT NULL
```

### Complex Nested Groups

```csharp
var group = new FluentRuleBuilder()
    .Where("IsActive", "equal", true)
    .BeginGroup("OR")
        .Where("Department", "equal", "IT")
        .BeginGroup("AND")
            .Where("Role", "equal", "Manager")
            .Where("Experience", "greater", 5)
        .EndGroup()
    .EndGroup()
    .Build();

// Generates: IsActive = true AND (Department = 'IT' OR (Role = 'Manager' AND Experience > 5))
```

### Collection Operations

```csharp
var group = new FluentRuleBuilder()
    .Where("Status", "in", new[] { "Active", "Pending", "Review" })
    .Where("Tags", "not_in", new[] { "Archived", "Deleted" })
    .Where("CategoryId", "in", new List<int> { 1, 2, 3, 4 })
    .Build();
```

### Date Range Filtering

```csharp
var group = new FluentRuleBuilder()
    .Where("CreatedDate", "between", new[] { "2023-01-01", "2023-12-31" })
    .Where("UpdatedDate", "greater_or_equal", DateTime.Now.AddDays(-30))
    .Build();
```

### String Operations

```csharp
var group = new FluentRuleBuilder()
    .Where("Name", "contains", "John")
    .Where("Email", "begins_with", "admin")
    .Where("Description", "ends_with", "completed")
    .Build();
```

## Integration with FilterBuilder

FluentRuleBuilder creates the rule structure that FilterBuilder uses to generate queries:

```csharp
// 1. Build the rule structure
var group = new FluentRuleBuilder()
    .Where("Age", "greater", 18)
    .Where("Name", "contains", "John")
    .Build();

// 2. Generate the query using FilterBuilder
var (query, parameters) = filterBuilder.Build(group);
```

## Best Practices

### 1. Always Call EndGroup()

```csharp
// ❌ Bad - Will throw InvalidOperationException
var builder = new FluentRuleBuilder()
    .BeginGroup("OR")
        .Where("Field", "equal", "value")
    .Build(); // Missing EndGroup()

// ✅ Good
var builder = new FluentRuleBuilder()
    .BeginGroup("OR")
        .Where("Field", "equal", "value")
    .EndGroup()
    .Build();
```

### 2. Use Meaningful Group Conditions

```csharp
// ✅ Clear intent with explicit conditions
var group = new FluentRuleBuilder()
    .Where("IsActive", "equal", true)
    .BeginGroup("OR") // Either department condition
        .Where("Department", "equal", "IT")
        .Where("Department", "equal", "Engineering")
    .EndGroup()
    .Build();
```

### 3. Leverage Method Chaining

```csharp
// ✅ Fluent and readable
var group = new FluentRuleBuilder()
    .Where("Status", "equal", "Active")
    .Where("Age", "greater", 18)
    .BeginGroup("OR")
        .Where("Role", "equal", "Admin")
        .Where("Role", "equal", "Manager")
    .EndGroup()
    .Build();
```

### 4. Use Explicit Types When Needed

```csharp
// ✅ Explicit type conversion for precision
var group = new FluentRuleBuilder()
    .Where("Price", "greater", "99.99", "decimal")
    .Where("CreatedDate", "between", new[] { "2023-01-01", "2023-12-31" }, "DateTime")
    .Build();
```

## Error Handling

### Common Exceptions

1. **InvalidOperationException**: Unclosed groups
```csharp
try {
    var group = builder.BeginGroup().Build(); // Missing EndGroup()
} catch (InvalidOperationException ex) {
    // Handle unclosed groups
    Console.WriteLine(ex.Message); // "Unclosed groups detected. Call EndGroup() 1 more time(s)."
}
```

2. **ArgumentNullException**: Null group in AddGroup
```csharp
try {
    builder.AddGroup(null);
} catch (ArgumentNullException ex) {
    // Handle null group
}
```

## Advanced Scenarios

### Combining with Existing FilterGroups

```csharp
// Create a reusable group
var commonFilters = new FilterGroup("AND");
commonFilters.Rules.Add(new FilterRule("IsActive", "equal", true));
commonFilters.Rules.Add(new FilterRule("IsDeleted", "equal", false));

// Use in FluentRuleBuilder
var group = new FluentRuleBuilder()
    .AddGroup(commonFilters)
    .Where("Department", "equal", "IT")
    .Build();
```

### Multi-level Nesting

```csharp
var group = new FluentRuleBuilder()
    .Where("TopLevel", "equal", "value")
    .BeginGroup("OR")
        .Where("Level1", "equal", "value1")
        .BeginGroup("AND")
            .Where("Level2A", "equal", "value2a")
            .BeginGroup("OR")
                .Where("Level3A", "equal", "value3a")
                .Where("Level3B", "equal", "value3b")
            .EndGroup()
        .EndGroup()
    .EndGroup()
    .Build();
```

This creates a complex nested structure that demonstrates the full power of FluentRuleBuilder for building sophisticated filter conditions.
