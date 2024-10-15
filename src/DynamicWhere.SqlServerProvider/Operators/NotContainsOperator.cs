using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;
using System.Linq;

namespace DynamicWhere.SqlServerProvider.Operators;

public class NotContainsOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        var parameters = GetParametersPart(rule);

        var notLikeConditions = Enumerable
            .Range(0, parameters!.Length)
            .Select(i => $"{rule.FieldName} NOT LIKE N'%' + @{parameterIndex + i} + N'%'");

        var query = string.Join(" AND ", notLikeConditions);

        return parameters.Length > 1 ? $"({query})" : query;
    }
}