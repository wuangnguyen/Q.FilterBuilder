using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.RuleTransformers;

/// <summary>
/// SQL Server rule transformer for the "not_contains" operator.
/// Generates query conditions like "field NOT LIKE '%' + @param + '%'".
/// For multiple values, generates AND conditions.
/// </summary>
public class NotContainsRuleTransformer : CollectionParameterTransformer
{
    /// <summary>
    /// Initializes a new instance of the NotContainsRuleTransformer class.
    /// </summary>
    public NotContainsRuleTransformer() : base("NOT_CONTAINS")
    {
    }

    /// <inheritdoc />
    protected override string BuildSingleCondition(string fieldName, string parameterName, int index)
    {
        return $"{fieldName} NOT LIKE '%' + {parameterName} + '%'";
    }

    /// <inheritdoc />
    protected override string GetConditionJoinOperator()
    {
        return " AND ";
    }
}
