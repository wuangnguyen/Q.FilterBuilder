namespace Q.FilterBuilder.JsonConverter;

/// <summary>
/// Configuration options for the QueryBuilderConverter to customize JSON property names.
/// Supports various query builder libraries including jQuery QueryBuilder, React QueryBuilder, and others.
/// </summary>
public class QueryBuilderOptions
{
    /// <summary>
    /// Gets or sets the property name for the logical condition (AND/OR) in groups.
    /// Default value is "condition".
    /// </summary>
    public string ConditionPropertyName { get; set; } = "condition";

    /// <summary>
    /// Gets or sets the property name for the rules array in groups.
    /// Default value is "rules".
    /// </summary>
    public string RulesPropertyName { get; set; } = "rules";

    /// <summary>
    /// Gets or sets the property name for the field name in rules.
    /// Default value is "field".
    /// </summary>
    public string FieldPropertyName { get; set; } = "field";

    /// <summary>
    /// Gets or sets the property name for the operator in rules.
    /// Default value is "operator".
    /// </summary>
    public string OperatorPropertyName { get; set; } = "operator";

    /// <summary>
    /// Gets or sets the property name for the explicit type in rules.
    /// Default value is "type".
    /// </summary>
    public string TypePropertyName { get; set; } = "type";

    /// <summary>
    /// Gets or sets the property name for the value in rules.
    /// Default value is "value".
    /// </summary>
    public string ValuePropertyName { get; set; } = "value";

    /// <summary>
    /// Gets or sets the property name for the metadata/data in rules.
    /// Default value is "data".
    /// </summary>
    public string DataPropertyName { get; set; } = "data";
}
