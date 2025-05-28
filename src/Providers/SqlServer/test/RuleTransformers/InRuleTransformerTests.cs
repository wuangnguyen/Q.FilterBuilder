using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class InRuleTransformerTests
{
    private readonly InRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateInQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", "Active");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", "@param");

        // Assert
        Assert.Equal("Status IN (@param0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("Active", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleStringValues_ShouldGenerateInQueryWithMultipleParameters()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", new[] { "Active", "Pending", "Completed" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", "@param");

        // Assert
        Assert.Equal("Status IN (@param0, @param1, @param2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("Active", parameters[0]);
        Assert.Equal("Pending", parameters[1]);
        Assert.Equal("Completed", parameters[2]);
    }

    [Fact]
    public void Transform_WithIntegerList_ShouldHandleIntegerValues()
    {
        // Arrange
        var rule = new FilterRule("Id", "in", new List<int> { 1, 2, 3, 4, 5 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Id", "@param");

        // Assert
        Assert.Equal("Id IN (@param0, @param1, @param2, @param3, @param4)", query);
        Assert.NotNull(parameters);
        Assert.Equal(5, parameters.Length);
        Assert.Equal(1, parameters[0]);
        Assert.Equal(2, parameters[1]);
        Assert.Equal(3, parameters[2]);
        Assert.Equal(4, parameters[3]);
        Assert.Equal(5, parameters[4]);
    }

    [Fact]
    public void Transform_WithMixedTypes_ShouldHandleMixedTypeArray()
    {
        // Arrange
        var rule = new FilterRule("Value", "in", new object[] { "text", 123, true, 45.67 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Value", "@param");

        // Assert
        Assert.Equal("Value IN (@param0, @param1, @param2, @param3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(4, parameters.Length);
        Assert.Equal("text", parameters[0]);
        Assert.Equal(123, parameters[1]);
        Assert.Equal(true, parameters[2]);
        Assert.Equal(45.67, parameters[3]);
    }

    [Fact]
    public void Transform_WithGuidValues_ShouldHandleGuidArray()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var rule = new FilterRule("UserId", "in", new[] { guid1, guid2 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "UserId", "@param");

        // Assert
        Assert.Equal("UserId IN (@param0, @param1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(guid1, parameters[0]);
        Assert.Equal(guid2, parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Status", "@param"));
        Assert.Contains("IN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", new string[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Status", "@param"));
        Assert.Contains("IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Status", "@param"));
        Assert.Contains("IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("Status", "in", new[] { "Active", "Pending" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", "@customParam");

        // Assert
        Assert.Equal("Status IN (@customParam0, @customParam1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Status", "in", new[] { "Active", "Pending" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Status]", "@param");

        // Assert
        Assert.Equal("[User].[Status] IN (@param0, @param1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithSingleNumericValue_ShouldHandleSingleNumber()
    {
        // Arrange
        var rule = new FilterRule("CategoryId", "in", 5);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CategoryId", "@param");

        // Assert
        Assert.Equal("CategoryId IN (@param0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(5, parameters[0]);
    }

    [Fact]
    public void Transform_WithDecimalValues_ShouldHandleDecimalArray()
    {
        // Arrange
        var rule = new FilterRule("Price", "in", new[] { 9.99m, 19.99m, 29.99m });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Price", "@param");

        // Assert
        Assert.Equal("Price IN (@param0, @param1, @param2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(9.99m, parameters[0]);
        Assert.Equal(19.99m, parameters[1]);
        Assert.Equal(29.99m, parameters[2]);
    }

    [Fact]
    public void Transform_WithDateTimeValues_ShouldHandleDateTimeArray()
    {
        // Arrange
        var date1 = new DateTime(2023, 1, 1);
        var date2 = new DateTime(2023, 6, 1);
        var date3 = new DateTime(2023, 12, 1);
        var rule = new FilterRule("CreatedDate", "in", new[] { date1, date2, date3 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", "@param");

        // Assert
        Assert.Equal("CreatedDate IN (@param0, @param1, @param2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(date1, parameters[0]);
        Assert.Equal(date2, parameters[1]);
        Assert.Equal(date3, parameters[2]);
    }

    [Fact]
    public void Transform_WithBooleanValues_ShouldHandleBooleanArray()
    {
        // Arrange
        var rule = new FilterRule("IsActive", "in", new[] { true, false });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "IsActive", "@param");

        // Assert
        Assert.Equal("IsActive IN (@param0, @param1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(true, parameters[0]);
        Assert.Equal(false, parameters[1]);
    }

    [Fact]
    public void Transform_WithLargeArray_ShouldHandleManyValues()
    {
        // Arrange
        var values = new int[10];
        for (int i = 0; i < 10; i++)
        {
            values[i] = i + 1;
        }
        var rule = new FilterRule("Id", "in", values);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Id", "@param");

        // Assert
        Assert.Equal("Id IN (@param0, @param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9)", query);
        Assert.NotNull(parameters);
        Assert.Equal(10, parameters.Length);
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(i + 1, parameters[i]);
        }
    }

    [Fact]
    public void Transform_WithNullValuesInArray_ShouldIncludeNullValues()
    {
        // Arrange
        var rule = new FilterRule("Value", "in", new object?[] { "test", null, 123 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Value", "@param");

        // Assert
        Assert.Equal("Value IN (@param0, @param1, @param2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("test", parameters[0]);
        Assert.Null(parameters[1]);
        Assert.Equal(123, parameters[2]);
    }
}
