using DynamicWhere.Core.Helpers;
using Shouldly;

namespace JsonConverter.Tests;

public class DateTimeHelperTests
{
    [Theory]
    [InlineData("2024-08-15 15:20:00", "2024-08-15 15:20:00")]
    [InlineData("2024-08-15 15:20", "2024-08-15 15:20:00")]
    [InlineData("2024-08-15", "2024-08-15 00:00:00")]
    [InlineData("15-08-2024", "2024-08-15 00:00:00")]
    [InlineData("20240815T152000Z", "2024-08-15T15:20:00Z")]
    public void TryParseDateTime_ValidDefaultFormats_ShouldReturnTrue(string input, string expected)
    {
        // Act
        bool result = DateTimeHelper.TryParseDateTime(input, out DateTime parsedDateTime);

        // Assert
        result.ShouldBeTrue();
        parsedDateTime.ShouldBe(DateTime.Parse(expected));
    }

    [Theory]
    [InlineData("2024/08/15 15:20", "yyyy/MM/dd HH:mm", "2024-08-15 15:20:00")]
    [InlineData("15-08-2024 15:20:00", "dd-MM-yyyy HH:mm:ss", "2024-08-15 15:20:00")]
    public void TryParseDateTime_ValidCustomFormats_ShouldReturnTrue(string input, string customFormat, string expected)
    {
        // Act
        bool result = DateTimeHelper.TryParseDateTime(input, out DateTime parsedDateTime, customFormat);

        // Assert
        result.ShouldBeTrue();
        parsedDateTime.ShouldBe(DateTime.Parse(expected));
    }

    [Theory]
    [InlineData("Invalid Date String")]
    [InlineData("2024-08-15T25:61:61Z")]  // Invalid time
    public void TryParseDateTime_InvalidDateStrings_ShouldReturnFalse(string input)
    {
        // Act
        bool result = DateTimeHelper.TryParseDateTime(input, out DateTime parsedDateTime);

        // Assert
        result.ShouldBeFalse();
        parsedDateTime.ShouldBe(default);
    }

    [Theory]
    [InlineData("2024-08-15 15:20", "yyyy/MM/dd HH:mm")] // Mismatched format
    [InlineData("15-08-2024", "MM/dd/yyyy")] // Mismatched format
    public void TryParseDateTime_InvalidCustomFormats_ShouldReturnFalse(string input, string customFormat)
    {
        // Act
        bool result = DateTimeHelper.TryParseDateTime(input, out DateTime parsedDateTime, customFormat);

        // Assert
        result.ShouldBeFalse();
        parsedDateTime.ShouldBe(default);
    }

    [Fact]
    public void TryParseDateTime_EmptyCustomFormats_ShouldFallbackToDefaultFormats()
    {
        // Arrange
        string input = "2024-08-15 15:20:00";

        // Act
        bool result = DateTimeHelper.TryParseDateTime(input, out DateTime parsedDateTime, Array.Empty<string>());

        // Assert
        result.ShouldBeTrue();
        parsedDateTime.ShouldBe(new DateTime(2024, 8, 15, 15, 20, 0));
    }
}