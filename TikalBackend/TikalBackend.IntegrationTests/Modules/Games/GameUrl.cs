namespace TikalBackend.IntegrationTests.Modules.Games;

internal static class GameUrl
{
    private const string gameUrl = "Games";

    public const string GetActiveGame = $"{gameUrl}/me";

    public static string LeaveGame(long id)
    {
        return $"Games/{id}/Players/me";
    }
}