using System;
using System.Collections.Generic;

namespace Q.FilterBuilder.Core.RuleTransformers;

/// <summary>
/// Default implementation of IRuleTransformerService.
/// Manages rule transformer registration and retrieval using a dictionary-based lookup pattern.
/// </summary>
public class RuleTransformerService : IRuleTransformerService
{
    private readonly Dictionary<string, IRuleTransformer> _transformers = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the RuleTransformerService class.
    /// </summary>
    public RuleTransformerService()
    {
        RegisterBuiltInTransformers();
    }

    /// <inheritdoc />
    public IRuleTransformer GetRuleTransformer(string operatorName)
    {
        if (string.IsNullOrWhiteSpace(operatorName))
        {
            throw new ArgumentNullException(nameof(operatorName));
        }

        if (_transformers.TryGetValue(operatorName, out var transformer))
        {
            return transformer;
        }

        throw new NotImplementedException($"Rule transformer for operator '{operatorName}' is not implemented.");
    }

    /// <inheritdoc />
    public void RegisterTransformer(string operatorName, IRuleTransformer transformer)
    {
        if (string.IsNullOrWhiteSpace(operatorName))
        {
            throw new ArgumentException("Operator name cannot be null or empty.", nameof(operatorName));
        }

        if (transformer == null)
        {
            throw new ArgumentNullException(nameof(transformer));
        }

        _transformers[operatorName] = transformer;
    }

    /// <summary>
    /// Registers built-in rule transformers for basic operators.
    /// Only includes operators that are universal across all database providers.
    /// Provider-specific operators (IS NULL, IN, etc.) should be registered in provider-specific services.
    /// </summary>
    private void RegisterBuiltInTransformers()
    {
        // Register basic comparison operators using BasicRuleTransformer
        // These operators work the same across all SQL databases
        RegisterTransformer("equal", new BasicRuleTransformer("="));
        RegisterTransformer("not_equal", new BasicRuleTransformer("!="));
        RegisterTransformer("less", new BasicRuleTransformer("<"));
        RegisterTransformer("less_or_equal", new BasicRuleTransformer("<="));
        RegisterTransformer("greater", new BasicRuleTransformer(">"));
        RegisterTransformer("greater_or_equal", new BasicRuleTransformer(">="));
    }
}
