using Microsoft.EntityFrameworkCore;
using Q.FilterBuilder.IntegrationTests.Configuration;
using Q.FilterBuilder.SqlServer.Extensions;
using Testcontainers.MsSql;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure.Providers;

/// <summary>
/// SQL Server provider strategy implementation
/// </summary>
public class SqlServerProviderStrategy : BaseProviderStrategy<MsSqlContainer, MsSqlBuilder>
{
    public override DatabaseProvider Provider => DatabaseProvider.SqlServer;

    public override void ConfigureFilterBuilder(IServiceCollection services)
    {
        services.AddSqlServerFilterBuilder();
    }

    public override void ConfigureDbContext(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseSqlServer(connectionString);
    }

    protected override MsSqlBuilder CreateContainerBuilder(DatabaseConfiguration configuration)
    {
        return new MsSqlBuilder();
    }

    protected override void ConfigureContainerBuilder(MsSqlBuilder builder, DatabaseConfiguration configuration)
    {
        builder.WithImage(configuration.ImageName)
               .WithPassword(configuration.Password)
               .WithCleanUp(true);
    }
}
