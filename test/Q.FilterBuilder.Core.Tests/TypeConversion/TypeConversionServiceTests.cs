using Q.FilterBuilder.Core.TypeConversion;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.TypeConversion;

public class TypeConversionServiceTests
{
    private readonly TypeConversionService _service;

    public TypeConversionServiceTests()
    {
        _service = new TypeConversionService();
    }

    [Theory]
    [InlineData("123", "int", 123)]
    [InlineData("123", "integer", 123)]
    [InlineData("456", "long", 456L)]
    [InlineData("3.14", "double", 3.14)]
    [InlineData("2.5", "float", 2.5f)]
    public void ConvertValue_WithNumericTypes_ShouldConvertCorrectly(string input, string type, object expected)
    {
        // Act
        var result = _service.ConvertValue(input, type);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertValue_WithDecimalType_ShouldConvertCorrectly()
    {
        // Arrange
        var input = "99.99";
        var expected = 99.99m;

        // Act
        var result = _service.ConvertValue(input, "decimal");

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("true", "bool", true)]
    [InlineData("false", "bool", false)]
    [InlineData("yes", "bool", true)]
    [InlineData("no", "bool", false)]
    [InlineData("1", "bool", true)]
    [InlineData("0", "bool", false)]
    [InlineData("on", "bool", true)]
    [InlineData("off", "bool", false)]
    [InlineData("TRUE", "bool", true)]
    [InlineData("FALSE", "bool", false)]
    public void ConvertValue_WithBooleanTypes_ShouldConvertCorrectly(string input, string type, bool expected)
    {
        // Act
        var result = _service.ConvertValue(input, type);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("2023-12-25", "datetime")]
    [InlineData("2023-12-25T14:30:00", "datetime")]
    [InlineData("25/12/2023", "datetime")]
    [InlineData("12/25/2023", "datetime")]
    public void ConvertValue_WithDateTimeTypes_ShouldConvertCorrectly(string input, string type)
    {
        // Act
        var result = _service.ConvertValue(input, type);

        // Assert
        Assert.IsType<DateTime>(result);
        var dateTime = (DateTime)result!;
        Assert.Equal(2023, dateTime.Year);
        Assert.Equal(12, dateTime.Month);
        Assert.Equal(25, dateTime.Day);
    }

    [Fact]
    public void ConvertValue_WithDateTimeAndCustomFormat_ShouldUseCustomFormat()
    {
        // Arrange
        var input = "25-Dec-2023";
        var metadata = new Dictionary<string, object?> { ["dateTimeFormats"] = "dd-MMM-yyyy" };

        // Act
        var result = _service.ConvertValue(input, "datetime", metadata);

        // Assert
        Assert.IsType<DateTime>(result);
        var dateTime = (DateTime)result!;
        Assert.Equal(2023, dateTime.Year);
        Assert.Equal(12, dateTime.Month);
        Assert.Equal(25, dateTime.Day);
    }

    [Fact]
    public void ConvertValue_WithDateTimeAndMultipleFormats_ShouldTryAllFormats()
    {
        // Arrange
        var input = "Dec 25, 2023";
        var metadata = new Dictionary<string, object?>
        {
            ["dateTimeFormats"] = new[] { "dd-MMM-yyyy", "MMM dd, yyyy", "yyyy-MM-dd" }
        };

        // Act
        var result = _service.ConvertValue(input, "datetime", metadata);

        // Assert
        Assert.IsType<DateTime>(result);
        var dateTime = (DateTime)result!;
        Assert.Equal(2023, dateTime.Year);
        Assert.Equal(12, dateTime.Month);
        Assert.Equal(25, dateTime.Day);
    }

    [Theory]
    [InlineData("550e8400-e29b-41d4-a716-446655440000", "guid")]
    public void ConvertValue_WithGuidType_ShouldConvertCorrectly(string input, string type)
    {
        // Act
        var result = _service.ConvertValue(input, type);

        // Assert
        Assert.IsType<Guid>(result);
        Assert.Equal(Guid.Parse(input), result);
    }

    [Fact]
    public void ConvertValue_WithStringType_ShouldReturnAsString()
    {
        // Arrange
        var input = "test string";

        // Act
        var result = _service.ConvertValue(input, "string");

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertValue_WithNullValue_ShouldReturnNull()
    {
        // Act
        var result = _service.ConvertValue(null, "int");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertValue_WithEmptyTypeString_ShouldReturnOriginalValue()
    {
        // Arrange
        var input = "test value";

        // Act
        var result = _service.ConvertValue(input, "");

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertValue_WithNullTypeString_ShouldReturnOriginalValue()
    {
        // Arrange
        var input = "test value";

        // Act
        var result = _service.ConvertValue(input, null!);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertValue_WithWhitespaceTypeString_ShouldReturnOriginalValue()
    {
        // Arrange
        var input = "test value";

        // Act
        var result = _service.ConvertValue(input, "   ");

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertValue_WithArrayInput_ShouldConvertEachElement()
    {
        // Arrange
        var input = new[] { "1", "2", "3" };

        // Act
        var result = _service.ConvertValue(input, "int");

        // Assert
        Assert.IsType<int[]>(result);
        var intArray = (int[])result!;
        Assert.Equal(new int[] { 1, 2, 3 }, intArray);
    }

    [Fact]
    public void ConvertValue_WithListInput_ShouldConvertEachElement()
    {
        // Arrange
        var input = new List<string> { "true", "false", "yes" };

        // Act
        var result = _service.ConvertValue(input, "bool");

        // Assert
        Assert.IsType<bool[]>(result);
        var boolArray = (bool[])result!;
        Assert.Equal(new bool[] { true, false, true }, boolArray);
    }

    [Fact]
    public void ConvertValue_WithInvalidConversion_ShouldThrowException()
    {
        // Arrange
        var input = "not a number";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _service.ConvertValue(input, "int"));

        Assert.Contains("Failed to convert", exception.Message);
    }

    [Fact]
    public void ConvertValue_WithUnsupportedType_ShouldThrowException()
    {
        // Arrange
        var input = "test";

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _service.ConvertValue(input, "unsupported_type"));

        Assert.Contains("Unsupported type conversion", exception.Message);
    }

    [Fact]
    public void RegisterConverter_WithCustomConverter_ShouldUseCustomConverter()
    {
        // Arrange
        var customConverter = new TestCustomConverter();
        _service.RegisterConverter("custom", customConverter);

        // Act
        var result = _service.ConvertValue("test_input", "custom");

        // Assert
        Assert.Equal("CUSTOM_test_input", result);
    }

    [Fact]
    public void ConvertValue_WithCustomTypeArray_ShouldCreateStronglyTypedArray()
    {
        // Arrange
        var customConverter = new TestCustomConverter();
        _service.RegisterConverter("custom", customConverter);
        var input = new[] { "value1", "value2", "value3" };

        // Act
        var result = _service.ConvertValue(input, "custom");

        // Assert
        Assert.IsType<string[]>(result);
        var stringArray = (string[])result!;
        Assert.Equal(new string[] { "CUSTOM_value1", "CUSTOM_value2", "CUSTOM_value3" }, stringArray);
    }

    [Fact]
    public void ConvertValue_WithCustomTypeList_ShouldCreateStronglyTypedArray()
    {
        // Arrange
        var customConverter = new TestCustomTypeConverter();
        _service.RegisterConverter("customtype", customConverter);
        var input = new List<string> { "A", "B", "C" };

        // Act
        var result = _service.ConvertValue(input, "customtype");

        // Assert
        Assert.IsType<TestCustomType[]>(result);
        var customArray = (TestCustomType[])result!;
        Assert.Equal(3, customArray.Length);
        Assert.Equal("A", customArray[0].Value);
        Assert.Equal("B", customArray[1].Value);
        Assert.Equal("C", customArray[2].Value);
    }

    private class TestCustomConverter : ITypeConverter<string>
    {
        public string Convert(object? value, Dictionary<string, object?>? metadata = null)
        {
            return $"CUSTOM_{value}";
        }
    }

    private class TestCustomType
    {
        public string Value { get; }

        public TestCustomType(string value)
        {
            Value = value;
        }
    }

    private class TestCustomTypeConverter : ITypeConverter<TestCustomType>
    {
        public TestCustomType Convert(object? value, Dictionary<string, object?>? metadata = null)
        {
            return new TestCustomType(value?.ToString() ?? "");
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("string", false)]
    [InlineData(123, false)]
    [InlineData(true, false)]
    public void IsCollection_WithNonCollectionValues_ShouldReturnFalse(object? value, bool expected)
    {
        // Act
        var result = _service.IsCollection(value);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsCollection_WithArrays_ShouldReturnTrue()
    {
        // Arrange
        var intArray = new[] { 1, 2, 3 };
        var stringArray = new[] { "a", "b", "c" };
        var objectArray = new object[] { 1, "test", true };

        // Act & Assert
        Assert.True(_service.IsCollection(intArray));
        Assert.True(_service.IsCollection(stringArray));
        Assert.True(_service.IsCollection(objectArray));
    }

    [Fact]
    public void IsCollection_WithLists_ShouldReturnTrue()
    {
        // Arrange
        var intList = new List<int> { 1, 2, 3 };
        var stringList = new List<string> { "a", "b", "c" };

        // Act & Assert
        Assert.True(_service.IsCollection(intList));
        Assert.True(_service.IsCollection(stringList));
    }
}
