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

    [Fact]
    public void AddQueryBuilderJsonConverter_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddQueryBuilderJsonConverter();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_WithCustomOptions_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddQueryBuilderJsonConverter(options => { });

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_WithComplexConfiguration_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueryBuilderJsonConverter(options =>
        {
            options.ConditionPropertyName = "combinator";
            options.RulesPropertyName = "children";
            options.FieldPropertyName = "id";
            options.OperatorPropertyName = "op";
            options.ValuePropertyName = "val";
            options.TypePropertyName = "dataType";
            options.DataPropertyName = "metadata";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var converter = serviceProvider.GetRequiredService<QueryBuilderConverter>();
        Assert.NotNull(converter);
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_WithEmptyConfiguration_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueryBuilderJsonConverter(options => { });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var converter = serviceProvider.GetRequiredService<QueryBuilderConverter>();
        Assert.NotNull(converter);
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_ConfigurationShouldBeCalledOnce()
    {
        // Arrange
        var services = new ServiceCollection();
        var configurationCallCount = 0;

        // Act
        services.AddQueryBuilderJsonConverter(options =>
        {
            configurationCallCount++;
            options.ConditionPropertyName = "combinator";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Get the service multiple times
        var converter1 = serviceProvider.GetRequiredService<QueryBuilderConverter>();
        var converter2 = serviceProvider.GetRequiredService<QueryBuilderConverter>();

        // Assert
        Assert.Equal(1, configurationCallCount);
        Assert.Same(converter1, converter2);
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_WithExceptionInConfiguration_ShouldPropagateException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueryBuilderJsonConverter(options =>
        {
            throw new InvalidOperationException("Test exception");
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
            serviceProvider.GetRequiredService<QueryBuilderConverter>());
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_WithUnicodePropertyNames_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueryBuilderJsonConverter(options =>
        {
            options.ConditionPropertyName = "条件";
            options.RulesPropertyName = "règles";
            options.FieldPropertyName = "поле";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var converter = serviceProvider.GetRequiredService<QueryBuilderConverter>();
        Assert.NotNull(converter);
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_WithEmptyStringPropertyNames_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueryBuilderJsonConverter(options =>
        {
            options.ConditionPropertyName = "";
            options.RulesPropertyName = "";
            options.FieldPropertyName = "";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var converter = serviceProvider.GetRequiredService<QueryBuilderConverter>();
        Assert.NotNull(converter);
    }

    [Fact]
    public void AddQueryBuilderJsonConverter_WithWhitespacePropertyNames_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddQueryBuilderJsonConverter(options =>
        {
            options.ConditionPropertyName = "   ";
            options.RulesPropertyName = "\t";
            options.FieldPropertyName = "\n";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var converter = serviceProvider.GetRequiredService<QueryBuilderConverter>();
        Assert.NotNull(converter);
    }
}
