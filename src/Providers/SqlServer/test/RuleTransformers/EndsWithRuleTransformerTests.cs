using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class EndsWithRuleTransformerTests
{
    private readonly EndsWithRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateSingleLikeCondition()
    {
        // Arrange
        var rule = new FilterRule("Name", "ends_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("Name LIKE N'%' + @param0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateOrConditions()
    {
        // Arrange
        var rule = new FilterRule("Name", "ends_with", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("(Name LIKE N'%' + @param0 OR Name LIKE N'%' + @param1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("test1", parameters[0]);
        Assert.Equal("test2", parameters[1]);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldGenerateThreeOrConditions()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", new[] { ".txt", ".pdf", ".doc" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "FileName", "@param");

        // Assert
        Assert.Equal("(FileName LIKE N'%' + @param0 OR FileName LIKE N'%' + @param1 OR FileName LIKE N'%' + @param2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(".txt", parameters[0]);
        Assert.Equal(".pdf", parameters[1]);
        Assert.Equal(".doc", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Email", "ends_with", new List<string> { "@gmail.com", "@yahoo.com" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Email", "@param");

        // Assert
        Assert.Equal("(Email LIKE N'%' + @param0 OR Email LIKE N'%' + @param1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("@gmail.com", parameters[0]);
        Assert.Equal("@yahoo.com", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "ends_with", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Name", "@param"));
        Assert.Contains("ENDS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "ends_with", new string[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", "@param"));
        Assert.Contains("ENDS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "ends_with", new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", "@param"));
        Assert.Contains("ENDS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "ends_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Name]", "@param");

        // Assert
        Assert.Equal("[User].[Profile].[Name] LIKE N'%' + @param0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Name", "ends_with", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@customParam");

        // Assert
        Assert.Equal("(Name LIKE N'%' + @customParam0 OR Name LIKE N'%' + @customParam1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithNumericValues_ShouldHandleNonStringTypes()
    {
        // Arrange
        var rule = new FilterRule("Code", "ends_with", new[] { 123, 456 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Code", "@param");

        // Assert
        Assert.Equal("(Code LIKE N'%' + @param0 OR Code LIKE N'%' + @param1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(123, parameters[0]);
        Assert.Equal(456, parameters[1]);
    }

    [Fact]
    public void Transform_WithUnicodeCharacters_ShouldHandleUnicodeStrings()
    {
        // Arrange
        var rule = new FilterRule("Name", "ends_with", "测试");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("Name LIKE N'%' + @param0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("测试", parameters[0]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var rule = new FilterRule("Url", "ends_with", ".html");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Url", "@param");

        // Assert
        Assert.Equal("Url LIKE N'%' + @param0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(".html", parameters[0]);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldHandleEmptyString()
    {
        // Arrange
        var rule = new FilterRule("Name", "ends_with", "");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", "@param");

        // Assert
        Assert.Equal("Name LIKE N'%' + @param0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }

    [Fact]
    public void Transform_WithFileExtensions_ShouldHandleCommonFileExtensions()
    {
        // Arrange
        var rule = new FilterRule("FileName", "ends_with", new[] { ".jpg", ".png", ".gif", ".bmp" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "FileName", "@param");

        // Assert
        Assert.Equal("(FileName LIKE N'%' + @param0 OR FileName LIKE N'%' + @param1 OR FileName LIKE N'%' + @param2 OR FileName LIKE N'%' + @param3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(4, parameters.Length);
        Assert.Equal(".jpg", parameters[0]);
        Assert.Equal(".png", parameters[1]);
        Assert.Equal(".gif", parameters[2]);
        Assert.Equal(".bmp", parameters[3]);
    }
}
