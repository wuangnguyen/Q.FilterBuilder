using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class NotBeginsWithOperatorTests
{
    private readonly NotBeginsWithOperator notBeginsWithOperator;

    public NotBeginsWithOperatorTests()
    {
        notBeginsWithOperator = new NotBeginsWithOperator();
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
        var result = notBeginsWithOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("!Name.StartsWith(@1)");
    }
}