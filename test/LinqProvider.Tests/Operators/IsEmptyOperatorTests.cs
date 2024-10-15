using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class IsEmptyOperatorTests
{
    private readonly IsEmptyOperator isEmptyOperator;

    public IsEmptyOperatorTests()
    {
        isEmptyOperator = new IsEmptyOperator();
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
        var result = isEmptyOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("Name == string.Empty");
    }
}