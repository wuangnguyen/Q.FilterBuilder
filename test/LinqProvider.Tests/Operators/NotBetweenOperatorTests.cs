using DynamicWhere.Core.Models;
using DynamicWhere.Core.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class NotBetweenOperatorTests
{
    [Fact]
    public void GetQueryPart_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = new object[] { 18, 65 }
        };

        var notBetweenOperator = new NotBetweenOperator();

        // Act
        var queryPart = notBetweenOperator.GetQueryPart(rule, 1);

        // Assert
        queryPart.ShouldBe("Age < @1 || Age > @2");
    }

    [Fact]
    public void GetParametersPart_ShouldReturnCorrectParameters()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = new object[] { 18, 65 }
        };

        var notBetweenOperator = new NotBetweenOperator();

        // Act
        var parameters = notBetweenOperator.GetParametersPart(rule);

        // Assert
        parameters.ShouldBe(new object[] { 18, 65 });
    }

    [Fact]
    public void GetParametersPart_ShouldThrowArgumentNullException_WhenValueIsNull()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = null
        };

        var notBetweenOperator = new NotBetweenOperator();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => notBetweenOperator.GetParametersPart(rule))
            .ParamName.ShouldBe(nameof(rule.Value));
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

        var notBetweenOperator = new NotBetweenOperator();

        // Act & Assert
        Should.Throw<ArgumentException>(() => notBetweenOperator.GetParametersPart(rule));
    }

    [Fact]
    public void GetParametersPart_ShouldReturnCorrectParameters_ForDateType()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "DateOfBirth",
            Value = new object[] { new DateTime(1990, 1, 1), new DateTime(2000, 12, 31) },
            Type = "date"
        };

        var notBetweenOperator = new NotBetweenOperator();

        // Act
        var parameters = notBetweenOperator.GetParametersPart(rule);

        // Assert
        parameters.ShouldBe([new DateTime(1990, 1, 1).Date, new DateTime(2000, 12, 31).Date.AddDays(1).AddTicks(-1)]);
    }
}
