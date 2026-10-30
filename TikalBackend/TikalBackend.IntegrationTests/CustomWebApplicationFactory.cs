using Accounts.Infrastructure.Database;
using Games.Infrastructure.Database;
using Lobbies.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:backendDb"] = databaseConnectionString
            });
        });

        return base.CreateHost(builder);
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

            services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    public static async Task RebuildDatabase(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var accountsDbContext = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();

        await accountsDbContext.Database.DropTables();
        await accountsDbContext.Database.MigrateAsync();

        var lobbiesDbContext = scope.ServiceProvider.GetRequiredService<LobbiesDbContext>();

        await lobbiesDbContext.Database.MigrateAsync();

        var gamesDbContext = scope.ServiceProvider.GetRequiredService<GamesDbContext>();

        await gamesDbContext.Database.MigrateAsync();
    }
}