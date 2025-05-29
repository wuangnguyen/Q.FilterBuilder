using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Providers;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.TypeConversion;

namespace Q.FilterBuilder.Core.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register FilterBuilder services.
/// Note: This file provides examples for DI registration.
/// To use with actual DI container, add Microsoft.Extensions.DependencyInjection package reference.
/// </summary>
public static class FilterBuilderServiceCollectionExtensions
{
    /// <summary>
    /// Adds the FilterBuilder service to the dependency injection container with the specified query syntax provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="querySyntaxProvider">The query syntax provider to use.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFilterBuilder(this IServiceCollection services, IQueryFormatProvider querySyntaxProvider)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (querySyntaxProvider == null)
        {
            throw new ArgumentNullException(nameof(querySyntaxProvider));
        }

        // Register the query syntax provider
        services.AddSingleton(querySyntaxProvider);

        // Register core services
        services.AddTypeConversion();
        services.AddRuleTransformers();

        // Register the FilterBuilder
        services.AddSingleton<IFilterBuilder, FilterBuilder>();

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service to the dependency injection container with the specified query syntax provider and rule transformer service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="querySyntaxProvider">The query syntax provider to use.</param>
    /// <param name="ruleTransformerService">The rule transformer service to use.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFilterBuilder(
        this IServiceCollection services,
        IQueryFormatProvider querySyntaxProvider,
        IRuleTransformerService ruleTransformerService)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (querySyntaxProvider == null)
        {
            throw new ArgumentNullException(nameof(querySyntaxProvider));
        }

        if (ruleTransformerService == null)
        {
            throw new ArgumentNullException(nameof(ruleTransformerService));
        }

        // Register the query syntax provider
        services.AddSingleton(querySyntaxProvider);

        // Register the rule transformer service
        services.AddSingleton(ruleTransformerService);

        // Register type conversion service
        services.AddTypeConversion();

        // Register the FilterBuilder
        services.AddSingleton<IFilterBuilder, FilterBuilder>();

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service to the dependency injection container with custom configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="querySyntaxProvider">The query syntax provider to use.</param>
    /// <param name="configureTypeConversion">Action to configure type conversion service.</param>
    /// <param name="configureRuleTransformers">Action to configure rule transformer service.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFilterBuilder(
        this IServiceCollection services,
        IQueryFormatProvider querySyntaxProvider,
        Action<ITypeConversionService>? configureTypeConversion = null,
        Action<IRuleTransformerService>? configureRuleTransformers = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (querySyntaxProvider == null)
        {
            throw new ArgumentNullException(nameof(querySyntaxProvider));
        }

        // Register the query syntax provider
        services.AddSingleton(querySyntaxProvider);

        // Register type conversion service with optional configuration
        if (configureTypeConversion != null)
        {
            services.AddTypeConversion(configureTypeConversion);
        }
        else
        {
            services.AddTypeConversion();
        }

        // Register rule transformer service with optional configuration
        if (configureRuleTransformers != null)
        {
            services.AddRuleTransformers(configureRuleTransformers);
        }
        else
        {
            services.AddRuleTransformers();
        }

        // Register the FilterBuilder
        services.AddSingleton<IFilterBuilder, FilterBuilder>();

        return services;
    }
}
