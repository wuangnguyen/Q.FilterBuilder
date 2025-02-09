using DynamicWhere.Core.Helpers;
using System.Text.Json;

namespace DynamicWhere.JsonConverter.ValueParsers;

public class DateTimeValueParser : IValueParser
{
    /// <summary>
    /// Parses the value of a JSON element into a DateTime object.
    /// </summary>
    /// <param name="element">The JSON element to parse.</param>
    /// <returns>A DateTime object if parsing is successful, otherwise null.</returns>
    public object? ParseValue(JsonElement element)
    {
        if (element.TryGetDateTime(out var parsedDateTime) == false)
        {
            DateTimeHelper.TryParseDateTime(element.GetString()!, out parsedDateTime);
        }
        
        return parsedDateTime == default ? null : parsedDateTime;
    }
}