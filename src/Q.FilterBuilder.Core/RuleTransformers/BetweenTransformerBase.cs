using System;
using System.Collections;
using System.Collections.Generic;

namespace Q.FilterBuilder.Core.RuleTransformers;

/// <summary>
/// Base class for BETWEEN and NOT BETWEEN rule transformers.
/// Provides common logic for handling two-value collections and date normalization.
/// </summary>
public abstract class BetweenTransformerBase : BaseRuleTransformer
{
    private readonly string _operatorName;

    /// <summary>
    /// Initializes a new instance of the BetweenTransformerBase class.
    /// </summary>
    /// <param name="operatorName">The name of the operator for error messages.</param>
    protected BetweenTransformerBase(string operatorName)
    {
        _operatorName = operatorName ?? throw new ArgumentNullException(nameof(operatorName));
    }

    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), $"{_operatorName} operator requires a non-null value");
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
                throw new ArgumentException($"{_operatorName} operator requires exactly 2 values", nameof(value));
            }

            // Handle date normalization if type is date
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

        throw new ArgumentException($"{_operatorName} operator requires an array or collection with exactly 2 values", nameof(value));
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var param1 = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        var param2 = context.FormatProvider!.FormatParameterName(context.ParameterIndex + 1);
        return BuildBetweenQuery(fieldName, param1, param2);
    }

    /// <summary>
    /// Builds the BETWEEN query string with specific parameter names.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="param1">The first parameter name.</param>
    /// <param name="param2">The second parameter name.</param>
    /// <returns>The BETWEEN query string.</returns>
    protected virtual string BuildBetweenQuery(string fieldName, string param1, string param2)
    {
        // Default implementation - can be overridden by providers
        return $"{fieldName} BETWEEN {param1} AND {param2}";
    }
}
