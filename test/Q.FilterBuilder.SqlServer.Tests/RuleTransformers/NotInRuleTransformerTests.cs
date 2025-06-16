using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class NotInRuleTransformerTests
{
    private readonly NotInRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithSingleValue_ShouldGenerateNotInQuery()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", "Inactive");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Status NOT IN (@p0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("Inactive", parameters[0]);
    }

    [Fact]
    public void Transform_WithMultipleValues_ShouldGenerateNotInQueryWithSequentialParameters()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new[] { "Inactive", "Deleted", "Archived" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Status NOT IN (@p0, @p1, @p2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("Inactive", parameters[0]);
        Assert.Equal("Deleted", parameters[1]);
        Assert.Equal("Archived", parameters[2]);
    }

    [Fact]
    public void Transform_WithIntegerList_ShouldHandleIntegerValues()
    {
        // Arrange
        var rule = new FilterRule("Id", "not_in", new List<int> { 1, 2, 3 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Id", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Id NOT IN (@p0, @p1, @p2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal(1, parameters[0]);
        Assert.Equal(2, parameters[1]);
        Assert.Equal(3, parameters[2]);
    }

    [Fact]
    public void Transform_WithMixedTypes_ShouldHandleMixedTypeArray()
    {
        // Arrange
        var rule = new FilterRule("Value", "not_in", new object[] { "bad", 666, false });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Value", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Value NOT IN (@p0, @p1, @p2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("bad", parameters[0]);
        Assert.Equal(666, parameters[1]);
        Assert.Equal(false, parameters[2]);
    }

    [Fact]
    public void Transform_WithNonIndexedParameterName_ShouldUseDefaultBehavior()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new[] { "Inactive", "Deleted" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Status NOT IN (@p0, @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithGuidValues_ShouldHandleGuidArray()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var rule = new FilterRule("UserId", "not_in", new[] { guid1, guid2 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "UserId", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("UserId NOT IN (@p0, @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(guid1, parameters[0]);
        Assert.Equal(guid2, parameters[1]);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "Status", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_IN operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new string[0]);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Status", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithEmptyList_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new List<string>());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "Status", 0, new SqlServerFormatProvider()));
        Assert.Contains("NOT_IN operator requires at least one value", exception.Message);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Status", "not_in", new[] { "Inactive", "Deleted" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Status]", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("[User].[Status] NOT IN (@p0, @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
    }

    [Fact]
    public void Transform_WithSingleNumericValue_ShouldHandleSingleNumber()
    {
        // Arrange
        var rule = new FilterRule("CategoryId", "not_in", 5);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CategoryId", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("CategoryId NOT IN (@p0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(5, parameters[0]);
    }

    [Fact]
    public void Transform_WithDecimalValues_ShouldHandleDecimalArray()
    {
        // Arrange
        var rule = new FilterRule("Price", "not_in", new[] { 9.99m, 19.99m, 29.99m });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Price", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Price NOT IN (@p0, @p1, @p2)", query);
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
        var rule = new FilterRule("CreatedDate", "not_in", new[] { date1, date2 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("CreatedDate NOT IN (@p0, @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal(date1, parameters[0]);
        Assert.Equal(date2, parameters[1]);
    }

    [Fact]
    public void Transform_WithBooleanValues_ShouldHandleBooleanArray()
    {
        // Arrange
        var rule = new FilterRule("IsActive", "not_in", new[] { false });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "IsActive", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("IsActive NOT IN (@p0)", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(false, parameters[0]);
    }

    [Fact]
    public void Transform_WithLargeArray_ShouldHandleManyValues()
    {
        // Arrange
        var values = new int[5];
        for (int i = 0; i < 5; i++)
        {
            values[i] = i + 100;
        }
        var rule = new FilterRule("Id", "not_in", values);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Id", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Id NOT IN (@p0, @p1, @p2, @p3, @p4)", query);
        Assert.NotNull(parameters);
        Assert.Equal(5, parameters.Length);
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(i + 100, parameters[i]);
        }
    }

    [Fact]
    public void Transform_WithNullValuesInArray_ShouldIncludeNullValues()
    {
        // Arrange
        var rule = new FilterRule("Value", "not_in", new object?[] { "test", null, 123 });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Value", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Value NOT IN (@p0, @p1, @p2)", query);
        Assert.NotNull(parameters);
        Assert.Equal(3, parameters.Length);
        Assert.Equal("test", parameters[0]);
        Assert.Null(parameters[1]);
        Assert.Equal(123, parameters[2]);
    }

    [Fact]
    public void Transform_WithZeroBasedIndex_ShouldStartFromZero()
    {
        // Arrange
        var rule = new FilterRule("Status", "not_in", new[] { "Draft", "Review" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Status", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Status NOT IN (@p0, @p1)", query);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Equal("Draft", parameters[0]);
        Assert.Equal("Review", parameters[1]);
    }

    [Fact]
    public void Transform_WithHighIndex_ShouldContinueSequence()
    {
        // Arrange
        var rule = new FilterRule("Priority", "not_in", new[] { "Low", "Medium", "High", "Critical" });

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Priority", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("Priority NOT IN (@p0, @p1, @p2, @p3)", query);
        Assert.NotNull(parameters);
        Assert.Equal(4, parameters.Length);
        Assert.Equal("Low", parameters[0]);
        Assert.Equal("Medium", parameters[1]);
        Assert.Equal("High", parameters[2]);
        Assert.Equal("Critical", parameters[3]);
    }
}
