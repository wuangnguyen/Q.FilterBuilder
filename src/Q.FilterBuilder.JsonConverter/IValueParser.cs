using System.Text.Json;

namespace Q.FilterBuilder.JsonConverter;

public interface IValueParser
{
    object? ParseValue(JsonElement element);
}