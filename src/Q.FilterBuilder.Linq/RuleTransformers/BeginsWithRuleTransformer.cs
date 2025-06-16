using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Linq.RuleTransformers;

/// <summary>
/// LINQ rule transformer for the "begins_with" operator.
/// Generates query conditions like "field.StartsWith(@param)".
/// For multiple values, generates OR conditions.
/// </summary>
public class BeginsWithRuleTransformer : BaseRuleTransformer
{
    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), "BEGINS_WITH operator requires a non-null value");
        }

        // Handle array or collection values
        if (value is IEnumerable enumerable && value is not string)
        {
            var values = new List<object>();
            foreach (var item in enumerable)
            {
                values.Add(item);
            }

            if (values.Count == 0)
            {
                throw new ArgumentException("BEGINS_WITH operator requires at least one value", nameof(value));
            }

            return values.ToArray();
        }

        // Handle single value - wrap in array
        return [value];
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        if (context.Parameters == null || context.Parameters.Length == 0)
        {
            throw new InvalidOperationException("BEGINS_WITH operator requires parameters");
        }

        // Generate StartsWith conditions for each parameter using sequential parameter indices
        var startsWithConditions = context.Parameters
            .Select((_, index) =>
            {
                var paramName = context.FormatProvider!.FormatParameterName(context.ParameterIndex + index);
                return $"{fieldName}.StartsWith({paramName})";
            })
            .ToArray();

        // If multiple conditions, wrap in parentheses and join with OR
        if (startsWithConditions.Length > 1)
        {
            var combinedConditions = string.Join(" || ", startsWithConditions);
            return $"({combinedConditions})";
        }

        return startsWithConditions[0];
    }
}
