using Q.FilterBuilder.IntegrationTests.Infrastructure;

namespace Q.FilterBuilder.IntegrationTests;

/// <summary>
/// Program entry point for the test web application
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TestStartup>();
            });
}
