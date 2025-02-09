using DynamicWhere.Core.Models;
using DynamicWhere.JsonConverter.ValueParsers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicWhere.JsonConverter;

/// <summary>
/// Converts JSON data to a <see cref="DynamicGroup"/> object and vice versa.
/// </summary>
public class JQueryBuilderConverter : JsonConverter<DynamicGroup>
{
    private readonly Dictionary<string, IValueParser> valueParsers;

    /// <summary>
    /// Initializes a new instance of the <see cref="JQueryBuilderConverter"/> class with optional custom parsers.
    /// </summary>
    /// <param name="customParsers">A dictionary of custom value parsers.</param>
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

    /// <summary>
    /// Reads the JSON data and converts it to a <see cref="DynamicGroup"/> object.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type of the object to convert.</param>
    /// <param name="options">The serialization options.</param>
    /// <returns>The converted <see cref="DynamicGroup"/> object.</returns>
    public override DynamicGroup Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var rootElement = document.RootElement;

        return ConvertGroup(rootElement);
    }

    /// <summary>
    /// Converts a JSON element to a <see cref="DynamicGroup"/> object.
    /// </summary>
    /// <param name="groupElement">The JSON element representing the group.</param>
    /// <returns>The converted <see cref="DynamicGroup"/> object.</returns>
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

    /// <summary>
    /// Converts a JSON element to its corresponding value based on the specified type.
    /// </summary>
    /// <param name="valueElement">The JSON element representing the value.</param>
    /// <param name="type">The type of the value.</param>
    /// <returns>The converted value.</returns>
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

    /// <summary>
    /// Writes the <see cref="DynamicGroup"/> object to a JSON writer.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The <see cref="DynamicGroup"/> object to write.</param>
    /// <param name="options">The serialization options.</param>
    public override void Write(Utf8JsonWriter writer, DynamicGroup value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
