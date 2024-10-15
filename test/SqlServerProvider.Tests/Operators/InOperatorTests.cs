using DynamicWhere.Core.Models;
using DynamicWhere.SqlServerProvider.Operators;
using Shouldly;

namespace SqlServerProvider.Tests.Operators
{
    public class InOperatorTests
    {
        [Fact]
        public void GetQueryPart_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var op = new InOperator();
            var rule = new DynamicRule { FieldName = "Status" };

            // Act
            var result = op.GetQueryPart(rule, 0);

            // Assert
            result.ShouldBe("Status IN (@0)");
        }
    }
}