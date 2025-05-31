using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.PostgreSql.RuleTransformers;

/// <summary>
/// PostgreSQL rule transformer for the "date_diff" operator.
/// Generates query conditions using PostgreSQL date functions like "EXTRACT(day FROM NOW() - field) = $1".
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

        // Validate interval type (PostgreSQL supported intervals)
        var validIntervals = new[] { "year", "month", "day", "hour", "minute", "second" };
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

        // PostgreSQL uses EXTRACT function for date differences
        // EXTRACT(day FROM NOW() - field) for day differences
        // EXTRACT(hour FROM NOW() - field) for hour differences, etc.
        return $"EXTRACT({intervalType.ToLowerInvariant()} FROM NOW() - {fieldName}) = {parameterName}";
    }
}
