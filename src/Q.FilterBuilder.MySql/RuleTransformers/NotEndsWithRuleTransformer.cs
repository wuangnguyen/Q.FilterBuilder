using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.MySql.RuleTransformers;

/// <summary>
/// MySQL rule transformer for the "not_ends_with" operator.
/// Generates query conditions like "field NOT LIKE CONCAT('%', ?)".
/// For multiple values, generates AND conditions.
/// </summary>
public class NotEndsWithRuleTransformer : CollectionParameterTransformer
{
    /// <summary>
    /// Initializes a new instance of the NotEndsWithRuleTransformer class.
    /// </summary>
    public NotEndsWithRuleTransformer() : base("NOT_ENDS_WITH")
    {
    }

    /// <inheritdoc />
    protected override string BuildSingleCondition(string fieldName, string parameterName, int index)
    {
        return $"{fieldName} NOT LIKE CONCAT('%', ?)";
    }

    /// <inheritdoc />
    protected override string GetConditionJoinOperator()
    {
        return " AND ";
    }
}
