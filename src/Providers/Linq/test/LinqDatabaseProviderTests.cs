using Q.FilterBuilder.Linq;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests;

public class LinqDatabaseProviderTests
{
    private readonly LinqDatabaseProvider _provider;

    public LinqDatabaseProviderTests()
    {
        _provider = new LinqDatabaseProvider();
    }

    [Fact]
    public void ParameterPrefix_ShouldReturnEmptyString()
    {
        // Act
        var result = _provider.ParameterPrefix;

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void AndOperator_ShouldReturnDoubleAmpersand()
    {
        // Act
        var result = _provider.AndOperator;

        // Assert
        Assert.Equal("&&", result);
    }

    [Fact]
    public void OrOperator_ShouldReturnDoublePipe()
    {
        // Act
        var result = _provider.OrOperator;

        // Assert
        Assert.Equal("||", result);
    }

    [Fact]
    public void FormatFieldName_ShouldReturnFieldNameAsIs()
    {
        // Arrange
        var fieldName = "UserName";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("UserName", result);
    }

    [Fact]
    public void FormatFieldName_WithSpecialCharacters_ShouldReturnFieldNameAsIs()
    {
        // Arrange
        var fieldName = "User Name";

        // Act
        var result = _provider.FormatFieldName(fieldName);

        // Assert
        Assert.Equal("User Name", result);
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
