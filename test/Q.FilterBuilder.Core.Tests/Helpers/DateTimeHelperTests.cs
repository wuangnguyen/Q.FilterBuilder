using Q.FilterBuilder.Core.Helpers;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.Helpers;

public class DateTimeHelperTests
{
    [Theory]
    [InlineData("2023-12-25", 2023, 12, 25)]
    [InlineData("2023-12-25T14:30:00", 2023, 12, 25, 14, 30, 0)]
    [InlineData("2023-12-25T14:30:00Z", 2023, 12, 25, 14, 30, 0)]
    [InlineData("2023-12-25T14:30:00.123Z", 2023, 12, 25, 14, 30, 0, 123)]
    public void TryParseDateTime_WithISO8601Formats_ShouldParseSuccessfully(string input, int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0)
    {
        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, []);

        // Assert
        Assert.True(result);
        Assert.Equal(year, parsedDate.Year);
        Assert.Equal(month, parsedDate.Month);
        Assert.Equal(day, parsedDate.Day);
        Assert.Equal(hour, parsedDate.Hour);
        Assert.Equal(minute, parsedDate.Minute);
        Assert.Equal(second, parsedDate.Second);
        Assert.Equal(millisecond, parsedDate.Millisecond);
    }

    [Theory]
    [InlineData("12/25/2023", 2023, 12, 25)]
    [InlineData("1/5/2023", 2023, 1, 5)]
    [InlineData("12/25/2023 14:30:00", 2023, 12, 25, 14, 30, 0)]
    public void TryParseDateTime_WithUSFormats_ShouldParseSuccessfully(string input, int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
    {
        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, []);

        // Assert
        Assert.True(result);
        Assert.Equal(year, parsedDate.Year);
        Assert.Equal(month, parsedDate.Month);
        Assert.Equal(day, parsedDate.Day);
        Assert.Equal(hour, parsedDate.Hour);
        Assert.Equal(minute, parsedDate.Minute);
        Assert.Equal(second, parsedDate.Second);
    }

    [Theory]
    [InlineData("25/12/2023", 2023, 12, 25)]
    [InlineData("5/1/2023", 2023, 1, 5)]
    [InlineData("25/12/2023 14:30:00", 2023, 12, 25, 14, 30, 0)]
    public void TryParseDateTime_WithEuropeanFormats_ShouldParseSuccessfully(string input, int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
    {
        // Act
        var result = DateTimeHelper.TryParseDateTime(
            input, out var parsedDate,
            [
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy",
                "d/M/yyyy"
            ]);

        // Assert
        Assert.True(result);
        Assert.Equal(year, parsedDate.Year);
        Assert.Equal(month, parsedDate.Month);
        Assert.Equal(day, parsedDate.Day);
        Assert.Equal(hour, parsedDate.Hour);
        Assert.Equal(minute, parsedDate.Minute);
        Assert.Equal(second, parsedDate.Second);
    }

    [Theory]
    [InlineData("25-12-2023", 2023, 12, 25)]
    [InlineData("2023-12-25", 2023, 12, 25)]
    [InlineData("25-12-2023 14:30:00", 2023, 12, 25, 14, 30, 0)]
    public void TryParseDateTime_WithDashFormats_ShouldParseSuccessfully(string input, int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
    {
        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, []);

        // Assert
        Assert.True(result);
        Assert.Equal(year, parsedDate.Year);
        Assert.Equal(month, parsedDate.Month);
        Assert.Equal(day, parsedDate.Day);
        Assert.Equal(hour, parsedDate.Hour);
        Assert.Equal(minute, parsedDate.Minute);
        Assert.Equal(second, parsedDate.Second);
    }

    [Theory]
    [InlineData("20231225", 2023, 12, 25)]
    [InlineData("20231225143000", 2023, 12, 25, 14, 30, 0)]
    public void TryParseDateTime_WithCompactFormats_ShouldParseSuccessfully(string input, int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
    {
        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, []);

        // Assert
        Assert.True(result);
        Assert.Equal(year, parsedDate.Year);
        Assert.Equal(month, parsedDate.Month);
        Assert.Equal(day, parsedDate.Day);
        Assert.Equal(hour, parsedDate.Hour);
        Assert.Equal(minute, parsedDate.Minute);
        Assert.Equal(second, parsedDate.Second);
    }

    [Fact]
    public void TryParseDateTime_WithCustomFormat_ShouldParseSuccessfully()
    {
        // Arrange
        var input = "25-Dec-2023";
        var customFormat = "dd-MMM-yyyy";

        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, [customFormat]);

        // Assert
        Assert.True(result);
        Assert.Equal(2023, parsedDate.Year);
        Assert.Equal(12, parsedDate.Month);
        Assert.Equal(25, parsedDate.Day);
    }

    [Fact]
    public void TryParseDateTime_WithMultipleCustomFormats_ShouldTryAllFormats()
    {
        // Arrange
        var input = "Dec 25, 2023";
        var customFormats = new[] { "dd-MMM-yyyy", "MMM dd, yyyy", "yyyy-MM-dd" };

        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, customFormats);

        // Assert
        Assert.True(result);
        Assert.Equal(2023, parsedDate.Year);
        Assert.Equal(12, parsedDate.Month);
        Assert.Equal(25, parsedDate.Day);
    }

    [Fact]
    public void TryParseDateTime_WithInvalidCustomFormats_ShouldFilterAndContinue()
    {
        // Arrange
        var input = "2023-12-25";
        var customFormats = new[] { "", null!, "   ", "yyyy-MM-dd" };

        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, customFormats!);

        // Assert
        Assert.True(result);
        Assert.Equal(2023, parsedDate.Year);
        Assert.Equal(12, parsedDate.Month);
        Assert.Equal(25, parsedDate.Day);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryParseDateTime_WithNullOrEmptyInput_ShouldReturnFalse(string? input)
    {
        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, []);

        // Assert
        Assert.False(result);
        Assert.Equal(default(DateTime), parsedDate);
    }

    [Theory]
    [InlineData("invalid date")]
    [InlineData("2023-13-45")]
    [InlineData("not a date")]
    [InlineData("32/12/2023")]
    public void TryParseDateTime_WithInvalidInput_ShouldReturnFalse(string input)
    {
        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, []);

        // Assert
        Assert.False(result);
        Assert.Equal(default(DateTime), parsedDate);
    }

    [Fact]
    public void ParseDateTimeOrNull_WithValidInput_ShouldReturnDateTime()
    {
        // Arrange
        var input = "2023-12-25";

        // Act
        var result = DateTimeHelper.ParseDateTimeOrNull(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2023, result!.Value.Year);
        Assert.Equal(12, result.Value.Month);
        Assert.Equal(25, result.Value.Day);
    }

    [Fact]
    public void ParseDateTimeOrNull_WithInvalidInput_ShouldReturnNull()
    {
        // Arrange
        var input = "invalid date";

        // Act
        var result = DateTimeHelper.ParseDateTimeOrNull(input);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseDateTimeOrDefault_WithValidInput_ShouldReturnDateTime()
    {
        // Arrange
        var input = "2023-12-25";

        // Act
        var result = DateTimeHelper.ParseDateTimeOrDefault(input);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void ParseDateTimeOrDefault_WithInvalidInput_ShouldReturnDefault()
    {
        // Arrange
        var input = "invalid date";
        var defaultValue = new DateTime(2020, 1, 1);

        // Act
        var result = DateTimeHelper.ParseDateTimeOrDefault(input, defaultValue);

        // Assert
        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseDateTimeOrDefault_WithInvalidInputAndNoDefault_ShouldReturnDefaultDateTime()
    {
        // Arrange
        var input = "invalid date";

        // Act
        var result = DateTimeHelper.ParseDateTimeOrDefault(input);

        // Assert
        Assert.Equal(default(DateTime), result);
    }

    [Fact]
    public void TryParseDateTime_WithCustomFormatPriority_ShouldUseCustomFormatFirst()
    {
        // Arrange - This date could be ambiguous (01/02/2023 could be Jan 2 or Feb 1)
        var input = "01/02/2023";
        var customFormat = "dd/MM/yyyy"; // European format - should interpret as Feb 1

        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, [customFormat]);

        // Assert
        Assert.True(result);
        Assert.Equal(2, parsedDate.Month); // February
        Assert.Equal(1, parsedDate.Day);
    }

    [Fact]
    public void TryParseDateTime_WithNoCustomFormats_ShouldUseDefaultFormats()
    {
        // Arrange
        var input = "2023-12-25T14:30:00Z";

        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, []);

        // Assert
        Assert.True(result);
        Assert.Equal(2023, parsedDate.Year);
        Assert.Equal(12, parsedDate.Month);
        Assert.Equal(25, parsedDate.Day);
        Assert.Equal(14, parsedDate.Hour);
        Assert.Equal(30, parsedDate.Minute);
    }

    [Fact]
    public void ParseDateTimeOrNull_WithCustomFormats_ShouldUseCustomFormats()
    {
        // Arrange
        var input = "25-Dec-2023";
        var customFormat = "dd-MMM-yyyy";

        // Act
        var result = DateTimeHelper.ParseDateTimeOrNull(input, customFormat);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2023, result!.Value.Year);
        Assert.Equal(12, result.Value.Month);
        Assert.Equal(25, result.Value.Day);
    }

    [Fact]
    public void ParseDateTimeOrDefault_WithCustomFormats_ShouldUseCustomFormats()
    {
        // Arrange
        var input = "25-Dec-2023";
        var customFormat = "dd-MMM-yyyy";
        var defaultValue = new DateTime(2020, 1, 1);

        // Act
        var result = DateTimeHelper.ParseDateTimeOrDefault(input, defaultValue, customFormat);

        // Assert
        Assert.Equal(2023, result.Year);
        Assert.Equal(12, result.Month);
        Assert.Equal(25, result.Day);
    }

    [Fact]
    public void TryParseDateTime_WithNullCustomFormats_ShouldUseDefaultFormats()
    {
        // Arrange
        var input = "2023-12-25";

        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, null);

        // Assert
        Assert.True(result);
        Assert.Equal(2023, parsedDate.Year);
        Assert.Equal(12, parsedDate.Month);
        Assert.Equal(25, parsedDate.Day);
    }

    [Fact]
    public void TryParseDateTime_WithEmptyCustomFormats_ShouldUseDefaultFormats()
    {
        // Arrange
        var input = "2023-12-25";

        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, []);

        // Assert
        Assert.True(result);
        Assert.Equal(2023, parsedDate.Year);
        Assert.Equal(12, parsedDate.Month);
        Assert.Equal(25, parsedDate.Day);
    }

    [Fact]
    public void TryParseDateTime_WithZuluTimeFormat_ShouldHandleTimeZone()
    {
        // Arrange
        var input = "20231225T143000Z";

        // Act
        var result = DateTimeHelper.TryParseDateTime(input, out var parsedDate, []);

        // Assert
        Assert.True(result);
        Assert.Equal(2023, parsedDate.Year);
        Assert.Equal(12, parsedDate.Month);
        Assert.Equal(25, parsedDate.Day);
        Assert.Equal(14, parsedDate.Hour);
        Assert.Equal(30, parsedDate.Minute);
    }
}
