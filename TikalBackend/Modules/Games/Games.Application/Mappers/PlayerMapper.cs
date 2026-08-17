using Accounts.Contracts.Models;
using Games.Contracts.Models;
using Games.Domain.Entities;
using Shared.Contracts.Enums;
using Shared.Domain.Enums;

namespace Games.Application.Mappers;

internal static class PlayerMapper
{
    public static List<Player> DictionaryToPlayers(IReadOnlyDictionary<ColourModel, string> dictionary)
    {
        return
        [
            .. dictionary
                .Select(x => new Player
                {
                    UserId = x.Value,
                    Colour = (Colour)x.Key,
                    Points = 0
                })
        ];
    }

    private static GamePlayerModel PlayerToGamePlayerModel(Player player, AccountModel account)
    {
        return new GamePlayerModel
        {
            UserId = player.UserId,
            Name = account.Name,
            Colour = (ColourModel)player.Colour,
            Points = player.Points
        };
    }

    public static List<GamePlayerModel> PlayersToGamePlayerModels(
        IEnumerable<Player> players,
        IEnumerable<AccountModel> accounts
    )
    {
        var accountDictionary = accounts.ToDictionary(a => a.UserId);

        return players.Select(p => PlayerToGamePlayerModel(p, accountDictionary[p.UserId])).ToList();
    }
}