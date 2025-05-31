using System;
using Q.FilterBuilder.PostgreSql.Extensions;
using Xunit;

namespace Q.FilterBuilder.PostgreSql.Tests.Extensions;

public class PostgreSqlFilterBuilderOptionsTests
{
    [Fact]
    public void ConfigureTypeConversions_WithValidAction_ShouldReturnSameInstance()
    {
        // Arrange
        var options = new PostgreSqlFilterBuilderOptions();

        // Act
        var result = options.ConfigureTypeConversions(tc => { });

        // Assert
        Assert.Same(options, result);
    }

    [Fact]
    public void ConfigureTypeConversions_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = new PostgreSqlFilterBuilderOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => options.ConfigureTypeConversions(null!));
        Assert.Equal("configure", exception.ParamName);
    }

    [Fact]
    public void ConfigureRuleTransformers_WithValidAction_ShouldReturnSameInstance()
    {
        // Arrange
        var options = new PostgreSqlFilterBuilderOptions();

        // Act
        var result = options.ConfigureRuleTransformers(rt => { });

        // Assert
        Assert.Same(options, result);
    }

    [Fact]
    public void ConfigureRuleTransformers_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = new PostgreSqlFilterBuilderOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => options.ConfigureRuleTransformers(null!));
        Assert.Equal("configure", exception.ParamName);
    }

    [Fact]
    public void FluentConfiguration_ShouldAllowChaining()
    {
        // Arrange
        var options = new PostgreSqlFilterBuilderOptions();

        // Act
        var result = options
            .ConfigureTypeConversions(tc => { })
            .ConfigureRuleTransformers(rt => { });

        // Assert
        Assert.Same(options, result);
    }

    [Fact]
    public void DefaultOptions_ShouldNotThrow()
    {
        // Arrange & Act
        var options = new PostgreSqlFilterBuilderOptions();

        // Assert
        Assert.NotNull(options);
    }

    [Fact]
    public void ConfigureTypeConversions_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var options = new PostgreSqlFilterBuilderOptions();

        // Act & Assert
        options.ConfigureTypeConversions(tc => { });
        options.ConfigureTypeConversions(tc => { });

        Assert.NotNull(options);
    }

    [Fact]
    public void ConfigureRuleTransformers_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var options = new PostgreSqlFilterBuilderOptions();

        // Act & Assert
        options.ConfigureRuleTransformers(rt => { });
        options.ConfigureRuleTransformers(rt => { });

        Assert.NotNull(options);
    }

    [Fact]
    public void Options_ShouldSupportMethodChaining()
    {
        // Arrange
        var options = new PostgreSqlFilterBuilderOptions();

        // Act
        var result = options
            .ConfigureTypeConversions(tc => { })
            .ConfigureRuleTransformers(rt => { })
            .ConfigureTypeConversions(tc => { });

        // Assert
        Assert.Same(options, result);
    }

    [Fact]
    public void Options_ShouldSupportComplexFluentChaining()
    {
        // Arrange & Act
        var options = new PostgreSqlFilterBuilderOptions()
            .ConfigureTypeConversions(tc =>
            {
                // Complex type conversion configuration
            })
            .ConfigureRuleTransformers(rt =>
            {
                // Complex rule transformer configuration
            })
            .ConfigureTypeConversions(tc =>
            {
                // Override previous configuration
            });

        // Assert
        Assert.NotNull(options);
    }
}
