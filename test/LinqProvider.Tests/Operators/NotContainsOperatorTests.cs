using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class NotContainsOperatorTests
{
    private readonly NotContainsOperator notContainsOperator;

    public NotContainsOperatorTests()
    {
        notContainsOperator = new NotContainsOperator();
    }

    [Fact]
    public void GetQueryPart_ShouldReturnCorrectQuery()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Name",
            Value = "John"
        };

        // Act
        var result = notContainsOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("!Name.Contains(@1)");
    }
}