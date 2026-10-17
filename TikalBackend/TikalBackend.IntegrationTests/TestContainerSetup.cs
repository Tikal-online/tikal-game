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
    public PostgreSqlContainer DatabaseContainer { get; } = PostgresDatabase.Instance;

    public async ValueTask InitializeAsync()
    {
        await DatabaseContainer.StartAsync();
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
            await DatabaseContainer.StopAsync();
        }
    }
}