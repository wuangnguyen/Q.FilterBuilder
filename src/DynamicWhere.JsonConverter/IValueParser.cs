using System.Text.Json;

namespace DynamicWhere.JsonConverter;

public interface IValueParser
{
    object? ParseValue(JsonElement element);
}