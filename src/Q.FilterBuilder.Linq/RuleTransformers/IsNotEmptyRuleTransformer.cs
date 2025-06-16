using System.Collections.Generic;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Linq.RuleTransformers;

/// <summary>
/// LINQ rule transformer for the "is_not_empty" operator.
/// Generates query conditions like "field != string.Empty".
/// </summary>
public class IsNotEmptyRuleTransformer : BaseRuleTransformer
{
    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        // IS NOT EMPTY operator doesn't need parameters in LINQ
        return null;
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, TransformContext context)
    {
        return $"{fieldName} != string.Empty";
    }
}
