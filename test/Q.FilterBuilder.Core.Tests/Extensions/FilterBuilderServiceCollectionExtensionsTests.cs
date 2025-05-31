using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Extensions;
using Q.FilterBuilder.Core.Providers;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.TypeConversion;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.Extensions;

public class FilterBuilderServiceCollectionExtensionsTests
{
    [Fact]
    public void AddFilterBuilder_WithProvider_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = new TestQueryFormatProvider();

        // Act
        services.AddFilterBuilder(provider);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IQueryFormatProvider>());
        Assert.NotNull(serviceProvider.GetService<ITypeConversionService>());
        Assert.NotNull(serviceProvider.GetService<IRuleTransformerService>());
        Assert.NotNull(serviceProvider.GetService<IFilterBuilder>());
        
        // Verify the provider is the same instance
        Assert.Same(provider, serviceProvider.GetService<IQueryFormatProvider>());
    }

    [Fact]
    public void AddFilterBuilder_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        var provider = new TestQueryFormatProvider();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            FilterBuilderServiceCollectionExtensions.AddFilterBuilder(null!, provider));
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddFilterBuilder_WithNullProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddFilterBuilder(null!));
        Assert.Equal("querySyntaxProvider", exception.ParamName);
    }

    [Fact]
    public void AddFilterBuilder_WithProviderAndRuleTransformerService_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = new TestQueryFormatProvider();
        var ruleTransformerService = new RuleTransformerService();

        // Act
        services.AddFilterBuilder(provider, ruleTransformerService);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IQueryFormatProvider>());
        Assert.NotNull(serviceProvider.GetService<ITypeConversionService>());
        Assert.NotNull(serviceProvider.GetService<IRuleTransformerService>());
        Assert.NotNull(serviceProvider.GetService<IFilterBuilder>());
        
        // Verify the instances are the same
        Assert.Same(provider, serviceProvider.GetService<IQueryFormatProvider>());
        Assert.Same(ruleTransformerService, serviceProvider.GetService<IRuleTransformerService>());
    }

    [Fact]
    public void AddFilterBuilder_WithProviderAndRuleTransformerService_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        var provider = new TestQueryFormatProvider();
        var ruleTransformerService = new RuleTransformerService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            FilterBuilderServiceCollectionExtensions.AddFilterBuilder(null!, provider, ruleTransformerService));
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddFilterBuilder_WithProviderAndRuleTransformerService_WithNullProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var ruleTransformerService = new RuleTransformerService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddFilterBuilder(null!, ruleTransformerService));
        Assert.Equal("querySyntaxProvider", exception.ParamName);
    }

    [Fact]
    public void AddFilterBuilder_WithProviderAndRuleTransformerService_WithNullRuleTransformerService_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = new TestQueryFormatProvider();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddFilterBuilder(provider, (IRuleTransformerService)null!));
        Assert.Equal("ruleTransformerService", exception.ParamName);
    }

    [Fact]
    public void AddFilterBuilder_WithConfigurationActions_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = new TestQueryFormatProvider();
        var typeConversionConfigured = false;
        var ruleTransformersConfigured = false;

        // Act
        services.AddFilterBuilder(
            provider,
            typeConversion => { typeConversionConfigured = true; },
            ruleTransformers => { ruleTransformersConfigured = true; });
        
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IQueryFormatProvider>());
        Assert.NotNull(serviceProvider.GetService<ITypeConversionService>());
        Assert.NotNull(serviceProvider.GetService<IRuleTransformerService>());
        Assert.NotNull(serviceProvider.GetService<IFilterBuilder>());
        Assert.True(typeConversionConfigured);
        Assert.True(ruleTransformersConfigured);
    }

    [Fact]
    public void AddFilterBuilder_WithNullConfigurationActions_ShouldRegisterDefaultServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = new TestQueryFormatProvider();

        // Act
        services.AddFilterBuilder(provider, null, null);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IQueryFormatProvider>());
        Assert.NotNull(serviceProvider.GetService<ITypeConversionService>());
        Assert.NotNull(serviceProvider.GetService<IRuleTransformerService>());
        Assert.NotNull(serviceProvider.GetService<IFilterBuilder>());
    }

    [Fact]
    public void AddFilterBuilder_WithConfigurationActions_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        var provider = new TestQueryFormatProvider();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            FilterBuilderServiceCollectionExtensions.AddFilterBuilder(null!, provider, null, null));
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddFilterBuilder_WithConfigurationActions_WithNullProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddFilterBuilder(null!, null, null));
        Assert.Equal("querySyntaxProvider", exception.ParamName);
    }

    [Fact]
    public void AddFilterBuilder_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = new TestQueryFormatProvider();

        // Act
        var result = services.AddFilterBuilder(provider);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddFilterBuilder_WithRuleTransformerService_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = new TestQueryFormatProvider();
        var ruleTransformerService = new RuleTransformerService();

        // Act
        var result = services.AddFilterBuilder(provider, ruleTransformerService);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddFilterBuilder_WithConfigurationActions_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = new TestQueryFormatProvider();

        // Act
        var result = services.AddFilterBuilder(provider, null, null);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddFilterBuilder_ShouldRegisterServicesAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = new TestQueryFormatProvider();

        // Act
        services.AddFilterBuilder(provider);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var filterBuilder1 = serviceProvider.GetService<IFilterBuilder>();
        var filterBuilder2 = serviceProvider.GetService<IFilterBuilder>();
        Assert.Same(filterBuilder1, filterBuilder2);

        var typeConversion1 = serviceProvider.GetService<ITypeConversionService>();
        var typeConversion2 = serviceProvider.GetService<ITypeConversionService>();
        Assert.Same(typeConversion1, typeConversion2);

        var ruleTransformers1 = serviceProvider.GetService<IRuleTransformerService>();
        var ruleTransformers2 = serviceProvider.GetService<IRuleTransformerService>();
        Assert.Same(ruleTransformers1, ruleTransformers2);
    }

    private class TestQueryFormatProvider : IQueryFormatProvider
    {
        public string ParameterPrefix => "@";
        public string AndOperator => "AND";
        public string OrOperator => "OR";

        public string FormatFieldName(string fieldName) => $"[{fieldName}]";
        public string FormatParameterName(int parameterIndex) => $"@p{parameterIndex}";
    }
}
