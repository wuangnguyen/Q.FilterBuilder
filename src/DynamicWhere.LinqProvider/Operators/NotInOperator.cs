using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;

namespace DynamicWhere.LinqProvider.Operators;

public class NotInOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        return $"!@{parameterIndex}.Contains({rule.FieldName})";
    }
}
