using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.RuleTransformers;

/// <summary>
/// SQL Server rule transformer for the "not_between" operator.
/// Generates query conditions like "field NOT BETWEEN @param0 AND @param1".
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
