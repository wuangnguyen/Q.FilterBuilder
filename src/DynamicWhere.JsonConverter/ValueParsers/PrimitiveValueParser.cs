using System.Text.Json;

namespace DynamicWhere.JsonConverter.ValueParsers;


public class PrimitiveValueParser : IValueParser
{
    public object? ParseValue(JsonElement element)
    {
        return JsonElementParser.ParseJsonElement(element);
    }
}