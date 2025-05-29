using System;
using System.Collections.Generic;
using System.Linq;
using Q.FilterBuilder.Core.Helpers;

namespace Q.FilterBuilder.Core.TypeConversion;

/// <summary>
/// Converts values to DateTime.
/// </summary>
public class DateTimeTypeConverter : ITypeConverter<DateTime>
{
    /// <inheritdoc />
    public DateTime Convert(object? value, Dictionary<string, object?>? metadata = null)
    {
        if (value is DateTime dateTime)
        {
            return dateTime;
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), "Cannot convert null to DateTime");
        }

        var stringValue = value.ToString()!;

        // Check for custom formats in metadata
        var customFormats = metadata?.TryGetValue("dateTimeFormats", out var formatsValue) == true
            ? ConvertToStringArray(formatsValue)
            : null;

        if (DateTimeHelper.TryParseDateTime(stringValue, out var result, customFormats))
        {
            return result;
        }

        throw new InvalidOperationException($"Cannot convert '{value}' to DateTime");
    }

    /// <summary>
    /// Converts a value to string array, handling single string, string array, and other cases.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>An array of strings.</returns>
    private static string[]? ConvertToStringArray(object? value)
    {
        return value switch
        {
            string[] stringArray => stringArray,
            string singleString => [singleString],
            System.Collections.IEnumerable enumerable and not string =>
                enumerable.Cast<object>()
                          .Select(item => item?.ToString())
                          .Where(str => !string.IsNullOrWhiteSpace(str))
                          .Cast<string>()
                          .ToArray(),
            _ => null
        };
    }
}
