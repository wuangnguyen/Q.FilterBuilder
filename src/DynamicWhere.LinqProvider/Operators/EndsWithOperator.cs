using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;

namespace DynamicWhere.LinqProvider.Operators;

public class EndsWithOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        return $"{rule.FieldName}.EndsWith(@{parameterIndex})";
    }
}
