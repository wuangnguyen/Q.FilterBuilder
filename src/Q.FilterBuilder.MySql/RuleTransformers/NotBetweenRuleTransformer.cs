using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.MySql.RuleTransformers;

/// <summary>
/// MySQL rule transformer for the "not_between" operator.
/// Generates query conditions like "field NOT BETWEEN ? AND ?".
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
        return $"{fieldName} NOT BETWEEN ? AND ?";
    }
}
