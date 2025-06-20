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

    [Fact]
    public void RegisterConverter_WithNullTypeString_ShouldThrowArgumentException()
    {
        // Arrange
        var converter = new TestCustomConverter();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _service.RegisterConverter(null!, converter));
        Assert.Equal("typeString", exception.ParamName);
    }

    [Fact]
    public void RegisterConverter_WithEmptyTypeString_ShouldThrowArgumentException()
    {
        // Arrange
        var converter = new TestCustomConverter();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _service.RegisterConverter("", converter));
        Assert.Equal("typeString", exception.ParamName);
    }

    [Fact]
    public void RegisterConverter_WithWhitespaceTypeString_ShouldThrowArgumentException()
    {
        // Arrange
        var converter = new TestCustomConverter();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _service.RegisterConverter("   ", converter));
        Assert.Equal("typeString", exception.ParamName);
    }

    [Fact]
    public void RegisterConverter_WithNullConverter_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            _service.RegisterConverter<string>("test", null!));
        Assert.Equal("converter", exception.ParamName);
    }

    [Fact]
    public void ConvertValue_WithEmptyArray_ShouldReturnEmptyArray()
    {
        // Arrange
        var input = new string[0];

        // Act
        var result = _service.ConvertValue(input, "int");

        // Assert
        Assert.IsType<int[]>(result);
        var intArray = (int[])result!;
        Assert.Empty(intArray);
    }

    [Fact]
    public void ConvertValue_WithNullElementsInArray_ShouldHandleNulls()
    {
        // Arrange
        var input = new string?[] { "1", null, "3" };

        // Act
        var result = _service.ConvertValue(input, "int");

        // Assert
        Assert.IsType<int[]>(result);
        var intArray = (int[])result!;
        Assert.Equal(3, intArray.Length);
        Assert.Equal(1, intArray[0]);
        Assert.Equal(0, intArray[1]); // null converts to default(int)
        Assert.Equal(3, intArray[2]);
    }

    [Fact]
    public void ConvertValue_WithHashSet_ShouldConvertToArray()
    {
        // Arrange
        var input = new HashSet<string> { "1", "2", "3" };

        // Act
        var result = _service.ConvertValue(input, "int");

        // Assert
        Assert.IsType<int[]>(result);
        var intArray = (int[])result!;
        Assert.Equal(3, intArray.Length);
        Assert.Contains(1, intArray);
        Assert.Contains(2, intArray);
        Assert.Contains(3, intArray);
    }

    [Fact]
    public void ConvertValue_WithEnumerable_ShouldConvertToArray()
    {
        // Arrange
        var input = Enumerable.Range(1, 3).Select(x => x.ToString());

        // Act
        var result = _service.ConvertValue(input, "int");

        // Assert
        Assert.IsType<int[]>(result);
        var intArray = (int[])result!;
        Assert.Equal(new int[] { 1, 2, 3 }, intArray);
    }

    [Fact]
    public void ConvertValue_WithBooleanAlias_ShouldWork()
    {
        // Act
        var result = _service.ConvertValue("true", "boolean");

        // Assert
        Assert.IsType<bool>(result);
        Assert.True((bool)result!);
    }

    [Fact]
    public void ConvertValue_WithDateAlias_ShouldWork()
    {
        // Act
        var result = _service.ConvertValue("2023-01-01", "date");

        // Assert
        Assert.IsType<DateTime>(result);
        var dateTime = (DateTime)result!;
        Assert.Equal(2023, dateTime.Year);
        Assert.Equal(1, dateTime.Month);
        Assert.Equal(1, dateTime.Day);
    }

    [Theory]
    [InlineData("TRUE", "bool", true)]
    [InlineData("123", "int", 123)]
    [InlineData("123", "integer", 123)]
    [InlineData("456", "long", 456L)]
    [InlineData("3.14", "double", 3.14)]
    [InlineData("2.5", "float", 2.5f)]
    [InlineData("255", "byte", (byte)255)]
    [InlineData("32767", "short", (short)32767)]
    [InlineData("4294967295", "uint", 4294967295u)]
    [InlineData("18446744073709551615", "ulong", 18446744073709551615ul)]
    [InlineData("65535", "ushort", (ushort)65535)]
    [InlineData("-128", "sbyte", (sbyte)-128)]
    [InlineData("test string", "string", "test string")]
    [InlineData("550e8400-e29b-41d4-a716-446655440000", "guid", "550e8400-e29b-41d4-a716-446655440000")]
    public void ConvertValue_WithAllPrimitiveTypes_ShouldConvertCorrectly(string input, string type, object expected)
    {
        // Act
        var result = _service.ConvertValue(input, type);

        // Assert
        if (type == "guid")
            Assert.Equal(Guid.Parse((string)expected), result);
        else
            Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("string", typeof(string))]
    [InlineData("int", typeof(int))]
    [InlineData("integer", typeof(int))]
    [InlineData("long", typeof(long))]
    [InlineData("double", typeof(double))]
    [InlineData("decimal", typeof(decimal))]
    [InlineData("float", typeof(float))]
    [InlineData("byte", typeof(byte))]
    [InlineData("short", typeof(short))]
    [InlineData("uint", typeof(uint))]
    [InlineData("ulong", typeof(ulong))]
    [InlineData("ushort", typeof(ushort))]
    [InlineData("sbyte", typeof(sbyte))]
    [InlineData("bool", typeof(bool))]
    [InlineData("boolean", typeof(bool))]
    [InlineData("datetime", typeof(DateTime))]
    [InlineData("date", typeof(DateTime))]
    [InlineData("guid", typeof(Guid))]
    [InlineData("unknown_type", null)]
    public void GetTargetType_ShouldReturnExpectedType(string typeString, Type expectedType)
    {
        // Act
        var result = typeof(TypeConversionService)
            .GetMethod("GetTargetType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            !.Invoke(_service, [typeString]);

        // Assert
        Assert.Equal(expectedType, result);
    }

    [Fact]
    public void GetTargetType_WithCustomConverter_ShouldReturnCustomType()
    {
        // Arrange
        var customConverter = new CustomTypeConverter();
        _service.RegisterConverter("customtype", customConverter);

        // Act
        var result = typeof(TypeConversionService)
            .GetMethod("GetTargetType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            !.Invoke(_service, ["customtype"]);

        // Assert
        Assert.Equal(typeof(CustomType), result);
    }

    private class CustomType { }
    private class CustomTypeConverter : ITypeConverter<CustomType>
    {
        public CustomType Convert(object? value, Dictionary<string, object?>? metadata = null) => new CustomType();
    }

    [Fact]
    public void TryDefaultConvert_WithInvalidGuid_ShouldThrow()
    {
        // Arrange
        var ex = Assert.Throws<InvalidOperationException>(() => _service.ConvertValue("not-a-guid", "guid"));
        Assert.Contains("Failed to convert", ex.Message);
    }

    [Fact]
    public void TryDefaultConvert_WithInvalidNumber_ShouldThrow()
    {
        // Arrange
        var ex = Assert.Throws<InvalidOperationException>(() => _service.ConvertValue("not-a-number", "int"));
        Assert.Contains("Failed to convert", ex.Message);
    }

    [Fact]
    public void TryDefaultConvert_WithUnsupportedType_ShouldThrow()
    {
        // Arrange
        var ex = Assert.Throws<InvalidOperationException>(() => _service.ConvertValue("value", "notatype"));
        Assert.Contains("Unsupported type conversion", ex.Message);
    }
}
