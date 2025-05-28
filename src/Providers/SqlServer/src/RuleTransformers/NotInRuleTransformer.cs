using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.RuleTransformers;

/// <summary>
/// SQL Server rule transformer for the "not_in" operator.
/// Generates query conditions like "field NOT IN (@param0, @param1, @param2)".
/// </summary>
public class NotInRuleTransformer : InTransformerBase
{
    /// <summary>
    /// Initializes a new instance of the NotInRuleTransformer class.
    /// </summary>
    public NotInRuleTransformer() : base("NOT_IN")
    {
    }

    /// <inheritdoc />
    protected override string BuildInQuery(string fieldName, string parameterList)
    {
        return $"{fieldName} NOT IN ({parameterList})";
    }
}
