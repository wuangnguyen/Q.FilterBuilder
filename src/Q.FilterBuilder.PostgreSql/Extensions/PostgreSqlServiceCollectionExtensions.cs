using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.Core.Extensions;
using Q.FilterBuilder.Core.TypeConversion;

namespace Q.FilterBuilder.PostgreSql.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register PostgreSQL FilterBuilder services.
/// </summary>
public static class PostgreSqlServiceCollectionExtensions
{
    /// <summary>
    /// Adds the FilterBuilder service configured for PostgreSQL to the dependency injection container.
    /// This method registers all necessary services including the PostgreSQL provider,
    /// type conversion service, rule transformer service, and the FilterBuilder itself.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddPostgreSqlFilterBuilder(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Create PostgreSQL provider
        var postgreSqlFormatProvider = new PostgreSqlFormatProvider();

        // Register FilterBuilder with PostgreSQL provider (uses default rule transformers)
        services.AddFilterBuilder(postgreSqlFormatProvider);

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for PostgreSQL to the dependency injection container
    /// with custom type conversion configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureTypeConversion">Action to configure custom type converters.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddPostgreSqlFilterBuilder(
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

        // Create PostgreSQL provider
        var postgreSqlFormatProvider = new PostgreSqlFormatProvider();

        // Register FilterBuilder with PostgreSQL provider and custom type conversion
        services.AddFilterBuilder(
            postgreSqlFormatProvider,
            configureTypeConversion);

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for PostgreSQL to the dependency injection container
    /// with custom rule transformer configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureRuleTransformers">Action to configure custom rule transformers.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddPostgreSqlFilterBuilder(
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

        // Create PostgreSQL provider
        var postgreSqlFormatProvider = new PostgreSqlFormatProvider();

        // Register FilterBuilder with PostgreSQL provider and custom rule transformers
        services.AddFilterBuilder(
            postgreSqlFormatProvider,
            null, // Use default type conversion
            configureRuleTransformers);

        return services;
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for PostgreSQL to the dependency injection container
    /// with custom type conversion and rule transformer configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureTypeConversion">Action to configure custom type converters.</param>
    /// <param name="configureRuleTransformers">Action to configure custom rule transformers.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddPostgreSqlFilterBuilder(
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

        // Create PostgreSQL provider
        var postgreSqlFormatProvider = new PostgreSqlFormatProvider();

        // Register FilterBuilder with PostgreSQL provider and custom configuration
        services.AddFilterBuilder(
            postgreSqlFormatProvider,
            configureTypeConversion,
            configureRuleTransformers);

        return services;
    }
}
