using Testcontainers.PostgreSql;

namespace Identity.IntegrationTests.Utils;

internal class TestContainerFixture
{
    protected PostgreSqlContainer DatabaseContainer { get; } = PostgresDatabase.Instance;
}