using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class BeginsWithOperatorTests
{
    private readonly BeginsWithOperator beginsWithOperator;

    public BeginsWithOperatorTests()
    {
        beginsWithOperator = new BeginsWithOperator();
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
        var result = beginsWithOperator.GetQueryPart(rule, 1);

        // Assert
        result.ShouldBe("Name.StartsWith(@1)");
    }
}