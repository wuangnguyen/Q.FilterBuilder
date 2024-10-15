using DynamicWhere.Core.Models;
using DynamicWhere.SqlServerProvider.Operators;
using Shouldly;

namespace SqlServerProvider.Tests.Operators
{
    public class IsNotEmptyOperatorTests
    {
        [Fact]
        public void GetQueryPart_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var op = new IsNotEmptyOperator();
            var rule = new DynamicRule { FieldName = "Name" };

            // Act
            var result = op.GetQueryPart(rule, 0);

            // Assert
            result.ShouldBe("Name <> ''");
        }
    }
}