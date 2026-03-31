using Testcontainers.PostgreSql;
using TikalBackend.IntegrationTests.Utils;

namespace TikalBackend.IntegrationTests;

[SetUpFixture]
internal sealed class TestContainerSetup
{
    private PostgreSqlContainer databaseContainer;

    [OneTimeSetUp]
    public async Task Setup()
    {
        databaseContainer = PostgresDatabase.Instance;

        await databaseContainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await databaseContainer.StopAsync();
    }
}