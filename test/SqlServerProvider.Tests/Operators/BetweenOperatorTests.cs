using DynamicWhere.Core.Models;
using DynamicWhere.SqlServerProvider.Operators;
using Shouldly;

namespace SqlServerProvider.Tests.Operators
{
    public class BetweenOperatorTests
    {
        [Fact]
        public void GetQueryPart_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var op = new BetweenOperator();
            var rule = new DynamicRule { FieldName = "Age" };

            // Act
            var result = op.GetQueryPart(rule, 0);

            // Assert
            result.ShouldBe("Age BETWEEN @0 AND @1");
        }

        [Fact]
        public void GetParametersPart_WithDateType_ShouldAdjustDateRange()
        {
            // Arrange
            var op = new BetweenOperator();
            var rule = new DynamicRule 
            { 
                FieldName = "Date", 
                Type = "date",
                Value = new[] { new DateTime(2023, 1, 1), new DateTime(2023, 1, 31) } 
            };

            // Act
            var result = op.GetParametersPart(rule)!;

            // Assert
            result.Length.ShouldBe(2);
            result[0].ShouldBe(new DateTime(2023, 1, 1));
            result[1].ShouldBe(new DateTime(2023, 1, 31).Date.AddDays(1).AddTicks(-1));
        }

        [Fact]
        public void GetParametersPart_WithDateTypeAndCustomFormat_ShouldAdjustDateRange()
        {
            var op = new BetweenOperator();
            var rule = new DynamicRule
            {
                FieldName = "Date",
                Type = "date",
                Data = new Dictionary<string, object>
                {
                    { "datetimeFormat", "dd/MM/yyyy" }
                },
                Value = new object[] { "01/01/2023", "31/01/2023" },
            };
            var result = op.GetParametersPart(rule)!;
            Assert.Equal(2, result.Length);
            Assert.Equal(new DateTime(2023, 1, 1), result[0]);
            Assert.Equal(new DateTime(2023, 2, 1).AddTicks(-1), result[1]);
        }
    }
}