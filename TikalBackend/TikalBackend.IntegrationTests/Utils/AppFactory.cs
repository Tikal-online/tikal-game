namespace TikalBackend.IntegrationTests.Utils;

internal static class AppFactory
{
    private static readonly Lazy<CustomWebApplicationFactory> instance = new(
        () => new CustomWebApplicationFactory(PostgresDatabase.Instance.GetConnectionString())
    );

    public static CustomWebApplicationFactory Instance => instance.Value;
}