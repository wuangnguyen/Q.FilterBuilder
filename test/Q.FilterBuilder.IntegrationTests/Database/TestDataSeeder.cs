using Microsoft.EntityFrameworkCore;
using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.IntegrationTests.Infrastructure.Providers;

namespace Q.FilterBuilder.IntegrationTests.Database;

/// <summary>
/// Seeds test data for integration testing
/// </summary>
public static class TestDataSeeder
{
    /// <summary>
    /// Ensures the database is created and seeded with test data
    /// </summary>
    public static async Task SeedAsync(TestDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await context.Users.AnyAsync())
        {
            return; // Data already seeded
        }

        // The seed data is defined in the DbContext OnModelCreating method
        // This method just ensures the database is created and migrations are applied
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a fresh database context for the specified provider using strategy pattern
    /// </summary>
    public static TestDbContext CreateContext(DatabaseProvider provider, string connectionString)
    {
        var providerFactory = new ProviderStrategyFactory();
        var providerStrategy = providerFactory.GetStrategy(provider);

        return providerStrategy.CreateDbContext(connectionString);
    }



    /// <summary>
    /// Cleans up the database by dropping all tables
    /// </summary>
    public static async Task CleanupAsync(TestDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
    }
}
