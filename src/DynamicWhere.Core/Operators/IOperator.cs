using DynamicWhere.Core.Models;

namespace DynamicWhere.Core.Operators;

/// <summary>
/// Defines the interface for query operators.
/// </summary>
public interface IOperator
{
    /// <summary>
    /// Gets the query part for the given rule and parameter index.
    /// </summary>
    /// <param name="rule">The dynamic rule to process.</param>
    /// <param name="parameterIndex">The index of the parameter in the query.</param>
    /// <returns>A string representing the query part for the rule.</returns>
    string GetQueryPart(DynamicRule rule, int parameterIndex);

    /// <summary>
    /// Gets the parameters part for the given rule.
    /// </summary>
    /// <param name="rule">The dynamic rule to process.</param>
    /// <returns>An array of objects representing the parameters for the rule, or null if no parameters are needed.</returns>
    object[]? GetParametersPart(DynamicRule rule);
}
