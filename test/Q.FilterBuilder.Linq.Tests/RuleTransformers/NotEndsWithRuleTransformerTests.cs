using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class NotEndsWithRuleTransformerTests
{
    private readonly NotEndsWithRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!Name.EndsWith(@p0)", query);
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
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(!Name.EndsWith(@p0) && !Name.EndsWith(@p1))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("test1", parameters[0]);
        Assert.Equal("test2", parameters[1]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Email", "not_ends_with", new List<string> { "@spam.com", "@fake.org" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Email", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(!Email.EndsWith(@p0) && !Email.EndsWith(@p1))", query);
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

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Name", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_ENDS_WITH operator requires a non-null value", ex.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", new string[0]);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_ENDS_WITH operator requires at least one value", ex.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "not_ends_with", new List<string>());

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("NOT_ENDS_WITH operator requires at least one value", ex.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "not_ends_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "User.Profile.Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!User.Profile.Name.EndsWith(@p0)", query);
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
        var (query, parameters) = _transformer.Transform(rule, "Name", 2, new LinqFormatProvider());

        // Assert
        Assert.Equal("(!Name.EndsWith(@p2) && !Name.EndsWith(@p3))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithNumericValues_ShouldHandleNonStringTypes()
    {
        // Arrange
        var rule = new FilterRule("Code", "not_ends_with", new[] { 123, 456 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Code", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(!Code.EndsWith(@p0) && !Code.EndsWith(@p1))", query);
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
        var (query, parameters) = _transformer.Transform(rule, "Url", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!Url.EndsWith(@p0)", query);
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
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!Name.EndsWith(@p0)", query);
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
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("!Name.EndsWith(@p0)", query);
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
        var (query, parameters) = _transformer.Transform(rule, "Data", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(!Data.EndsWith(@p0) && !Data.EndsWith(@p1) && !Data.EndsWith(@p2))", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("bad", parameters[0]);
        Assert.Equal(999, parameters[1]);
        Assert.Equal(false, parameters[2]);
    }
}
