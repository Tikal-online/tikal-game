using Accounts.Contracts.Models;
using Lobbies.Contracts.Enums;
using Lobbies.Contracts.Models;
using Lobbies.Domain.Entities;
using Shared.Application.Contexts;

namespace Lobbies.Application.Mappers;

internal static class PlayerMapper
{
    public static LobbyPlayerModel PlayerToLobbyPlayerModel(Player player, UserAccount account)
    {
        return new LobbyPlayerModel
        {
            UserId = player.UserId,
            Name = account.Name,
            SelectedColour = (ColourModel)player.SelectedColour,
            IsReady = player.IsReady,
            IsOwner = player.IsOwner
        };
    }

    private static LobbyPlayerModel PlayerToLobbyPlayerModel(Player player, AccountModel account)
    {
        return new LobbyPlayerModel
        {
            UserId = player.UserId,
            Name = account.Name,
            SelectedColour = (ColourModel)player.SelectedColour,
            IsReady = player.IsReady,
            IsOwner = player.IsOwner
        };
    }

    public static List<LobbyPlayerModel> PlayersToLobbyPlayerModels(
        IEnumerable<Player> players,
        IEnumerable<AccountModel> accounts
    )
    {
        var accountDictionary = accounts.ToDictionary(a => a.UserId);

        return players.Select(p => PlayerToLobbyPlayerModel(p, accountDictionary[p.UserId])).ToList();
    }
}