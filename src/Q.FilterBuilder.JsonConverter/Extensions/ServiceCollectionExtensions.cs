using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Q.FilterBuilder.JsonConverter.Extensions;

/// <summary>
/// Extension methods for configuring Q.FilterBuilder JsonConverter services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Q.FilterBuilder JsonConverter services to the specified <see cref="IServiceCollection"/> with default options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddQueryBuilderJsonConverter(this IServiceCollection services)
    {
        return services.AddQueryBuilderJsonConverter(_ => { });
    }

    /// <summary>
    /// Adds Q.FilterBuilder JsonConverter services to the specified <see cref="IServiceCollection"/> with custom options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="QueryBuilderOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddQueryBuilderJsonConverter(
        this IServiceCollection services,
        Action<QueryBuilderOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        // Register the converter as a singleton with configured options
        services.TryAddSingleton(_ =>
        {
            var options = new QueryBuilderOptions();
            configureOptions(options);
            return new QueryBuilderConverter(options);
        });

        return services;
    }
}
