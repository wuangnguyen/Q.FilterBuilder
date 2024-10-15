using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;

namespace DynamicWhere.LinqProvider.Operators;

public class BeginsWithOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        return $"{rule.FieldName}.StartsWith(@{parameterIndex})";
    }
}
