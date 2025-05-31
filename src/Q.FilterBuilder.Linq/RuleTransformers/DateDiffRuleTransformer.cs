using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Linq.RuleTransformers;

/// <summary>
/// LINQ rule transformer for the "date_diff" operator.
/// Generates query conditions using LINQ date operations like "(DateTime.Now - field).TotalDays == @param".
/// The interval type can be specified in metadata with key "intervalType".
/// </summary>
public class DateDiffRuleTransformer : BaseRuleTransformer
{
    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), "DATE_DIFF operator requires a non-null value");
        }

        return [value];
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        // Get interval type from metadata, default to "day"
        var intervalType = "day";
        if (context.Metadata?.TryGetValue("intervalType", out var intervalValue) == true && intervalValue != null)
        {
            intervalType = intervalValue.ToString()!;
        }

        // Validate interval type (LINQ supported intervals)
        var validIntervals = new[] { "year", "month", "day", "hour", "minute", "second", "millisecond" };
        var lowerIntervalType = intervalType.ToLowerInvariant();
        var isValidInterval = false;
        foreach (var validInterval in validIntervals)
        {
            if (validInterval == lowerIntervalType)
            {
                isValidInterval = true;
                break;
            }
        }

        if (!isValidInterval)
        {
            throw new ArgumentException($"Invalid interval type '{intervalType}'. Valid types are: {string.Join(", ", validIntervals)}", nameof(intervalType));
        }

        // LINQ uses TimeSpan properties for different intervals
        return lowerIntervalType switch
        {
            "year" => $"(DateTime.Now.Year - {fieldName}.Year) == @{parameterName}",
            "month" => $"((DateTime.Now.Year - {fieldName}.Year) * 12 + DateTime.Now.Month - {fieldName}.Month) == @{parameterName}",
            "day" => $"(DateTime.Now - {fieldName}).TotalDays == @{parameterName}",
            "hour" => $"(DateTime.Now - {fieldName}).TotalHours == @{parameterName}",
            "minute" => $"(DateTime.Now - {fieldName}).TotalMinutes == @{parameterName}",
            "second" => $"(DateTime.Now - {fieldName}).TotalSeconds == @{parameterName}",
            "millisecond" => $"(DateTime.Now - {fieldName}).TotalMilliseconds == @{parameterName}",
            _ => $"(DateTime.Now - {fieldName}).TotalDays == @{parameterName}" // Default to day
        };
    }
}
