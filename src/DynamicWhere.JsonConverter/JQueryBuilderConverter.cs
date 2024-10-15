using DynamicWhere.Core.Models;
using DynamicWhere.JsonConverter.ValueParsers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicWhere.JsonConverter;

public class JQueryBuilderConverter : JsonConverter<DynamicGroup>
{
    private readonly Dictionary<string, IValueParser> valueParsers;

    public JQueryBuilderConverter(Dictionary<string, IValueParser>? customParsers = null)
    {
        valueParsers = new Dictionary<string, IValueParser>
        {
            { "datetime", new DateTimeValueParser() }
        };

        if (customParsers != null)
        {
            foreach (var parser in customParsers)
            {
                valueParsers[parser.Key] = parser.Value;
            }
        }
    }

    public override DynamicGroup Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var rootElement = document.RootElement;

        return ConvertGroup(rootElement);
    }

    private DynamicGroup ConvertGroup(JsonElement groupElement)
    {
        var group = new DynamicGroup
        {
            Condition = groupElement.GetProperty("condition").GetString()!
        };

        foreach (var ruleElement in groupElement.GetProperty("rules").EnumerateArray())
        {
            if (ruleElement.TryGetProperty("condition", out _))
            {
                group.Groups.Add(ConvertGroup(ruleElement));
            }
            else
            {
                var type = ruleElement.GetProperty("type").GetString()!;
                var value = ConvertValue(ruleElement.GetProperty("value"), type);
                var data = ruleElement.TryGetProperty("data", out var dataElement) ? JsonElementParser.ParseJsonElement(dataElement) : null;

                group.Rules.Add(new DynamicRule
                {
                    FieldName = ruleElement.GetProperty("field").GetString()!,
                    Operator = ruleElement.GetProperty("operator").GetString()!,
                    Value = value,
                    Type = type,
                    Data = data
                });
            }
        }

        return group;
    }

    private object? ConvertValue(JsonElement valueElement, string type)
    {
        if (valueParsers.TryGetValue(type, out var parser))
        {
            return parser.ParseValue(valueElement);
        }

        return valueElement.ValueKind switch
        {
            JsonValueKind.Object => JsonElementParser.ParseObject(valueElement),
            JsonValueKind.Array => JsonElementParser.ParseArray(valueElement),
            JsonValueKind.Null => null,
            _ => JsonElementParser.ParseJsonElement(valueElement)
        };
    }

    public override void Write(Utf8JsonWriter writer, DynamicGroup value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}