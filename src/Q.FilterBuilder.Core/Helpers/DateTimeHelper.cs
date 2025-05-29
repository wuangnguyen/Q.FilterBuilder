using System;
using System.Globalization;
using System.Linq;

namespace Q.FilterBuilder.Core.Helpers;

/// <summary>
/// Provides helper methods for parsing date and time strings with support for default formats and custom formats.
/// Custom formats are automatically filtered out if they are invalid. Standard .NET parsing is used as a fallback.
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// Default date and time formats supported by the parser.
    /// </summary>
    private static readonly string[] _defaultFormats =
    [
        // ISO 8601 formats (preferred)
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm",
        "yyyy-MM-dd",

        // Standard formats
        "yyyy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm",

        // US formats
        "MM/dd/yyyy HH:mm:ss",
        "MM/dd/yyyy HH:mm",
        "MM/dd/yyyy",
        "M/d/yyyy",

        // European formats
        "dd/MM/yyyy HH:mm:ss",
        "dd/MM/yyyy HH:mm",
        "dd/MM/yyyy",
        "d/M/yyyy",

        // Dash-separated formats
        "dd-MM-yyyy HH:mm:ss",
        "dd-MM-yyyy HH:mm",
        "dd-MM-yyyy",
        "d-M-yyyy",

        // Compact formats
        "yyyyMMdd",
        "yyyyMMddHHmmss",
        "yyyyMMddTHHmmssZ"
    ];

    /// <summary>
    /// Attempts to parse a string representation of a date and time into a DateTime object, using default formats and custom formats if provided. Invalid custom formats are automatically filtered out.
    /// </summary>
    /// <param name="value">The string to parse. Can be null or empty.</param>
    /// <param name="parsedValue">When this method returns, contains the parsed DateTime value if the conversion succeeded, or default(DateTime) if the conversion failed.</param>
    /// <param name="customFormats">Optional. An array of custom date and time format strings to use for parsing. Invalid formats are automatically filtered out.</param>
    /// <returns>true if the parsing was successful; otherwise, false.</returns>
    public static bool TryParseDateTime(
        string? value,
        out DateTime parsedValue,
        string[]? customFormats)
    {
        parsedValue = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        // Auto-remove invalid custom formats and merge with defaults (custom formats first)
        var validCustomFormats = customFormats?.Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();
        var allFormats = validCustomFormats?.Concat(_defaultFormats).ToArray() ?? _defaultFormats;

        // Try parsing with the final merged list
        return TryParseWithFormats(value, allFormats, out parsedValue);
    }

    /// <summary>
    /// Parses a string to DateTime, returning null if parsing fails.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="customFormats">Optional custom formats to try first.</param>
    /// <returns>The parsed DateTime, or null if parsing failed.</returns>
    public static DateTime? ParseDateTimeOrNull(string? value, params string[] customFormats)
    {
        return TryParseDateTime(value, out var result, customFormats) ? result : null;
    }

    /// <summary>
    /// Parses a string to DateTime, returning a default value if parsing fails.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails.</param>
    /// <param name="customFormats">Optional custom formats to try first.</param>
    /// <returns>The parsed DateTime, or the default value if parsing failed.</returns>
    public static DateTime ParseDateTimeOrDefault(string? value, DateTime defaultValue = default, params string[] customFormats)
    {
        return TryParseDateTime(value, out var result, customFormats) ? result : defaultValue;
    }

    /// <summary>
    /// Attempts to parse a value using the specified formats.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <param name="formats">The formats to try.</param>
    /// <param name="parsedValue">The parsed value if successful.</param>
    /// <returns>true if parsing succeeded; otherwise, false.</returns>
    private static bool TryParseWithFormats(string value, string[] formats, out DateTime parsedValue)
    {
        parsedValue = default;

        foreach (var format in formats)
        {
            var styles = format.EndsWith("Z")
                ? DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal
                : DateTimeStyles.AssumeLocal;

            if (DateTime.TryParseExact(
                value,
                format,
                CultureInfo.InvariantCulture,
                styles,
                out parsedValue))
            {
                return true;
            }
        }

        return false;
    }
}
