using Q.FilterBuilder.MySql;
using Xunit;

namespace Q.FilterBuilder.MySql.Tests;

public class MySqlProviderTests
{
    private readonly MySqlProvider _provider;

    public MySqlProviderTests()
    {
        _provider = new MySqlProvider();
    }

    [Fact]
    public void ParameterPrefix_ShouldReturnQuestionMark()
    {
        // Act
        var result = _provider.ParameterPrefix;

        // Assert
        Assert.Equal("?", result);
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
    public void FormatParameterName_ShouldReturnQuestionMark()
    {
        // Arrange
        var parameterIndex = 0;

        // Act
        var result = _provider.FormatParameterName(parameterIndex);

        // Assert
        Assert.Equal("?", result);
    }

    [Fact]
    public void FormatParameterName_WithDifferentIndex_ShouldReturnQuestionMark()
    {
        // Arrange
        var parameterIndex = 5;

        // Act
        var result = _provider.FormatParameterName(parameterIndex);

        // Assert
        Assert.Equal("?", result);
    }
}
