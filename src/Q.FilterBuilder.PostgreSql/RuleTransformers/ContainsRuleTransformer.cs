using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.PostgreSql.RuleTransformers;

/// <summary>
/// PostgreSQL rule transformer for the "contains" operator.
/// Generates query conditions like "field LIKE '%' || $1 || '%'".
/// For multiple values, generates OR conditions.
/// </summary>
public class ContainsRuleTransformer : CollectionParameterTransformer
{
    /// <summary>
    /// Initializes a new instance of the ContainsRuleTransformer class.
    /// </summary>
    public ContainsRuleTransformer() : base("CONTAINS")
    {
    }

    /// <inheritdoc />
    protected override string BuildSingleCondition(string fieldName, string parameterName, int index)
    {
        return $"{fieldName} LIKE '%' || {parameterName} || '%'";
    }

    /// <inheritdoc />
    protected override string GetConditionJoinOperator()
    {
        return " OR ";
    }
}
