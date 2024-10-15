using DynamicWhere.Core.Operators;
using System;
using System.Collections.Generic;

namespace DynamicWhere.Core.Providers;

/// <summary>
/// Provides a base implementation for operator providers.
/// </summary>
public class BaseOperatorProvider : IOperatorProvider
{
    private static readonly Dictionary<string, IOperator> operators = new(StringComparer.OrdinalIgnoreCase)
    {
        { "equal", new SimpleOperator("==") },
        { "not_equal", new SimpleOperator("!=") },
        { "less", new SimpleOperator("<") },
        { "less_or_equal", new SimpleOperator("<=") },
        { "greater", new SimpleOperator(">") },
        { "greater_or_equal", new SimpleOperator(">=") }
    };

    /// <summary>
    /// Gets a value indicating whether conditions should be converted to logical operators.
    /// </summary>
    public virtual bool ConvertConditionToLogicalOperator { get => false; }

    /// <summary>
    /// Gets the operator instance for the given operator name.
    /// </summary>
    /// <param name="operatorName">The name of the operator.</param>
    /// <returns>An IOperator instance for the given operator name.</returns>
    /// <exception cref="NotImplementedException">Thrown when the operator is not implemented.</exception>
    public virtual IOperator GetOperator(string operatorName)
    {
        if (operators.TryGetValue(operatorName, out var @operator))
        {
            return @operator;
        }

        throw new NotImplementedException($"Operator '{operatorName}' is not implemented.");
    }

    /// <summary>
    /// Adds a custom operator to the provider.
    /// </summary>
    /// <param name="key">The key for the operator.</param>
    /// <param name="operatorInstance">The IOperator instance to add.</param>
    public void AddOperator(string key, IOperator operatorInstance)
    {
        operators[key] = operatorInstance;
    }
}