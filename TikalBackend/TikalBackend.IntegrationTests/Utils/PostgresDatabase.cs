using Testcontainers.PostgreSql;

namespace TikalBackend.IntegrationTests.Utils;

internal static class PostgresDatabase
{
    private const string imageTag = "postgres:17.5";

    private static readonly Lazy<PostgreSqlContainer> instance = new(() => new PostgreSqlBuilder(imageTag).Build());

    public static PostgreSqlContainer Instance => instance.Value;
}