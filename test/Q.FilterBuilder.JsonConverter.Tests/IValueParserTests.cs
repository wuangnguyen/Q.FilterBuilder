using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests;

public class IValueParserTests
{
    [Fact]
    public void IValueParser_Interface_ShouldHaveParseValueMethod()
    {
        // Arrange & Act
        var parser = new TestValueParser();

        // Assert
        Assert.IsAssignableFrom<IValueParser>(parser);
    }

    [Fact]
    public void IValueParser_ParseValue_ShouldBeImplementable()
    {
        // Arrange
        var parser = new TestValueParser();
        var json = "\"test\"";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = parser.ParseValue(element);

        // Assert
        Assert.Equal("TEST_test", result);
    }

    [Fact]
    public void IValueParser_ParseValue_WithNullElement_ShouldHandleGracefully()
    {
        // Arrange
        var parser = new TestValueParser();
        var json = "null";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = parser.ParseValue(element);

        // Assert
        Assert.Equal("TEST_null", result);
    }

    [Fact]
    public void IValueParser_ParseValue_WithComplexElement_ShouldHandleGracefully()
    {
        // Arrange
        var parser = new TestValueParser();
        var json = """{"name": "John", "age": 30}""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = parser.ParseValue(element);

        // Assert
        Assert.Equal("TEST_Object", result);
    }

    [Fact]
    public void IValueParser_ParseValue_WithArrayElement_ShouldHandleGracefully()
    {
        // Arrange
        var parser = new TestValueParser();
        var json = """[1, 2, 3]""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = parser.ParseValue(element);

        // Assert
        Assert.Equal("TEST_Array", result);
    }

    [Fact]
    public void IValueParser_ParseValue_WithNumberElement_ShouldHandleGracefully()
    {
        // Arrange
        var parser = new TestValueParser();
        var json = "42";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = parser.ParseValue(element);

        // Assert
        Assert.Equal("TEST_42", result);
    }

    [Fact]
    public void IValueParser_ParseValue_WithBooleanElement_ShouldHandleGracefully()
    {
        // Arrange
        var parser = new TestValueParser();
        var json = "true";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = parser.ParseValue(element);

        // Assert
        Assert.Equal("TEST_True", result);
    }

    [Fact]
    public void IValueParser_MultipleImplementations_ShouldWorkIndependently()
    {
        // Arrange
        var parser1 = new TestValueParser();
        var parser2 = new AlternativeValueParser();
        var json = "\"test\"";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result1 = parser1.ParseValue(element);
        var result2 = parser2.ParseValue(element);

        // Assert
        Assert.Equal("TEST_test", result1);
        Assert.Equal("ALT_test", result2);
    }

    [Fact]
    public void IValueParser_CanBeUsedPolymorphically()
    {
        // Arrange
        var parsers = new List<IValueParser>
        {
            new TestValueParser(),
            new AlternativeValueParser()
        };
        var json = "\"test\"";
        var element = JsonDocument.Parse(json).RootElement;

        // Act & Assert
        foreach (var parser in parsers)
        {
            var result = parser.ParseValue(element);
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }
    }

    [Fact]
    public void IValueParser_Interface_ShouldAllowNullReturn()
    {
        // Arrange
        var parser = new NullReturningValueParser();
        var json = "\"test\"";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = parser.ParseValue(element);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void IValueParser_Interface_ShouldAllowDifferentReturnTypes()
    {
        // Arrange
        var stringParser = new TestValueParser();
        var intParser = new IntValueParser();
        var json1 = "\"test\"";
        var json2 = "42";
        var element1 = JsonDocument.Parse(json1).RootElement;
        var element2 = JsonDocument.Parse(json2).RootElement;

        // Act
        var result1 = stringParser.ParseValue(element1);
        var result2 = intParser.ParseValue(element2);

        // Assert
        Assert.IsType<string>(result1);
        Assert.IsType<int>(result2);
        Assert.Equal("TEST_test", result1);
        Assert.Equal(42, result2);
    }

    // Test implementations of IValueParser
    private class TestValueParser : IValueParser
    {
        public object? ParseValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => $"TEST_{element.GetString()}",
                JsonValueKind.Number => $"TEST_{element.GetRawText()}",
                JsonValueKind.True => "TEST_True",
                JsonValueKind.False => "TEST_False",
                JsonValueKind.Null => "TEST_null",
                JsonValueKind.Object => "TEST_Object",
                JsonValueKind.Array => "TEST_Array",
                _ => "TEST_Unknown"
            };
        }
    }

    private class AlternativeValueParser : IValueParser
    {
        public object? ParseValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => $"ALT_{element.GetString()}",
                JsonValueKind.Number => $"ALT_{element.GetRawText()}",
                _ => "ALT_Other"
            };
        }
    }

    private class NullReturningValueParser : IValueParser
    {
        public object? ParseValue(JsonElement element)
        {
            return null;
        }
    }

    private class IntValueParser : IValueParser
    {
        public object? ParseValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Number when element.TryGetInt32(out int value) => value,
                _ => 0
            };
        }
    }
}
