using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class BetweenOperatorTests
{
    private readonly BetweenOperator betweenOperator;

    public BetweenOperatorTests()
    {
        betweenOperator = new BetweenOperator();
    }

    [Fact]
    public void GetParametersPart_ValidRange_ShouldReturnCorrectParameters()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = new object[] { 18, 30 },
            Type = "int"
        };

        // Act
        var parameters = betweenOperator.GetParametersPart(rule);

        // Assert
        parameters.ShouldBe([18, 30]);
    }

    [Fact]
    public void GetParametersPart_NullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = null
        };

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => betweenOperator.GetParametersPart(rule));
        exception.ParamName.ShouldBe(nameof(rule.Value));
    }

    [Fact]
    public void GetParametersPart_ShouldThrowArgumentException_WhenValueIsNotAnArray()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = 18 // Invalid, should be an array
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => betweenOperator.GetParametersPart(rule));
    }

    [Fact]
    public void GetQueryPart_ValidRange_ShouldReturnCorrectQuery()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = new object[] { 18, 30 },
            Type = "int"
        };

        // Act
        var query = betweenOperator.GetQueryPart(rule, 1);

        // Assert
        query.ShouldBe("Age >= @1 && Age <= @2");
    }

    [Fact]
    public void GetQueryPart_MultipleParameters_ShouldIncrementParameterIndexCorrectly()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Price",
            Value = new object[] { 100.5, 200.75 },
            Type = "double"
        };

        // Act
        var query = betweenOperator.GetQueryPart(rule, 2);

        // Assert
        query.ShouldBe("Price >= @2 && Price <= @3");
    }

    [Fact]
    public void GetParametersPart_DateType_ShouldReturnCorrectParameters()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Date",
            Value = new object[] { new DateTime(2022, 1, 1), new DateTime(2022, 1, 31) },
            Type = "date"
        };

        // Act
        var parameters = betweenOperator.GetParametersPart(rule);

        // Assert
        parameters.ShouldBe([new DateTime(2022, 1, 1), new DateTime(2022, 2, 1).AddTicks(-1)]);
    }

    [Fact]
    public void GetParametersPart_DateType_HasCustomFormat_ShouldReturnCorrectParameters()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Date",
            Value = new object[] { "01/09/2024", "02/09/2024" },
            Type = "date",
            Data = new Dictionary<string, object>
            {
                { "datetimeFormat", "dd/MM/yyyy" }
            }
        };

        // Act
        var parameters = betweenOperator.GetParametersPart(rule)!;

        // Assert
        parameters.ShouldBe([new DateTime(2024, 9, 1), new DateTime(2024, 9, 3).AddTicks(-1)]);
    }

    [Fact]
    public void GetParametersPart_DateType_NullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Date",
            Value = null,
            Type = "date"
        };

        // Act & Assert
        var exception = Should.Throw<ArgumentNullException>(() => betweenOperator.GetParametersPart(rule));
        exception.ParamName.ShouldBe(nameof(rule.Value));
    }

    [Fact]
    public void GetParametersPart_DateType_ShouldThrowArgumentException_WhenValueIsNotAnArray()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Date",
            Value = new DateTime(2022, 1, 1), // Invalid, should be an array
            Type = "date"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => betweenOperator.GetParametersPart(rule));
    }

    [Fact]
    public void GetQueryPart_DateType_ShouldReturnCorrectQuery()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Date",
            Value = new object[] { new DateTime(2022, 1, 1), new DateTime(2022, 1, 31) },
            Type = "date"
        };

        // Act
        var query = betweenOperator.GetQueryPart(rule, 1);

        // Assert
        query.ShouldBe("Date >= @1 && Date <= @2");
    }
}
