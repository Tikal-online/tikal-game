using Identity.IntegrationTests.Utils;
using NUnit.Framework;
using Testcontainers.PostgreSql;

namespace Identity.IntegrationTests;

[SetUpFixture]
public class TestContainerSetup
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