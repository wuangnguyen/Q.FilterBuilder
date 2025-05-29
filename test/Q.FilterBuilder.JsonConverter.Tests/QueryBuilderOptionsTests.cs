using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests;

public class QueryBuilderOptionsTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Act
        var options = new QueryBuilderOptions();

        // Assert
        Assert.Equal("condition", options.ConditionPropertyName);
        Assert.Equal("rules", options.RulesPropertyName);
        Assert.Equal("field", options.FieldPropertyName);
        Assert.Equal("operator", options.OperatorPropertyName);
        Assert.Equal("type", options.TypePropertyName);
        Assert.Equal("value", options.ValuePropertyName);
        Assert.Equal("data", options.DataPropertyName);
    }

    [Fact]
    public void ConditionPropertyName_ShouldBeSettable()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.ConditionPropertyName = "combinator";

        // Assert
        Assert.Equal("combinator", options.ConditionPropertyName);
    }

    [Fact]
    public void RulesPropertyName_ShouldBeSettable()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.RulesPropertyName = "children";

        // Assert
        Assert.Equal("children", options.RulesPropertyName);
    }

    [Fact]
    public void FieldPropertyName_ShouldBeSettable()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.FieldPropertyName = "id";

        // Assert
        Assert.Equal("id", options.FieldPropertyName);
    }

    [Fact]
    public void OperatorPropertyName_ShouldBeSettable()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.OperatorPropertyName = "op";

        // Assert
        Assert.Equal("op", options.OperatorPropertyName);
    }

    [Fact]
    public void TypePropertyName_ShouldBeSettable()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.TypePropertyName = "dataType";

        // Assert
        Assert.Equal("dataType", options.TypePropertyName);
    }

    [Fact]
    public void ValuePropertyName_ShouldBeSettable()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.ValuePropertyName = "val";

        // Assert
        Assert.Equal("val", options.ValuePropertyName);
    }

    [Fact]
    public void DataPropertyName_ShouldBeSettable()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.DataPropertyName = "metadata";

        // Assert
        Assert.Equal("metadata", options.DataPropertyName);
    }

    [Fact]
    public void AllProperties_ShouldBeSettableToCustomValues()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.ConditionPropertyName = "combinator";
        options.RulesPropertyName = "children";
        options.FieldPropertyName = "id";
        options.OperatorPropertyName = "op";
        options.TypePropertyName = "dataType";
        options.ValuePropertyName = "val";
        options.DataPropertyName = "metadata";

        // Assert
        Assert.Equal("combinator", options.ConditionPropertyName);
        Assert.Equal("children", options.RulesPropertyName);
        Assert.Equal("id", options.FieldPropertyName);
        Assert.Equal("op", options.OperatorPropertyName);
        Assert.Equal("dataType", options.TypePropertyName);
        Assert.Equal("val", options.ValuePropertyName);
        Assert.Equal("metadata", options.DataPropertyName);
    }

    [Fact]
    public void Properties_ShouldAllowNullValues()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.ConditionPropertyName = null!;
        options.RulesPropertyName = null!;
        options.FieldPropertyName = null!;
        options.OperatorPropertyName = null!;
        options.TypePropertyName = null!;
        options.ValuePropertyName = null!;
        options.DataPropertyName = null!;

        // Assert
        Assert.Null(options.ConditionPropertyName);
        Assert.Null(options.RulesPropertyName);
        Assert.Null(options.FieldPropertyName);
        Assert.Null(options.OperatorPropertyName);
        Assert.Null(options.TypePropertyName);
        Assert.Null(options.ValuePropertyName);
        Assert.Null(options.DataPropertyName);
    }

    [Fact]
    public void Properties_ShouldAllowEmptyStrings()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.ConditionPropertyName = "";
        options.RulesPropertyName = "";
        options.FieldPropertyName = "";
        options.OperatorPropertyName = "";
        options.TypePropertyName = "";
        options.ValuePropertyName = "";
        options.DataPropertyName = "";

        // Assert
        Assert.Equal("", options.ConditionPropertyName);
        Assert.Equal("", options.RulesPropertyName);
        Assert.Equal("", options.FieldPropertyName);
        Assert.Equal("", options.OperatorPropertyName);
        Assert.Equal("", options.TypePropertyName);
        Assert.Equal("", options.ValuePropertyName);
        Assert.Equal("", options.DataPropertyName);
    }

    [Fact]
    public void Properties_ShouldAllowWhitespaceStrings()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.ConditionPropertyName = "   ";
        options.RulesPropertyName = "\t";
        options.FieldPropertyName = "\n";
        options.OperatorPropertyName = " \t\n ";
        options.TypePropertyName = "  ";
        options.ValuePropertyName = "\r\n";
        options.DataPropertyName = " ";

        // Assert
        Assert.Equal("   ", options.ConditionPropertyName);
        Assert.Equal("\t", options.RulesPropertyName);
        Assert.Equal("\n", options.FieldPropertyName);
        Assert.Equal(" \t\n ", options.OperatorPropertyName);
        Assert.Equal("  ", options.TypePropertyName);
        Assert.Equal("\r\n", options.ValuePropertyName);
        Assert.Equal(" ", options.DataPropertyName);
    }

    [Fact]
    public void Properties_ShouldAllowSpecialCharacters()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.ConditionPropertyName = "@condition";
        options.RulesPropertyName = "rules$";
        options.FieldPropertyName = "field-name";
        options.OperatorPropertyName = "operator_type";
        options.TypePropertyName = "type.name";
        options.ValuePropertyName = "value[0]";
        options.DataPropertyName = "data:metadata";

        // Assert
        Assert.Equal("@condition", options.ConditionPropertyName);
        Assert.Equal("rules$", options.RulesPropertyName);
        Assert.Equal("field-name", options.FieldPropertyName);
        Assert.Equal("operator_type", options.OperatorPropertyName);
        Assert.Equal("type.name", options.TypePropertyName);
        Assert.Equal("value[0]", options.ValuePropertyName);
        Assert.Equal("data:metadata", options.DataPropertyName);
    }

    [Fact]
    public void Properties_ShouldAllowUnicodeCharacters()
    {
        // Arrange
        var options = new QueryBuilderOptions();

        // Act
        options.ConditionPropertyName = "条件";
        options.RulesPropertyName = "règles";
        options.FieldPropertyName = "поле";
        options.OperatorPropertyName = "演算子";
        options.TypePropertyName = "τύπος";
        options.ValuePropertyName = "قيمة";
        options.DataPropertyName = "डेटा";

        // Assert
        Assert.Equal("条件", options.ConditionPropertyName);
        Assert.Equal("règles", options.RulesPropertyName);
        Assert.Equal("поле", options.FieldPropertyName);
        Assert.Equal("演算子", options.OperatorPropertyName);
        Assert.Equal("τύπος", options.TypePropertyName);
        Assert.Equal("قيمة", options.ValuePropertyName);
        Assert.Equal("डेटा", options.DataPropertyName);
    }
}
