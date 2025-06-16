using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.RuleTransformers;

public class BetweenTransformerBaseTests
{
    [Fact]
    public void Constructor_WithNullOperatorName_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestBetweenTransformer(null!));
    }

    [Fact]
    public void Constructor_WithValidOperatorName_ShouldCreateInstance()
    {
        // Act
        var transformer = new TestBetweenTransformer("BETWEEN");

        // Assert
        Assert.NotNull(transformer);
    }

    [Fact]
    public void BuildParameters_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            transformer.TestBuildParameters(null, null));
        Assert.Contains("BETWEEN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithNonCollectionValue_ShouldThrowArgumentException()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            transformer.TestBuildParameters("single_value", null));
        Assert.Contains("BETWEEN operator requires an array or collection with exactly 2 values", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");
        var emptyArray = new object[0];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            transformer.TestBuildParameters(emptyArray, null));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithSingleElementArray_ShouldThrowArgumentException()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");
        var singleArray = new object[] { 1 };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            transformer.TestBuildParameters(singleArray, null));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithThreeElementArray_ShouldThrowArgumentException()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");
        var threeArray = new object[] { 1, 2, 3 };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            transformer.TestBuildParameters(threeArray, null));
        Assert.Contains("BETWEEN operator requires exactly 2 values", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithValidTwoElementArray_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");
        var validArray = new object[] { 1, 10 };

        // Act
        var result = transformer.TestBuildParameters(validArray, null);

        // Assert
        Assert.Equal(2, result!.Length);
        Assert.Equal(1, result[0]);
        Assert.Equal(10, result[1]);
    }

    [Fact]
    public void BuildParameters_WithValidList_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");
        var validList = new List<object> { "start", "end" };

        // Act
        var result = transformer.TestBuildParameters(validList, null);

        // Assert
        Assert.Equal(2, result!.Length);
        Assert.Equal("start", result[0]);
        Assert.Equal("end", result[1]);
    }

    [Fact]
    public void BuildParameters_WithDateTimeStrings_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");
        var dateArray = new object[] { "2023-01-01", "2023-12-31" };

        // Act
        var result = transformer.TestBuildParameters(dateArray, null);

        // Assert
        Assert.Equal(2, result!.Length);
        Assert.Equal("2023-01-01", result[0]);
        Assert.Equal("2023-12-31", result[1]);
    }

    [Fact]
    public void BuildParameters_WithMixedTypes_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");
        var mixedArray = new object[] { 100, "200" };

        // Act
        var result = transformer.TestBuildParameters(mixedArray, null);

        // Assert
        Assert.Equal(2, result!.Length);
        Assert.Equal(100, result[0]);
        Assert.Equal("200", result[1]);
    }

    [Fact]
    public void BuildParameters_WithNullElements_ShouldReturnArrayWithNulls()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");
        var arrayWithNulls = new object?[] { null, 10 };

        // Act
        var result = transformer.TestBuildParameters(arrayWithNulls, null);

        // Assert
        Assert.Equal(2, result!.Length);
        Assert.Null(result[0]);
        Assert.Equal(10, result[1]);
    }

    [Fact]
    public void Transform_WithValidRule_ShouldReturnCorrectQueryAndParameters()
    {
        // Arrange
        var transformer = new TestBetweenTransformer("BETWEEN");
        var rule = new FilterRule("Age", "between", new object[] { 18, 65 });
        var fieldName = "[Age]";

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, fieldName, 0, formatProvider);

        // Assert
        Assert.Equal("[Age] BETWEEN @p0 AND @p1", query);
        Assert.Equal(2, parameters!.Length);
        Assert.Equal(18, parameters[0]);
        Assert.Equal(65, parameters[1]);
    }

    private class TestBetweenTransformer : BetweenTransformerBase
    {
        public TestBetweenTransformer(string operatorName) : base(operatorName)
        {
        }

        protected override string BuildBetweenQuery(string fieldName, string param1, string param2)
        {
            return $"{fieldName} BETWEEN {param1} AND {param2}";
        }

        // Expose protected method for testing
        public object[]? TestBuildParameters(object? value, Dictionary<string, object?>? metadata)
        {
            return BuildParameters(value, metadata);
        }
    }
}
