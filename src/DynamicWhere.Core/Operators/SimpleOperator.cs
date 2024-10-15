using DynamicWhere.Core.Models;

namespace DynamicWhere.Core.Operators;

/// <summary>
/// Represents a simple comparison operator.
/// </summary>
public class SimpleOperator : BaseOperator
{
    private readonly string compareOperator;

    /// <summary>
    /// Initializes a new instance of the SimpleOperator class.
    /// </summary>
    /// <param name="compareOperator">The comparison operator string (e.g., "==", "!=", "<", "<=", ">", ">=").</param>
    public SimpleOperator(string compareOperator)
    {
        this.compareOperator = compareOperator;
    }

    /// <summary>
    /// Gets the query part for the given rule and parameter index.
    /// </summary>
    /// <param name="rule">The dynamic rule to process.</param>
    /// <param name="parameterIndex">The index of the parameter in the query.</param>
    /// <returns>A string representing the query part for the rule.</returns>
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        return $"{rule.FieldName} {compareOperator} @{parameterIndex}";
    }
}