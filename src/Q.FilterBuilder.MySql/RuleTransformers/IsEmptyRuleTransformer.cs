using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.MySql.RuleTransformers;

/// <summary>
/// MySQL rule transformer for the "is_empty" operator.
/// Generates query conditions like "field = ''".
/// </summary>
public class IsEmptyRuleTransformer : SimpleNoParameterTransformer
{
    /// <inheritdoc />
    protected override string BuildSimpleQuery(string fieldName)
    {
        return $"{fieldName} = ''";
    }
}
