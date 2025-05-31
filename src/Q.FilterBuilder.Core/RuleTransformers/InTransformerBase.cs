using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Q.FilterBuilder.Core.RuleTransformers;

/// <summary>
/// Base class for IN and NOT IN rule transformers.
/// Provides common logic for handling collection parameters and generating parameter lists.
/// </summary>
public abstract class InTransformerBase : BaseRuleTransformer
{
    private readonly string _operatorName;

    /// <summary>
    /// Initializes a new instance of the InTransformerBase class.
    /// </summary>
    /// <param name="operatorName">The name of the operator for error messages.</param>
    protected InTransformerBase(string operatorName)
    {
        _operatorName = operatorName ?? throw new ArgumentNullException(nameof(operatorName));
    }

    /// <inheritdoc />
    protected override object[]? BuildParameters(object? value, Dictionary<string, object?>? metadata)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), $"{_operatorName} operator requires a non-null value");
        }

        // Handle array or collection values
        if (value is IEnumerable enumerable && value is not string)
        {
            var values = new List<object>();
            foreach (var item in enumerable)
            {
                values.Add(item);
            }

            if (values.Count == 0)
            {
                throw new ArgumentException($"{_operatorName} operator requires at least one value", nameof(value));
            }

            return values.ToArray();
        }

        // Handle single value - wrap in array
        return [value];
    }

    /// <inheritdoc />
    protected override string BuildQuery(string fieldName, string parameterName, TransformContext context)
    {
        if (context.Parameters == null || context.Parameters.Length == 0)
        {
            throw new InvalidOperationException($"{_operatorName} operator requires parameters");
        }

        // Generate parameter placeholders
        var parameterPlaceholders = GenerateParameterPlaceholders(parameterName, context.Parameters.Length);
        var parameterList = string.Join(", ", parameterPlaceholders);
        
        return BuildInQuery(fieldName, parameterList);
    }

    /// <summary>
    /// Generates parameter placeholders for the IN clause.
    /// </summary>
    /// <param name="parameterName">The base parameter name.</param>
    /// <param name="count">The number of parameters.</param>
    /// <returns>An array of parameter placeholder strings.</returns>
    protected virtual string[] GenerateParameterPlaceholders(string parameterName, int count)
    {
        // Check if parameterName is in the format "@0", "@1", etc. (used by NOT IN)
        if (parameterName.StartsWith("@") && int.TryParse(parameterName.TrimStart('@'), out var baseIndex))
        {
            // Generate parameter placeholders: @0, @1, @2, etc.
            return Enumerable.Range(baseIndex, count)
                .Select(index => $"@{index}")
                .ToArray();
        }

        // Default behavior: parameterName0, parameterName1, etc.
        return Enumerable.Range(0, count)
            .Select(index => $"{parameterName}{index}")
            .ToArray();
    }

    /// <summary>
    /// Builds the IN query string.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="parameterList">The comma-separated parameter list.</param>
    /// <returns>The IN query string.</returns>
    protected abstract string BuildInQuery(string fieldName, string parameterList);
}
