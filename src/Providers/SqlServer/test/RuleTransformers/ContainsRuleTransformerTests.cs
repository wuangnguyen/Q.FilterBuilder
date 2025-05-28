using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class ContainsRuleTransformerTests
{
    private readonly ContainsRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateSingleLikeCondition()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("Name LIKE '%' + @param0 + '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateOrConditions()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("(Name LIKE '%' + @param0 + '%' OR Name LIKE '%' + @param1 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("test1", parameters[0]);
        Assert.Equal("test2", parameters[1]);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldGenerateThreeOrConditions()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", new[] { "a", "b", "c" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("(Name LIKE '%' + @param0 + '%' OR Name LIKE '%' + @param1 + '%' OR Name LIKE '%' + @param2 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("a", parameters[0]);
        Assert.Equal("b", parameters[1]);
        Assert.Equal("c", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", new List<string> { "item1", "item2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("(Name LIKE '%' + @param0 + '%' OR Name LIKE '%' + @param1 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("item1", parameters[0]);
        Assert.Equal("item2", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Name", "@param"));
        Assert.Contains("CONTAINS operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", new string[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", "@param"));
        Assert.Contains("CONTAINS operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", "@param"));
        Assert.Contains("CONTAINS operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "contains", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Name]", "@param");

        // Assert
        Assert.Equal("[User].[Profile].[Name] LIKE '%' + @param0 + '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Name", "contains", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@customParam");

        // Assert
        Assert.Equal("(Name LIKE '%' + @customParam0 + '%' OR Name LIKE '%' + @customParam1 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithNumericValues_ShouldHandleNonStringTypes()
    {
        // Arrange
        var rule = new FilterRule("Code", "contains", new[] { 123, 456 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Code", "@param");

        // Assert
        Assert.Equal("(Code LIKE '%' + @param0 + '%' OR Code LIKE '%' + @param1 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(123, parameters[0]);
        Assert.Equal(456, parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var rule = new FilterRule("Description", "contains", "test@domain.com");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Description", "@param");

        // Assert
        Assert.Equal("Description LIKE '%' + @param0 + '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test@domain.com", parameters[0]);
    }

    [Fact]
    public void Transform_WithMixedTypes_ShouldHandleMixedTypeArray()
    {
        // Arrange
        var rule = new FilterRule("Data", "contains", new object[] { "text", 123, true });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Data", "@param");

        // Assert
        Assert.Equal("(Data LIKE '%' + @param0 + '%' OR Data LIKE '%' + @param1 + '%' OR Data LIKE '%' + @param2 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("text", parameters[0]);
        Assert.Equal(123, parameters[1]);
        Assert.Equal(true, parameters[2]);
    }
}
