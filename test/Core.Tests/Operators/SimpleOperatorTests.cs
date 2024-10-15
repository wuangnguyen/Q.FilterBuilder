using DynamicWhere.Core.Operators;
using DynamicWhere.Core.Models;
using Shouldly;

namespace Core.Tests.Operators;

public class SimpleOperatorTests
{
    [Theory]
    [InlineData("==")]
    [InlineData("!=")]
    [InlineData("<")]
    [InlineData("<=")]
    [InlineData(">")]
    [InlineData(">=")]
    public void SimpleOperator_GetQueryPart_ShouldReturnCorrectQuery(string @operator)
    {
        var rule = new DynamicRule { FieldName = "Age", Type = "int", Value = "25" };
        var simpleOperator = new SimpleOperator(@operator);

        var query = simpleOperator.GetQueryPart(rule, 1);
        query.ShouldBe($"{rule.FieldName} {@operator} @1");
    }

    [Fact]
    public void SimpleOperator_GetParametersPart_ShouldReturnConvertedParameter()
    {
        var rule = new DynamicRule { FieldName = "Age", Type = "int", Value = "25" };
        var simpleOperator = new SimpleOperator("=");

        var parameters = simpleOperator.GetParametersPart(rule);
        parameters.ShouldBe([25]);
    }

    [Fact]
    public void SimpleOperator_GetParametersPart_ShouldReturnConvertedArrayParameter()
    {
        var rule = new DynamicRule { FieldName = "Age", Type = "int", Value = new[] { "20", "30" } };
        var simpleOperator = new SimpleOperator("==");

        var parameters = simpleOperator.GetParametersPart(rule);
        parameters.ShouldBe([20, 30]);
    }
}
