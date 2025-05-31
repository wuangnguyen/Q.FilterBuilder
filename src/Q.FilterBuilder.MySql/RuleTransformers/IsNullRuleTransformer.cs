using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.MySql.RuleTransformers;

/// <summary>
/// MySQL rule transformer for the "is_null" operator.
/// Generates query conditions like "field IS NULL".
/// </summary>
public class IsNullRuleTransformer : SimpleNoParameterTransformer
{
    /// <inheritdoc />
    protected override string BuildSimpleQuery(string fieldName)
    {
        return $"{fieldName} IS NULL";
    }
}
