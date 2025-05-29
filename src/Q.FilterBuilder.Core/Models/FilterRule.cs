using System;
using System.Collections.Generic;

namespace Q.FilterBuilder.Core.Models;

/// <summary>
/// Represents a single rule in a filter query.
/// </summary>
public class FilterRule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterRule"/> class.
    /// </summary>
    /// <param name="fieldName">The field name for the rule.</param>
    /// <param name="operator">The operator for the rule.</param>
    /// <param name="value">The value for the rule.</param>
    /// <param name="explicitType">The explicit type for the rule. If null or empty, no type conversion will be applied.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the required parameters are null or whitespace.</exception>
    public FilterRule(string fieldName, string @operator, object? value, string? explicitType = null)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentNullException(nameof(fieldName), "FieldName cannot be null or whitespace.");
        }

        if (string.IsNullOrWhiteSpace(@operator))
        {
            throw new ArgumentNullException(nameof(@operator), "Operator cannot be null or whitespace.");
        }

        FieldName = fieldName;
        Operator = @operator;
        Value = value;
        Type = explicitType ?? string.Empty;
    }

    /// <summary>
    /// Gets the field name for the rule.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the operator for the rule.
    /// </summary>
    public string Operator { get; }

    /// <summary>
    /// Gets the data type for the rule's value.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the value for the rule.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets additional metadata associated with the rule.
    /// </summary>
    public Dictionary<string, object?>? Metadata { get; set; }
}
