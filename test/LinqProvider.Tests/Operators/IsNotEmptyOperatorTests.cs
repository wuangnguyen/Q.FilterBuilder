using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class IsNotEmptyOperatorTests
{
    private readonly IsNotEmptyOperator isNotEmptyOperator;

    public IsNotEmptyOperatorTests()
    {
        isNotEmptyOperator = new IsNotEmptyOperator();
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
        var result = isNotEmptyOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("Name != string.Empty");
    }
}