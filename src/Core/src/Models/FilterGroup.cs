using System;
using System.Collections.Generic;

namespace Q.FilterBuilder.Core.Models;

/// <summary>
/// Represents a group of rules and sub-groups in a filter query.
/// </summary>
public class FilterGroup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterGroup"/> class with the required condition.
    /// </summary>
    /// <param name="condition">The condition for combining rules and sub-groups (e.g., "AND", "OR").</param>
    /// <exception cref="ArgumentNullException">Thrown when the condition is null or whitespace.</exception>
    public FilterGroup(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
            throw new ArgumentNullException(
                nameof(condition),
                "Condition cannot be null or whitespace."
            );

        Condition = condition;
        Rules = [];
        Groups = [];
    }

    /// <summary>
    /// Gets the condition for combining rules and sub-groups (e.g., "AND", "OR").
    /// </summary>
    public string Condition { get; }

    /// <summary>
    /// Gets the list of rules in this group.
    /// </summary>
    public List<FilterRule> Rules { get; }

    /// <summary>
    /// Gets the list of sub-groups in this group.
    /// </summary>
    public List<FilterGroup> Groups { get; }
}
