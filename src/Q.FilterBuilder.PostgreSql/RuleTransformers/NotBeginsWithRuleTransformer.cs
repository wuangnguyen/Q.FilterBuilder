using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.PostgreSql.RuleTransformers;

/// <summary>
/// PostgreSQL rule transformer for the "not_begins_with" operator.
/// Generates query conditions like "field NOT LIKE $1 || '%'".
/// For multiple values, generates AND conditions.
/// </summary>
public class NotBeginsWithRuleTransformer : CollectionParameterTransformer
{
    /// <summary>
    /// Initializes a new instance of the NotBeginsWithRuleTransformer class.
    /// </summary>
    public NotBeginsWithRuleTransformer() : base("NOT_BEGINS_WITH")
    {
    }

    /// <inheritdoc />
    protected override string BuildSingleCondition(string fieldName, string parameterName, int index)
    {
        return $"{fieldName} NOT LIKE {parameterName} || '%'";
    }

    /// <inheritdoc />
    protected override string GetConditionJoinOperator()
    {
        return " AND ";
    }
}
