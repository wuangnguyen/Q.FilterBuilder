using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.PostgreSql.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.RuleTransformers;

public class DateDiffRuleTransformerTests
{
    private readonly DateDiffRuleTransformer _transformer;

    public DateDiffRuleTransformerTests()
    {
        _transformer = new DateDiffRuleTransformer();
    }

    [Fact]
    public void Transform_WithDefaultInterval_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 30);
        var fieldName = "\"CreatedDate\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("EXTRACT(day FROM NOW() - \"CreatedDate\") = $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(30, parameters[0]);
    }

    [Fact]
    public void Transform_WithDayInterval_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("LastLogin", "date_diff", 7);
        rule.Metadata = new Dictionary<string, object?> { { "intervalType", "day" } };
        var fieldName = "\"LastLogin\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("EXTRACT(day FROM NOW() - \"LastLogin\") = $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(7, parameters[0]);
    }

    [Fact]
    public void Transform_WithHourInterval_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("LastActivity", "date_diff", 24);
        rule.Metadata = new Dictionary<string, object?> { { "intervalType", "hour" } };
        var fieldName = "\"LastActivity\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("EXTRACT(hour FROM NOW() - \"LastActivity\") = $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(24, parameters[0]);
    }

    [Fact]
    public void Transform_WithMonthInterval_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("BirthDate", "date_diff", 6);
        rule.Metadata = new Dictionary<string, object?> { { "intervalType", "month" } };
        var fieldName = "\"BirthDate\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("EXTRACT(month FROM NOW() - \"BirthDate\") = $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(6, parameters[0]);
    }

    [Fact]
    public void Transform_WithYearInterval_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("StartDate", "date_diff", 2);
        rule.Metadata = new Dictionary<string, object?> { { "intervalType", "year" } };
        var fieldName = "\"StartDate\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("EXTRACT(year FROM NOW() - \"StartDate\") = $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(2, parameters[0]);
    }

    [Fact]
    public void Transform_WithMinuteInterval_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("Timestamp", "date_diff", 30);
        rule.Metadata = new Dictionary<string, object?> { { "intervalType", "minute" } };
        var fieldName = "\"Timestamp\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("EXTRACT(minute FROM NOW() - \"Timestamp\") = $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(30, parameters[0]);
    }

    [Fact]
    public void Transform_WithSecondInterval_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("EventTime", "date_diff", 45);
        rule.Metadata = new Dictionary<string, object?> { { "intervalType", "second" } };
        var fieldName = "\"EventTime\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("EXTRACT(second FROM NOW() - \"EventTime\") = $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(45, parameters[0]);
    }

    [Fact]
    public void Transform_WithInvalidInterval_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 30);
        rule.Metadata = new Dictionary<string, object?> { { "intervalType", "invalid" } };
        var fieldName = "\"CreatedDate\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("Invalid interval type 'invalid'", exception.Message);
        Assert.Contains("Valid types are: year, month, day, hour, minute, second", exception.Message);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", null);
        var fieldName = "\"CreatedDate\"";
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider()));
        Assert.Contains("DATE_DIFF operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithCaseInsensitiveInterval_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var rule = new FilterRule("UpdatedDate", "date_diff", 15);
        rule.Metadata = new Dictionary<string, object?> { { "intervalType", "HOUR" } };
        var fieldName = "\"UpdatedDate\"";
        // Act
        var (query, parameters) = _transformer.Transform(rule, fieldName, 0, new PostgreSqlFormatProvider());

        // Assert
        Assert.Equal("EXTRACT(hour FROM NOW() - \"UpdatedDate\") = $1", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(15, parameters[0]);
    }
}
