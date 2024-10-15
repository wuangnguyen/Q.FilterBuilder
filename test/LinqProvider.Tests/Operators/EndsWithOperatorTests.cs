using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class EndsWithOperatorTests
{
    private readonly EndsWithOperator endsWithOperator;

    public EndsWithOperatorTests()
    {
        endsWithOperator = new EndsWithOperator();
    }

    [Fact]
    public void GetQueryPart_ShouldReturnCorrectQuery()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Name",
            Value = "Doe"
        };

        // Act
        var result = endsWithOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("Name.EndsWith(@1)");
    }
}