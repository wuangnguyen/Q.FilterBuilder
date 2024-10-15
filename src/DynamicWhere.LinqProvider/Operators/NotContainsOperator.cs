using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;
using System.Linq;

namespace DynamicWhere.LinqProvider.Operators;

public class NotContainsOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        var parameters = GetParametersPart(rule);

        var notContainsConditions = Enumerable
            .Range(0, parameters!.Length)
            .Select(i => $"!{rule.FieldName}.Contains(@{parameterIndex + i})");

        var query = string.Join(" && ", notContainsConditions);

        return parameters.Length > 1 ? $"({query})" : query;
    }
}
