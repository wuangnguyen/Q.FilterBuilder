using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;

namespace DynamicWhere.SqlServerProvider.Operators;

public class EndsWithOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        return $"{rule.FieldName} LIKE N'%' + @{parameterIndex}";
    }
}