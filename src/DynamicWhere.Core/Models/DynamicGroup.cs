using System.Collections.Generic;

namespace DynamicWhere.Core.Models;

/// <summary>
/// Represents a group of rules and sub-groups in a dynamic query.
/// </summary>
public class DynamicGroup
{
    /// <summary>
    /// Gets or sets the list of rules in this group.
    /// </summary>
    public List<DynamicRule> Rules { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of sub-groups in this group.
    /// </summary>
    public List<DynamicGroup> Groups { get; set; } = [];

    /// <summary>
    /// Gets or sets the condition for combining rules and sub-groups (e.g., "AND", "OR").
    /// </summary>
    public string Condition { get; set; }
}