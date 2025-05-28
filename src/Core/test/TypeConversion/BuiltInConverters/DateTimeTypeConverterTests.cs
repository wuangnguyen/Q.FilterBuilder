using Q.FilterBuilder.Core.TypeConversion;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.TypeConversion.BuiltInConverters;

public class DateTimeTypeConverterTests
{
    private readonly DateTimeTypeConverter _converter;

    public DateTimeTypeConverterTests()
    {
        _converter = new DateTimeTypeConverter();
    }

    [Fact]
    public void Convert_WithDateTimeInput_ShouldReturnSameDateTime()
    {
        // Arrange
        var input = new DateTime(2023, 12, 25, 14, 30, 0);

        // Act
        var result = _converter.Convert(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Convert_WithNullInput_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _converter.Convert(null));
        Assert.Equal("value", exception.ParamName);
        Assert.Contains("Cannot convert null to DateTime", exception.Message);
    }

    [Theory]
    [InlineData("2023-12-25")]
    [InlineData("2023-12-25T14:30:00")]
    [InlineData("2023-12-25T14:30:00Z")]
    [InlineData("25/12/2023")]
    [InlineData("12/25/2023")]
    [InlineData("20231225")]
    public void Convert_WithValidDateStrings_ShouldParseCorrectly(string input)
    {
        // Act
        var result = _converter.Convert(input);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithCustomSingleFormat_ShouldUseCustomFormat()
    {
        // Arrange
        var input = "25-Dec-2023";
        var metadata = new Dictionary<string, object?> { ["dateTimeFormats"] = "dd-MMM-yyyy" };

        // Act
        var result = _converter.Convert(input, metadata);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithCustomArrayFormats_ShouldTryAllFormats()
    {
        // Arrange
        var input = "Dec 25, 2023";
        var metadata = new Dictionary<string, object?>
        {
            ["dateTimeFormats"] = new[] { "dd-MMM-yyyy", "MMM dd, yyyy", "yyyy-MM-dd" }
        };

        // Act
        var result = _converter.Convert(input, metadata);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithCustomListFormats_ShouldTryAllFormats()
    {
        // Arrange
        var input = "2023/12/25";
        var metadata = new Dictionary<string, object?>
        {
            ["dateTimeFormats"] = new List<string> { "dd-MMM-yyyy", "yyyy/MM/dd", "MM/dd/yyyy" }
        };

        // Act
        var result = _converter.Convert(input, metadata);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithNullMetadata_ShouldUseDefaultFormats()
    {
        // Arrange
        var input = "2023-12-25";

        // Act
        var result = _converter.Convert(input, null);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithEmptyMetadata_ShouldUseDefaultFormats()
    {
        // Arrange
        var input = "2023-12-25";
        var metadata = new Dictionary<string, object?>();

        // Act
        var result = _converter.Convert(input, metadata);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithNullCustomFormats_ShouldUseDefaultFormats()
    {
        // Arrange
        var input = "2023-12-25";
        var metadata = new Dictionary<string, object?> { ["dateTimeFormats"] = null };

        // Act
        var result = _converter.Convert(input, metadata);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithInvalidCustomFormats_ShouldFallbackToDefaults()
    {
        // Arrange
        var input = "2023-12-25";
        var metadata = new Dictionary<string, object?>
        {
            ["dateTimeFormats"] = new[] { "invalid-format", "", null! }
        };

        // Act
        var result = _converter.Convert(input, metadata);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithMixedValidInvalidFormats_ShouldUseValidOnes()
    {
        // Arrange
        var input = "25-Dec-2023";
        var metadata = new Dictionary<string, object?>
        {
            ["dateTimeFormats"] = new[] { "invalid", "dd-MMM-yyyy", "", "another-invalid" }
        };

        // Act
        var result = _converter.Convert(input, metadata);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithNonStringNonArrayFormats_ShouldConvertToString()
    {
        // Arrange
        var input = "2023-12-25";
        var metadata = new Dictionary<string, object?>
        {
            ["dateTimeFormats"] = 12345 // Will be converted to string "12345"
        };

        // Act
        var result = _converter.Convert(input, metadata);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Theory]
    [InlineData("invalid date string")]
    [InlineData("2023-13-45")]
    [InlineData("not a date")]
    [InlineData("32/12/2023")]
    public void Convert_WithInvalidDateString_ShouldThrowInvalidOperationException(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _converter.Convert(input));
        Assert.Contains($"Cannot convert '{input}' to DateTime", exception.Message);
    }

    [Fact]
    public void Convert_WithInvalidDateAndCustomFormats_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var input = "invalid date";
        var metadata = new Dictionary<string, object?> { ["dateTimeFormats"] = "dd-MMM-yyyy" };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _converter.Convert(input, metadata));
        Assert.Contains($"Cannot convert '{input}' to DateTime", exception.Message);
    }

    [Fact]
    public void Convert_WithNumericInput_ShouldConvertToStringThenParse()
    {
        // Arrange - This will be converted to string "20231225" which is a valid compact format
        var input = 20231225;

        // Act
        var result = _converter.Convert(input);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void Convert_WithEnumerableOfStrings_ShouldUseFirstValidFormat()
    {
        // Arrange
        var input = "25-Dec-2023";
        var formatList = new List<object> { "invalid", "dd-MMM-yyyy", "yyyy-MM-dd" };
        var metadata = new Dictionary<string, object?> { ["dateTimeFormats"] = formatList };

        // Act
        var result = _converter.Convert(input, metadata);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }
}
