using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.RuleTransformers;

/// <summary>
/// SQL Server rule transformer for the "date_diff" operator.
/// Generates query conditions like "DATEDIFF(day, field, GETDATE()) = @param".
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
    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName = context.FormatProvider!.FormatParameterName(context.ParameterIndex);

        // Get interval type from metadata, default to "day"
        var intervalType = "day";
        if (context.Metadata?.TryGetValue("intervalType", out var intervalValue) == true && intervalValue != null)
        {
            intervalType = intervalValue.ToString()!;
        }

        // Validate interval type (SQL Server supported intervals)
        var validIntervals = new[] { "year", "quarter", "month", "dayofyear", "day", "week", "hour", "minute", "second", "millisecond", "microsecond", "nanosecond" };
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

        return $"DATEDIFF({intervalType.ToLowerInvariant()}, {fieldName}, GETDATE()) = {parameterName}";
    }
}
