using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Linq.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Linq.Tests.RuleTransformers;

public class BeginsWithRuleTransformerTests
{
    private readonly BeginsWithRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("Name.StartsWith(@p0)", query);
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
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(Name.StartsWith(@p0) || Name.StartsWith(@p1))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("test1", parameters[0]);
        Assert.Equal("test2", parameters[1]);
    }

    [Fact]
    public void Transform_WithList_ShouldHandleListValues()
    {
        // Arrange
        var rule = new FilterRule("Email", "begins_with", new List<string> { "noreply", "donotreply" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Email", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(Email.StartsWith(@p0) || Email.StartsWith(@p1))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("noreply", parameters[0]);
        Assert.Equal("donotreply", parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", null);

        // Act
        var ex = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Name", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("BEGINS_WITH operator requires a non-null value", ex.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", new string[0]);

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("BEGINS_WITH operator requires at least one value", ex.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", new List<string>());

        // Act
        var ex = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Name", 0, new LinqFormatProvider()));

        // Assert
        Assert.Contains("BEGINS_WITH operator requires at least one value", ex.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.Name", "begins_with", "test");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "User.Profile.Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("User.Profile.Name.StartsWith(@p0)", query);
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
        var (query, parameters) = _transformer.Transform(rule, "Name", 2, new LinqFormatProvider());

        // Assert
        Assert.Equal("(Name.StartsWith(@p2) || Name.StartsWith(@p3))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithNumericValues_ShouldHandleNonStringTypes()
    {
        // Arrange
        var rule = new FilterRule("Code", "begins_with", new[] { 123, 456 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Code", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(Code.StartsWith(@p0) || Code.StartsWith(@p1))", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(123, parameters[0]);
        Assert.Equal(456, parameters[1]);
    }

    [Fact]
    public void Transform_WithSpecialCharacters_ShouldPreserveSpecialCharacters()
    {
        // Arrange
        var rule = new FilterRule("Path", "begins_with", "/tmp/");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Path", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("Path.StartsWith(@p0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("/tmp/", parameters[0]);
    }

    [Fact]
    public void Transform_WithUnicodeCharacters_ShouldHandleUnicodeStrings()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", "测试");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("Name.StartsWith(@p0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("测试", parameters[0]);
    }

    [Fact]
    public void Transform_WithEmptyString_ShouldHandleEmptyString()
    {
        // Arrange
        var rule = new FilterRule("Name", "begins_with", "");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Name", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("Name.StartsWith(@p0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("", parameters[0]);
    }
    
    [Fact]
    public void Transform_WithMixedTypes_ShouldHandleMixedTypeArray()
    {
        // Arrange
        var rule = new FilterRule("Data", "begins_with", new object[] { "bad", 999, true });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Data", 0, new LinqFormatProvider());

        // Assert
        Assert.Equal("(Data.StartsWith(@p0) || Data.StartsWith(@p1) || Data.StartsWith(@p2))", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("bad", parameters[0]);
        Assert.Equal(999, parameters[1]);
        Assert.Equal(true, parameters[2]);
    }
}
