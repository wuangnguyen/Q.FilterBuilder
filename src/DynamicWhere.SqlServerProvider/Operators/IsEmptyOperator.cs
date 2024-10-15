using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;

namespace DynamicWhere.SqlServerProvider.Operators;

public class IsEmptyOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        return $"{rule.FieldName} = ''";
    }
}