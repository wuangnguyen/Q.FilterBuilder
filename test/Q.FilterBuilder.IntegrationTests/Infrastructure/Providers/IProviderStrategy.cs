using Microsoft.EntityFrameworkCore;
using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.IntegrationTests.Database;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure.Providers;

/// <summary>
/// Strategy interface for database provider-specific configuration and setup
/// </summary>
public interface IProviderStrategy
{
    /// <summary>
    /// The database provider this strategy handles
    /// </summary>
    DatabaseProvider Provider { get; }

    /// <summary>
    /// Configure FilterBuilder services for this provider
    /// </summary>
    /// <param name="services">Service collection to configure</param>
    void ConfigureFilterBuilder(IServiceCollection services);

    /// <summary>
    /// Configure Entity Framework DbContext for this provider
    /// </summary>
    /// <param name="options">DbContext options builder</param>
    /// <param name="connectionString">Database connection string</param>
    void ConfigureDbContext(DbContextOptionsBuilder options, string connectionString);

    /// <summary>
    /// Create a database context for this provider
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Configured TestDbContext instance</returns>
    TestDbContext CreateDbContext(string connectionString);

    /// <summary>
    /// Initialize and start the database container
    /// </summary>
    /// <param name="configuration">Database configuration</param>
    /// <returns>Connection string for the started container</returns>
    Task<string> InitializeContainerAsync(DatabaseConfiguration configuration);

    /// <summary>
    /// Stop and dispose the database container
    /// </summary>
    Task DisposeContainerAsync();
}
