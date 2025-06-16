using System.Linq;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.MySql.RuleTransformers;

/// <summary>
/// MySQL rule transformer for the "not_in" operator.
/// Generates query conditions like "field NOT IN (?, ?, ?)".
/// </summary>
public class NotInRuleTransformer : InTransformerBase
{
    /// <summary>
    /// Initializes a new instance of the NotInRuleTransformer class.
    /// </summary>
    public NotInRuleTransformer() : base("NOT_IN")
    {
    }

    /// <inheritdoc />
    protected override string BuildInQuery(string fieldName, string parameterList)
    {
        return $"{fieldName} NOT IN ({parameterList})";
    }

    /// <inheritdoc />
    protected override string[] GenerateParameterPlaceholders(string parameterName, int count, TransformContext context)
    {
        // MySQL uses positional parameters - all parameters are just "?"
        return Enumerable.Repeat("?", count).ToArray();
    }
}
