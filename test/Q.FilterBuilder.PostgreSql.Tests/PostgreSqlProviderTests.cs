using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests;

public class PostgreSqlProviderTests
{
    private readonly PostgreSqlFormatProvider _provider;

    public PostgreSqlProviderTests()
    {
        _provider = new PostgreSqlFormatProvider();
    }

    [Fact]
    public void Properties_ShouldReturnCorrectValues()
    {
        // Act & Assert
        Assert.Equal("$", _provider.ParameterPrefix);
        Assert.Equal("AND", _provider.AndOperator);
        Assert.Equal("OR", _provider.OrOperator);
    }

    [Fact]
    public void FormatFieldName_ShouldWrapInDoubleQuotes()
    {
        // Arrange
        var fieldName = "UserName";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("\"UserName\"", result);
    }

    [Fact]
    public void FormatFieldName_WithSpecialCharacters_ShouldWrapInDoubleQuotes()
    {
        // Arrange
        var fieldName = "User Name";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("\"User Name\"", result);
    }

    [Theory]
    [InlineData(0, "$1")]
    [InlineData(1, "$2")]
    [InlineData(5, "$6")]
    [InlineData(10, "$11")]
    [InlineData(99, "$100")]
    public void FormatParameterName_WithVariousIndices_ShouldReturnCorrectFormat(int index, string expected)
    {
        // Act
        var result = _provider.FormatParameterName(index);

        // Assert
        Assert.Equal(expected, result);
    }
}
