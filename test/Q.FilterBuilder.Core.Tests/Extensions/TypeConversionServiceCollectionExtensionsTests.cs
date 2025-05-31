using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Extensions;
using Q.FilterBuilder.Core.TypeConversion;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.Extensions;

public class TypeConversionServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTypeConversion_ShouldRegisterTypeConversionService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ITypeConversionService>();
        Assert.NotNull(service);
        Assert.IsType<TypeConversionService>(service);
    }

    [Fact]
    public void AddTypeConversion_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddTypeConversion();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddTypeConversion_ShouldRegisterAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service1 = serviceProvider.GetService<ITypeConversionService>();
        var service2 = serviceProvider.GetService<ITypeConversionService>();
        Assert.Same(service1, service2);
    }

    [Fact]
    public void AddTypeConversion_WithConfiguration_ShouldRegisterAndConfigureService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion(service =>
        {
            service.RegisterConverter("test", new TestConverter());
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ITypeConversionService>();
        Assert.NotNull(service);

        // Test that custom converter was registered
        var result = service.ConvertValue("test_input", "test");
        Assert.Equal("TEST_test_input", result);
    }

    [Fact]
    public void AddTypeConversion_WithConfiguration_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddTypeConversion(service => { });

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddTypeConversion_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddTypeConversion(null!));
        Assert.Equal("configureConverters", exception.ParamName);
    }

    [Fact]
    public void AddTypeConversion_WithConfiguration_ShouldRegisterAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion(service => { });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service1 = serviceProvider.GetService<ITypeConversionService>();
        var service2 = serviceProvider.GetService<ITypeConversionService>();
        Assert.Same(service1, service2);
    }

    [Fact]
    public void AddTypeConversion_WithConfiguration_ShouldAllowMultipleConverters()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion(service =>
        {
            service.RegisterConverter("test1", new TestConverter());
            service.RegisterConverter("test2", new TestConverter2());
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ITypeConversionService>();
        Assert.NotNull(service);
        
        var result1 = service.ConvertValue("input1", "test1");
        var result2 = service.ConvertValue("input2", "test2");
        
        Assert.Equal("TEST_input1", result1);
        Assert.Equal("TEST2_input2", result2);
    }

    [Fact]
    public void AddTypeConversion_WithConfiguration_ShouldPreserveBuiltInConverters()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion(service =>
        {
            service.RegisterConverter("custom", new TestConverter());
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ITypeConversionService>();
        Assert.NotNull(service);
        
        // Test built-in converter still works
        var boolResult = service.ConvertValue("true", "bool");
        Assert.IsType<bool>(boolResult);
        Assert.True((bool)boolResult!);
        
        // Test custom converter works
        var customResult = service.ConvertValue("test", "custom");
        Assert.Equal("TEST_test", customResult);
    }

    private class TestConverter : ITypeConverter<string>
    {
        public string Convert(object? value, Dictionary<string, object?>? metadata = null)
        {
            return $"TEST_{value}";
        }
    }

    private class TestConverter2 : ITypeConverter<string>
    {
        public string Convert(object? value, Dictionary<string, object?>? metadata = null)
        {
            return $"TEST2_{value}";
        }
    }
}
