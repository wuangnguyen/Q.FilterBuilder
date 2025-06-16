using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.JsonConverter.ValueParsers;

namespace Q.FilterBuilder.JsonConverter;

/// <summary>
/// Converts JSON data from various query builder libraries to a <see cref="FilterGroup"/> object and vice versa.
/// Supports jQuery QueryBuilder, React QueryBuilder, and other query builders through configurable property names.
/// </summary>
public class QueryBuilderConverter : JsonConverter<FilterGroup>
{
    private readonly QueryBuilderOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryBuilderConverter"/> class with default options.
    /// </summary>
    public QueryBuilderConverter() : this(new QueryBuilderOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryBuilderConverter"/> class with custom options.
    /// </summary>
    /// <param name="options">The options for configuring property names.</param>
    public QueryBuilderConverter(QueryBuilderOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Reads the JSON data and converts it to a <see cref="FilterGroup"/> object.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type of the object to convert.</param>
    /// <param name="options">The serialization options.</param>
    /// <returns>The converted <see cref="FilterGroup"/> object.</returns>
    public override FilterGroup Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var rootElement = document.RootElement;

        return ConvertGroup(rootElement);
    }

    /// <summary>
    /// Converts a JSON element to a <see cref="FilterGroup"/> object.
    /// </summary>
    /// <param name="groupElement">The JSON element representing the group.</param>
    /// <returns>The converted <see cref="FilterGroup"/> object.</returns>
    private FilterGroup ConvertGroup(JsonElement groupElement)
    {
        var group = new FilterGroup(groupElement.GetProperty(_options.ConditionPropertyName).GetString()!);

        foreach (var ruleElement in groupElement.GetProperty(_options.RulesPropertyName).EnumerateArray())
        {
            if (ruleElement.TryGetProperty(_options.ConditionPropertyName, out _))
            {
                group.Groups.Add(ConvertGroup(ruleElement));
            }
            else
            {
                var field = ruleElement.GetProperty(_options.FieldPropertyName).GetString()!;
                var operatorName = ruleElement.GetProperty(_options.OperatorPropertyName).GetString()!;
                var explicitType = ruleElement.TryGetProperty(_options.TypePropertyName, out var typeElement)
                    ? typeElement.GetString()
                    : null;
                var rawValue = ruleElement.TryGetProperty(_options.ValuePropertyName, out var valueElement)
                    ? ExtractRawJsonValue(valueElement)
                    : null;
                var metadata = ruleElement.TryGetProperty(_options.DataPropertyName, out var dataElement)
                    ? JsonElementParser.ParseJsonElement(dataElement) as Dictionary<string, object?>
                    : null;

                var rule = new FilterRule(field, operatorName, rawValue, explicitType)
                {
                    Metadata = metadata
                };

                group.Rules.Add(rule);
            }
        }

        return group;
    }

    /// <summary>
    /// Extracts raw JSON value without any type conversion.
    /// </summary>
    /// <param name="valueElement">The JSON element representing the value.</param>
    /// <returns>The raw value extracted from JSON.</returns>
    private object? ExtractRawJsonValue(JsonElement valueElement)
    {
        return JsonElementParser.ParseJsonElement(valueElement);
    }

    /// <summary>
    /// Writes the <see cref="FilterGroup"/> object to a JSON writer.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The <see cref="FilterGroup"/> object to write.</param>
    /// <param name="options">The serialization options.</param>
    public override void Write(
        Utf8JsonWriter writer,
        FilterGroup value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}
