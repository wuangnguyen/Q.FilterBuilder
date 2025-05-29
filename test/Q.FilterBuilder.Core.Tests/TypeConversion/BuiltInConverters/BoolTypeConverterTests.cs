using Q.FilterBuilder.Core.TypeConversion;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.TypeConversion.BuiltInConverters;

public class BoolTypeConverterTests
{
    private readonly BoolTypeConverter _converter;

    public BoolTypeConverterTests()
    {
        _converter = new BoolTypeConverter();
    }

    [Fact]
    public void Convert_WithBoolInput_ShouldReturnSameBool()
    {
        // Arrange
        var trueInput = true;
        var falseInput = false;

        // Act
        var trueResult = _converter.Convert(trueInput);
        var falseResult = _converter.Convert(falseInput);

        // Assert
        Assert.True(trueResult);
        Assert.False(falseResult);
    }

    [Fact]
    public void Convert_WithNullInput_ShouldReturnFalse()
    {
        // Act
        var result = _converter.Convert(null);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("TRUE", true)]
    [InlineData("FALSE", false)]
    [InlineData("True", true)]
    [InlineData("False", false)]
    [InlineData("tRuE", true)]
    [InlineData("fAlSe", false)]
    public void Convert_WithTrueFalseStrings_ShouldConvertCorrectly(string input, bool expected)
    {
        // Act
        var result = _converter.Convert(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    public void Convert_WithNumericStrings_ShouldConvertCorrectly(string input, bool expected)
    {
        // Act
        var result = _converter.Convert(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("yes", true)]
    [InlineData("no", false)]
    [InlineData("YES", true)]
    [InlineData("NO", false)]
    [InlineData("Yes", true)]
    [InlineData("No", false)]
    [InlineData("yEs", true)]
    [InlineData("nO", false)]
    public void Convert_WithYesNoStrings_ShouldConvertCorrectly(string input, bool expected)
    {
        // Act
        var result = _converter.Convert(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("on", true)]
    [InlineData("off", false)]
    [InlineData("ON", true)]
    [InlineData("OFF", false)]
    [InlineData("On", true)]
    [InlineData("Off", false)]
    [InlineData("oN", true)]
    [InlineData("oFf", false)]
    public void Convert_WithOnOffStrings_ShouldConvertCorrectly(string input, bool expected)
    {
        // Act
        var result = _converter.Convert(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, true)]
    [InlineData(42, true)]
    public void Convert_WithNumericValues_ShouldConvertCorrectly(int input, bool expected)
    {
        // Act
        var result = _converter.Convert(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("maybe")]
    [InlineData("2")]
    [InlineData("-1")]
    [InlineData("")]
    [InlineData("   ")]
    public void Convert_WithInvalidStrings_ShouldFallbackToSystemConvert(string input)
    {
        // Act & Assert
        // These should throw exceptions as they can't be converted to bool
        Assert.Throws<FormatException>(() => _converter.Convert(input));
    }

    [Fact]
    public void Convert_WithMetadata_ShouldIgnoreMetadata()
    {
        // Arrange
        var metadata = new Dictionary<string, object?> { ["someKey"] = "someValue" };

        // Act
        var result = _converter.Convert("true", metadata);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Convert_WithEmptyString_ShouldFallbackToSystemConvert()
    {
        // Act & Assert
        Assert.Throws<FormatException>(() => _converter.Convert(""));
    }

    [Fact]
    public void Convert_WithWhitespaceString_ShouldFallbackToSystemConvert()
    {
        // Act & Assert
        Assert.Throws<FormatException>(() => _converter.Convert("   "));
    }

    [Theory]
    [InlineData(1.0, true)]
    [InlineData(0.0, false)]
    [InlineData(3.14, true)]
    [InlineData(-2.5, true)]
    public void Convert_WithDoubleValues_ShouldConvertCorrectly(double input, bool expected)
    {
        // Act
        var result = _converter.Convert(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_WithDecimalValues_ShouldConvertCorrectly()
    {
        // Test individual cases since decimal literals can't be used in InlineData
        Assert.True(_converter.Convert(1m));
        Assert.False(_converter.Convert(0m));
        Assert.True(_converter.Convert(99.99m));
        Assert.True(_converter.Convert(-10.5m));
    }

    [Fact]
    public void Convert_WithComplexObject_ShouldUseToStringThenConvert()
    {
        // Arrange
        var obj = new TestObject("true");

        // Act
        var result = _converter.Convert(obj);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Convert_WithComplexObjectInvalidString_ShouldThrowException()
    {
        // Arrange
        var obj = new TestObject("invalid");

        // Act & Assert
        Assert.Throws<InvalidCastException>(() => _converter.Convert(obj));
    }

    private class TestObject
    {
        private readonly string _value;

        public TestObject(string value)
        {
            _value = value;
        }

        public override string ToString() => _value;
    }
}
