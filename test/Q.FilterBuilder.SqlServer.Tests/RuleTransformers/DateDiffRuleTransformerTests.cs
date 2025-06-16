using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.Models;

using Q.FilterBuilder.SqlServer.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.RuleTransformers;

public class DateDiffRuleTransformerTests
{
    private readonly DateDiffRuleTransformer _transformer = new();

    [Fact]
    public void Transform_WithDefaultInterval_ShouldUseDayInterval()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 30);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(30, parameters[0]);
    }

    [Fact]
    public void Transform_WithSpecificInterval_ShouldUseSpecifiedInterval()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 6)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "month" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(month, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(6, parameters[0]);
    }

    [Fact]
    public void Transform_WithYearInterval_ShouldUseYearInterval()
    {
        // Arrange
        var rule = new FilterRule("BirthDate", "date_diff", 25)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "year" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "BirthDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(year, BirthDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(25, parameters[0]);
    }

    [Fact]
    public void Transform_WithHourInterval_ShouldUseHourInterval()
    {
        // Arrange
        var rule = new FilterRule("LastLoginDate", "date_diff", 24)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "hour" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "LastLoginDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(hour, LastLoginDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(24, parameters[0]);
    }

    [Fact]
    public void Transform_WithWeekInterval_ShouldUseWeekInterval()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 2)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "week" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(week, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(2, parameters[0]);
    }

    [Fact]
    public void Transform_WithMinuteInterval_ShouldUseMinuteInterval()
    {
        // Arrange
        var rule = new FilterRule("LastActivity", "date_diff", 30)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "minute" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "LastActivity", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(minute, LastActivity, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(30, parameters[0]);
    }

    [Fact]
    public void Transform_WithSecondInterval_ShouldUseSecondInterval()
    {
        // Arrange
        var rule = new FilterRule("Timestamp", "date_diff", 45)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "second" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Timestamp", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(second, Timestamp, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(45, parameters[0]);
    }

    [Fact]
    public void Transform_WithQuarterInterval_ShouldUseQuarterInterval()
    {
        // Arrange
        var rule = new FilterRule("ReportDate", "date_diff", 1)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "quarter" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "ReportDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(quarter, ReportDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(1, parameters[0]);
    }

    [Fact]
    public void Transform_WithInvalidInterval_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 30)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "invalid_interval" } }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider()));
        Assert.Contains("Invalid interval type 'invalid_interval'", exception.Message);
    }

    [Fact]
    public void Transform_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider()));
        Assert.Contains("DATE_DIFF operator requires a non-null value", exception.Message);
    }

    [Fact]
    public void Transform_WithCaseInsensitiveInterval_ShouldWork()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 7)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "WEEK" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(week, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(7, parameters[0]);
    }

    [Fact]
    public void Transform_WithAllValidIntervals_ShouldWork()
    {
        // Arrange
        var validIntervals = new[] { "year", "quarter", "month", "dayofyear", "day", "week", "hour", "minute", "second", "millisecond", "microsecond", "nanosecond" };

        foreach (var interval in validIntervals)
        {
            var rule = new FilterRule("TestDate", "date_diff", 1)
            {
                Metadata = new Dictionary<string, object?> { { "intervalType", interval } }
            };

            // Act
            var (query, parameters) = _transformer.Transform(rule, "TestDate", 0, new SqlServerFormatProvider());

            // Assert
            Assert.Equal($"DATEDIFF({interval}, TestDate, GETDATE()) = @p0", query);
            Assert.NotNull(parameters);
            Assert.Single(parameters);
            Assert.Equal(1, parameters[0]);
        }
    }

    [Fact]
    public void Transform_WithNegativeValue_ShouldHandleNegativeNumbers()
    {
        // Arrange
        var rule = new FilterRule("FutureDate", "date_diff", -30);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "FutureDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, FutureDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(-30, parameters[0]);
    }

    [Fact]
    public void Transform_WithZeroValue_ShouldHandleZero()
    {
        // Arrange
        var rule = new FilterRule("TodayDate", "date_diff", 0);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "TodayDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, TodayDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(0, parameters[0]);
    }

    [Fact]
    public void Transform_WithComplexFieldName_ShouldUseFieldNameAsIs()
    {
        // Arrange
        var rule = new FilterRule("User.Profile.CreatedDate", "date_diff", 30);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "[User].[Profile].[CreatedDate]", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, [User].[Profile].[CreatedDate], GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(30, parameters[0]);
    }

    [Fact]
    public void Transform_WithDifferentParameterName_ShouldUseProvidedParameterName()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 15);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(15, parameters[0]);
    }

    [Fact]
    public void Transform_WithNoMetadata_ShouldUseDefaultDayInterval()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 10);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(10, parameters[0]);
    }

    [Fact]
    public void Transform_WithEmptyMetadata_ShouldUseDefaultDayInterval()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 5)
        {
            Metadata = new Dictionary<string, object?>()
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(5, parameters[0]);
    }

    [Fact]
    public void Transform_WithNullIntervalTypeInMetadata_ShouldUseDefaultDayInterval()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 12)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", null } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(12, parameters[0]);
    }

    [Fact]
    public void Transform_WithEmptyStringIntervalType_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 30)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "" } }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider()));
        Assert.Contains("Invalid interval type ''", exception.Message);
    }

    [Fact]
    public void Transform_WithWhitespaceIntervalType_ShouldThrowArgumentException()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 30)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "   " } }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider()));
        Assert.Contains("Invalid interval type '   '", exception.Message);
    }

    [Fact]
    public void Transform_WithMixedCaseInterval_ShouldWork()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 7)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "MoNtH" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(month, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(7, parameters[0]);
    }

    [Fact]
    public void Transform_WithNonStringIntervalType_ShouldConvertToString()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 30)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", 123 } }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider()));
        Assert.Contains("Invalid interval type '123'", exception.Message);
    }

    [Fact]
    public void Transform_WithDayOfYearInterval_ShouldUseDayOfYearInterval()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 100)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "dayofyear" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(dayofyear, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(100, parameters[0]);
    }

    [Fact]
    public void Transform_WithMillisecondInterval_ShouldUseMillisecondInterval()
    {
        // Arrange
        var rule = new FilterRule("Timestamp", "date_diff", 5000)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "millisecond" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Timestamp", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(millisecond, Timestamp, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(5000, parameters[0]);
    }

    [Fact]
    public void Transform_WithMicrosecondInterval_ShouldUseMicrosecondInterval()
    {
        // Arrange
        var rule = new FilterRule("Timestamp", "date_diff", 1000000)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "microsecond" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Timestamp", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(microsecond, Timestamp, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(1000000, parameters[0]);
    }

    [Fact]
    public void Transform_WithNanosecondInterval_ShouldUseNanosecondInterval()
    {
        // Arrange
        var rule = new FilterRule("Timestamp", "date_diff", 1000000000)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", "nanosecond" } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "Timestamp", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(nanosecond, Timestamp, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(1000000000, parameters[0]);
    }

    [Theory]
    [InlineData("year")]
    [InlineData("YEAR")]
    [InlineData("Year")]
    [InlineData("YeAr")]
    public void Transform_WithCaseVariationsOfYear_ShouldWork(string intervalType)
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 2)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", intervalType } }
        };

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(year, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(2, parameters[0]);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("days")]
    [InlineData("years")]
    [InlineData("months")]
    [InlineData("hours")]
    [InlineData("minutes")]
    [InlineData("seconds")]
    [InlineData("weeks")]
    [InlineData("quarters")]
    [InlineData("milliseconds")]
    [InlineData("microseconds")]
    [InlineData("nanoseconds")]
    [InlineData("dayofyears")]
    [InlineData("unknown")]
    [InlineData("test")]
    [InlineData("123")]
    [InlineData("!@#")]
    public void Transform_WithInvalidIntervalTypes_ShouldThrowArgumentException(string invalidInterval)
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 30)
        {
            Metadata = new Dictionary<string, object?> { { "intervalType", invalidInterval } }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider()));
        Assert.Contains($"Invalid interval type '{invalidInterval}'", exception.Message);
        Assert.Contains("Valid types are:", exception.Message);
    }

    [Fact]
    public void Transform_WithStringValue_ShouldAcceptStringValue()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", "30");

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal("30", parameters[0]);
    }

    [Fact]
    public void Transform_WithDecimalValue_ShouldAcceptDecimalValue()
    {
        // Arrange
        var rule = new FilterRule("CreatedDate", "date_diff", 30.5);

        // Act
        var (query, parameters) = _transformer.Transform(rule, "CreatedDate", 0, new SqlServerFormatProvider());

        // Assert
        Assert.Equal("DATEDIFF(day, CreatedDate, GETDATE()) = @p0", query);
        Assert.NotNull(parameters);
        Assert.Single(parameters);
        Assert.Equal(30.5, parameters[0]);
    }
}
