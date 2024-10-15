namespace DynamicWhere.Core.Models;

/// <summary>
/// Represents a single rule in a dynamic query.
/// </summary>
public class DynamicRule
{
    /// <summary>
    /// Gets or sets the field name for the rule.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Gets or sets the operator for the rule.
    /// </summary>
    public string Operator { get; set; }

    /// <summary>
    /// Gets or sets the value for the rule.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets the data type for the rule's value.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets additional data associated with the rule.
    /// </summary>
    public object? Data { get; set; }
}
