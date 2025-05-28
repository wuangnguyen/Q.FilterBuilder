using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.RuleTransformers;

/// <summary>
/// SQL Server rule transformer for the "begins_with" operator.
/// Generates query conditions like "field LIKE @param + N'%'".
/// For multiple values, generates OR conditions.
/// </summary>
public class BeginsWithRuleTransformer : CollectionParameterTransformer
{
    /// <summary>
    /// Initializes a new instance of the BeginsWithRuleTransformer class.
    /// </summary>
    public BeginsWithRuleTransformer() : base("BEGINS_WITH")
    {
    }

    /// <inheritdoc />
    protected override string BuildSingleCondition(string fieldName, string parameterName, int index)
    {
        return $"{fieldName} LIKE {parameterName}{index} + N'%'";
    }

    /// <inheritdoc />
    protected override string GetConditionJoinOperator()
    {
        return " OR ";
    }
}
