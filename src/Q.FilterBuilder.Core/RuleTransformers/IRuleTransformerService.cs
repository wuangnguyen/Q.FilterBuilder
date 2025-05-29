namespace Q.FilterBuilder.Core.RuleTransformers;

/// <summary>
/// Provides rule transformer services for FilterBuilder.
/// Manages the registration and retrieval of rule transformers.
/// </summary>
public interface IRuleTransformerService
{
    /// <summary>
    /// Gets a rule transformer for the specified operator name.
    /// </summary>
    /// <param name="operatorName">The name of the operator.</param>
    /// <returns>An IRuleTransformer instance for the given operator name.</returns>
    IRuleTransformer GetRuleTransformer(string operatorName);

    /// <summary>
    /// Registers a rule transformer for a specific operator name.
    /// </summary>
    /// <param name="operatorName">The operator name identifier.</param>
    /// <param name="transformer">The transformer instance.</param>
    void RegisterTransformer(string operatorName, IRuleTransformer transformer);
}
