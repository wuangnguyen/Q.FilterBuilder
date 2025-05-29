using System;
using System.Collections;
using System.Collections.Generic;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Linq.RuleTransformers;

/// <summary>
/// LINQ rule transformer for the "not_between" operator.
/// Generates query conditions like "field < @param0 || field > @param1".
/// </summary>
public class NotBetweenRuleTransformer : BaseRuleTransformer
{
    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), "NOT_BETWEEN operator requires a non-null value");
        }

        // Handle array or collection values
        if (value is IEnumerable enumerable && value is not string)
        {
            var values = new List<object>();
            foreach (var item in enumerable)
            {
                values.Add(item);
            }

            if (values.Count != 2)
            {
                throw new ArgumentException("NOT_BETWEEN operator requires exactly 2 values", nameof(value));
            }

            if (metadata?["type"]?.ToString() == "date")
            {
                var firstValue = DateTime.SpecifyKind(
                    ((DateTime)values[0]).Date,
                    DateTimeKind.Unspecified
                );
                var secondValue = DateTime.SpecifyKind(
                    ((DateTime)values[1]).Date.AddDays(1).AddTicks(-1),
                    DateTimeKind.Unspecified
                );

                return [firstValue, secondValue];
            }

            return values.ToArray();
        }

        throw new ArgumentException("NOT_BETWEEN operator requires an array or collection with exactly 2 values", nameof(value));
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return $"({fieldName} < {parameterName}0 || {fieldName} > {parameterName}1)";
    }
}
