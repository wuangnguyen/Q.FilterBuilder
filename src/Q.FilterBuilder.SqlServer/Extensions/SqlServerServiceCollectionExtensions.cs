using System;
using Microsoft.Extensions.DependencyInjection;
using Q.FilterBuilder.Core.Extensions;
using Q.FilterBuilder.Core.TypeConversion;
using Q.FilterBuilder.Core.RuleTransformers;
using Q.FilterBuilder.SqlServer.RuleTransformers;

namespace Q.FilterBuilder.SqlServer.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register SQL Server FilterBuilder services.
/// </summary>
public static class SqlServerServiceCollectionExtensions
{
    /// <summary>
    /// Adds the FilterBuilder service configured for SQL Server to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional action to configure SQL Server FilterBuilder options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static IServiceCollection AddSqlServerFilterBuilder(
        this IServiceCollection services,
        Action<SqlServerFilterBuilderOptions>? configureOptions = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var options = new SqlServerFilterBuilderOptions();
        configureOptions?.Invoke(options);

        return services.AddFilterBuilder(
            new SqlServerFormatProvider(),
            options.TypeConversionConfiguration,
            ruleTransformers =>
            {
                // Core transformers (=, !=, <, <=, >, >=) are already registered
                // by RuleTransformerService constructor

                // Add SQL Server-specific transformers
                RegisterSqlServerTransformers(ruleTransformers);

                // Apply any custom configuration
                options.RuleTransformerConfiguration?.Invoke(ruleTransformers);
            });
    }

    /// <summary>
    /// Registers SQL Server specific rule transformers.
    /// Note: Core transformers (=, !=, <, <=, >, >=) are already registered by the base RuleTransformerService.
    /// </summary>
    /// <param name="service">The rule transformer service to register transformers with.</param>
    private static void RegisterSqlServerTransformers(IRuleTransformerService service)
    {
        // Register range operators
        service.RegisterTransformer("between", new BetweenRuleTransformer());
        service.RegisterTransformer("not_between", new NotBetweenRuleTransformer());

        // Register collection operators
        service.RegisterTransformer("in", new InRuleTransformer());
        service.RegisterTransformer("not_in", new NotInRuleTransformer());

        // Register string operators
        service.RegisterTransformer("contains", new ContainsRuleTransformer());
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
/// Configuration options for SQL Server FilterBuilder.
/// </summary>
public class SqlServerFilterBuilderOptions
{
    internal Action<ITypeConversionService>? TypeConversionConfiguration { get; private set; }
    internal Action<IRuleTransformerService>? RuleTransformerConfiguration { get; private set; }

    /// <summary>
    /// Configures custom type conversions for the SQL Server FilterBuilder.
    /// </summary>
    /// <param name="configure">Action to configure type conversions.</param>
    /// <returns>The options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when configure is null.</exception>
    public SqlServerFilterBuilderOptions ConfigureTypeConversion(Action<ITypeConversionService> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        TypeConversionConfiguration = configure;
        return this;
    }

    /// <summary>
    /// Configures custom rule transformers for the SQL Server FilterBuilder.
    /// </summary>
    /// <param name="configure">Action to configure rule transformers.</param>
    /// <returns>The options instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when configure is null.</exception>
    public SqlServerFilterBuilderOptions ConfigureRuleTransformers(Action<IRuleTransformerService> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        RuleTransformerConfiguration = configure;
        return this;
    }
}
