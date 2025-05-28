using Q.FilterBuilder.Core.Models;

namespace Q.FilterBuilder.Core.RuleTransformers;

/// <summary>
/// Defines the interface for rule transformers.
/// Each rule transformer handles the transformation logic for a specific operator.
/// </summary>
public interface IRuleTransformer
{
    /// <summary>
    /// Transforms a FilterRule into query string and parameters.
    /// This method orchestrates the transformation process by calling BuildParameters and BuildQuery.
    /// </summary>
    /// <param name="rule">The FilterRule to transform.</param>
    /// <param name="fieldName">The formatted field name.</param>
    /// <param name="parameterName">The formatted parameter name.</param>
    /// <returns>A tuple containing the query string and parameters array.</returns>
    (string query, object[]? parameters) Transform(FilterRule rule, string fieldName, string parameterName);
}
