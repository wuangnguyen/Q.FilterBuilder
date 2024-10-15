using DynamicWhere.Core.Models;
using DynamicWhere.SqlServerProvider.Operators;
using Shouldly;

namespace SqlServerProvider.Tests.Operators
{
    public class ContainsOperatorTests
    {
        [Fact]
        public void GetQueryPart_SingleValue_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var op = new ContainsOperator();
            var rule = new DynamicRule { FieldName = "Name", Value = "John" };

            // Act
            var result = op.GetQueryPart(rule, 0);

            // Assert
            result.ShouldBe("Name LIKE N'%' + @0 + N'%'");
        }

        [Fact]
        public void GetQueryPart_MultipleValues_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var op = new ContainsOperator();
            var rule = new DynamicRule { FieldName = "Name", Value = new[] { "John", "Doe" } };

            // Act
            var result = op.GetQueryPart(rule, 0);

            // Assert
            result.ShouldBe("(Name LIKE N'%' + @0 + N'%' OR Name LIKE N'%' + @1 + N'%')");
        }
    }
}