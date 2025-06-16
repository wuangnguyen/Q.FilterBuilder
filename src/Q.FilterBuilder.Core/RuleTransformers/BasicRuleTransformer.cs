using System;
using System.Collections;
using System.Collections.Generic;

namespace Q.FilterBuilder.Core.RuleTransformers;

/// <summary>
/// Represents a basic comparison rule transformer.
/// Similar to BasicOperator but for the rule transformer pattern.
/// </summary>
public class BasicRuleTransformer : BaseRuleTransformer
{
    private readonly string _operator;

    /// <summary>
    /// Initializes a new instance of the BasicRuleTransformer class.
    /// </summary>
    /// <param name="operator">The comparison operator string (e.g., "=", "!=", "<", "<=", ">", ">=").</param>
    public BasicRuleTransformer(string @operator)
    {
        if (string.IsNullOrWhiteSpace(@operator))
        {
            throw new ArgumentException("Operator cannot be null or empty.", nameof(@operator));
        }

        _operator = @operator;
    }

    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            return null;
        }

        // Basic operators cannot compare with collections - throw exception early
        if (value is IEnumerable && value is not string)
        {
            throw new ArgumentException($"Basic operator '{_operator}' cannot compare with collections. Use collection-specific operators instead.", nameof(value));
        }

        return [value];
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        var parameterName = context.FormatProvider!.FormatParameterName(context.ParameterIndex);
        return $"{fieldName} {_operator} {parameterName}";
    }
}
