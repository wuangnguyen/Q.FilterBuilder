using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.IntegrationTests.Infrastructure.Providers;
using Xunit;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure;

/// <summary>
/// Shared database container fixture that creates one container per provider for all tests
/// </summary>
public class DatabaseContainerFixture : IAsyncLifetime
{
    private IProviderStrategy? _providerStrategy;
    private readonly IConfiguration _configuration;

    public DatabaseProvider Provider { get; private set; }
    public string ConnectionString { get; private set; } = string.Empty;

    public DatabaseContainerFixture()
    {
        // Load configuration from appsettings.test.json
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var testConfig = new TestConfiguration(_configuration);
        Provider = testConfig.GetDatabaseProvider();
    }

    public async Task InitializeAsync()
    {
        // Create provider strategy for the provider
        var providerFactory = new ProviderStrategyFactory();
        _providerStrategy = providerFactory.GetStrategy(Provider);

        // Get database configuration for the specific provider
        var databaseConfig = new DatabaseConfiguration();
        _configuration.GetSection(Provider.ToString()).Bind(databaseConfig);

        // Initialize container and get connection string
        ConnectionString = await _providerStrategy.InitializeContainerAsync(databaseConfig);
    }

    public async Task DisposeAsync()
    {
        if (_providerStrategy != null)
        {
            await _providerStrategy.DisposeContainerAsync();
        }
    }
}

/// <summary>
/// Test collection definition to share the database container across all test classes
/// </summary>
[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<DatabaseContainerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
