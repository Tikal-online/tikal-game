using Lobbies.Contracts.Models;
using Lobbies.Domain.Entities;
using Lobbies.Domain.EventData;
using Shared.Contracts.Enums;

namespace Lobbies.Application.Mappers;

internal static class PlayerMapper
{
    // Entity --> Model
    public static LobbyPlayerModel PlayerToLobbyPlayerModel(Player player)
    {
        return new LobbyPlayerModel
        {
            UserId = player.UserId,
            Name = player.Name,
            SelectedColour = (ColourModel)player.SelectedColour,
            IsReady = player.IsReady,
            IsOwner = player.IsOwner
        };
    }

    // EventData --> Model
    public static LobbyPlayerModel PlayerEventDataToLobbyPlayerModel(PlayerEventData player)
    {
        return new LobbyPlayerModel
        {
            UserId = player.UserId,
            Name = player.Name,
            SelectedColour = (ColourModel)player.SelectedColour,
            IsReady = player.IsReady,
            IsOwner = player.IsOwner
        };
    }
}