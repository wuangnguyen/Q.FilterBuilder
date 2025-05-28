using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Extensions;
using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Core.RuleTransformers;

namespace Q.FilterBuilder.MySql.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register MySQL FilterBuilder services.
/// </summary>
public static class MySqlServiceCollectionExtensions
{
    /// <summary>
    /// Adds the FilterBuilder service configured for MySQL to the dependency injection container.
    /// This method registers all necessary services including the MySQL provider,
    /// type conversion service, rule transformer service, and the FilterBuilder itself.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddMySqlFilterBuilder(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Create MySQL provider
        var mySqlProvider = new MySqlProvider();

        // Register FilterBuilder with MySQL provider (uses default rule transformers)
        services.AddFilterBuilder(mySqlProvider);

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for MySQL to the dependency injection container
    /// with custom type conversion configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureTypeConversion">Action to configure custom type converters.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddMySqlFilterBuilder(
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

        // Create MySQL provider
        var mySqlProvider = new MySqlProvider();

        // Register FilterBuilder with MySQL provider and custom type conversion
        services.AddFilterBuilder(
            mySqlProvider,
            configureTypeConversion);

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for MySQL to the dependency injection container
    /// with custom rule transformer configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureRuleTransformers">Action to configure custom rule transformers.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddMySqlFilterBuilder(
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

        // Create MySQL provider
        var mySqlProvider = new MySqlProvider();

        // Register FilterBuilder with MySQL provider and custom rule transformers
        services.AddFilterBuilder(
            mySqlProvider,
            null, // Use default type conversion
            configureRuleTransformers);

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for MySQL to the dependency injection container
    /// with custom type conversion and rule transformer configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureTypeConversion">Action to configure custom type converters.</param>
    /// <param name="configureRuleTransformers">Action to configure custom rule transformers.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddMySqlFilterBuilder(
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

        // Create MySQL provider
        var mySqlProvider = new MySqlProvider();

        // Register FilterBuilder with MySQL provider and custom configuration
        services.AddFilterBuilder(
            mySqlProvider,
            configureTypeConversion,
            configureRuleTransformers);

        return services;
    }
}
