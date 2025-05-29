using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.Core.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register rule transformer services.
/// Note: This file provides examples for DI registration.
/// To use with actual DI container, add Microsoft.Extensions.DependencyInjection package reference.
/// </summary>
public static class RuleTransformerServiceCollectionExtensions
{
    /// <summary>
    /// Adds the rule transformer service to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRuleTransformers(this IServiceCollection services)
    {
        services.AddSingleton<IRuleTransformerService, RuleTransformerService>();
        return services;
    }

    /// <summary>
    /// Adds the rule transformer service with custom transformer registration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureTransformers">Action to configure custom transformers.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRuleTransformers(
        this IServiceCollection services,
        Action<IRuleTransformerService> configureTransformers)
    {
        if (configureTransformers == null)
        {
            throw new ArgumentNullException(nameof(configureTransformers));
        }

        services.AddSingleton<IRuleTransformerService>(serviceProvider =>
        {
            var service = new RuleTransformerService();
            configureTransformers(service);
            return service;
        });

        return services;
    }
}
