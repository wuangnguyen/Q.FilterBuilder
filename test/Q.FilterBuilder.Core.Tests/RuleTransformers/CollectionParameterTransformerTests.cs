using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.RuleTransformers;

public class CollectionParameterTransformerTests
{
    [Fact]
    public void Constructor_WithNullOperatorName_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestCollectionParameterTransformer(null!));
    }

    [Fact]
    public void Constructor_WithValidOperatorName_ShouldCreateInstance()
    {
        // Act
        var transformer = new TestCollectionParameterTransformer("CONTAINS");

        // Assert
        Assert.NotNull(transformer);
    }

    [Fact]
    public void BuildParameters_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            transformer.TestBuildParameters(null, null));
        Assert.Contains("CONTAINS operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithSingleValue_ShouldReturnSingleElementArray()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");

        // Act
        var result = transformer.TestBuildParameters("single_value", null);

        // Assert
        Assert.Single(result!);
        Assert.Equal("single_value", result![0]);
    }

    [Fact]
    public void BuildParameters_WithArray_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
        var array = new object[] { 1, 2, 3, 4, 5 };

        // Act
        var result = transformer.TestBuildParameters(array, null);

        // Assert
        Assert.Equal(5, result!.Length);
        Assert.Equal([1, 2, 3, 4, 5], result);
    }

    [Fact]
    public void BuildParameters_WithList_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
        var list = new List<string> { "apple", "banana", "cherry" };

        // Act
        var result = transformer.TestBuildParameters(list, null);

        // Assert
        Assert.Equal(3, result!.Length);
        Assert.Equal("apple", result[0]);
        Assert.Equal("banana", result[1]);
        Assert.Equal("cherry", result[2]);
    }

    [Fact]
    public void BuildParameters_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
        var emptyArray = new object[0];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            transformer.TestBuildParameters(emptyArray, null));
        Assert.Contains("CONTAINS operator requires at least one value", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
        var emptyList = new List<object>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            transformer.TestBuildParameters(emptyList, null));
        Assert.Contains("CONTAINS operator requires at least one value", exception.Message);
    }

    [Fact]
    public void BuildParameters_WithStringValue_ShouldTreatAsNonCollection()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");

        // Act
        var result = transformer.TestBuildParameters("test_string", null);

        // Assert
        Assert.Single(result!);
        Assert.Equal("test_string", result![0]);
    }

    [Fact]
    public void BuildParameters_WithEnumerable_ShouldReturnArray()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
        var enumerable = Enumerable.Range(1, 3);

        // Act
        var result = transformer.TestBuildParameters(enumerable, null);

        // Assert
        Assert.Equal(3, result!.Length);
        Assert.Equal(1, result[0]);
        Assert.Equal(2, result[1]);
        Assert.Equal(3, result[2]);
    }

    [Fact]
    public void BuildParameters_WithNullElementsInCollection_ShouldIncludeNulls()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
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
    public void Transform_WithSingleValue_ShouldReturnSingleCondition()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
        var rule = new FilterRule("Name", "contains", "John");
        var fieldName = "[Name]";
        var parameterName = "@p";

        // Act
        var (query, parameters) = transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("[Name] LIKE @p0", query);
        Assert.Single(parameters!);
        Assert.Equal("John", parameters![0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldReturnJoinedConditions()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
        var rule = new FilterRule("Name", "contains", new[] { "John", "Jane", "Bob" });
        var fieldName = "[Name]";
        var parameterName = "@p";

        // Act
        var (query, parameters) = transformer.Transform(rule, fieldName, parameterName);

        // Assert
        Assert.Equal("([Name] LIKE @p0 OR [Name] LIKE @p1 OR [Name] LIKE @p2)", query);
        Assert.Equal(3, parameters!.Length);
        Assert.Equal("John", parameters[0]);
        Assert.Equal("Jane", parameters[1]);
        Assert.Equal("Bob", parameters[2]);
    }

    [Fact]
    public void BuildQuery_WithNullParameters_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
        var context = new TestTransformContext { Parameters = null };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            transformer.TestBuildQuery("[Field]", "@p", context));
        Assert.Contains("CONTAINS operator requires parameters", exception.Message);
    }

    [Fact]
    public void BuildQuery_WithEmptyParameters_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var transformer = new TestCollectionParameterTransformer("CONTAINS");
        var context = new TestTransformContext { Parameters = [] };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            transformer.TestBuildQuery("[Field]", "@p", context));
        Assert.Contains("CONTAINS operator requires parameters", exception.Message);
    }

    private class TestCollectionParameterTransformer : CollectionParameterTransformer
    {
        public TestCollectionParameterTransformer(string operatorName) : base(operatorName)
        {
        }

        protected override string BuildSingleCondition(string fieldName, string parameterName, int index)
        {
            return $"{fieldName} LIKE {parameterName}{index}";
        }

        protected override string GetConditionJoinOperator()
        {
            return " OR ";
        }

        // Expose protected methods for testing
        public object[]? TestBuildParameters(object? value, Dictionary<string, object?>? metadata)
        {
            return BuildParameters(value, metadata);
        }

        public string TestBuildQuery(string fieldName, string parameterName, TestTransformContext context)
        {
            var baseContext = new TransformContext
            {
                Parameters = context.Parameters,
                Metadata = context.Metadata
            };
            return BuildQuery(fieldName, parameterName, baseContext);
        }
    }

    private class TestTransformContext
    {
        public object[]? Parameters { get; set; }
        public Dictionary<string, object?>? Metadata { get; set; }

        public TestTransformContext()
        {
            Metadata = new Dictionary<string, object?>();
        }
    }
}
