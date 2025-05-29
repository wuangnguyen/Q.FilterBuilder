using System;
using Q.FilterBuilder.Core.TypeConversion;
using Microsoft.Extensions.DependencyInjection;

namespace Q.FilterBuilder.Core.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register type conversion services.
/// Note: This file provides examples for DI registration.
/// To use with actual DI container, add Microsoft.Extensions.DependencyInjection package reference.
/// </summary>
public static class TypeConversionServiceCollectionExtensions
{
    /// <summary>
    /// Adds the type conversion service to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTypeConversion(this IServiceCollection services)
    {
        services.AddSingleton<ITypeConversionService, TypeConversionService>();
        return services;
    }

    /// <summary>
    /// Adds the type conversion service with custom converter registration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureConverters">Action to configure custom converters.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTypeConversion(
        this IServiceCollection services,
        Action<ITypeConversionService> configureConverters)
    {
        if (configureConverters == null)
        {
            throw new ArgumentNullException(nameof(configureConverters));
        }

        services.AddSingleton<ITypeConversionService>(serviceProvider =>
        {
            var service = new TypeConversionService();
            configureConverters(service);
            return service;
        });

        return services;
    }
}
