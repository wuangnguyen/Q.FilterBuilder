using DynamicWhere.Core.Helpers;
using System.Text.Json;

namespace DynamicWhere.JsonConverter.ValueParsers;

public class DateTimeValueParser : IValueParser
{
    public object? ParseValue(JsonElement element)
    {
        if (element.TryGetDateTime(out var parsedDateTime) == false)
        {
            DateTimeHelper.TryParseDateTime(element.GetString()!, out parsedDateTime);
        }
        
        return parsedDateTime == default ? null : parsedDateTime;
    }
}