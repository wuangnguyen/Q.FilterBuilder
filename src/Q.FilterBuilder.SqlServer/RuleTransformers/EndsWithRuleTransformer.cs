using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.RuleTransformers;

/// <summary>
/// SQL Server rule transformer for the "ends_with" operator.
/// Generates query conditions like "field LIKE N'%' + @param".
/// For multiple values, generates OR conditions.
/// </summary>
public class EndsWithRuleTransformer : CollectionParameterTransformer
{
    /// <summary>
    /// Initializes a new instance of the EndsWithRuleTransformer class.
    /// </summary>
    public EndsWithRuleTransformer() : base("ENDS_WITH")
    {
    }

    /// <inheritdoc />
    protected override string BuildSingleCondition(string fieldName, string parameterName, int index)
    {
        return $"{fieldName} LIKE N'%' + {parameterName}{index}";
    }

    /// <inheritdoc />
    protected override string GetConditionJoinOperator()
    {
        return " OR ";
    }
}
