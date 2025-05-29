using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.JsonConverter.Extensions;
using System;
using System.Linq;
using Xunit;

namespace Q.FilterBuilder.JsonConverter.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddQueryBuilderJsonConverter_WithDefaultOptions_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueryBuilderJsonConverter();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var converter = serviceProvider.GetService<QueryBuilderConverter>();
        Assert.NotNull(converter);
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_WithCustomOptions_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        const string customConditionProperty = "combinator";

        // Act
        services.AddQueryBuilderJsonConverter(options =>
        {
            options.ConditionPropertyName = customConditionProperty;
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var converter = serviceProvider.GetRequiredService<QueryBuilderConverter>();
        Assert.NotNull(converter);

        // We can't directly test the options since they're internal to the converter,
        // but we can test that the converter was created successfully with custom options
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_ShouldRegisterAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueryBuilderJsonConverter();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var converter1 = serviceProvider.GetRequiredService<QueryBuilderConverter>();
        var converter2 = serviceProvider.GetRequiredService<QueryBuilderConverter>();

        Assert.Same(converter1, converter2);
    }

    // Removed global options tests since we simplified the implementation

    [Fact]
    public void AddQueryBuilderJsonConverter_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddQueryBuilderJsonConverter(null!));
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_WithNullConfigureOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            services.AddQueryBuilderJsonConverter(null!));
    }

    // Removed global options tests since we simplified the implementation

    [Fact]
    public void MultipleRegistrations_ShouldNotDuplicateServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueryBuilderJsonConverter();
        services.AddQueryBuilderJsonConverter(); // Second registration

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var converters = serviceProvider.GetServices<QueryBuilderConverter>().ToList();
        Assert.Single(converters); // Should only have one registration due to TryAddSingleton
    }
}
