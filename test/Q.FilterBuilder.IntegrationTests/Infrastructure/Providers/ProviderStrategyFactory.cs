using Q.FilterBuilder.IntegrationTests.Configuration;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure.Providers;

/// <summary>
/// Factory for creating provider strategy instances
/// </summary>
public class ProviderStrategyFactory
{
    private readonly Dictionary<DatabaseProvider, IProviderStrategy> _strategies;

    public ProviderStrategyFactory()
    {
        _strategies = new Dictionary<DatabaseProvider, IProviderStrategy>
        {
            [DatabaseProvider.SqlServer] = new SqlServerProviderStrategy(),
            [DatabaseProvider.MySql] = new MySqlProviderStrategy(),
            [DatabaseProvider.PostgreSql] = new PostgreSqlProviderStrategy()
        };
    }

    /// <summary>
    /// Get the strategy for the specified provider
    /// </summary>
    /// <param name="provider">Database provider</param>
    /// <returns>Provider strategy instance</returns>
    /// <exception cref="NotSupportedException">Thrown when provider is not supported</exception>
    public IProviderStrategy GetStrategy(DatabaseProvider provider)
    {
        if (_strategies.TryGetValue(provider, out var strategy))
        {
            return strategy;
        }

        throw new NotSupportedException($"Provider {provider} is not supported");
    }

    /// <summary>
    /// Get all available provider strategies
    /// </summary>
    /// <returns>Collection of all provider strategies</returns>
    public IEnumerable<IProviderStrategy> GetAllStrategies()
    {
        return _strategies.Values;
    }

    /// <summary>
    /// Get strategies for the specified providers
    /// </summary>
    /// <param name="providers">Database providers to get strategies for</param>
    /// <returns>Collection of provider strategies</returns>
    public IEnumerable<IProviderStrategy> GetStrategies(IEnumerable<DatabaseProvider> providers)
    {
        return providers.Select(GetStrategy);
    }

    /// <summary>
    /// Check if a provider is supported
    /// </summary>
    /// <param name="provider">Database provider to check</param>
    /// <returns>True if provider is supported</returns>
    public bool IsProviderSupported(DatabaseProvider provider)
    {
        return _strategies.ContainsKey(provider);
    }
}
