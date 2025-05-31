using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.MySql.RuleTransformers;

/// <summary>
/// MySQL rule transformer for the "between" operator.
/// Generates query conditions like "field BETWEEN ? AND ?".
/// </summary>
public class BetweenRuleTransformer : BetweenTransformerBase
{
    /// <summary>
    /// Initializes a new instance of the BetweenRuleTransformer class.
    /// </summary>
    public BetweenRuleTransformer() : base("BETWEEN")
    {
    }

    /// <inheritdoc />
    protected override string BuildBetweenQuery(string fieldName, string parameterName)
    {
        return $"{fieldName} BETWEEN ? AND ?";
    }
}
