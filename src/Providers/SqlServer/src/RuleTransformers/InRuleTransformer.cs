using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.RuleTransformers;

/// <summary>
/// SQL Server rule transformer for the "in" operator.
/// Generates query conditions like "field IN (@param0, @param1, @param2)".
/// </summary>
public class InRuleTransformer : InTransformerBase
{
    /// <summary>
    /// Initializes a new instance of the InRuleTransformer class.
    /// </summary>
    public InRuleTransformer() : base("IN")
    {
    }

    /// <inheritdoc />
    protected override string BuildInQuery(string fieldName, string parameterList)
    {
        return $"{fieldName} IN ({parameterList})";
    }
}
