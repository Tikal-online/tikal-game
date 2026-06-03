using Accounts.Contracts.Models;
using Lobbies.Contracts.Models;
using Lobbies.Domain.Entities;

namespace Lobbies.Application.Mappers;

internal static class PlayerMapper
{
    public static LobbyPlayerModel PlayerToLobbyPlayerModel(Player player, AccountModel account)
    {
        return new LobbyPlayerModel
        {
            UserId = player.UserId,
            Name = account.Name,
            IsReady = player.IsReady,
            IsOwner = player.IsOwner
        };
    }
}