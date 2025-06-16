using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class BeginsWithRuleTransformerTests
{
    private readonly BeginsWithRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateSingleLikeCondition()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateOrConditions()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name LIKE @p0 + N'%' OR Name LIKE @p1 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("test1", parameters[0]);
        Assert.Equal("test2", parameters[1]);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldGenerateThreeOrConditions()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", new[] { "A", "B", "C" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name LIKE @p0 + N'%' OR Name LIKE @p1 + N'%' OR Name LIKE @p2 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("A", parameters[0]);
        Assert.Equal("B", parameters[1]);
        Assert.Equal("C", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", new List<string> { "prefix1", "prefix2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name LIKE @p0 + N'%' OR Name LIKE @p1 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("prefix1", parameters[0]);
        Assert.Equal("prefix2", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("BEGINS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", new string[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("BEGINS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("BEGINS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "begins_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Name]", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("[User].[Profile].[Name] LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name LIKE @p0 + N'%' OR Name LIKE @p1 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithNumericValues_ShouldHandleNonStringTypes()
    {
        // Arrange
        var rule = new FilterRule("Code", "begins_with", new[] { 123, 456 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Code", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Code LIKE @p0 + N'%' OR Code LIKE @p1 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(123, parameters[0]);
        Assert.Equal(456, parameters[1]);
    }

    [Fact]
    public void Transform_WithUnicodeCharacters_ShouldHandleUnicodeStrings()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", "测试");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("测试", parameters[0]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var rule = new FilterRule("Email", "begins_with", "admin@");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Email", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Email LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("admin@", parameters[0]);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldHandleEmptyString()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", "");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
}
