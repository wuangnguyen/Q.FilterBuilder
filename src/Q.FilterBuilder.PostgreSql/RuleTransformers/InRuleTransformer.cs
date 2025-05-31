using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.PostgreSql.RuleTransformers;

/// <summary>
/// PostgreSQL rule transformer for the "in" operator.
/// Generates query conditions like "field IN ($1, $2, $3)".
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
}
