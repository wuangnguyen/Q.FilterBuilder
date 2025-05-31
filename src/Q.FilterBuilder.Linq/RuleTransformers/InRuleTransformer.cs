using System;
using System.Collections;
using System.Collections.Generic;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Linq.RuleTransformers;

/// <summary>
/// LINQ rule transformer for the "in" operator.
/// Generates query conditions like "@parameterName.Contains(field)".
/// This is the LINQ-specific approach where the collection contains the field value.
/// </summary>
public class InRuleTransformer : BaseRuleTransformer
{
    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), "IN operator requires a non-null value");
        }

        // For LINQ, we need to pass the entire collection as a single parameter
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
                throw new ArgumentException("IN operator requires at least one value", nameof(value));
            }

            // Return the collection as a single parameter for LINQ
            return [values];
        }

        // Handle single value - wrap in array
        return [new[] { value }];
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        // LINQ uses the reverse approach: collection.Contains(field)
        return $"@{parameterName}.Contains({fieldName})";
    }
}
