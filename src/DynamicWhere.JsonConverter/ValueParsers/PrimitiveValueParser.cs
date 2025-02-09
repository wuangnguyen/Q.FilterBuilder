using System.Text.Json;

namespace DynamicWhere.JsonConverter.ValueParsers;

public class PrimitiveValueParser : IValueParser
{
    /// <summary>
    /// Parses a JSON value to its corresponding .NET type. This is used for primitive types like int, string etc.
    /// </summary>
    /// <param name="element"></param>
    /// <returns>Object of the parsed type.</returns>
    public object? ParseValue(JsonElement element)
    {
        return JsonElementParser.ParseJsonElement(element);
    }
}
