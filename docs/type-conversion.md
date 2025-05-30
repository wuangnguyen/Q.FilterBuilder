# Type Conversion System Documentation

The Q.FilterBuilder type conversion system provides automatic and customizable type conversion for filter values. It handles both simple values and collections, with support for custom converters and metadata-driven conversion.

## Overview

The type conversion system ensures that filter values are properly converted to their target types before being used in queries. It supports:

- **Built-in converters** for common types (DateTime, bool, primitives)
- **Custom converters** for specialized data types
- **Collection handling** for arrays and lists
- **Metadata-driven conversion** for context-specific behavior

## Core Interface

### ITypeConversionService

The main service interface for type conversion operations.

```csharp
public interface ITypeConversionService
{
    object? ConvertValue(object? value, string typeString, Dictionary<string, object?>? metadata = null);
    void RegisterConverter<T>(string typeString, ITypeConverter<T> converter);
    bool IsCollection(object? value);
}
```

### ITypeConverter<T>

Interface for implementing custom type converters.

```csharp
public interface ITypeConverter<T>
{
    T Convert(object? value, Dictionary<string, object?>? metadata = null);
}
```

## Built-in Type Converters

### DateTimeTypeConverter

Converts values to DateTime with support for custom formats.

```csharp
public class DateTimeTypeConverter : ITypeConverter<DateTime>
{
    public DateTime Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        // Handles DateTime objects, strings with custom formats
        // Uses DateTimeHelper for parsing with custom formats
    }
}
```

**Usage:**
```csharp
// Basic date conversion
var date = typeConversionService.ConvertValue("2023-12-25", "datetime");

// With custom formats in metadata
var metadata = new Dictionary<string, object?>
{
    ["dateTimeFormats"] = new[] { "dd/MM/yyyy", "dd-MM-yyyy" }
};
var date = typeConversionService.ConvertValue("25/12/2023", "datetime", metadata);
```

**Supported Formats:**
- ISO 8601 formats (preferred)
- Standard formats (yyyy-MM-dd HH:mm:ss)
- Custom formats via metadata
- European formats (dd/MM/yyyy)
- US formats (MM/dd/yyyy)

### BoolTypeConverter

Converts values to boolean with support for common string representations.

```csharp
public class BoolTypeConverter : ITypeConverter<bool>
{
    public bool Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        // Handles: true/false, 1/0, yes/no, on/off
    }
}
```

**Usage:**
```csharp
var result1 = typeConversionService.ConvertValue("true", "bool");    // true
var result2 = typeConversionService.ConvertValue("1", "bool");       // true
var result3 = typeConversionService.ConvertValue("yes", "bool");     // true
var result4 = typeConversionService.ConvertValue("on", "bool");      // true
var result5 = typeConversionService.ConvertValue("false", "bool");   // false
var result6 = typeConversionService.ConvertValue("0", "bool");       // false
```

## Primitive Type Conversion

The service handles primitive types using built-in .NET conversion:

```csharp
// Supported primitive types
var intValue = typeConversionService.ConvertValue("123", "int");
var longValue = typeConversionService.ConvertValue("123456789", "long");
var doubleValue = typeConversionService.ConvertValue("123.45", "double");
var decimalValue = typeConversionService.ConvertValue("99.99", "decimal");
var floatValue = typeConversionService.ConvertValue("123.45", "float");
var stringValue = typeConversionService.ConvertValue(123, "string");
var guidValue = typeConversionService.ConvertValue("550e8400-e29b-41d4-a716-446655440000", "guid");
```

**Supported Type Strings:**
- `"string"` - String conversion
- `"int"`, `"integer"` - 32-bit integer
- `"long"` - 64-bit integer
- `"double"` - Double-precision floating point
- `"decimal"` - Decimal number
- `"float"` - Single-precision floating point
- `"byte"`, `"short"`, `"uint"`, `"ulong"`, `"ushort"`, `"sbyte"` - Other numeric types
- `"guid"` - GUID/UUID

## Collection Handling

The type conversion service automatically handles collections by converting each element:

```csharp
// Convert array of strings to integers
var intArray = typeConversionService.ConvertValue(new[] { "1", "2", "3" }, "int");
// Result: int[] { 1, 2, 3 }

// Convert list to DateTime array
var dateList = new List<string> { "2023-01-01", "2023-12-31" };
var dateArray = typeConversionService.ConvertValue(dateList, "datetime");
// Result: DateTime[] with parsed dates
```

**Collection Detection:**
```csharp
var isCollection1 = typeConversionService.IsCollection(new[] { 1, 2, 3 });     // true
var isCollection2 = typeConversionService.IsCollection(new List<string>());     // true
var isCollection3 = typeConversionService.IsCollection("string");               // false
var isCollection4 = typeConversionService.IsCollection(123);                    // false
```

## Custom Type Converters

### Creating Custom Converters

```csharp
// Example: Currency converter
public class CurrencyConverter : ITypeConverter<decimal>
{
    public decimal Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var stringValue = value.ToString()!;

        // Remove currency symbols and formatting
        var cleanValue = stringValue.Replace("$", "").Replace(",", "").Trim();

        if (decimal.TryParse(cleanValue, NumberStyles.Currency, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        throw new InvalidOperationException($"Cannot convert '{value}' to currency");
    }
}

// Example: Phone number converter
public class PhoneNumberConverter : ITypeConverter<string>
{
    public string Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null)
            return string.Empty;

        var phone = value.ToString()!;

        // Remove all non-digit characters
        var digits = new string(phone.Where(char.IsDigit).ToArray());

        // Format as (XXX) XXX-XXXX for US numbers
        if (digits.Length == 10)
        {
            return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 4)}";
        }

        return digits; // Return digits only for other formats
    }
}
```

### Registering Custom Converters

```csharp
// Register during service configuration
services.AddFilterBuilder(
    new SqlServerFormatProvider(),
    typeConversion =>
    {
        typeConversion.RegisterConverter("currency", new CurrencyConverter());
        typeConversion.RegisterConverter("phone", new PhoneNumberConverter());
    });

// Or register directly on the service
var typeConversionService = serviceProvider.GetRequiredService<ITypeConversionService>();
typeConversionService.RegisterConverter("currency", new CurrencyConverter());
```

### Using Custom Converters

```csharp
// In FilterRule with explicit type
var rule = new FilterRule("Price", "greater", "$1,234.56", "currency");

// In FluentRuleBuilder
var group = new FluentRuleBuilder()
    .Where("Price", "greater", "$1,234.56", "currency")
    .Where("Phone", "equal", "1234567890", "phone")
    .Build();
```

## Metadata Usage

Metadata provides additional context for type conversion:

### DateTime with Custom Formats

```csharp
var rule = new FilterRule("Date", "equal", "25/12/2023", "datetime");
rule.Metadata = new Dictionary<string, object?>
{
    ["dateTimeFormats"] = new[] { "dd/MM/yyyy", "dd-MM-yyyy" }
};

// The DateTimeTypeConverter will use these custom formats
```

### Custom Converter with Metadata

```csharp
public class ConfigurableConverter : ITypeConverter<string>
{
    public string Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        var prefix = metadata?.TryGetValue("prefix", out var prefixValue) == true
            ? prefixValue?.ToString() ?? ""
            : "";

        return $"{prefix}{value}";
    }
}

// Usage with metadata
var rule = new FilterRule("Code", "equal", "123", "configurable");
rule.Metadata = new Dictionary<string, object?>
{
    ["prefix"] = "CODE-"
};
// Result: "CODE-123"
```

## Error Handling

### Common Exceptions

1. **ArgumentNullException**: Null value for non-nullable conversion
```csharp
try {
    var result = typeConversionService.ConvertValue(null, "int");
} catch (ArgumentNullException ex) {
    // Handle null value error
}
```

2. **InvalidOperationException**: Conversion failure
```csharp
try {
    var result = typeConversionService.ConvertValue("invalid", "int");
} catch (InvalidOperationException ex) {
    // Handle conversion error
    Console.WriteLine(ex.Message); // "Failed to convert 'invalid' to type 'int'"
}
```

3. **NotImplementedException**: Unsupported type
```csharp
try {
    var result = typeConversionService.ConvertValue("value", "unsupported_type");
} catch (InvalidOperationException ex) {
    // Handle unsupported type
}
```

## Integration with FilterBuilder

Type conversion happens automatically during query building:

```csharp
// 1. Create rule with string value and explicit type
var rule = new FilterRule("Age", "greater", "25", "int");

// 2. FilterBuilder automatically converts "25" to int before processing
var (query, parameters) = filterBuilder.Build(group);
// The parameter will be an integer 25, not string "25"
```

## Best Practices

### 1. Use Explicit Types for Precision

```csharp
// ✅ Good - Explicit decimal type for financial data
var rule = new FilterRule("Price", "greater", "99.99", "decimal");

// ❌ Avoid - May lose precision with double conversion
var rule = new FilterRule("Price", "greater", "99.99", "double");
```

### 2. Provide Custom Formats for Dates

```csharp
// ✅ Good - Specify expected date format
var rule = new FilterRule("Date", "equal", "25/12/2023", "datetime");
rule.Metadata = new Dictionary<string, object?>
{
    ["dateTimeFormats"] = new[] { "dd/MM/yyyy" }
};
```

### 3. Handle Collections Appropriately

```csharp
// ✅ Good - Let the service handle collection conversion
var rule = new FilterRule("Ids", "in", new[] { "1", "2", "3" }, "int");
// Service will convert to int[] { 1, 2, 3 }
```

## Advanced Scenarios

### Conditional Conversion

```csharp
public class SmartDateConverter : ITypeConverter<DateTime>
{
    public DateTime Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value is DateTime dt)
            return dt;

        var stringValue = value?.ToString();
        if (string.IsNullOrEmpty(stringValue))
            throw new ArgumentException("Cannot convert empty value to DateTime");

        // Try different strategies based on metadata
        var strategy = metadata?.TryGetValue("strategy", out var strategyValue) == true
            ? strategyValue?.ToString()
            : "default";

        return strategy switch
        {
            "utc" => DateTime.Parse(stringValue, null, DateTimeStyles.AssumeUniversal),
            "local" => DateTime.Parse(stringValue, null, DateTimeStyles.AssumeLocal),
            _ => DateTime.Parse(stringValue)
        };
    }
}
```

## Complete Converter Examples

### Enum Converter

```csharp
public class EnumConverter<T> : ITypeConverter<T> where T : struct, Enum
{
    public T Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (value is T enumValue)
            return enumValue;

        var stringValue = value.ToString()!;

        // Try parse by name (case-insensitive)
        if (Enum.TryParse<T>(stringValue, true, out var result))
            return result;

        // Try parse by numeric value
        if (int.TryParse(stringValue, out var numericValue) &&
            Enum.IsDefined(typeof(T), numericValue))
        {
            return (T)Enum.ToObject(typeof(T), numericValue);
        }

        throw new InvalidOperationException($"Cannot convert '{value}' to enum {typeof(T).Name}");
    }
}

// Usage
services.AddFilterBuilder(
    new SqlServerFormatProvider(),
    typeConversion =>
    {
        typeConversion.RegisterConverter("status", new EnumConverter<OrderStatus>());
    });
```

### Complex Object Converter

```csharp
public class AddressConverter : ITypeConverter<Address>
{
    public Address Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (value is Address address)
            return address;

        var stringValue = value.ToString()!;

        // Parse different address formats
        if (TryParseFullAddress(stringValue, out var fullAddress))
            return fullAddress;

        if (TryParseZipCode(stringValue, out var zipAddress))
            return zipAddress;

        throw new InvalidOperationException($"Cannot convert '{value}' to Address");
    }

    private bool TryParseFullAddress(string input, out Address address)
    {
        address = new Address();

        // Parse "123 Main St, City, State 12345" format
        var parts = input.Split(',').Select(p => p.Trim()).ToArray();
        if (parts.Length >= 3)
        {
            address.Street = parts[0];
            address.City = parts[1];

            var stateZip = parts[2].Split(' ');
            if (stateZip.Length >= 2)
            {
                address.State = stateZip[0];
                address.ZipCode = stateZip[1];
                return true;
            }
        }

        return false;
    }

    private bool TryParseZipCode(string input, out Address address)
    {
        address = new Address();

        if (input.Length == 5 && input.All(char.IsDigit))
        {
            address.ZipCode = input;
            return true;
        }

        return false;
    }
}
```

### Conditional Converter

```csharp
public class SmartNumberConverter : ITypeConverter<decimal>
{
    public decimal Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (value is decimal decimalValue)
            return decimalValue;

        var stringValue = value.ToString()!;

        // Get conversion strategy from metadata
        var strategy = metadata?.TryGetValue("strategy", out var strategyValue) == true
            ? strategyValue?.ToString()
            : "default";

        return strategy switch
        {
            "currency" => ParseCurrency(stringValue),
            "percentage" => ParsePercentage(stringValue),
            "scientific" => ParseScientific(stringValue),
            _ => decimal.Parse(stringValue, CultureInfo.InvariantCulture)
        };
    }

    private decimal ParseCurrency(string value)
    {
        // Remove currency symbols and formatting
        var cleaned = value.Replace("$", "").Replace(",", "").Trim();
        return decimal.Parse(cleaned, CultureInfo.InvariantCulture);
    }

    private decimal ParsePercentage(string value)
    {
        var cleaned = value.Replace("%", "").Trim();
        var percentage = decimal.Parse(cleaned, CultureInfo.InvariantCulture);
        return percentage / 100m; // Convert to decimal
    }

    private decimal ParseScientific(string value)
    {
        return decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
    }
}

// Usage with metadata
var rule = new FilterRule("Amount", "greater", "$1,234.56", "smart_number");
rule.Metadata = new Dictionary<string, object?>
{
    ["strategy"] = "currency"
};
```

### Collection-Aware Converter

```csharp
public class TagConverter : ITypeConverter<string[]>
{
    public string[] Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value == null)
            return Array.Empty<string>();

        if (value is string[] stringArray)
            return stringArray;

        if (value is IEnumerable<string> stringEnumerable)
            return stringEnumerable.ToArray();

        var stringValue = value.ToString()!;

        // Get delimiter from metadata
        var delimiter = metadata?.TryGetValue("delimiter", out var delimiterValue) == true
            ? delimiterValue?.ToString()
            : ",";

        // Parse delimited string
        var tags = stringValue
            .Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
            .Select(tag => tag.Trim())
            .Where(tag => !string.IsNullOrEmpty(tag))
            .ToArray();

        // Apply transformations based on metadata
        var toLowerCase = metadata?.TryGetValue("toLowerCase", out var lowerValue) == true
            && bool.Parse(lowerValue?.ToString() ?? "false");

        if (toLowerCase)
        {
            tags = tags.Select(tag => tag.ToLowerInvariant()).ToArray();
        }

        return tags;
    }
}

// Usage
var rule = new FilterRule("Tags", "contains_any", "Technology, Programming, C#", "tags");
rule.Metadata = new Dictionary<string, object?>
{
    ["delimiter"] = ",",
    ["toLowerCase"] = "true"
};
```
