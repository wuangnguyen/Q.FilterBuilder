using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators
{
    public class ContainsOperatorTests
    {
        private readonly ContainsOperator containsOperator;

        public ContainsOperatorTests()
        {
            containsOperator = new ContainsOperator();
        }

        [Fact]
        public void GetParametersPart_ValidValues_ShouldReturnCorrectParameters()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "Name",
                Value = new object[] { "John", "Doe" },
                Type = "string"
            };

            // Act
            var parameters = containsOperator.GetParametersPart(rule);

            // Assert
            parameters.ShouldBe(new object[] { "John", "Doe" });
        }

        [Fact]
        public void GetParametersPart_NullValue_ShouldThrowArgumentNullException()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "Name",
                Value = null
            };

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => containsOperator.GetParametersPart(rule));
        }

        [Fact]
        public void GetParametersPart_EmptyValue_ShouldThrowArgumentNullException()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "Name",
                Value = new object[] { },
                Type = "string"
            };

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => containsOperator.GetParametersPart(rule));
        }

        [Fact]
        public void GetQueryPart_ValidValues_ShouldReturnCorrectQuery()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "Name",
                Value = new object[] { "John", "Doe" },
                Type = "string"
            };

            // Act
            var query = containsOperator.GetQueryPart(rule, 1);

            // Assert
            query.ShouldBe("(Name.Contains(@1) || Name.Contains(@2))");
        }

        [Fact]
        public void GetQueryPart_SingleValue_ShouldReturnCorrectQuery()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "Name",
                Value = new object[] { "John" },
                Type = "string"
            };

            // Act
            var query = containsOperator.GetQueryPart(rule, 1);

            // Assert
            query.ShouldBe("Name.Contains(@1)");
        }

        [Fact]
        public void GetQueryPart_NullValue_ShouldThrowArgumentNullException()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "Name",
                Value = null
            };

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => containsOperator.GetQueryPart(rule, 1));
        }

        [Fact]
        public void GetQueryPart_EmptyValue_ShouldThrowArgumentNullException()
        {
            // Arrange
            var rule = new DynamicRule
            {
                FieldName = "Name",
                Value = new object[] { },
                Type = "string"
            };

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => containsOperator.GetQueryPart(rule, 1));
        }
    }
}
