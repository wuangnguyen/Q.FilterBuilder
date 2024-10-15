using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;
using System;
using DynamicWhere.Core.Helpers;

namespace DynamicWhere.SqlServerProvider.Operators;

public class BetweenOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        return $"{rule.FieldName} BETWEEN @{parameterIndex} AND @{++parameterIndex}";
    }

    public override object[]? GetParametersPart(DynamicRule rule)
    {
        if (rule.Value == null || !TypeConversionHelper.IsCollection(rule.Value))
        {
            throw new ArgumentNullException(nameof(rule.Value), $"Value of {rule.FieldName} is not valid.");
        }

        var parameters = base.GetParametersPart(rule)!;

        if (rule.Type == "date")
        {
            var firstValue = DateTime.SpecifyKind(((DateTime)parameters[0]).Date, DateTimeKind.Unspecified);
            var secondValue = DateTime.SpecifyKind(((DateTime)parameters[1]).Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified);

            return [firstValue, secondValue];
        }

        return parameters;
    }
}