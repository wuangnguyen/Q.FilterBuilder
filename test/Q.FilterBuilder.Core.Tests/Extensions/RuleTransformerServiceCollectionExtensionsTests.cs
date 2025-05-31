using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Extensions;
using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.RuleTransformers;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.Extensions;

public class RuleTransformerServiceCollectionExtensionsTests
{
    [Fact]
    public void AddRuleTransformers_ShouldRegisterRuleTransformerService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRuleTransformers();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<IRuleTransformerService>();
        Assert.NotNull(service);
        Assert.IsType<RuleTransformerService>(service);
    }

    [Fact]
    public void AddRuleTransformers_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddRuleTransformers();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddRuleTransformers_ShouldRegisterAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRuleTransformers();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service1 = serviceProvider.GetService<IRuleTransformerService>();
        var service2 = serviceProvider.GetService<IRuleTransformerService>();
        Assert.Same(service1, service2);
    }

    [Fact]
    public void AddRuleTransformers_ShouldRegisterBuiltInTransformers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRuleTransformers();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<IRuleTransformerService>();
        Assert.NotNull(service);
        
        // Test that built-in transformers are registered
        var equalTransformer = service.GetRuleTransformer("equal");
        Assert.NotNull(equalTransformer);
        Assert.IsType<BasicRuleTransformer>(equalTransformer);
    }

    [Fact]
    public void AddRuleTransformers_WithConfiguration_ShouldRegisterAndConfigureService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRuleTransformers(service =>
        {
            service.RegisterTransformer("custom", new TestRuleTransformer());
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<IRuleTransformerService>();
        Assert.NotNull(service);

        // Test that custom transformer was registered
        var transformer = service.GetRuleTransformer("custom");
        Assert.NotNull(transformer);
        Assert.IsType<TestRuleTransformer>(transformer);
    }

    [Fact]
    public void AddRuleTransformers_WithConfiguration_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddRuleTransformers(service => { });

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddRuleTransformers_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddRuleTransformers(null!));
        Assert.Equal("configureTransformers", exception.ParamName);
    }

    [Fact]
    public void AddRuleTransformers_WithConfiguration_ShouldRegisterAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRuleTransformers(service => { });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service1 = serviceProvider.GetService<IRuleTransformerService>();
        var service2 = serviceProvider.GetService<IRuleTransformerService>();
        Assert.Same(service1, service2);
    }

    [Fact]
    public void AddRuleTransformers_WithConfiguration_ShouldAllowMultipleTransformers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRuleTransformers(service =>
        {
            service.RegisterTransformer("custom1", new TestRuleTransformer());
            service.RegisterTransformer("custom2", new TestRuleTransformer2());
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<IRuleTransformerService>();
        Assert.NotNull(service);
        
        var transformer1 = service.GetRuleTransformer("custom1");
        var transformer2 = service.GetRuleTransformer("custom2");
        
        Assert.IsType<TestRuleTransformer>(transformer1);
        Assert.IsType<TestRuleTransformer2>(transformer2);
    }

    [Fact]
    public void AddRuleTransformers_WithConfiguration_ShouldPreserveBuiltInTransformers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRuleTransformers(service =>
        {
            service.RegisterTransformer("custom", new TestRuleTransformer());
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<IRuleTransformerService>();
        Assert.NotNull(service);
        
        // Test built-in transformer still works
        var equalTransformer = service.GetRuleTransformer("equal");
        Assert.NotNull(equalTransformer);
        Assert.IsType<BasicRuleTransformer>(equalTransformer);
        
        // Test custom transformer works
        var customTransformer = service.GetRuleTransformer("custom");
        Assert.IsType<TestRuleTransformer>(customTransformer);
    }

    [Fact]
    public void AddRuleTransformers_WithConfiguration_ShouldAllowOverridingBuiltInTransformers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddRuleTransformers(service =>
        {
            service.RegisterTransformer("equal", new TestRuleTransformer());
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<IRuleTransformerService>();
        Assert.NotNull(service);
        
        // Test that built-in transformer was overridden
        var equalTransformer = service.GetRuleTransformer("equal");
        Assert.IsType<TestRuleTransformer>(equalTransformer);
    }

    private class TestRuleTransformer : IRuleTransformer
    {
        public (string query, object[]? parameters) Transform(FilterRule rule, string fieldName, string parameterName)
        {
            return ("TEST QUERY", new object[] { "test" });
        }
    }

    private class TestRuleTransformer2 : IRuleTransformer
    {
        public (string query, object[]? parameters) Transform(FilterRule rule, string fieldName, string parameterName)
        {
            return ("TEST QUERY 2", new object[] { "test2" });
        }
    }
}
