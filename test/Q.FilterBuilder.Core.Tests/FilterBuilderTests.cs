using Moq;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.Providers;
using Q.FilterBuilder.Core.TypeConversion;
using Xunit;

namespace Q.FilterBuilder.Core.Tests;

public class FilterBuilderTests
{
    private readonly Mock<IQueryFormatProvider> _mockQuerySyntaxProvider;
    private readonly Mock<ITypeConversionService> _mockTypeConversionService;
    private readonly FilterBuilder _filterBuilder;

    public FilterBuilderTests()
    {
        _mockQuerySyntaxProvider = new Mock<IQueryFormatProvider>();
        _mockTypeConversionService = new Mock<ITypeConversionService>();

        // Setup query syntax provider to return the operator provider
        _mockQuerySyntaxProvider.Setup(p => p.FormatFieldName(It.IsAny<string>())).Returns<string>(name => name);
        _mockQuerySyntaxProvider.Setup(p => p.FormatParameterName(It.IsAny<int>())).Returns<int>(index => $"@{index}");
        _mockQuerySyntaxProvider.Setup(p => p.AndOperator).Returns("AND");
        _mockQuerySyntaxProvider.Setup(p => p.OrOperator).Returns("OR");

        // Setup type conversion service to return values as-is by default
        _mockTypeConversionService.Setup(s => s.ConvertValue(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                                  .Returns<object, string, Dictionary<string, object?>>((value, type, metadata) => value);

        _filterBuilder = new FilterBuilder(_mockQuerySyntaxProvider.Object, _mockTypeConversionService.Object);
    }

    [Fact]
    public void Constructor_WithNullProvider_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new FilterBuilder(null!, _mockTypeConversionService.Object));
        Assert.Equal("queryFormatProvider", exception.ParamName);
    }

    [Fact]
    public void Build_WithEmptyGroup_ShouldReturnEmptyQuery()
    {
        // Arrange
        var group = new FilterGroup("AND");

        // Act
        var (query, parameters) = _filterBuilder.Build(group);

        // Assert
        Assert.Empty(query);
        Assert.Empty(parameters);
    }

    [Fact]
    public void Build_WithNullGroup_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _filterBuilder.Build(null!));
        Assert.Equal("group", exception.ParamName);
    }
}
