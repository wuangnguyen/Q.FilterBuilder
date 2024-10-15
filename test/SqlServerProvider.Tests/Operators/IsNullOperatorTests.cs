using DynamicWhere.Core.Models;
using DynamicWhere.SqlServerProvider.Operators;
using Shouldly;

namespace SqlServerProvider.Tests.Operators
{
    public class IsNullOperatorTests
    {
        [Fact]
        public void GetQueryPart_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var op = new IsNullOperator();
            var rule = new DynamicRule { FieldName = "Name" };

            // Act
            var result = op.GetQueryPart(rule, 0);

            // Assert
            result.ShouldBe("Name IS NULL");
        }
    }
}