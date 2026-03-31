using Testcontainers.PostgreSql;

namespace TikalBackend.IntegrationTests.Utils;

internal abstract class TestContainerFixture
{
    protected PostgreSqlContainer DatabaseContainer { get; } = PostgresDatabase.Instance;
}