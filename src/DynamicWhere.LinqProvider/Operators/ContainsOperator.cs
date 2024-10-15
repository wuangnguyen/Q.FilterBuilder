using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;
using System;
using System.Linq;

namespace DynamicWhere.LinqProvider.Operators;

public class ContainsOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        var parameters = GetParametersPart(rule);

        var containsConditions = Enumerable
            .Range(0, parameters!.Length)
            .Select(i => $"{rule.FieldName}.Contains(@{parameterIndex + i})");

        var query = string.Join(" || ", containsConditions);

        return parameters.Length > 1 ? $"({query})" : query;
    }

    public override object[]? GetParametersPart(DynamicRule rule)
    {
        var parameters = base.GetParametersPart(rule);
        if (parameters == null || parameters.Length == 0)
        {
            throw new ArgumentNullException(nameof(rule.Value), "Values can not be null or empty");
        }

        return parameters;
    }
}
