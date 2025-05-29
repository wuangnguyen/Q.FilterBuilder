using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests;

public class SqlServerProviderTests
{
    private readonly SqlServerFormatProvider _provider;

    public SqlServerProviderTests()
    {
        _provider = new SqlServerFormatProvider();
    }

    [Fact]
    public void ParameterPrefix_ShouldReturnAtSymbol()
    {
        // Act
        var result = _provider.ParameterPrefix;

        // Assert
        Assert.Equal("@", result);
    }

    [Fact]
    public void AndOperator_ShouldReturnAND()
    {
        // Act
        var result = _provider.AndOperator;

        // Assert
        Assert.Equal("AND", result);
    }

    [Fact]
    public void OrOperator_ShouldReturnOR()
    {
        // Act
        var result = _provider.OrOperator;

        // Assert
        Assert.Equal("OR", result);
    }

    [Fact]
    public void FormatFieldName_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "UserName";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[UserName]", result);
    }

    [Fact]
    public void FormatFieldName_WithSpecialCharacters_ShouldWrapInSquareBrackets()
    {
        // Arrange
        var fieldName = "User Name";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("[User Name]", result);
    }

    [Fact]
    public void FormatParameterName_ShouldReturnCorrectFormat()
    {
        // Arrange
        var parameterIndex = 0;

        // Act
        var result = _provider.FormatParameterName(parameterIndex);

        // Assert
        Assert.Equal("@p0", result);
    }

    [Fact]
    public void FormatParameterName_WithDifferentIndex_ShouldReturnCorrectFormat()
    {
        // Arrange
        var parameterIndex = 5;

        // Act
        var result = _provider.FormatParameterName(parameterIndex);

        // Assert
        Assert.Equal("@p5", result);
    }
}
