using DynamicWhere.Core.Providers;
using DynamicWhere.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWhere.Core;

/// <summary>
/// Builds dynamic WHERE clauses based on DynamicGroup input.
/// </summary>
public class DynamicWhereBuilder : IDynamicWhereBuilder
{
    private readonly IOperatorProvider operatorProvider;

    /// <summary>
    /// Initializes a new instance of the DynamicWhereBuilder class.
    /// </summary>
    /// <param name="operatorProvider">The operator provider to use. If null, a BaseOperatorProvider will be used.</param>
    public DynamicWhereBuilder(IOperatorProvider? operatorProvider = null)
    {
        operatorProvider ??= new BaseOperatorProvider();
        this.operatorProvider = operatorProvider;
    }

    /// <summary>
    /// Builds a WHERE clause based on the provided DynamicGroup.
    /// </summary>
    /// <param name="group">The DynamicGroup to build the WHERE clause from.</param>
    /// <returns>A tuple containing the parsed query string and an array of parameters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input group is null.</exception>
    public (string parsedQuery, object[] parameters) Build(DynamicGroup group)
    {
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group), "Input can not be null.");
        }

        var queryBuilder = new StringBuilder();
        var parameters = new List<object>();
        var parameterIndex = 0;

        (string, object[], int) BuildInternal(DynamicGroup currentGroup, int startParameterIndex)
        {
            var localQueryBuilder = new StringBuilder();
            var localParameters = new List<object>();
            var localParameterIndex = startParameterIndex;
            bool isFirst = true;

            void AppendCondition(string condition)
            {
                if (!isFirst)
                {
                    var logicalOperator = operatorProvider.ConvertConditionToLogicalOperator
                        ? (condition.ToUpper() == "AND" ? "&&" : "||")
                        : condition;
                    localQueryBuilder.Append($" {logicalOperator} ");
                }
            }

            void ParseRule(DynamicRule rule)
            {
                var @operator = operatorProvider.GetOperator(rule.Operator);
                var @query = @operator.GetQueryPart(rule, localParameterIndex);
                var @params = @operator.GetParametersPart(rule);

                localQueryBuilder.Append(@query);

                if (@params == null)
                {
                    return;
                }

                localParameters.AddRange(@params!);
                localParameterIndex += @params.Length;
            }

            foreach (var rule in currentGroup.Rules)
            {
                AppendCondition(currentGroup.Condition);
                ParseRule(rule);
                isFirst = false;
            }

            foreach (var subGroup in currentGroup.Groups)
            {
                AppendCondition(currentGroup.Condition);
                localQueryBuilder.Append("(");
                var (subQuery, subParams, subIndex) = BuildInternal(subGroup, localParameterIndex);
                localQueryBuilder.Append(subQuery);
                localQueryBuilder.Append(")");
                localParameters.AddRange(subParams);
                localParameterIndex = subIndex;
                isFirst = false;
            }

            return (localQueryBuilder.ToString(), localParameters.ToArray(), localParameterIndex);
        }

        var (finalQuery, finalParameters, _) = BuildInternal(group, parameterIndex);
        return (finalQuery, finalParameters);
    }
}
