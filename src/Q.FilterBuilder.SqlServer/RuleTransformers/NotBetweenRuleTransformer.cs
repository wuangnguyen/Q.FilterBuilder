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
    protected override string BuildBetweenQuery(string fieldName, string parameterName)
    {
        return $"{fieldName} NOT BETWEEN {parameterName}0 AND {parameterName}1";
    }
}
