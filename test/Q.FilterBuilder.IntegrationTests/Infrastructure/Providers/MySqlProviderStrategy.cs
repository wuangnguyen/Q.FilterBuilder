using Microsoft.EntityFrameworkCore;
using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.MySql.Extensions;
using Testcontainers.MySql;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure.Providers;

/// <summary>
/// MySQL provider strategy implementation
/// </summary>
public class MySqlProviderStrategy : BaseProviderStrategy<MySqlContainer, MySqlBuilder>
{
    public override DatabaseProvider Provider => DatabaseProvider.MySql;

    public override void ConfigureFilterBuilder(IServiceCollection services)
    {
        services.AddMySqlFilterBuilder();
    }

    public override void ConfigureDbContext(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override MySqlBuilder CreateContainerBuilder(DatabaseConfiguration configuration)
    {
        return new MySqlBuilder();
    }

    protected override void ConfigureContainerBuilder(MySqlBuilder builder, DatabaseConfiguration configuration)
    {
        builder.WithImage(configuration.ImageName)
               .WithDatabase(configuration.Database)
               .WithUsername(configuration.Username)
               .WithPassword(configuration.Password)
               .WithCleanUp(true);
    }
}
