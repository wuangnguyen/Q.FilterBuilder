# Helpers Documentation

Q.FilterBuilder includes helper classes and utility methods that provide essential functionality for date parsing.

## DateTimeHelper

The `DateTimeHelper` class provides robust date and time parsing with support for default formats and custom formats. It's used internally by the `DateTimeTypeConverter` and can be used directly in custom converters.

### Public Methods

```csharp
public static class DateTimeHelper
{
    // Primary parsing method
    public static bool TryParseDateTime(string? value, out DateTime parsedValue, string[]? customFormats);

    // Convenience methods
    public static DateTime? ParseDateTimeOrNull(string? value, params string[] customFormats);
    public static DateTime ParseDateTimeOrDefault(string? value, DateTime defaultValue = default, params string[] customFormats);
}
```

### Default Supported Formats

The helper includes comprehensive support for common date formats:

```csharp
// ISO 8601 formats (preferred)
"yyyy-MM-ddTHH:mm:ss.fffZ"
"yyyy-MM-ddTHH:mm:ssZ"
"yyyy-MM-ddTHH:mm:ss.fff"
"yyyy-MM-ddTHH:mm:ss"
"yyyy-MM-ddTHH:mm"
"yyyy-MM-dd"

// Standard formats
"yyyy-MM-dd HH:mm:ss.fff"
"yyyy-MM-dd HH:mm:ss"
"yyyy-MM-dd HH:mm"

// US formats
"MM/dd/yyyy HH:mm:ss"
"MM/dd/yyyy HH:mm"
"MM/dd/yyyy"
"M/d/yyyy"

// European formats
"dd/MM/yyyy HH:mm:ss"
"dd/MM/yyyy HH:mm"
"dd/MM/yyyy"
"d/M/yyyy"

// Dash-separated formats
"dd-MM-yyyy HH:mm:ss"
"dd-MM-yyyy HH:mm"
"dd-MM-yyyy"
"d-M-yyyy"

// Compact formats
"yyyyMMdd"
"yyyyMMddHHmmss"
"yyyyMMddTHHmmssZ"
```

### Usage Examples

#### Basic Date Parsing

```csharp
// ISO 8601 formats
var success1 = DateTimeHelper.TryParseDateTime("2023-12-25", out var date1, null);
var success2 = DateTimeHelper.TryParseDateTime("2023-12-25T14:30:00Z", out var date2, null);

// US formats (automatically detected)
var success3 = DateTimeHelper.TryParseDateTime("12/25/2023", out var date3, null);

// Compact formats
var success4 = DateTimeHelper.TryParseDateTime("20231225", out var date4, null);
```

#### Custom Format Parsing

```csharp
// Define custom formats (tried before defaults)
var customFormats = new[]
{
    "dd/MM/yyyy HH:mm:ss",
    "dd/MM/yyyy",
    "dd-MM-yyyy"
};

// Parse with custom formats
var success = DateTimeHelper.TryParseDateTime("25/12/2023 14:30:00", out var date, customFormats);

// Resolve ambiguous dates with custom formats
var ambiguousDate = "01/02/2023"; // Could be Jan 2 or Feb 1
var europeanFormat = new[] { "dd/MM/yyyy" };
var success = DateTimeHelper.TryParseDateTime(ambiguousDate, out var date, europeanFormat);
// Result: February 1, 2023 (European interpretation)
```

#### Convenience Methods

```csharp
// ParseDateTimeOrNull - returns null on failure
var date1 = DateTimeHelper.ParseDateTimeOrNull("2023-12-25"); // DateTime?
var date2 = DateTimeHelper.ParseDateTimeOrNull("invalid"); // null

// ParseDateTimeOrDefault - returns default value on failure
var date3 = DateTimeHelper.ParseDateTimeOrDefault("2023-12-25"); // DateTime
var date4 = DateTimeHelper.ParseDateTimeOrDefault("invalid", DateTime.Now); // DateTime.Now

// With custom formats
var date5 = DateTimeHelper.ParseDateTimeOrNull("25/12/2023", "dd/MM/yyyy");
```

#### Error Handling

```csharp
// Invalid input returns false
var success1 = DateTimeHelper.TryParseDateTime(null, out var date1, null); // false
var success2 = DateTimeHelper.TryParseDateTime("", out var date2, null); // false
var success3 = DateTimeHelper.TryParseDateTime("invalid", out var date3, null); // false

// Invalid custom formats are automatically filtered
var invalidFormats = new[] { "", null, "   ", "yyyy-MM-dd" }; // Only "yyyy-MM-dd" is valid
var success = DateTimeHelper.TryParseDateTime("2023-12-25", out var date, invalidFormats);
// Still succeeds using the valid format
```

## See Also

- **[Type Conversion Documentation](type-conversion.md)** - Complete type conversion system reference
- **[Database Providers Documentation](providers.md)** - Provider-specific extension methods
- **[Main Project Documentation](../README.md)** - Getting started guide
