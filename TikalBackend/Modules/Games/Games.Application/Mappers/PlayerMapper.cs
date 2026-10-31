using Games.Domain.Entities;
using Lobbies.Contracts.Models;
using Shared.Domain.Enums;

namespace Games.Application.Mappers;

internal static class PlayerMapper
{
    private static Player LobbyPlayerModelToPlayer(LobbyPlayerModel player)
    {
        return new Player
        {
            UserId = player.UserId,
            Name = player.Name,
            Colour = (Colour)player.SelectedColour,
            Points = 0
        };
    }

    public static List<Player> LobbyPlayerModelsToPlayers(IEnumerable<LobbyPlayerModel> players)
    {
        return [.. players.Select(LobbyPlayerModelToPlayer)];
    }
}