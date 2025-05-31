using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.MySql.RuleTransformers;

/// <summary>
/// MySQL rule transformer for the "is_not_null" operator.
/// Generates query conditions like "field IS NOT NULL".
/// </summary>
public class IsNotNullRuleTransformer : SimpleNoParameterTransformer
{
    /// <inheritdoc />
    protected override string BuildSimpleQuery(string fieldName)
    {
        return $"{fieldName} IS NOT NULL";
    }
}
