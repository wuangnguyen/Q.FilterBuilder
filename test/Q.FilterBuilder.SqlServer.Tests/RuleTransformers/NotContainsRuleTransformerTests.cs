using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class NotContainsRuleTransformerTests
{
    private readonly NotContainsRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateSingleNotLikeCondition()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name NOT LIKE '%' + @p0 + '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name NOT LIKE '%' + @p0 + '%' AND Name NOT LIKE '%' + @p1 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("test1", parameters[0]);
        Assert.Equal("test2", parameters[1]);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldGenerateThreeAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Description", "not_contains", new[] { "spam", "virus", "malware" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Description", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Description NOT LIKE '%' + @p0 + '%' AND Description NOT LIKE '%' + @p1 + '%' AND Description NOT LIKE '%' + @p2 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("spam", parameters[0]);
        Assert.Equal("virus", parameters[1]);
        Assert.Equal("malware", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Content", "not_contains", new List<string> { "forbidden", "blocked" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Content", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Content NOT LIKE '%' + @p0 + '%' AND Content NOT LIKE '%' + @p1 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("forbidden", parameters[0]);
        Assert.Equal("blocked", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_CONTAINS operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", new string[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_CONTAINS operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_CONTAINS operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "not_contains", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Name]", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("[User].[Profile].[Name] NOT LIKE '%' + @p0 + '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name NOT LIKE '%' + @p0 + '%' AND Name NOT LIKE '%' + @p1 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithNumericValues_ShouldHandleNonStringTypes()
    {
        // Arrange
        var rule = new FilterRule("Code", "not_contains", new[] { 123, 456 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Code", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Code NOT LIKE '%' + @p0 + '%' AND Code NOT LIKE '%' + @p1 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(123, parameters[0]);
        Assert.Equal(456, parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var rule = new FilterRule("Email", "not_contains", "@spam.com");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Email", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Email NOT LIKE '%' + @p0 + '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("@spam.com", parameters[0]);
    }

    [Fact]
    public void Transform_WithUnicodeCharacters_ShouldHandleUnicodeStrings()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", "测试");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name NOT LIKE '%' + @p0 + '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("测试", parameters[0]);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldHandleEmptyString()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_contains", "");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name NOT LIKE '%' + @p0 + '%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }

    [Fact]
    public void Transform_WithMixedTypes_ShouldHandleMixedTypeArray()
    {
        // Arrange
        var rule = new FilterRule("Data", "not_contains", new object[] { "bad", 666, false });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Data", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Data NOT LIKE '%' + @p0 + '%' AND Data NOT LIKE '%' + @p1 + '%' AND Data NOT LIKE '%' + @p2 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("bad", parameters[0]);
        Assert.Equal(666, parameters[1]);
        Assert.Equal(false, parameters[2]);
    }

    [Fact]
    public void Transform_WithFourValues_ShouldGenerateFourAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Comment", "not_contains", new[] { "hate", "spam", "scam", "fake" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Comment", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Comment NOT LIKE '%' + @p0 + '%' AND Comment NOT LIKE '%' + @p1 + '%' AND Comment NOT LIKE '%' + @p2 + '%' AND Comment NOT LIKE '%' + @p3 + '%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(4, parameters.Length);
        Assert.Equal("hate", parameters[0]);
        Assert.Equal("spam", parameters[1]);
        Assert.Equal("scam", parameters[2]);
        Assert.Equal("fake", parameters[3]);
    }
}
