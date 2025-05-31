using System;
using System.Collections.Generic;
using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.SqlServer.Extensions;
using Xunit;

namespace Q.FilterBuilder.SqlServer.Tests.Extensions;

public class SqlServerFilterBuilderOptionsTests
{
    [Fact]
    public void ConfigureTypeConversion_WithValidAction_ShouldReturnSameInstance()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act
        var result = options.ConfigureTypeConversion(tc =>
        {
            // Configuration action
        });

        // Assert
        Assert.Same(options, result); // Should return same instance for chaining

        // Note: We can't test the internal configuration directly, but we can test
        // that the method doesn't throw and returns the correct instance
    }

    [Fact]
    public void ConfigureTypeConversion_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => options.ConfigureTypeConversion(null!));
        Assert.Equal("configure", exception.ParamName);
    }

    [Fact]
    public void ConfigureRuleTransformers_WithValidAction_ShouldReturnSameInstance()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act
        var result = options.ConfigureRuleTransformers(rt =>
        {
            // Configuration action
        });

        // Assert
        Assert.Same(options, result); // Should return same instance for chaining

        // Note: We can't test the internal configuration directly, but we can test
        // that the method doesn't throw and returns the correct instance
    }

    [Fact]
    public void ConfigureRuleTransformers_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => options.ConfigureRuleTransformers(null!));
        Assert.Equal("configure", exception.ParamName);
    }

    [Fact]
    public void FluentConfiguration_ShouldAllowChaining()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act
        var result = options
            .ConfigureTypeConversion(tc =>
            {
                // Configuration action
            })
            .ConfigureRuleTransformers(rt =>
            {
                // Configuration action
            });

        // Assert
        Assert.Same(options, result);

        // Note: We can't test the internal configurations directly, but we can test
        // that the fluent chaining works correctly
    }

    [Fact]
    public void DefaultOptions_ShouldNotThrow()
    {
        // Arrange & Act
        var options = new SqlServerFilterBuilderOptions();

        // Assert - Just verify that creating default options doesn't throw
        Assert.NotNull(options);
    }

    [Fact]
    public void ConfigureTypeConversion_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act & Assert - Should not throw when called multiple times
        options.ConfigureTypeConversion(tc => { });
        options.ConfigureTypeConversion(tc => { });

        Assert.NotNull(options);
    }

    [Fact]
    public void ConfigureRuleTransformers_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act & Assert - Should not throw when called multiple times
        options.ConfigureRuleTransformers(rt => { });
        options.ConfigureRuleTransformers(rt => { });

        Assert.NotNull(options);
    }
}
