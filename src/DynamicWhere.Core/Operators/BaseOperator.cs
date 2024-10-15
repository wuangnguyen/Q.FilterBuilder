using DynamicWhere.Core.Helpers;
using DynamicWhere.Core.Models;
using System.Collections.Generic;

namespace DynamicWhere.Core.Operators;

/// <summary>
/// Provides a base implementation for query operators.
/// </summary>
public abstract class BaseOperator : IOperator
{
    /// <summary>
    /// Gets the query part for the given rule and parameter index.
    /// </summary>
    /// <param name="rule">The dynamic rule to process.</param>
    /// <param name="parameterIndex">The index of the parameter in the query.</param>
    /// <returns>A string representing the query part for the rule.</returns>
    public abstract string GetQueryPart(DynamicRule rule, int parameterIndex);

    /// <summary>
    /// Gets the parameters part for the given rule.
    /// </summary>
    /// <param name="rule">The dynamic rule to process.</param>
    /// <returns>An array of objects representing the parameters for the rule, or null if no parameters are needed.</returns>
    public virtual object[]? GetParametersPart(DynamicRule rule)
    {
        if (rule.Data is Dictionary<string, object?> extraInfo && extraInfo.TryGetValue("datetimeFormat", out var datetimeFormat))
        {
            return TypeConversionHelper.ConvertValueToObjectArray(rule.Value, rule.Type, datetimeFormat?.ToString());
        }

        return TypeConversionHelper.ConvertValueToObjectArray(rule.Value, rule.Type);
    }
}