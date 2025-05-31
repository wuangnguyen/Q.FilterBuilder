using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Q.FilterBuilder.Core.RuleTransformers;

/// <summary>
/// Base class for rule transformers that handle collection parameters.
/// Provides common logic for processing single values and collections.
/// </summary>
public abstract class CollectionParameterTransformer : BaseRuleTransformer
{
    private readonly string _operatorName;

    /// <summary>
    /// Initializes a new instance of the CollectionParameterTransformer class.
    /// </summary>
    /// <param name="operatorName">The name of the operator for error messages.</param>
    protected CollectionParameterTransformer(string operatorName)
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

        // Generate conditions for each parameter
        var conditions = context.Parameters
            .Select((_, index) => BuildSingleCondition(fieldName, parameterName, index))
            .ToArray();

        // If multiple conditions, wrap in parentheses and join with appropriate operator
        if (conditions.Length > 1)
        {
            var combinedConditions = string.Join(GetConditionJoinOperator(), conditions);
            return $"({combinedConditions})";
        }

        return conditions[0];
    }

    /// <summary>
    /// Builds a single condition for the specified parameter index.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="parameterName">The base parameter name.</param>
    /// <param name="index">The parameter index.</param>
    /// <returns>The condition string for this parameter.</returns>
    protected abstract string BuildSingleCondition(string fieldName, string parameterName, int index);

    /// <summary>
    /// Gets the operator used to join multiple conditions (e.g., " OR ", " AND ").
    /// </summary>
    /// <returns>The join operator string.</returns>
    protected abstract string GetConditionJoinOperator();
}
