using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

internal static class LobbyTestExtensions
{
    /// <summary>
    /// Sets the back reference from every player to the lobby they are part of,
    /// since an object initializer cannot reference the object it is creating.
    /// </summary>
    public static Lobby WithLinkedPlayers(this Lobby lobby)
    {
        foreach (var player in lobby.Players)
        {
            player.Lobby = lobby;
            player.LobbyId = lobby.Id;
        }

        return lobby;
    }
}