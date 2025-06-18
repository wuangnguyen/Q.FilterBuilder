using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.IntegrationTests.Database;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure.Providers;

/// <summary>
/// Base provider strategy class that contains common functionality for all database providers
/// </summary>
/// <typeparam name="TContainer">The type of Testcontainer for this provider</typeparam>
/// <typeparam name="TBuilder">The type of container builder for this provider</typeparam>
public abstract class BaseProviderStrategy<TContainer, TBuilder> : IProviderStrategy
    where TContainer : class, IDatabaseContainer
    where TBuilder : class
{
    protected TContainer? _container;

    /// <summary>
    /// Gets the database provider type
    /// </summary>
    public abstract DatabaseProvider Provider { get; }

    /// <summary>
    /// Configure provider-specific FilterBuilder services
    /// </summary>
    /// <param name="services">Service collection</param>
    public abstract void ConfigureFilterBuilder(IServiceCollection services);

    /// <summary>
    /// Configure provider-specific DbContext options
    /// </summary>
    /// <param name="options">DbContext options builder</param>
    /// <param name="connectionString">Database connection string</param>
    public abstract void ConfigureDbContext(DbContextOptionsBuilder options, string connectionString);

    /// <summary>
    /// Create a database context for this provider
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Configured TestDbContext instance</returns>
    public TestDbContext CreateDbContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>();
        ConfigureDbContext(options, connectionString);
        return new TestDbContext(options.Options);
    }

    /// <summary>
    /// Initialize and start the database container
    /// </summary>
    /// <param name="configuration">Database configuration</param>
    /// <returns>Connection string for the started container</returns>
    public async Task<string> InitializeContainerAsync(DatabaseConfiguration configuration)
    {
        var builder = CreateContainerBuilder(configuration);
        ConfigureContainerBuilder(builder, configuration);

        foreach (var env in configuration.Environment)
        {
            // Use dynamic to call WithEnvironment method on any builder type
            ((dynamic)builder).WithEnvironment(env.Key, env.Value);
        }

        _container = ((dynamic)builder).Build();

        await _container.StartAsync();

        return _container.GetConnectionString();
    }

    /// <summary>
    /// Stop and dispose the database container
    /// </summary>
    public async Task DisposeContainerAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
            _container = null;
        }
    }

    /// <summary>
    /// Create the container builder for this provider
    /// </summary>
    /// <param name="configuration">Database configuration</param>
    /// <returns>Container builder instance</returns>
    protected abstract TBuilder CreateContainerBuilder(DatabaseConfiguration configuration);

    /// <summary>
    /// Configure the container builder with provider-specific settings
    /// </summary>
    /// <param name="builder">Container builder</param>
    /// <param name="configuration">Database configuration</param>
    protected abstract void ConfigureContainerBuilder(TBuilder builder, DatabaseConfiguration configuration);
}
