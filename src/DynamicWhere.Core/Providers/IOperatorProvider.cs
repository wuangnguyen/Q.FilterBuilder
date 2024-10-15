using DynamicWhere.Core.Operators;

namespace DynamicWhere.Core.Providers;

/// <summary>
/// Defines the interface for operator providers.
/// </summary>
public interface IOperatorProvider
{
    /// <summary>
    /// Gets a value indicating whether conditions should be converted to logical operators.
    /// </summary>
    bool ConvertConditionToLogicalOperator { get; }

    /// <summary>
    /// Gets the operator instance for the given operator name.
    /// </summary>
    /// <param name="operatorName">The name of the operator.</param>
    /// <returns>An IOperator instance for the given operator name.</returns>
    IOperator GetOperator(string operatorName);

    /// <summary>
    /// Adds a custom operator to the provider.
    /// </summary>
    /// <param name="key">The key for the operator.</param>
    /// <param name="operatorInstance">The IOperator instance to add.</param>
    void AddOperator(string key, IOperator operatorInstance);
}
