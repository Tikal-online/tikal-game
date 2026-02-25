using Testcontainers.PostgreSql;

namespace Identity.IntegrationTests.Utils;

public class TestContainerFixture
{
    protected PostgreSqlContainer DatabaseContainer { get; } = PostgresDatabase.Instance;
}