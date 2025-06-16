using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.RuleTransformers;

/// <summary>
/// SQL Server rule transformer for the "between" operator.
/// Generates query conditions like "field BETWEEN @param0 AND @param1".
/// </summary>
public class BetweenRuleTransformer : BetweenTransformerBase
{
    /// <summary>
    /// Initializes a new instance of the BetweenRuleTransformer class.
    /// </summary>
    public BetweenRuleTransformer() : base("BETWEEN")
    {
    }


}
