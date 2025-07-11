using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.Providers;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.RuleTransformers;

public class InTransformerBaseTests
{
    [Fact]
    public void Constructor_WithNullOperatorName_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestInTransformer(null!));
    }

    [Fact]
    public void Constructor_WithValidOperatorName_ShouldCreateInstance()
    {
        // Act
        var transformer = new TestInTransformer("IN");

        // Assert
        Assert.NotNull(transformer);
    }

    [Fact]
    public void BuildParameters_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            transformer.TestBuildParameters(null, null));
        Assert.Contains("IN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithNonCollectionValue_ShouldWrapInArray()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");

        // Act
        var result = transformer.TestBuildParameters("single_value", null);

        // Assert
        Assert.Single(result!);
        Assert.Equal("single_value", result![0]);
    }

    [Fact]
    public void BuildParameters_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var emptyArray = new object[0];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            transformer.TestBuildParameters(emptyArray, null));
        Assert.Contains("IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithValidArray_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var validArray = new object[] { 1, 2, 3, 4, 5 };

        // Act
        var result = transformer.TestBuildParameters(validArray, null);

        // Assert
        Assert.Equal(5, result!.Length);
        Assert.Equal([1, 2, 3, 4, 5], result);
    }

    [Fact]
    public void BuildParameters_WithValidList_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var validList = new List<string> { "apple", "banana", "cherry" };

        // Act
        var result = transformer.TestBuildParameters(validList, null);

        // Assert
        Assert.Equal(3, result!.Length);
        Assert.Equal("apple", result[0]);
        Assert.Equal("banana", result[1]);
        Assert.Equal("cherry", result[2]);
    }

    [Fact]
    public void BuildParameters_WithSingleElementArray_ShouldReturnSingleElementArray()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var singleArray = new object[] { "only_value" };

        // Act
        var result = transformer.TestBuildParameters(singleArray, null);

        // Assert
        Assert.Single(result!);
        Assert.Equal("only_value", result![0]);
    }

    [Fact]
    public void BuildParameters_WithMixedTypes_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var mixedArray = new object[] { 1, "two", 3.0, true };

        // Act
        var result = transformer.TestBuildParameters(mixedArray, null);

        // Assert
        Assert.Equal(4, result!.Length);
        Assert.Equal(1, result[0]);
        Assert.Equal("two", result[1]);
        Assert.Equal(3.0, result[2]);
        Assert.Equal(true, result[3]);
    }

    [Fact]
    public void BuildParameters_WithNullElements_ShouldReturnArrayWithNulls()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var arrayWithNulls = new object?[] { 1, null, 3 };

        // Act
        var result = transformer.TestBuildParameters(arrayWithNulls, null);

        // Assert
        Assert.Equal(3, result!.Length);
        Assert.Equal(1, result[0]);
        Assert.Null(result[1]);
        Assert.Equal(3, result[2]);
    }

    [Fact]
    public void BuildParameters_WithEnumerable_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var enumerable = Enumerable.Range(1, 5);

        // Act
        var result = transformer.TestBuildParameters(enumerable, null);

        // Assert
        Assert.Equal(5, result!.Length);
        Assert.Equal([1, 2, 3, 4, 5], result);
    }

    [Fact]
    public void BuildParameters_WithHashSet_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var hashSet = new HashSet<int> { 3, 1, 4, 1, 5 }; // Note: HashSet will remove duplicates

        // Act
        var result = transformer.TestBuildParameters(hashSet, null);

        // Assert
        Assert.Equal(4, result!.Length); // 4 unique values
        Assert.Contains(1, result);
        Assert.Contains(3, result);
        Assert.Contains(4, result);
        Assert.Contains(5, result);
    }

    [Fact]
    public void Transform_WithValidRule_ShouldReturnCorrectQueryAndParameters()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var rule = new FilterRule("Status", "in", new object[] { "Active", "Pending", "Approved" });
        var fieldName = "[Status]";

        // Act
        var formatProvider = new TestFormatProvider();
        var (query, parameters) = transformer.Transform(rule, fieldName, 0, formatProvider);

        // Assert
        Assert.Equal("[Status] IN (@p0, @p1, @p2)", query);
        Assert.Equal(3, parameters!.Length);
        Assert.Equal("Active", parameters[0]);
        Assert.Equal("Pending", parameters[1]);
        Assert.Equal("Approved", parameters[2]);
    }

    [Fact]
    public void BuildParameters_WithStringValue_ShouldWrapInArray()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");

        // Act
        var result = transformer.TestBuildParameters("not_a_collection", null);

        // Assert
        Assert.Single(result!);
        Assert.Equal("not_a_collection", result![0]);
    }

    [Fact]
    public void GenerateParameterPlaceholders_WithFormatProvider_ShouldUseFormatProvider()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var context = new BaseRuleTransformer.TransformContext { ParameterIndex = 2 };
        context.FormatProvider = new TestFormatProvider();

        // Act
        var result = transformer.TestGenerateParameterPlaceholders("ignored", 3, context);

        // Assert
        Assert.Equal(new[] { "@p2", "@p3", "@p4" }, result);
    }

    [Fact]
    public void GenerateParameterPlaceholders_WithAtParameterName_ShouldUseAtIndex()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var context = new BaseRuleTransformer.TransformContext { ParameterIndex = 0 };
        context.FormatProvider = null;

        // Act
        var result = transformer.TestGenerateParameterPlaceholders("@5", 3, context);

        // Assert
        Assert.Equal(new[] { "@5", "@6", "@7" }, result);
    }

    [Fact]
    public void GenerateParameterPlaceholders_DefaultBehavior_ShouldUseParameterNameWithIndex()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var context = new BaseRuleTransformer.TransformContext { ParameterIndex = 0 };
        context.FormatProvider = null;

        // Act
        var result = transformer.TestGenerateParameterPlaceholders("param", 2, context);

        // Assert
        Assert.Equal(new[] { "param0", "param1" }, result);
    }

    [Fact]
    public void BuildQuery_WithNullParameters_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var context = new BaseRuleTransformer.TransformContext { Parameters = null };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => transformer.TestBuildQuery("Field", context));
        Assert.Contains("IN operator requires parameters", ex.Message);
    }

    [Fact]
    public void BuildQuery_WithEmptyParameters_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var transformer = new TestInTransformer("IN");
        var context = new BaseRuleTransformer.TransformContext { Parameters = [] };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => transformer.TestBuildQuery("Field", context));
        Assert.Contains("IN operator requires parameters", ex.Message);
    }

    private class TestInTransformer : InTransformerBase
    {
        public TestInTransformer(string operatorName) : base(operatorName) { }
        protected override string BuildInQuery(string fieldName, string parameterList) => $"{fieldName} IN ({parameterList})";
        public object[]? TestBuildParameters(object? value, Dictionary<string, object?>? metadata) => BuildParameters(value, metadata);
        public string[] TestGenerateParameterPlaceholders(string parameterName, int count, object context) => GenerateParameterPlaceholders(parameterName, count, (TransformContext)context);
        public string TestBuildQuery(string fieldName, object context) => BuildQuery(fieldName, (TransformContext)context);
    }

    // Dummy format provider for testing
    private class TestFormatProvider : IQueryFormatProvider
    {
        public string FormatParameterName(int index) => $"@p{index}";
        public string FormatFieldName(string fieldName) => fieldName;
        public string AndOperator => "&&";
        public string OrOperator => "||";
        public string ParameterPrefix => string.Empty;
    }
}
