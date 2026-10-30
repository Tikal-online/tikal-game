using Testcontainers.PostgreSql;
using TikalBackend.IntegrationTests;
using TikalBackend.IntegrationTests.Utils;
using Xunit.Sdk;
using Xunit.v3;

[assembly: AssemblyFixture(typeof(TestContainerSetup))]
[assembly: Parallelization(Mode = ParallelMode.None)]

namespace TikalBackend.IntegrationTests;


public class TestContainerSetup : IAsyncLifetime
{
    private readonly PostgreSqlContainer databaseContainer = PostgresDatabase.Instance;

    private CustomWebApplicationFactory customWebApplicationFactory;

    public async ValueTask InitializeAsync()
    {
        await databaseContainer.StartAsync();
        customWebApplicationFactory = AppFactory.Instance;
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
            await customWebApplicationFactory.DisposeAsync();
            await databaseContainer.StopAsync();
        }
    }
}