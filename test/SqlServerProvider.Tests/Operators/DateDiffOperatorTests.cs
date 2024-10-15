using DynamicWhere.Core.Models;
using DynamicWhere.SqlServerProvider.Operators;
using Shouldly;
using System;
using System.Collections.Generic;

namespace SqlServerProvider.Tests.Operators
{
    public class DateDiffOperatorTests
    {
        private readonly DateDiffOperator _operator;

        public DateDiffOperatorTests()
        {
            _operator = new DateDiffOperator();
        }

        [Fact]
        public void GetQueryPart_WithDayInterval_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "CreatedDate",
                Data = new Dictionary<string, object?> { { "intervalType", "d" } }
            };

            // Act
            var result = _operator.GetQueryPart(rule, 0);

            // Assert
            result.ShouldBe("DATEDIFF(day, CreatedDate, GETDATE()) = @0");
        }

        [Fact]
        public void GetQueryPart_WithMonthInterval_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "CreatedDate",
                Data = new Dictionary<string, object?> { { "intervalType", "m" } }
            };

            // Act
            var result = _operator.GetQueryPart(rule, 1);

            // Assert
            result.ShouldBe("DATEDIFF(month, CreatedDate, GETDATE()) = @1");
        }

        [Fact]
        public void GetQueryPart_WithYearInterval_ShouldReturnCorrectSqlExpression()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "CreatedDate",
                Data = new Dictionary<string, object?> { { "intervalType", "y" } }
            };

            // Act
            var result = _operator.GetQueryPart(rule, 2);

            // Assert
            result.ShouldBe("DATEDIFF(year, CreatedDate, GETDATE()) = @2");
        }

        [Fact]
        public void GetQueryPart_WithInvalidIntervalType_ShouldThrowArgumentException()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "CreatedDate",
                Data = new Dictionary<string, object?> { { "intervalType", "invalid" } }
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => _operator.GetQueryPart(rule, 0))
                .Message.ShouldBe("Invalid intervalType for DateDiff operator");
        }

        [Fact]
        public void GetQueryPart_WithMissingIntervalType_ShouldThrowArgumentException()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "CreatedDate",
                Data = new Dictionary<string, object?>()
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => _operator.GetQueryPart(rule, 0))
                .Message.ShouldBe("IntervalType is required for DateDiff operator");
        }

        [Fact]
        public void GetQueryPart_WithNullData_ShouldThrowArgumentException()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "CreatedDate",
                Data = null
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => _operator.GetQueryPart(rule, 0))
                .Message.ShouldBe("IntervalType is required for DateDiff operator");
        }

        [Fact]
        public void GetQueryPart_WithEmptyData_ShouldThrowArgumentException()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "CreatedDate",
                Data = new Dictionary<string, object?>()
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => _operator.GetQueryPart(rule, 0))
                .Message.ShouldBe("IntervalType is required for DateDiff operator");
        }
    }
}