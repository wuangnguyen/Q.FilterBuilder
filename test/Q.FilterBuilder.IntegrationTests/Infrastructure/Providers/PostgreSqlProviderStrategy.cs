using Microsoft.EntityFrameworkCore;
using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.PostgreSql.Extensions;
using Testcontainers.PostgreSql;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure.Providers;

/// <summary>
/// PostgreSQL provider strategy implementation
/// </summary>
public class PostgreSqlProviderStrategy : BaseProviderStrategy<PostgreSqlContainer, PostgreSqlBuilder>
{
    public override DatabaseProvider Provider => DatabaseProvider.PostgreSql;

    public override void ConfigureFilterBuilder(IServiceCollection services)
    {
        services.AddPostgreSqlFilterBuilder();
    }

    public override void ConfigureDbContext(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseNpgsql(connectionString);
    }

    protected override PostgreSqlBuilder CreateContainerBuilder(DatabaseConfiguration configuration)
    {
        return new PostgreSqlBuilder();
    }

    protected override void ConfigureContainerBuilder(PostgreSqlBuilder builder, DatabaseConfiguration configuration)
    {
        builder.WithImage(configuration.ImageName)
               .WithDatabase(configuration.Database)
               .WithUsername(configuration.Username)
               .WithPassword(configuration.Password)
               .WithCleanUp(true);
    }
}
