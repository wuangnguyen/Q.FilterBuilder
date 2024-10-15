using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class IsNotNullOperatorTests
{
    private readonly IsNotNullOperator isNotNullOperator;

    public IsNotNullOperatorTests()
    {
        isNotNullOperator = new IsNotNullOperator();
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
        var result = isNotNullOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("Name != null");
    }
}