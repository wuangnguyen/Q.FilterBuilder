using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.IntegrationTests.Database;
using Q.FilterBuilder.IntegrationTests.Infrastructure.Providers;
using Q.FilterBuilder.IntegrationTests.Services;
using Q.FilterBuilder.JsonConverter;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure;

/// <summary>
/// Test startup configuration for different database providers
/// </summary>
public class TestStartup
{
    public IConfiguration Configuration { get; }

    public TestStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add controllers with JSON converter
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new QueryBuilderConverter());
            });

        // Add Swagger for API documentation
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Get configuration values
        var testConfig = new TestConfiguration(Configuration);
        var provider = testConfig.GetDatabaseProvider();

        // Get database configuration for the specific provider
        var databaseConfig = new DatabaseConfiguration();
        Configuration.GetSection(provider.ToString()).Bind(databaseConfig);

        var providerFactory = new ProviderStrategyFactory();
        var providerStrategy = providerFactory.GetStrategy(provider);
        var connectionString = Configuration.GetConnectionString("TestDatabase");

        // Configure provider-specific FilterBuilder
        providerStrategy.ConfigureFilterBuilder(services);

        // Configure provider-specific DbContext
        services.AddDbContext<TestDbContext>(options =>
            providerStrategy.ConfigureDbContext(options, connectionString ?? string.Empty));

        // Add services for dependency injection
        services.AddSingleton(testConfig);
        services.AddSingleton(databaseConfig);
        services.AddSingleton(providerFactory);
        services.AddSingleton(providerStrategy);
        services.AddScoped<IOrmExecutionService, OrmExecutionService>();

        // Add HTTP client for testing
        services.AddHttpClient();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Initialize database and seed test data
        InitializeDatabaseAsync(app.ApplicationServices).GetAwaiter().GetResult();
    }

    private async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        // Ensure database is created and seed test data
        await TestDataSeeder.SeedAsync(context);
    }
}
