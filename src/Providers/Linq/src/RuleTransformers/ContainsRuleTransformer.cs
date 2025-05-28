using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Linq.RuleTransformers;

/// <summary>
/// LINQ rule transformer for the "contains" operator.
/// Generates query conditions like "field.Contains(@param)".
/// For multiple values, generates OR conditions.
/// </summary>
public class ContainsRuleTransformer : BaseRuleTransformer
{
    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), "CONTAINS operator requires a non-null value");
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
                throw new ArgumentException("CONTAINS operator requires at least one value", nameof(value));
            }

            return values.ToArray();
        }

        // Handle single value - wrap in array
        return new[] { value };
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        if (context.Parameters == null || context.Parameters.Length == 0)
        {
            throw new InvalidOperationException("CONTAINS operator requires parameters");
        }

        // Generate Contains conditions for each parameter
        var containsConditions = context.Parameters
            .Select((_, index) => $"{fieldName}.Contains(@{parameterName}{index})")
            .ToArray();

        // If multiple conditions, wrap in parentheses and join with OR
        if (containsConditions.Length > 1)
        {
            var combinedConditions = string.Join(" || ", containsConditions);
            return $"({combinedConditions})";
        }

        return containsConditions[0];
    }
}
