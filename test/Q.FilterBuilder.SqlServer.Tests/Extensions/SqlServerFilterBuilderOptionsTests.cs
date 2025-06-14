using System;
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
        var result = options.ConfigureTypeConversion(tc => { });

        // Assert
        Assert.Same(options, result);
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
        var result = options.ConfigureRuleTransformers(rt => { });

        // Assert
        Assert.Same(options, result);
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
            .ConfigureTypeConversion(tc => { })
            .ConfigureRuleTransformers(rt => { });

        // Assert
        Assert.Same(options, result);
    }

    [Fact]
    public void DefaultOptions_ShouldNotThrow()
    {
        // Arrange & Act
        var options = new SqlServerFilterBuilderOptions();

        // Assert
        Assert.NotNull(options);
    }

    [Fact]
    public void ConfigureTypeConversion_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act & Assert
        options.ConfigureTypeConversion(tc => { });
        options.ConfigureTypeConversion(tc => { });

        Assert.NotNull(options);
    }

    [Fact]
    public void ConfigureRuleTransformers_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act & Assert
        options.ConfigureRuleTransformers(rt => { });
        options.ConfigureRuleTransformers(rt => { });

        Assert.NotNull(options);
    }

    [Fact]
    public void Options_ShouldSupportMethodChaining()
    {
        // Arrange
        var options = new SqlServerFilterBuilderOptions();

        // Act
        var result = options
            .ConfigureTypeConversion(tc => { })
            .ConfigureRuleTransformers(rt => { })
            .ConfigureTypeConversion(tc => { });

        // Assert
        Assert.Same(options, result);
    }

    [Fact]
    public void Options_ShouldSupportComplexFluentChaining()
    {
        // Arrange & Act
        var options = new SqlServerFilterBuilderOptions()
            .ConfigureTypeConversion(tc =>
            {
                // Complex type conversion configuration
            })
            .ConfigureRuleTransformers(rt =>
            {
                // Complex rule transformer configuration
            })
            .ConfigureTypeConversion(tc =>
            {
                // Override previous configuration
            });

        // Assert
        Assert.NotNull(options);
    }

}
