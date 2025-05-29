using System.Collections.Generic;

namespace Q.FilterBuilder.Core.RuleTransformers;

/// <summary>
/// Base class for rule transformers that don't require parameters.
/// Provides common logic for operators like IS NULL, IS NOT NULL, IS EMPTY, etc.
/// </summary>
public abstract class SimpleNoParameterTransformer : BaseRuleTransformer
{
    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        // These operators don't need parameters
        return null;
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        return BuildSimpleQuery(fieldName);
    }

    /// <summary>
    /// Builds the simple query string using only the field name.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <returns>The query string for this operator.</returns>
    protected abstract string BuildSimpleQuery(string fieldName);
}
