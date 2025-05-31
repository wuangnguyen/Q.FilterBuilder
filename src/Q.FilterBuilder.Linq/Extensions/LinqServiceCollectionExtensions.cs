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

        return services.AddLinqFilterBuilder(null);
    }

    /// <summary>
    /// Adds the FilterBuilder service configured for LINQ to the dependency injection container
    /// with custom configuration options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure LINQ FilterBuilder options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddLinqFilterBuilder(
        this IServiceCollection services,
        Action<LinqFilterBuilderOptions>? configureOptions = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var options = new LinqFilterBuilderOptions();
        configureOptions?.Invoke(options);

        return services.AddFilterBuilder(
            new LinqFormatProvider(),
            options.TypeConversionConfiguration,
            ruleTransformers =>
            {
                // Core transformers (=, !=, <, <=, >, >=) are already registered
                // by RuleTransformerService constructor

                // Add LINQ-specific transformers
                RegisterLinqTransformers(ruleTransformers);

                // Apply any custom configuration
                options.RuleTransformerConfiguration?.Invoke(ruleTransformers);
            });
    }

    /// <summary>
    /// Registers LINQ specific rule transformers.
    /// Note: Core transformers (=, !=, <, <=, >, >=) are already registered by the base RuleTransformerService.
    /// </summary>
    /// <param name="service">The rule transformer service to register transformers with.</param>
    private static void RegisterLinqTransformers(IRuleTransformerService service)
    {
        // Register range operators
        service.RegisterTransformer("between", new BetweenRuleTransformer());
        service.RegisterTransformer("not_between", new NotBetweenRuleTransformer());

        // Register collection operators
        service.RegisterTransformer("in", new InRuleTransformer());
        service.RegisterTransformer("not_in", new NotInRuleTransformer());

        // Register string operators
        service.RegisterTransformer("contains", new ContainsRuleTransformer());
        service.RegisterTransformer("contains_any", new ContainsRuleTransformer()); // Alias for contains
        service.RegisterTransformer("not_contains", new NotContainsRuleTransformer());
        service.RegisterTransformer("begins_with", new BeginsWithRuleTransformer());
        service.RegisterTransformer("not_begins_with", new NotBeginsWithRuleTransformer());
        service.RegisterTransformer("ends_with", new EndsWithRuleTransformer());
        service.RegisterTransformer("not_ends_with", new NotEndsWithRuleTransformer());

        // Register null check operators
        service.RegisterTransformer("is_null", new IsNullRuleTransformer());
        service.RegisterTransformer("is_not_null", new IsNotNullRuleTransformer());
        service.RegisterTransformer("is_empty", new IsEmptyRuleTransformer());
        service.RegisterTransformer("is_not_empty", new IsNotEmptyRuleTransformer());

        // Register date operators
        service.RegisterTransformer("date_diff", new DateDiffRuleTransformer());
    }
}

/// <summary>
/// Configuration options for LINQ FilterBuilder.
/// </summary>
public class LinqFilterBuilderOptions
{
    internal Action<ITypeConversionService>? TypeConversionConfiguration { get; private set; }
    internal Action<IRuleTransformerService>? RuleTransformerConfiguration { get; private set; }

    /// <summary>
    /// Configures custom type converters for the LINQ FilterBuilder.
    /// </summary>
    /// <param name="configure">Action to configure type converters.</param>
    /// <returns>The options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when configure is null.</exception>
    public LinqFilterBuilderOptions ConfigureTypeConversions(Action<ITypeConversionService> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        TypeConversionConfiguration = configure;
        return this;
    }

    /// <summary>
    /// Configures custom rule transformers for the LINQ FilterBuilder.
    /// </summary>
    /// <param name="configure">Action to configure rule transformers.</param>
    /// <returns>The options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when configure is null.</exception>
    public LinqFilterBuilderOptions ConfigureRuleTransformers(Action<IRuleTransformerService> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        RuleTransformerConfiguration = configure;
        return this;
    }
}
