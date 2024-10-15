using DynamicWhere.Core.Models;
using DynamicWhere.Core.Operators;
using System;
using System.Collections.Generic;

namespace DynamicWhere.SqlServerProvider.Operators;

public class DateDiffOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        if (rule.Data is Dictionary<string, object?> extraInfo && extraInfo.TryGetValue("intervalType", out var intervalType))
        {
            return (intervalType?.ToString()) switch
            {
                "d" => $"DATEDIFF(day, {rule.FieldName}, GETDATE()) = @{parameterIndex}",
                "m" => $"DATEDIFF(month, {rule.FieldName}, GETDATE()) = @{parameterIndex}",
                "y" => $"DATEDIFF(year, {rule.FieldName}, GETDATE()) = @{parameterIndex}",
                _ => throw new ArgumentException("Invalid intervalType for DateDiff operator"),
            };
        }
        throw new ArgumentException("IntervalType is required for DateDiff operator");
    }
}
