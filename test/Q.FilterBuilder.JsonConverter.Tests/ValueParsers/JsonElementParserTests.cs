using System.Collections.Generic;
using System.Text.Json;
using Q.FilterBuilder.JsonConverter.ValueParsers;
using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests.ValueParsers;

public class JsonElementParserTests
{
    [Fact]
    public void ParseJsonElement_WithStringValue_ShouldReturnString()
    {
        // Arrange
        var json = "\"test string\"";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("test string", result);
    }

    [Fact]
    public void ParseJsonElement_WithIntegerValue_ShouldReturnInt()
    {
        // Arrange
        var json = "42";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<int>(result);
        Assert.Equal(42, result);
    }

    [Fact]
    public void ParseJsonElement_WithLargeIntegerValue_ShouldReturnDouble()
    {
        // Arrange
        var json = "9223372036854775808"; // Larger than int.MaxValue
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<double>(result);
        Assert.Equal(9223372036854775808.0, result);
    }

    [Fact]
    public void ParseJsonElement_WithDecimalValue_ShouldReturnDouble()
    {
        // Arrange
        var json = "3.14159";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<double>(result);
        Assert.Equal(3.14159, result);
    }

    [Fact]
    public void ParseJsonElement_WithTrueValue_ShouldReturnTrue()
    {
        // Arrange
        var json = "true";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<bool>(result);
        Assert.True((bool)result!);
    }

    [Fact]
    public void ParseJsonElement_WithFalseValue_ShouldReturnFalse()
    {
        // Arrange
        var json = "false";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<bool>(result);
        Assert.False((bool)result!);
    }

    [Fact]
    public void ParseJsonElement_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        var json = "null";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseJsonElement_WithSimpleObject_ShouldReturnDictionary()
    {
        // Arrange
        var json = """{"name": "John", "age": 30}""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<Dictionary<string, object?>>(result);
        var dict = (Dictionary<string, object?>)result!;
        Assert.Equal(2, dict.Count);
        Assert.Equal("John", dict["name"]);
        Assert.Equal(30, dict["age"]);
    }

    [Fact]
    public void ParseJsonElement_WithNestedObject_ShouldReturnNestedDictionary()
    {
        // Arrange
        var json = """{"user": {"name": "John", "details": {"age": 30, "active": true}}}""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<Dictionary<string, object?>>(result);
        var dict = (Dictionary<string, object?>)result!;
        Assert.Single(dict);
        
        var userDict = (Dictionary<string, object?>)dict["user"]!;
        Assert.Equal(2, userDict.Count);
        Assert.Equal("John", userDict["name"]);
        
        var detailsDict = (Dictionary<string, object?>)userDict["details"]!;
        Assert.Equal(2, detailsDict.Count);
        Assert.Equal(30, detailsDict["age"]);
        Assert.True((bool)detailsDict["active"]!);
    }

    [Fact]
    public void ParseJsonElement_WithSimpleArray_ShouldReturnList()
    {
        // Arrange
        var json = """[1, 2, 3, 4, 5]""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<List<object?>>(result);
        var list = (List<object?>)result!;
        Assert.Equal(5, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
        Assert.Equal(4, list[3]);
        Assert.Equal(5, list[4]);
    }

    [Fact]
    public void ParseJsonElement_WithMixedArray_ShouldReturnListWithMixedTypes()
    {
        // Arrange
        var json = """["string", 42, true, null, 3.14]""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<List<object?>>(result);
        var list = (List<object?>)result!;
        Assert.Equal(5, list.Count);
        Assert.Equal("string", list[0]);
        Assert.Equal(42, list[1]);
        Assert.True((bool)list[2]!);
        Assert.Null(list[3]);
        Assert.Equal(3.14, list[4]);
    }

    [Fact]
    public void ParseJsonElement_WithNestedArray_ShouldReturnNestedList()
    {
        // Arrange
        var json = """[[1, 2], [3, 4], [5, 6]]""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<List<object?>>(result);
        var list = (List<object?>)result!;
        Assert.Equal(3, list.Count);
        
        var subList1 = (List<object?>)list[0]!;
        Assert.Equal(new object[] { 1, 2 }, subList1);
        
        var subList2 = (List<object?>)list[1]!;
        Assert.Equal(new object[] { 3, 4 }, subList2);
        
        var subList3 = (List<object?>)list[2]!;
        Assert.Equal(new object[] { 5, 6 }, subList3);
    }

    [Fact]
    public void ParseJsonElement_WithEmptyObject_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var json = "{}";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<Dictionary<string, object?>>(result);
        var dict = (Dictionary<string, object?>)result!;
        Assert.Empty(dict);
    }

    [Fact]
    public void ParseJsonElement_WithEmptyArray_ShouldReturnEmptyList()
    {
        // Arrange
        var json = "[]";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<List<object?>>(result);
        var list = (List<object?>)result!;
        Assert.Empty(list);
    }

    [Fact]
    public void ParseJsonElement_WithComplexNestedStructure_ShouldParseCorrectly()
    {
        // Arrange
        var json = """
        {
            "users": [
                {"name": "John", "age": 30, "active": true},
                {"name": "Jane", "age": 25, "active": false}
            ],
            "metadata": {
                "total": 2,
                "filters": ["active", "age"]
            }
        }
        """;
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<Dictionary<string, object?>>(result);
        var dict = (Dictionary<string, object?>)result!;
        Assert.Equal(2, dict.Count);
        
        var users = (List<object?>)dict["users"]!;
        Assert.Equal(2, users.Count);
        
        var user1 = (Dictionary<string, object?>)users[0]!;
        Assert.Equal("John", user1["name"]);
        Assert.Equal(30, user1["age"]);
        Assert.True((bool)user1["active"]!);
        
        var metadata = (Dictionary<string, object?>)dict["metadata"]!;
        Assert.Equal(2, metadata["total"]);
        
        var filters = (List<object?>)metadata["filters"]!;
        Assert.Equal(new object[] { "active", "age" }, filters);
    }

    [Fact]
    public void ParseObject_WithSimpleObject_ShouldReturnDictionary()
    {
        // Arrange
        var json = """{"key1": "value1", "key2": 42}""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseObject(element);

        // Assert
        Assert.IsType<Dictionary<string, object?>>(result);
        var dict = (Dictionary<string, object?>)result!;
        Assert.Equal(2, dict.Count);
        Assert.Equal("value1", dict["key1"]);
        Assert.Equal(42, dict["key2"]);
    }

    [Fact]
    public void ParseArray_WithSimpleArray_ShouldReturnList()
    {
        // Arrange
        var json = """["a", "b", "c"]""";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseArray(element);

        // Assert
        Assert.IsType<List<object?>>(result);
        var list = (List<object?>)result!;
        Assert.Equal(3, list.Count);
        Assert.Equal("a", list[0]);
        Assert.Equal("b", list[1]);
        Assert.Equal("c", list[2]);
    }

    [Fact]
    public void ParseJsonElement_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        var json = "0";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<int>(result);
        Assert.Equal(0, result);
    }

    [Fact]
    public void ParseJsonElement_WithNegativeNumber_ShouldReturnNegativeValue()
    {
        // Arrange
        var json = "-42";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<int>(result);
        Assert.Equal(-42, result);
    }

    [Fact]
    public void ParseJsonElement_WithEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var json = "\"\"";
        var element = JsonDocument.Parse(json).RootElement;

        // Act
        var result = JsonElementParser.ParseJsonElement(element);

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("", result);
    }
}
