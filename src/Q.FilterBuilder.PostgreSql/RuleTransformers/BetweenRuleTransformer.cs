using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.PostgreSql.RuleTransformers;

/// <summary>
/// PostgreSQL rule transformer for the "between" operator.
/// Generates query conditions like "field BETWEEN $1 AND $2".
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
    protected override string BuildBetweenQuery(string fieldName, string param1, string param2)
    {
        return $"{fieldName} BETWEEN {param1} AND {param2}";
    }
}
