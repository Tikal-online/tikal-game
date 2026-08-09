namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal static class LobbyUrl
{
    private const string playerUrl = "Players";

    private const string lobbyUrl = "Lobbies";

    public const string SetPlayerReady = $"{playerUrl}/me/ready";

    public const string ActiveLobbyHub = "hub/activeLobby";

    public const string CreateLobby = $"{lobbyUrl}";

    public const string GetLobbies = $"{lobbyUrl}";

    public const string GetActiveLobby = $"{lobbyUrl}/me";

    public static string GetLobby(long id)
    {
        return $"Lobbies/{id}";
    }

    public static string JoinLobby(long id)
    {
        return $"Lobbies/{id}/Players";
    }

    public static string LeaveLobby(long id)
    {
        return $"Lobbies/{id}/Players/me";
    }

    public static string SendMessage(long id)
    {
        return $"Lobbies/{id}/Messages";
    }
}