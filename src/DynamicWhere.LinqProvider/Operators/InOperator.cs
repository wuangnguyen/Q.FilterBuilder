using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;
using DynamicWhere.Core.Helpers;
using System;

namespace DynamicWhere.LinqProvider.Operators;

public class InOperator : BaseOperator
{
    public override string GetQueryPart(DynamicRule rule, int parameterIndex)
    {
        return $"@{parameterIndex}.Contains({rule.FieldName})";
    }

    public override object[]? GetParametersPart(DynamicRule rule)
    {
        if (!TypeConversionHelper.IsCollection(rule.Value))
        {
            throw new InvalidOperationException("Value is not a valid collection");
        }

        var values = base.GetParametersPart(rule);

        if (values == null || values.Length == 0)
        {
            throw new ArgumentNullException(nameof(rule.Value), "Values can not be null or empty");
        }

        return [values];
    }
}
