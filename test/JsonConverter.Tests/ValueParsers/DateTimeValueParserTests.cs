using DynamicWhere.JsonConverter.ValueParsers;
using Shouldly;
using System.Text.Json;

namespace JsonConverter.Tests.ValueParsers;

public class DateTimeValueParserTests
{
    [Theory]
    // TODO: Add more cases to this test
    [InlineData("2024-08-15T15:20:00Z", "2024-08-15T15:20:00Z")]
    [InlineData("2024-08-15T00:00:00Z", "2024-08-15T00:00:00Z")]
    public void DateTimeValueParser_ValidDateTime_ShouldReturnCorrectDateTime(string jsonString, string expectedDateTimeString)
    {
        // Arrange
        var jsonElement = JsonDocument.Parse($"\"{jsonString}\"").RootElement;
        var parser = new DateTimeValueParser();
        var expectedDateTime = DateTime.Parse(expectedDateTimeString).ToUniversalTime();

        // Act
        var result = parser.ParseValue(jsonElement);

        // Assert
        result.ShouldBeOfType<DateTime>();
        ((DateTime)result).ShouldBe(expectedDateTime);
    }

    [Theory]
    [InlineData("Invalid Date")]
    [InlineData("")]
    public void DateTimeValueParser_InvalidDate_ShouldReturnNull(string jsonString)
    {
        // Arrange
        var jsonElement = JsonDocument.Parse($"\"{jsonString}\"").RootElement;
        var parser = new DateTimeValueParser();

        // Act
        var result = parser.ParseValue(jsonElement);

        // Assert
        result.ShouldBeNull();
    }
}