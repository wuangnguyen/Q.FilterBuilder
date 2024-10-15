using DynamicWhere.Core.Models;
using DynamicWhere.SqlServerProvider.Operators;
using Shouldly;

namespace SqlServerProvider.Tests.Operators
{
    public class BeginsWithOperatorTests
    {
        [Fact]
        public void GetQueryPart_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var op = new BeginsWithOperator();
            var rule = new DynamicRule { FieldName = "Name" };

            // Act
            var result = op.GetQueryPart(rule, 0);

            // Assert
            result.ShouldBe("Name LIKE @0 + N'%'");
        }
    }
}