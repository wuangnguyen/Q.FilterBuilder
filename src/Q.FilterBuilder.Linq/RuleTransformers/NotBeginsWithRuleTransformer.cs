using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Linq.RuleTransformers;

/// <summary>
/// LINQ rule transformer for the "not_begins_with" operator.
/// Generates query conditions like "!field.StartsWith(@param)".
/// For multiple values, generates AND conditions.
/// </summary>
public class NotBeginsWithRuleTransformer : BaseRuleTransformer
{
    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), "NOT_BEGINS_WITH operator requires a non-null value");
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
                throw new ArgumentException("NOT_BEGINS_WITH operator requires at least one value", nameof(value));
            }

            return values.ToArray();
        }

        // Handle single value - wrap in array
        return [value];
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        if (context.Parameters == null || context.Parameters.Length == 0)
        {
            throw new InvalidOperationException("NOT_BEGINS_WITH operator requires parameters");
        }

        // Generate NOT StartsWith conditions for each parameter
        var notStartsWithConditions = context.Parameters
            .Select((_, index) => $"!{fieldName}.StartsWith(@{parameterName}{index})")
            .ToArray();

        // If multiple conditions, wrap in parentheses and join with AND
        if (notStartsWithConditions.Length > 1)
        {
            var combinedConditions = string.Join(" && ", notStartsWithConditions);
            return $"({combinedConditions})";
        }

        return notStartsWithConditions[0];
    }
}
