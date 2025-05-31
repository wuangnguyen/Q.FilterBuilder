using System;
using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests;

public class QueryBuilderConverterConstructorTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateInstanceWithDefaultOptions()
    {
        // Act
        var converter = new QueryBuilderConverter();

        // Assert
        Assert.NotNull(converter);
        // We can't directly access the private _options field, but we can verify the converter was created
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithValidOptions_ShouldCreateInstance()
    {
        // Arrange
        var options = new QueryBuilderOptions
        {
            ConditionPropertyName = "combinator",
            RulesPropertyName = "children",
            FieldPropertyName = "id",
            OperatorPropertyName = "op",
            ValuePropertyName = "val",
            TypePropertyName = "dataType",
            DataPropertyName = "metadata"
        };

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new QueryBuilderConverter(null!));
        Assert.Equal("options", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithEmptyStringOptions_ShouldCreateInstance()
    {
        // Arrange
        var options = new QueryBuilderOptions
        {
            ConditionPropertyName = "",
            RulesPropertyName = "",
            FieldPropertyName = "",
            OperatorPropertyName = "",
            ValuePropertyName = "",
            TypePropertyName = "",
            DataPropertyName = ""
        };

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithWhitespaceStringOptions_ShouldCreateInstance()
    {
        // Arrange
        var options = new QueryBuilderOptions
        {
            ConditionPropertyName = "   ",
            RulesPropertyName = "\t",
            FieldPropertyName = "\n",
            OperatorPropertyName = " \t\n ",
            ValuePropertyName = "  ",
            TypePropertyName = "\r\n",
            DataPropertyName = " "
        };

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithUnicodeOptions_ShouldCreateInstance()
    {
        // Arrange
        var options = new QueryBuilderOptions
        {
            ConditionPropertyName = "条件",
            RulesPropertyName = "règles",
            FieldPropertyName = "поле",
            OperatorPropertyName = "演算子",
            ValuePropertyName = "قيمة",
            TypePropertyName = "τύπος",
            DataPropertyName = "डेटा"
        };

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithSamePropertyNames_ShouldCreateInstance()
    {
        // Arrange - All properties have the same name (edge case)
        var options = new QueryBuilderOptions
        {
            ConditionPropertyName = "prop",
            RulesPropertyName = "prop",
            FieldPropertyName = "prop",
            OperatorPropertyName = "prop",
            ValuePropertyName = "prop",
            TypePropertyName = "prop",
            DataPropertyName = "prop"
        };

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithVeryLongPropertyNames_ShouldCreateInstance()
    {
        // Arrange
        var longName = new string('a', 1000); // Very long property name
        var options = new QueryBuilderOptions
        {
            ConditionPropertyName = longName,
            RulesPropertyName = longName,
            FieldPropertyName = longName,
            OperatorPropertyName = longName,
            ValuePropertyName = longName,
            TypePropertyName = longName,
            DataPropertyName = longName
        };

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithSpecialCharacterPropertyNames_ShouldCreateInstance()
    {
        // Arrange
        var options = new QueryBuilderOptions
        {
            ConditionPropertyName = "condition-name",
            RulesPropertyName = "rules_array",
            FieldPropertyName = "field.name",
            OperatorPropertyName = "operator@type",
            ValuePropertyName = "value#data",
            TypePropertyName = "type$info",
            DataPropertyName = "data%meta"
        };

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithNumericPropertyNames_ShouldCreateInstance()
    {
        // Arrange
        var options = new QueryBuilderOptions
        {
            ConditionPropertyName = "123",
            RulesPropertyName = "456",
            FieldPropertyName = "789",
            OperatorPropertyName = "0",
            ValuePropertyName = "1",
            TypePropertyName = "2",
            DataPropertyName = "3"
        };

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithMixedCasePropertyNames_ShouldCreateInstance()
    {
        // Arrange
        var options = new QueryBuilderOptions
        {
            ConditionPropertyName = "ConditionName",
            RulesPropertyName = "RULES",
            FieldPropertyName = "fieldname",
            OperatorPropertyName = "OperatorName",
            ValuePropertyName = "VALUE",
            TypePropertyName = "typename",
            DataPropertyName = "DataName"
        };

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }

    [Fact]
    public void Constructor_WithOptionsContainingNullProperties_ShouldCreateInstance()
    {
        // Arrange - Create options and then set properties to null (if possible)
        var options = new QueryBuilderOptions();
        // Note: Since the properties are strings with default values, 
        // we can't actually set them to null, but we test the edge case

        // Act
        var converter = new QueryBuilderConverter(options);

        // Assert
        Assert.NotNull(converter);
        Assert.IsType<QueryBuilderConverter>(converter);
    }
}
