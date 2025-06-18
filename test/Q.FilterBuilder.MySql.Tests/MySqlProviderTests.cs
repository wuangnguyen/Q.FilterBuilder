using System;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests;

public class MySqlProviderTests
{
    private readonly MySqlFormatProvider _provider;

    public MySqlProviderTests()
    {
        _provider = new MySqlFormatProvider();
    }

    [Fact]
    public void Properties_ShouldReturnCorrectValues()
    {
        // Act & Assert
        Assert.Equal("?", _provider.ParameterPrefix);
        Assert.Equal("AND", _provider.AndOperator);
        Assert.Equal("OR", _provider.OrOperator);
    }

    [Fact]
    public void FormatFieldName_ShouldWrapInBackticks()
    {
        // Arrange
        var fieldName = "UserName";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("`UserName`", result);
    }

    [Fact]
    public void FormatFieldName_WithSpecialCharacters_ShouldWrapInBackticks()
    {
        // Arrange
        var fieldName = "User Name";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("`User Name`", result);
    }

    [Fact]
    public void FormatFieldName_WithMultipleSegments_ShouldWrapEachSegmentInBackticks()
    {
        // Arrange
        var fieldName = "Products.Name";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("`Products`.`Name`", result);
    }

    [Fact]
    public void FormatFieldName_WithThreeSegments_ShouldWrapEachSegmentInBackticks()
    {
        // Arrange
        var fieldName = "A.B.C";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("`A`.`B`.`C`", result);
    }

    [Fact]
    public void FormatFieldName_WithEmptyString_ShouldThrowArgumentException()
    {
        // Arrange
        var fieldName = "";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _provider.FormatFieldName(fieldName));
        Assert.Contains("Field name cannot be null or empty", ex.Message);
    }

    [Fact]
    public void FormatFieldName_WithNullString_ShouldThrowArgumentException()
    {
        // Arrange
        string fieldName = null!;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _provider.FormatFieldName(fieldName));
        Assert.Contains("Field name cannot be null or empty", ex.Message);
    }

    [Theory]
    [InlineData(0, "?")]
    [InlineData(1, "?")]
    [InlineData(5, "?")]
    [InlineData(10, "?")]
    [InlineData(99, "?")]
    public void FormatParameterName_WithVariousIndices_ShouldAlwaysReturnQuestionMark(int index, string expected)
    {
        // Act
        var result = _provider.FormatParameterName(index);

        // Assert
        Assert.Equal(expected, result);
    }
}
