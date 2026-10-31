using Testcontainers.PostgreSql;
using TikalBackend.IntegrationTests;
using Xunit.Sdk;
using Xunit.v3;

[assembly: AssemblyFixture(typeof(AppFactoryFixture))]
[assembly: Parallelization(Mode = ParallelMode.None)]

namespace TikalBackend.IntegrationTests;

public class AppFactoryFixture : IAsyncLifetime
{
    private const string imageTag = "postgres:17.5";

    private readonly PostgreSqlContainer databaseContainer = new PostgreSqlBuilder(imageTag).Build();

    public CustomWebApplicationFactory WebApplicationFactory { get; private set; }

    public async ValueTask InitializeAsync()
    {
        await databaseContainer.StartAsync();
        WebApplicationFactory = new CustomWebApplicationFactory(databaseContainer.GetConnectionString());
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            await WebApplicationFactory.DisposeAsync();
            await databaseContainer.StopAsync();
        }
    }
}