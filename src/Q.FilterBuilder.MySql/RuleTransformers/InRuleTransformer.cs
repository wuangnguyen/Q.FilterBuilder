using System.Linq;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.MySql.RuleTransformers;

/// <summary>
/// MySQL rule transformer for the "in" operator.
/// Generates query conditions like "field IN (?, ?, ?)".
/// </summary>
public class InRuleTransformer : InTransformerBase
{
    /// <summary>
    /// Initializes a new instance of the InRuleTransformer class.
    /// </summary>
    public InRuleTransformer() : base("IN")
    {
    }

    /// <inheritdoc />
    protected override string BuildInQuery(string fieldName, string parameterList)
    {
        return $"{fieldName} IN ({parameterList})";
    }

    /// <inheritdoc />
    protected override string[] GenerateParameterPlaceholders(string parameterName, int count, TransformContext context)
    {
        // MySQL uses positional parameters - all parameters are just "?"
        return Enumerable.Repeat("?", count).ToArray();
    }
}
