using Microsoft.AspNetCore.Mvc.Testing;
using Q.FilterBuilder.IntegrationTests.Configuration;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure;

/// <summary>
/// Web application factory for integration tests
/// </summary>
public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private string _connectionString = string.Empty;

    public void ConfigureProvider(DatabaseProvider provider, string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Load the base configuration from appsettings.test.json
            config.AddJsonFile("appsettings.test.json", optional: false);

            // Override specific values for the test
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TestDatabase"] = _connectionString
            });
        });

        builder.UseEnvironment("Testing");

        // Set the content root to the test project directory
        var testProjectPath = Path.GetDirectoryName(typeof(IntegrationTestWebApplicationFactory).Assembly.Location);
        if (testProjectPath != null)
        {
            // Navigate up to find the test project root (where appsettings.test.json is located)
            var projectRoot = FindProjectRoot(testProjectPath);
            if (projectRoot != null)
            {
                builder.UseContentRoot(projectRoot);
            }
        }
    }

    private static string? FindProjectRoot(string startPath)
    {
        var directory = new DirectoryInfo(startPath);

        while (directory != null)
        {
            // Look for appsettings.test.json or .csproj file to identify project root
            if (directory.GetFiles("appsettings.test.json").Length > 0 ||
                directory.GetFiles("*.csproj").Length > 0)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
