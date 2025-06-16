using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class NotBeginsWithRuleTransformerTests
{
    private readonly NotBeginsWithRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateSingleNotLikeCondition()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name NOT LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name NOT LIKE @p0 + N'%' AND Name NOT LIKE @p1 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("test1", parameters[0]);
        Assert.Equal("test2", parameters[1]);
    }

    [Fact]
    public void Transform_WithThreeValues_ShouldGenerateThreeAndConditions()
    {
        // Arrange
        var rule = new FilterRule("Username", "not_begins_with", new[] { "admin", "root", "system" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Username", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Username NOT LIKE @p0 + N'%' AND Username NOT LIKE @p1 + N'%' AND Username NOT LIKE @p2 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("admin", parameters[0]);
        Assert.Equal("root", parameters[1]);
        Assert.Equal("system", parameters[2]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Email", "not_begins_with", new List<string> { "noreply", "donotreply" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Email", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Email NOT LIKE @p0 + N'%' AND Email NOT LIKE @p1 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("noreply", parameters[0]);
        Assert.Equal("donotreply", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_BEGINS_WITH operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", new string[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_BEGINS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_BEGINS_WITH operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "not_begins_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[Name]", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("[User].[Profile].[Name] NOT LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("test", parameters[0]);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", new[] { "test1", "test2" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Name NOT LIKE @p0 + N'%' AND Name NOT LIKE @p1 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithNumericValues_ShouldHandleNonStringTypes()
    {
        // Arrange
        var rule = new FilterRule("Code", "not_begins_with", new[] { 123, 456 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Code", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Code NOT LIKE @p0 + N'%' AND Code NOT LIKE @p1 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(123, parameters[0]);
        Assert.Equal(456, parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var rule = new FilterRule("Path", "not_begins_with", "/tmp/");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Path", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Path NOT LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("/tmp/", parameters[0]);
    }

    [Fact]
    public void Transform_WithUnicodeCharacters_ShouldHandleUnicodeStrings()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", "测试");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name NOT LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("测试", parameters[0]);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldHandleEmptyString()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_begins_with", "");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Name NOT LIKE @p0 + N'%'", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }

    [Fact]
    public void Transform_WithMixedTypes_ShouldHandleMixedTypeArray()
    {
        // Arrange
        var rule = new FilterRule("Data", "not_begins_with", new object[] { "bad", 999, true });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Data", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(Data NOT LIKE @p0 + N'%' AND Data NOT LIKE @p1 + N'%' AND Data NOT LIKE @p2 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("bad", parameters[0]);
        Assert.Equal(999, parameters[1]);
        Assert.Equal(true, parameters[2]);
    }

    [Fact]
    public void Transform_WithSystemPrefixes_ShouldHandleSystemPrefixes()
    {
        // Arrange
        var rule = new FilterRule("TableName", "not_begins_with", new[] { "sys_", "temp_", "bak_", "old_" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "TableName", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("(TableName NOT LIKE @p0 + N'%' AND TableName NOT LIKE @p1 + N'%' AND TableName NOT LIKE @p2 + N'%' AND TableName NOT LIKE @p3 + N'%')", query);
        Assert.NotNull(parameters);
        Assert.Equal(4, parameters.Length);
        Assert.Equal("sys_", parameters[0]);
        Assert.Equal("temp_", parameters[1]);
        Assert.Equal("bak_", parameters[2]);
        Assert.Equal("old_", parameters[3]);
    }
}
