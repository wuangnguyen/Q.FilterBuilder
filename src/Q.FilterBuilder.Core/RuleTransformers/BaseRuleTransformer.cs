using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

namespace Q.FilterBuilder.Core.RuleTransformers;

/// <summary>
/// Provides a base implementation for rule transformers.
/// Contains the orchestration logic and default parameter building behavior.
/// </summary>
public abstract class BaseRuleTransformer : IRuleTransformer
{
    /// <summary>
    /// Internal context to store parameters and other transformation state.
    /// </summary>
    protected class TransformContext
    {
        public object[]? Parameters { get; set; }
        public Dictionary<string, object?>? Metadata { get; set; }
    }

    /// <inheritdoc />
    public virtual (string query, object[]? parameters) Transform(FilterRule rule, string fieldName, string parameterName)
    {
        rule.Metadata ??= new Dictionary<string, object?>();
        if (!rule.Metadata.ContainsKey("type"))
        {
            rule.Metadata["type"] = rule.Type;
        }

        var context = new TransformContext
        {
            Metadata = rule.Metadata
        };

        // Step 1: Build parameters from rule value and metadata
        context.Parameters = BuildParameters(rule.Value, rule.Metadata);

        // Step 2: Build query using field name and parameter name
        var query = BuildQuery(fieldName, parameterName, context);

        return (query, context.Parameters);
    }

    /// <summary>
    /// Builds parameters from the rule value and metadata.
    /// Default implementation returns the rule value as a single parameter.
    /// Override this method to implement custom parameter building logic.
    /// </summary>
    /// <param name="value">The rule value.</param>
    /// <param name="metadata">The rule metadata.</param>
    /// <returns>An array of parameters, or null if no parameters are needed.</returns>
    protected virtual object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            return null;
        }

        return [value];
    }

    /// <summary>
    /// Builds the query string using the field name, parameter name, and transformation context.
    /// This method must be implemented by concrete rule transformers.
    /// </summary>
    /// <param name="fieldName">The formatted field name.</param>
    /// <param name="parameterName">The formatted parameter name.</param>
    /// <param name="context">The transformation context containing parameters and metadata.</param>
    /// <returns>The query string for this rule.</returns>
    protected abstract string BuildQuery(string fieldName, string parameterName, TransformContext context);
}
