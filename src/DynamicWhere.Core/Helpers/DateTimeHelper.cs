using System;
using System.Globalization;
using System.Linq;

namespace DynamicWhere.Core.Helpers;

/// <summary>
/// Provides helper methods for parsing date and time strings.
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// Attempts to parse a string representation of a date and time into a DateTime object.
    /// </summary>
    /// <param name="dateTimeString">The string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed DateTime value if the conversion succeeded, or default(DateTime) if the conversion failed.</param>
    /// <param name="customFormats">Optional. An array of custom date and time format strings to use for parsing.</param>
    /// <returns>true if the parsing was successful; otherwise, false.</returns>
    public static bool TryParseDateTime(string dateTimeString, out DateTime result, params string[] customFormats)
    {
        string[] defaultFormats =
        [
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd",
            "MM/dd/yyyy",
            "MM/dd/yyyy HH:mm",
            "MM/dd/yyyy HH:mm:ss",
            "dd/MM/yyyy",
            "dd/MM/yyyy HH:mm",
            "dd/MM/yyyy HH:mm:ss",
            "dd-MM-yyyy",
            "dd-MM-yyyy HH:mm",
            "dd-MM-yyyy HH:mm:ss",
            "yyyyMMddTHHmmssZ",  // ISO 8601 format
            "yyyy-MM-ddTHH:mm:ssZ"  // ISO 8601 format with T
        ];

        var areCustomFormatsValid = customFormats.All(x => !string.IsNullOrEmpty(x));

        string[] formats = (customFormats.Length > 0 && areCustomFormatsValid) ? customFormats : defaultFormats;

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
            {
                return true;
            }
        }

        result = default;
        return false;
    }
}
