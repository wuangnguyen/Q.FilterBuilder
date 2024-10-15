using DynamicWhere.Core.Models;
using DynamicWhere.LinqProvider.Operators;
using Shouldly;

namespace LinqProvider.Tests.Operators;

public class InOperatorTests
{
    [Theory]
    [InlineData("Name", new[] { "Alice", "Bob" }, "name", 1, "@1.Contains(Name)")]
    [InlineData("Age", new[] { 20, 25, 30 }, "int", 1, "@1.Contains(Age)")]
    [InlineData("DateOfBirth", new[] { "2023-01-01", "2023-12-31" }, "datetime", 1, "@1.Contains(DateOfBirth)")]
    public void GetQueryPart_ShouldReturnCorrectQueryPart_ForValidInput(string fieldName, object value, string type, int parameterIndex, string expectedQueryPart)
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = fieldName,
            Value = value,
            Type = type
        };

        var inOperator = new InOperator();

        // Act
        var queryPart = inOperator.GetQueryPart(rule, parameterIndex);

        // Assert
        Assert.Equal(expectedQueryPart, queryPart);
    }

    [Fact]
    public void GetParametersPart_ShouldReturnCorrectParameters_ForStringArray()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Name",
            Value = new[] { "Alice", "Bob" },
            Type = "string"
        };

        var inOperator = new InOperator();

        // Act
        var parameters = inOperator.GetParametersPart(rule);

        // Assert
        parameters.ShouldBe([new object[] { "Alice", "Bob" }]);
    }

    [Fact]
    public void GetParametersPart_ShouldReturnCorrectParameters_ForIntegerArray()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = new[] { 20, 25, 30 },
            Type = "int"
        };

        var inOperator = new InOperator();

        // Act
        var parameters = inOperator.GetParametersPart(rule);

        // Assert
        parameters.ShouldBe([new object[] { 20, 25, 30 }]);
    }

    [Fact]
    public void GetParametersPart_ShouldReturnCorrectParameters_ForIntegerArrayAsString()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = new[] { "20", "25" },
            Type = "int"
        };

        var inOperator = new InOperator();

        // Act
        var parameters = inOperator.GetParametersPart(rule);

        // Assert
        parameters.ShouldBe([new object[] { 20, 25 }]);
    }

    [Fact]
    public void GetParametersPart_ShouldReturnCorrectParameters_ForDateTimeArray()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "DateOfBirth",
            Value = new[] { new DateTime(2023, 1, 1), new DateTime(2023, 12, 31) },
            Type = "datetime"
        };

        var inOperator = new InOperator();

        // Act
        var parameters = inOperator.GetParametersPart(rule);

        // Assert
        parameters.ShouldBe([new object[] { new DateTime(2023, 1, 1), new DateTime(2023, 12, 31) }]);
    }

    [Fact]
    public void GetParametersPart_ShouldReturnCorrectParameters_ForDateTimeArray1()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "DateOfBirth",
            Value = new[] { "2023/1/1", "31/12/2023" },
            Type = "datetime"
        };

        var inOperator = new InOperator();

        // Act
        var parameters = inOperator.GetParametersPart(rule);

        // Assert
        parameters.ShouldBe([new object[] { new DateTime(2023, 1, 1), new DateTime(2023, 12, 31) }]);
    }


    [Fact]
    public void GetParametersPart_ShouldThrowArgumentNullException_ForEmptyArray()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Age",
            Value = Array.Empty<int>(),
            Type = "int"
        };

        var inOperator = new InOperator();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => inOperator.GetParametersPart(rule));
    }

    [Fact]
    public void GetParametersPart_ShouldThrowInvalidOperationException_ForSingleValue()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Name",
            Value = "Alice",
            Type = "string"
        };

        var inOperator = new InOperator();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => inOperator.GetParametersPart(rule));
    }

    [Fact]
    public void GetParametersPart_ShouldThrowInvalidOperationException_ForNullValue()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Name",
            Value = null,
            Type = "string"
        };

        var inOperator = new InOperator();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => inOperator.GetParametersPart(rule));
    }

    [Fact]
    public void GetParametersPart_ShouldThrowArgumentNullException_ForEmptyValue()
    {
        // Arrange
        var rule = new DynamicRule
        {
            FieldName = "Name",
            Value = new string[] { },
            Type = "string"
        };

        var inOperator = new InOperator();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => inOperator.GetParametersPart(rule));
    }
}
