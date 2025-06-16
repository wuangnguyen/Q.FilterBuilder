using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class NotEndsWithRuleTransformerTests
{
    private readonly NotEndsWithRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateSingleNotLikeCondition()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name NOT LIKE N'%' + @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name NOT LIKE N'%' + @p0 AND Name NOT LIKE N'%' + @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("test1", parameters[0]);
        Assert.Equal("test2", parameters[1]);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldGenerateThreeAndConditions()
    {
        // Arrange
        var rule = new FilterRule("FileName", "not_ends_with", new[] { ".tmp", ".bak", ".old" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "FileName", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(FileName NOT LIKE N'%' + @p0 AND FileName NOT LIKE N'%' + @p1 AND FileName NOT LIKE N'%' + @p2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(".tmp", parameters[0]);
        Assert.Equal(".bak", parameters[1]);
        Assert.Equal(".old", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Email", "not_ends_with", new List<string> { "@spam.com", "@fake.org" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Email", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Email NOT LIKE N'%' + @p0 AND Email NOT LIKE N'%' + @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("@spam.com", parameters[0]);
        Assert.Equal("@fake.org", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_ENDS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", new string[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_ENDS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_ENDS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "not_ends_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Name]", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("[User].[Profile].[Name] NOT LIKE N'%' + @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name NOT LIKE N'%' + @p0 AND Name NOT LIKE N'%' + @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithNumericValues_ShouldHandleNonStringTypes()
    {
        // Arrange
        var rule = new FilterRule("Code", "not_ends_with", new[] { 123, 456 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Code", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Code NOT LIKE N'%' + @p0 AND Code NOT LIKE N'%' + @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(123, parameters[0]);
        Assert.Equal(456, parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var rule = new FilterRule("Url", "not_ends_with", ".exe");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Url", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Url NOT LIKE N'%' + @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(".exe", parameters[0]);
    }

    [Fact]
    public void Transform_WithUnicodeCharacters_ShouldHandleUnicodeStrings()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", "测试");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name NOT LIKE N'%' + @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("测试", parameters[0]);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldHandleEmptyString()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", "");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name NOT LIKE N'%' + @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }

    [Fact]
    public void Transform_WithMixedTypes_ShouldHandleMixedTypeArray()
    {
        // Arrange
        var rule = new FilterRule("Data", "not_ends_with", new object[] { "bad", 999, false });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Data", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Data NOT LIKE N'%' + @p0 AND Data NOT LIKE N'%' + @p1 AND Data NOT LIKE N'%' + @p2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("bad", parameters[0]);
        Assert.Equal(999, parameters[1]);
        Assert.Equal(false, parameters[2]);
    }

    [Fact]
    public void Transform_WithExecutableExtensions_ShouldHandleExecutableExtensions()
    {
        // Arrange
        var rule = new FilterRule("FileName", "not_ends_with", new[] { ".exe", ".bat", ".cmd", ".scr" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "FileName", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(FileName NOT LIKE N'%' + @p0 AND FileName NOT LIKE N'%' + @p1 AND FileName NOT LIKE N'%' + @p2 AND FileName NOT LIKE N'%' + @p3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(4, parameters.Length);
        Assert.Equal(".exe", parameters[0]);
        Assert.Equal(".bat", parameters[1]);
        Assert.Equal(".cmd", parameters[2]);
        Assert.Equal(".scr", parameters[3]);
    }

    [Fact]
    public void Transform_WithDomainSuffixes_ShouldHandleDomainSuffixes()
    {
        // Arrange
        var rule = new FilterRule("Website", "not_ends_with", new[] { ".ru", ".cn", ".tk", ".ml" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Website", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Website NOT LIKE N'%' + @p0 AND Website NOT LIKE N'%' + @p1 AND Website NOT LIKE N'%' + @p2 AND Website NOT LIKE N'%' + @p3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(4, parameters.Length);
        Assert.Equal(".ru", parameters[0]);
        Assert.Equal(".cn", parameters[1]);
        Assert.Equal(".tk", parameters[2]);
        Assert.Equal(".ml", parameters[3]);
    }
}
