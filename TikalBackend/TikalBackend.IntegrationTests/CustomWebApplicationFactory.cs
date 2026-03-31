using Accounts.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Utils;

namespace TikalBackend.IntegrationTests;

internal sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string databaseConnectionString;

    public CustomWebApplicationFactory(string databaseConnectionString)
    {
        this.databaseConnectionString = databaseConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // configure test authentication
            services.Configure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            });

            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            // configure test db contexts
            var contextOptionsDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AccountsDbContext>));

            if (contextOptionsDescriptor != null)
            {
                services.Remove(contextOptionsDescriptor);
            }

            services.AddDbContext<AccountsDbContext>(options =>
            {
                options.UseNpgsql(
                    databaseConnectionString,
                    npgOptions => npgOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        AccountsDbContext.Schema
                    )
                );
            });

            RebuildDatabase(services);
        });
    }

    private static void RebuildDatabase(IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();

        var databaseContext = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();

        databaseContext.Database.DropTables();
        databaseContext.Database.Migrate();
    }
}