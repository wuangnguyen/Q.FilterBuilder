using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;
using System;
using System.Linq;

namespace DynamicWhere.SqlServerProvider.Operators;

public class ContainsOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        var parameters = GetParametersPart(rule);

        var likeConditions = Enumerable
            .Range(0, parameters!.Length)
            .Select(i => $"{rule.FieldName} LIKE N'%' + @{parameterIndex + i} + N'%'");

        var query = string.Join(" OR ", likeConditions);

        return parameters.Length > 1 ? $"({query})" : query;
    }
}