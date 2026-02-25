using Identity.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Users.Infrastructure.Database;

namespace Identity.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
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
            var contextOptionsDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UsersDbContext>));

            if (contextOptionsDescriptor != null)
            {
                services.Remove(contextOptionsDescriptor);
            }

            services.AddDbContext<UsersDbContext>(options => { options.UseNpgsql(databaseConnectionString); });

            RebuildDatabase(services);
        });
    }

    private static void RebuildDatabase(IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();

        var databaseContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        databaseContext.Database.DropTables();
        databaseContext.Database.Migrate();
    }
}