using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Extensions;
using Q.FilterBuilder.Core.TypeConversion;
using Xunit;

namespace Q.FilterBuilder.Core.Tests.Extensions;

public class ServiceCollectionExtensionsTests
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

        Assert.Same(service2, service1);
    }

    [Fact]
    public void AddTypeConversion_WithBuiltInConverters_ShouldHaveDefaultConverters()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion();
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<ITypeConversionService>();

        // Assert - Test that the service can convert basic types
        Assert.Equal(123, service.ConvertValue("123", "int"));
        Assert.Equal(true, service.ConvertValue("true", "bool"));
        Assert.IsType<DateTime>(service.ConvertValue("2023-12-25", "datetime"));
        Assert.Equal("test", service.ConvertValue("test", "string"));
        Assert.Equal(3.14, service.ConvertValue("3.14", "double"));
        Assert.Equal(99.99m, service.ConvertValue("99.99", "decimal"));
        Assert.IsType<Guid>(service.ConvertValue("550e8400-e29b-41d4-a716-446655440000", "guid"));
    }

    [Fact]
    public void AddTypeConversion_ShouldAllowMultipleCalls()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion();
        services.AddTypeConversion(); // Second call should not cause issues
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ITypeConversionService>();
        Assert.NotNull(service);
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
    public void AddTypeConversion_WithExistingServices_ShouldNotAffectOtherServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton("test string");
        services.AddSingleton<object>(42);

        // Act
        services.AddTypeConversion();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var stringService = serviceProvider.GetService<string>();
        var objectService = serviceProvider.GetService<object>();
        var typeConversionService = serviceProvider.GetService<ITypeConversionService>();

        Assert.Equal("test string", stringService);
        Assert.Equal(42, objectService);
        Assert.NotNull(typeConversionService);
    }

    [Fact]
    public void AddTypeConversion_ServiceShouldBeUsableImmediately()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTypeConversion();
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<ITypeConversionService>();

        // Act
        var result = service.ConvertValue("123", "int");

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public void AddTypeConversion_ShouldWorkWithDependencyInjection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTypeConversion();
        services.AddTransient<TestService>();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var testService = serviceProvider.GetRequiredService<TestService>();

        // Assert
        Assert.NotNull(testService);
        Assert.NotNull(testService.TypeConversionService);

        // Test that the injected service works
        var result = testService.ConvertToInt("456");
        Assert.Equal(456, result);
    }

    [Fact]
    public void AddTypeConversion_WithCustomServiceCollection_ShouldWork()
    {
        // Arrange
        var services = new ServiceCollection();

        // Add some other services first
        services.AddSingleton("test service");

        // Act
        services.AddTypeConversion();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ITypeConversionService>();
        Assert.NotNull(service);
        Assert.IsType<TypeConversionService>(service);
    }

    [Fact]
    public void AddTypeConversion_ShouldRegisterCorrectServiceDescriptor()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion();

        // Assert
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITypeConversionService));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor!.Lifetime);
        Assert.Equal(typeof(TypeConversionService), descriptor.ImplementationType);
    }

    [Fact]
    public void AddTypeConversion_WithNullServiceCollection_ShouldThrowArgumentNullException()
    {
        // Arrange
        ServiceCollection? services = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => services!.AddTypeConversion());
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddTypeConversion_WithCustomConverters_ShouldRegisterCustomConverters()
    {
        // Arrange
        var services = new ServiceCollection();
        var customConverter = new TestCustomConverter();

        // Act
        services.AddTypeConversion(typeConversionService =>
        {
            typeConversionService.RegisterConverter("custom", customConverter);
        });
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<ITypeConversionService>();

        // Assert
        var result = service.ConvertValue("test_input", "custom");
        Assert.Equal("CUSTOM_test_input", result);
    }

    [Fact]
    public void AddTypeConversion_WithMultipleCustomConverters_ShouldRegisterAllConverters()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion(typeConversionService =>
        {
            typeConversionService.RegisterConverter("uppercase", new UppercaseConverter());
            typeConversionService.RegisterConverter("currency", new CurrencyConverter());
            typeConversionService.RegisterConverter("person", new PersonConverter());
        });
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<ITypeConversionService>();

        // Assert
        Assert.Equal("HELLO WORLD", service.ConvertValue("hello world", "uppercase"));
        Assert.Equal(123.45m, service.ConvertValue("$123.45", "currency"));

        var person = service.ConvertValue("John,30", "person") as TestPerson;
        Assert.NotNull(person);
        Assert.Equal("John", person!.Name);
        Assert.Equal(30, person.Age);
    }

    [Fact]
    public void AddTypeConversion_WithCustomConverters_ShouldStillHaveBuiltInConverters()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion(typeConversionService =>
        {
            typeConversionService.RegisterConverter("custom", new TestCustomConverter());
        });
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<ITypeConversionService>();

        // Assert - Custom converter works
        Assert.Equal("CUSTOM_test", service.ConvertValue("test", "custom"));

        // Assert - Built-in converters still work
        Assert.Equal(123, service.ConvertValue("123", "int"));
        Assert.Equal(true, service.ConvertValue("true", "bool"));
        Assert.Equal(3.14, service.ConvertValue("3.14", "double"));
    }

    [Fact]
    public void AddTypeConversion_WithCustomConverters_ShouldRegisterAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion(typeConversionService =>
        {
            typeConversionService.RegisterConverter("custom", new TestCustomConverter());
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service1 = serviceProvider.GetService<ITypeConversionService>();
        var service2 = serviceProvider.GetService<ITypeConversionService>();

        Assert.Same(service1, service2);
    }

    [Fact]
    public void AddTypeConversion_WithCustomConverters_ShouldWorkWithCollections()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTypeConversion(typeConversionService =>
        {
            typeConversionService.RegisterConverter("uppercase", new UppercaseConverter());
        });
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<ITypeConversionService>();

        // Assert
        var input = new[] { "hello", "world", "test" };
        var result = service.ConvertValue(input, "uppercase");

        Assert.IsType<string[]>(result);
        var stringArray = (string[])result!;
        Assert.Equal(new[] { "HELLO", "WORLD", "TEST" }, stringArray);
    }

    [Fact]
    public void AddTypeConversion_WithNullConfigureAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<ITypeConversionService>? configureConverters = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddTypeConversion(configureConverters!));
        Assert.Equal("configureConverters", exception.ParamName);
    }

    // Helper class for testing dependency injection
    private class TestService
    {
        public ITypeConversionService TypeConversionService { get; }

        public TestService(ITypeConversionService typeConversionService)
        {
            TypeConversionService = typeConversionService;
        }

        public int ConvertToInt(string value)
        {
            return (int)TypeConversionService.ConvertValue(value, "int")!;
        }
    }

    // Test converter classes
    private class TestCustomConverter : ITypeConverter<string>
    {
        public string Convert(object? value, Dictionary<string, object?>? metadata = null)
        {
            return $"CUSTOM_{value}";
        }
    }

    private class UppercaseConverter : ITypeConverter<string>
    {
        public string Convert(object? value, Dictionary<string, object?>? metadata = null)
        {
            return value?.ToString()?.ToUpperInvariant() ?? "";
        }
    }

    private class CurrencyConverter : ITypeConverter<decimal>
    {
        public decimal Convert(object? value, Dictionary<string, object?>? metadata = null)
        {
            var stringValue = value?.ToString() ?? "";
            if (stringValue.StartsWith("$"))
            {
                stringValue = stringValue.Substring(1);
            }
            return decimal.Parse(stringValue);
        }
    }

    private class TestPerson
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    private class PersonConverter : ITypeConverter<TestPerson>
    {
        public TestPerson Convert(object? value, Dictionary<string, object?>? metadata = null)
        {
            var stringValue = value?.ToString() ?? "";
            var parts = stringValue.Split(',');
            return new TestPerson
            {
                Name = parts.Length > 0 ? parts[0] : "",
                Age = parts.Length > 1 && int.TryParse(parts[1], out var age) ? age : 0
            };
        }
    }
}
