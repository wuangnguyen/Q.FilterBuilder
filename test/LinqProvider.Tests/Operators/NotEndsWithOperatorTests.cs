using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class NotEndsWithOperatorTests
{
    private readonly NotEndsWithOperator notEndsWithOperator;

    public NotEndsWithOperatorTests()
    {
        notEndsWithOperator = new NotEndsWithOperator();
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
        var result = notEndsWithOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("!Name.EndsWith(@1)");
    }
}