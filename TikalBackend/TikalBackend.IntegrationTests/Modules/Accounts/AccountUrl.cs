namespace TikalBackend.IntegrationTests.Modules.Accounts;

internal static class AccountUrl
{
    private const string accountUrl = "Accounts";

    public const string GetAccount = $"{accountUrl}/me";

    public const string CreateAccount = $"{accountUrl}";
}