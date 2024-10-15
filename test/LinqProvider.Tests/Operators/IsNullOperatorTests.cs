using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class IsNullOperatorTests
{
    private readonly IsNullOperator isNullOperator;

    public IsNullOperatorTests()
    {
        isNullOperator = new IsNullOperator();
    }

    [Fact]
    public void GetQueryPart_ShouldReturnCorrectQuery()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Name"
        };

        // Act
        var result = isNullOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("Name == null");
    }
}