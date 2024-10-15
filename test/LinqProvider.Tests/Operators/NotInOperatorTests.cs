using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class NotInOperatorTests
{
    private readonly NotInOperator notInOperator;

    public NotInOperatorTests()
    {
        notInOperator = new NotInOperator();
    }

    [Fact]
    public void GetQueryPart_ShouldReturnCorrectQuery()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Name",
            Value = new[] { "John", "Jane" }
        };

        // Act
        var result = notInOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("!@1.Contains(Name)");
    }
}