using Games.Contracts.Models;
using Games.Domain.Entities;
using Games.Domain.EventData;
using Lobbies.Contracts.Models;
using Shared.Contracts.Enums;
using Shared.Domain.Enums;

namespace Games.Application.Mappers;

internal static class PlayerMapper
{
    // Model --> Entity
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

    // EventData --> Model
    public static GamePlayerModel PlayerEventDataToGamePlayerModel(PlayerEventData player)
    {
        return new GamePlayerModel
        {
            UserId = player.UserId,
            Name = player.Name,
            Colour = (ColourModel)player.Colour,
            Points = player.Points,
        };
    }
}