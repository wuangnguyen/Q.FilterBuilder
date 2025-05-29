using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

namespace Q.FilterBuilder.Core;

/// <summary>
/// A fluent rule builder for creating dynamic rule structures.
/// This builds the rule structure that will be passed to FilterBuilder (the actual query builder).
/// </summary>
public class FluentRuleBuilder
{
    private readonly List<FilterRule> _rules = new();
    private readonly List<FilterGroup> _groups = new();
    private readonly Stack<FilterGroup> _groupStack = new();

    /// <summary>
    /// Adds a where condition to the rule structure.
    /// </summary>
    /// <param name="fieldName">The field name.</param>
    /// <param name="operator">The operator (e.g., "equal", "greater", "contains").</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="explicitType">The explicit type for the rule. If null or empty, no type conversion will be applied.</param>
    /// <returns>The fluent rule builder for method chaining.</returns>
    public FluentRuleBuilder Where(string fieldName, string @operator, object? value, string? explicitType = null)
    {
        var rule = new FilterRule(fieldName, @operator, value, explicitType);

        if (_groupStack.Count > 0)
        {
            _groupStack.Peek().Rules.Add(rule);
        }
        else
        {
            _rules.Add(rule);
        }

        return this;
    }

    /// <summary>
    /// Begins a logical group with the specified condition.
    /// </summary>
    /// <param name="condition">The logical condition ("AND" or "OR").</param>
    /// <returns>The fluent rule builder for method chaining.</returns>
    public FluentRuleBuilder BeginGroup(string condition = "AND")
    {
        var group = new FilterGroup(condition);
        _groupStack.Push(group);
        return this;
    }

    /// <summary>
    /// Ends the current logical group.
    /// </summary>
    /// <returns>The fluent rule builder for method chaining.</returns>
    public FluentRuleBuilder EndGroup()
    {
        if (_groupStack.Count == 0)
            throw new InvalidOperationException("No group to end. Call BeginGroup first.");

        var completedGroup = _groupStack.Pop();

        if (_groupStack.Count > 0)
        {
            _groupStack.Peek().Groups.Add(completedGroup);
        }
        else
        {
            _groups.Add(completedGroup);
        }

        return this;
    }

    /// <summary>
    /// Builds the FilterGroup structure from the fluent rules and groups.
    /// This is the primary responsibility of FluentRuleBuilder - building rule structures, not generating queries.
    /// </summary>
    /// <param name="condition">The root condition for combining top-level rules and groups (default: "AND").</param>
    /// <returns>The FilterGroup containing all rules and groups.</returns>
    public FilterGroup Build(string condition = "AND")
    {
        if (_groupStack.Count > 0)
            throw new InvalidOperationException($"Unclosed groups detected. Call EndGroup() {_groupStack.Count} more time(s).");

        var rootGroup = new FilterGroup(condition);

        // Add all top-level rules
        foreach (var rule in _rules)
        {
            rootGroup.Rules.Add(rule);
        }

        // Add all completed groups
        foreach (var group in _groups)
        {
            rootGroup.Groups.Add(group);
        }

        return rootGroup;
    }

    /// <summary>
    /// Concatenates two FilterGroups with the specified condition.
    /// </summary>
    /// <param name="group1">The first FilterGroup to concatenate.</param>
    /// <param name="group2">The second FilterGroup to concatenate.</param>
    /// <param name="condition">The logical condition to use for concatenation ("AND" or "OR").</param>
    /// <returns>A new FilterGroup containing both groups combined with the specified condition.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either group is null.</exception>
    public static FilterGroup ConcatGroups(FilterGroup group1, FilterGroup group2, string condition = "AND")
    {
        if (group1 == null)
        {
            throw new ArgumentNullException(nameof(group1));
        }

        if (group2 == null)
        {
            throw new ArgumentNullException(nameof(group2));
        }

        var combinedGroup = new FilterGroup(condition);
        combinedGroup.Groups.Add(group1);
        combinedGroup.Groups.Add(group2);

        return combinedGroup;
    }

    /// <summary>
    /// Adds an existing FilterGroup to the current builder with the specified condition.
    /// </summary>
    /// <param name="group">The FilterGroup to add.</param>
    /// <returns>The fluent rule builder for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when group is null.</exception>
    public FluentRuleBuilder AddGroup(FilterGroup group)
    {
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        if (_groupStack.Count > 0)
        {
            _groupStack.Peek().Groups.Add(group);
        }
        else
        {
            _groups.Add(group);
        }

        return this;
    }

    /// <summary>
    /// Clears all rules and groups from the builder.
    /// </summary>
    /// <returns>The fluent rule builder for method chaining.</returns>
    public FluentRuleBuilder Clear()
    {
        _rules.Clear();
        _groups.Clear();
        _groupStack.Clear();
        return this;
    }
}
