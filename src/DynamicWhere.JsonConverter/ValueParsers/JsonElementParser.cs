using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DynamicWhere.JsonConverter.ValueParsers;

public static class JsonElementParser
{
    public static object? ParseJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt32(out int intValue) => intValue,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.True or JsonValueKind.False => element.GetBoolean(),
            JsonValueKind.Object => ParseObject(element),
            JsonValueKind.Array => ParseArray(element),
            _ => throw new NotSupportedException($"Unsupported JsonValueKind: {element.ValueKind}"),
        };
    }

    public static object? ParseObject(JsonElement element)
    {
        var dictionary = new Dictionary<string, object?>();
        foreach (var property in element.EnumerateObject())
        {
            dictionary[property.Name] = ParseJsonElement(property.Value);
        }
        return dictionary;
    }

    public static object? ParseArray(JsonElement element)
    {
        var list = new List<object?>();
        foreach (var item in element.EnumerateArray())
        {
            list.Add(ParseJsonElement(item));
        }
        return list;
    }
}
