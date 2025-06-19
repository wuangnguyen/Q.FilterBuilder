namespace Q.FilterBuilder.IntegrationTests.Configuration;

/// <summary>
/// Configuration provider for integration tests
/// </summary>
public class TestConfiguration
{
    private readonly IConfiguration _configuration;

    public TestConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the database provider to use for testing
    /// </summary>
    public DatabaseProvider GetDatabaseProvider()
    {
        var providerName = Environment.GetEnvironmentVariable("DatabaseProvider") ??
                           _configuration["DatabaseProvider"] ??
                           "SqlServer";

        if (Enum.TryParse<DatabaseProvider>(providerName, true, out var provider))
        {
            return provider;
        }

        throw new InvalidOperationException($"Invalid database provider: {providerName}. Supported providers: {string.Join(", ", Enum.GetNames<DatabaseProvider>())}");
    }
}
