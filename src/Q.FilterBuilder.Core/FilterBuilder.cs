using System;
using System.Collections.Generic;
using System.Text;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.Providers;
using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Core;

/// <summary>
/// The main filter builder that orchestrates the building process.
/// This is the actual "FilterBuilder" that integrates with database providers.
/// </summary>
public class FilterBuilder : IFilterBuilder
{
    private readonly IQueryFormatProvider _queryFormatProvider;
    private readonly ITypeConversionService _typeConversionService;
    private readonly IRuleTransformerService _ruleTransformerService;

    /// <summary>
    /// Initializes a new instance of the FilterBuilder class with a specific query format provider.
    /// </summary>
    /// <param name="queryFormatProvider">The query format provider (SqlServer, MySQL, Postgres, etc.).</param>
    /// <param name="typeConversionService">The type conversion service. If null, a default service will be created.</param>
    /// <param name="ruleTransformerService">The rule transformer service. If null, a default service will be created.</param>
    public FilterBuilder(IQueryFormatProvider queryFormatProvider, ITypeConversionService? typeConversionService = null, IRuleTransformerService? ruleTransformerService = null)
    {
        _queryFormatProvider = queryFormatProvider ?? throw new ArgumentNullException(nameof(queryFormatProvider));
        _typeConversionService = typeConversionService ?? new TypeConversionService();
        _ruleTransformerService = ruleTransformerService ?? new RuleTransformerService();
    }

    /// <summary>
    /// Builds the final query from a FilterGroup.
    /// This is the main entry point that receives FilterGroup, traverses recursively,
    /// applies provider-specific logic, and returns query string with parameters.
    /// </summary>
    /// <param name="group">The FilterGroup to build the query from.</param>
    /// <returns>A tuple containing the parsed query and parameters array.</returns>
    public (string parsedQuery, object[] parameters) Build(FilterGroup group)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));

        var context = new BuildContext();
        var query = BuildGroup(group, context);
        return (query, context.Parameters.ToArray());
    }

    /// <summary>
    /// Recursively builds a query from a FilterGroup.
    /// </summary>
    private string BuildGroup(FilterGroup group, BuildContext context)
    {
        var queryBuilder = new StringBuilder();
        var isFirst = true;

        // Process all rules in this group
        foreach (var rule in group.Rules)
        {
            if (!isFirst)
            {
                AppendLogicalOperator(queryBuilder, group.Condition);
            }

            var ruleQuery = BuildRule(rule, context);
            queryBuilder.Append(ruleQuery);
            isFirst = false;
        }

        // Process all sub-groups recursively
        foreach (var subGroup in group.Groups)
        {
            if (!isFirst)
            {
                AppendLogicalOperator(queryBuilder, group.Condition);
            }

            queryBuilder.Append("(");
            var subQuery = BuildGroup(subGroup, context); // Recursive call
            queryBuilder.Append(subQuery);
            queryBuilder.Append(")");
            isFirst = false;
        }

        return queryBuilder.ToString();
    }

    /// <summary>
    /// Builds a query condition from a FilterRule.
    /// </summary>
    private string BuildRule(FilterRule rule, BuildContext context)
    {
        // Convert rule value based on rule.Type before rule transformation
        rule.Value = _typeConversionService.ConvertValue(rule.Value, rule.Type, rule.Metadata);

        // Get rule transformer instance
        var ruleTransformer = _ruleTransformerService.GetRuleTransformer(rule.Operator);
        var fieldName = _queryFormatProvider.FormatFieldName(rule.FieldName);
        var parameterName = _queryFormatProvider.FormatParameterName(context.ParameterIndex);

        // Transform the rule using the rule transformer
        var (query, parameters) = ruleTransformer.Transform(rule, fieldName, parameterName);

        // Update context with parameters
        if (parameters != null)
        {
            context.Parameters.AddRange(parameters);
            context.ParameterIndex += parameters.Length;
        }

        return query;
    }

    /// <summary>
    /// Appends the appropriate logical operator (AND/OR) to the query.
    /// </summary>
    private void AppendLogicalOperator(StringBuilder queryBuilder, string condition)
    {
        var logicalOperator = condition.ToUpper() switch
        {
            "AND" => _queryFormatProvider.AndOperator,
            "OR" => _queryFormatProvider.OrOperator,
            _ => condition
        };

        queryBuilder.Append($" {logicalOperator} ");
    }

    /// <summary>
    /// Context object to maintain state during query building.
    /// </summary>
    private class BuildContext
    {
        public List<object> Parameters { get; } = new();
        public int ParameterIndex { get; set; } = 0;
    }
}
