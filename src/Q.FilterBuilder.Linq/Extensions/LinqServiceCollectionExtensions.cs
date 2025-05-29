using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.Extensions;
using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Linq.RuleTransformers;

namespace Q.FilterBuilder.Linq.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register LINQ FilterBuilder services.
/// </summary>
public static class LinqServiceCollectionExtensions
{
    /// <summary>
    /// Adds the FilterBuilder service configured for LINQ to the dependency injection container.
    /// This method registers all necessary services including the LINQ provider,
    /// LINQ-specific rule transformers, type conversion service, and the FilterBuilder itself.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddLinqFilterBuilder(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Create LINQ provider and rule transformer service
        var linqFormatProvider = new LinqFormatProvider();
        var linqRuleTransformerService = new LinqRuleTransformerService();

        // Register FilterBuilder with LINQ provider and rule transformer service
        services.AddFilterBuilder(linqFormatProvider, linqRuleTransformerService);

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for LINQ to the dependency injection container
    /// with custom type conversion configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureTypeConversion">Action to configure custom type converters.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddLinqFilterBuilder(
        this IServiceCollection services,
        Action<ITypeConversionService> configureTypeConversion)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureTypeConversion == null)
        {
            throw new ArgumentNullException(nameof(configureTypeConversion));
        }

        // Create LINQ provider and rule transformer service
        var linqFormatProvider = new LinqFormatProvider();
        var linqRuleTransformerService = new LinqRuleTransformerService();

        // Register FilterBuilder with LINQ provider and custom type conversion
        services.AddFilterBuilder(
            linqFormatProvider,
            configureTypeConversion,
            ruleTransformers =>
            {
                // Copy all LINQ transformers to the new service
                CopyTransformersFromService(linqRuleTransformerService, ruleTransformers);
            });

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for LINQ to the dependency injection container
    /// with custom rule transformer configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureRuleTransformers">Action to configure custom rule transformers.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddLinqFilterBuilder(
        this IServiceCollection services,
        Action<IRuleTransformerService> configureRuleTransformers)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureRuleTransformers == null)
        {
            throw new ArgumentNullException(nameof(configureRuleTransformers));
        }

        // Create LINQ provider and rule transformer service
        var linqFormatProvider = new LinqFormatProvider();
        var linqRuleTransformerService = new LinqRuleTransformerService();

        // Register FilterBuilder with LINQ provider and custom rule transformers
        services.AddFilterBuilder(
            linqFormatProvider,
            null, // Use default type conversion
            ruleTransformers =>
            {
                // First copy LINQ specific transformers
                CopyTransformersFromService(linqRuleTransformerService, ruleTransformers);
                // Then allow custom configuration
                configureRuleTransformers(ruleTransformers);
            });

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for LINQ to the dependency injection container
    /// with custom type conversion and rule transformer configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureTypeConversion">Action to configure custom type converters.</param>
    /// <param name="configureRuleTransformers">Action to configure custom rule transformers.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddLinqFilterBuilder(
        this IServiceCollection services,
        Action<ITypeConversionService> configureTypeConversion,
        Action<IRuleTransformerService> configureRuleTransformers)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureTypeConversion == null)
        {
            throw new ArgumentNullException(nameof(configureTypeConversion));
        }

        if (configureRuleTransformers == null)
        {
            throw new ArgumentNullException(nameof(configureRuleTransformers));
        }

        // Create LINQ provider and rule transformer service
        var linqFormatProvider = new LinqFormatProvider();
        var linqRuleTransformerService = new LinqRuleTransformerService();

        // Register FilterBuilder with LINQ provider and custom configuration
        services.AddFilterBuilder(
            linqFormatProvider,
            configureTypeConversion,
            ruleTransformers =>
            {
                // First copy LINQ specific transformers
                CopyTransformersFromService(linqRuleTransformerService, ruleTransformers);
                // Then allow custom configuration
                configureRuleTransformers(ruleTransformers);
            });

        return services;
    }



    /// <summary>
    /// Copies transformers from a source service to a target service.
    /// This method attempts to get known LINQ transformers and register them in the target service.
    /// </summary>
    /// <param name="sourceService">The source service to copy transformers from.</param>
    /// <param name="targetService">The target service to register transformers to.</param>
    private static void CopyTransformersFromService(IRuleTransformerService sourceService, IRuleTransformerService targetService)
    {
        // List of known LINQ transformer operators
        var linqOperators = new[]
        {
            "between", "not_between",
            "in", "not_in",
            "contains", "contains_any", "not_contains", "begins_with", "not_begins_with", "ends_with", "not_ends_with",
            "is_null", "is_not_null", "is_empty", "is_not_empty"
        };

        // Copy each transformer from source to target
        foreach (var operatorName in linqOperators)
        {
            try
            {
                var transformer = sourceService.GetRuleTransformer(operatorName);
                targetService.RegisterTransformer(operatorName, transformer);
            }
            catch (NotImplementedException)
            {
                // Skip transformers that don't exist in the source service
                // This provides resilience if the LinqRuleTransformerService changes
            }
        }
    }
}
