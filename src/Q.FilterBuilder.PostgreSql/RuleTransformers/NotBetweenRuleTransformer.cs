using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.PostgreSql.RuleTransformers;

/// <summary>
/// PostgreSQL rule transformer for the "not_between" operator.
/// Generates query conditions like "field NOT BETWEEN $1 AND $2".
/// </summary>
public class NotBetweenRuleTransformer : BetweenTransformerBase
{
    /// <summary>
    /// Initializes a new instance of the NotBetweenRuleTransformer class.
    /// </summary>
    public NotBetweenRuleTransformer() : base("NOT_BETWEEN")
    {
    }

    /// <inheritdoc />
    protected override string BuildBetweenQuery(string fieldName, string param1, string param2)
    {
        return $"{fieldName} NOT BETWEEN {param1} AND {param2}";
    }
}
